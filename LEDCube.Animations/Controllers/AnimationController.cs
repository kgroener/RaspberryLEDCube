using LEDCube.Animations.Enums;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LEDCube.Animations.Controllers
{
    public class AnimationController
    {
        private readonly ConcurrentQueue<ILEDCubeAnimation> _animationHighPriorityQueue;

        private readonly ConcurrentQueue<ILEDCubeAnimation> _animationLowPriorityQueue;

        private readonly ConcurrentQueue<ILEDCubeAnimation> _animationNormalPriorityQueue;

        private readonly object _animationThreadLock;

        private readonly ILEDCubeController _cube;

        private readonly Random _random;

        private readonly TimeSpan _updateCooldown;

        private Task _animationThread;

        private TaskCompletionSource<bool> _animationThreadCompletionSource;

        public AnimationController(ILEDCubeController cube, TimeSpan updateCooldown = default(TimeSpan))
        {
            if (updateCooldown == default(TimeSpan))
            {
                updateCooldown = TimeSpan.Zero;
            }

            _updateCooldown = updateCooldown;

            _cube = cube;
            _random = new Random();
            _animationThreadLock = new object();
            _animationHighPriorityQueue = new ConcurrentQueue<ILEDCubeAnimation>();
            _animationNormalPriorityQueue = new ConcurrentQueue<ILEDCubeAnimation>();
            _animationLowPriorityQueue = new ConcurrentQueue<ILEDCubeAnimation>();
            Animations = LoadAnimations();
        }

        private IEnumerable<ILEDCubeAnimation> Animations { get; }

        private ILEDCubeAnimation CurrentAnimation { get; set; }

        public void RequestAnimation<T>(AnimationPriority priority) where T : ILEDCubeAnimation
        {
            var animation = RandomNumber.GetRandomItem(Animations.OfType<T>());

            switch (priority)
            {
                case AnimationPriority.High:
                    _animationHighPriorityQueue.Enqueue(animation);
                    break;

                case AnimationPriority.Normal:
                    _animationNormalPriorityQueue.Enqueue(animation);
                    break;

                case AnimationPriority.Low:
                    _animationLowPriorityQueue.Enqueue(animation);
                    break;

                default:
                    throw new ArgumentException($"Unknown priority: {priority}");
            }
        }

        public void Start()
        {
            lock (_animationThreadLock)
            {
                if (_animationThread != null)
                {
                    throw new InvalidOperationException("Animation controller has already been started.");
                }

                _animationThread = RunAnimationThread();
            }
        }

        public Task StopAsync()
        {
            lock (_animationThreadLock)
            {
                if (_animationThreadCompletionSource != null)
                {
                    throw new InvalidOperationException("Animation controller is already being stopped");
                }

                _animationThreadCompletionSource = new TaskCompletionSource<bool>();
                return _animationThreadCompletionSource.Task;
            }
        }

        private TimeSpan GetAllowedTimeToStop()
        {
            if (_animationHighPriorityQueue.Any())
            {
                return TimeSpan.FromSeconds(2);
            }

            if (_animationNormalPriorityQueue.Any())
            {
                return TimeSpan.FromSeconds(4);
            }

            return TimeSpan.FromSeconds(6);
        }

        private ILEDCubeAnimation GetAutoScheduledAnimation()
        {
            var autoSchedulableAnimations = Animations.Where(a => a.AutomaticSchedulingAllowed);
            return autoSchedulableAnimations.Skip(_random.Next(0, autoSchedulableAnimations.Count())).First();
        }

        private IEnumerable<ILEDCubeAnimation> LoadAnimations()
        {
            var animationType = typeof(ILEDCubeAnimation);
            return GetType().Assembly.ExportedTypes
                .Where(t => !t.IsAbstract)
                .Where(t => animationType.IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t) as ILEDCubeAnimation)
                .ToArray();
        }

        private Task RunAnimationThread()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    TimeSpan animationDuration = TimeSpan.Zero;
                    TimeSpan prefferedDuration = TimeSpan.Zero;

                    while (_animationThreadCompletionSource == null)
                    {
                        if (CurrentAnimation == null || CurrentAnimation.IsFinished || animationDuration >= prefferedDuration)
                        {
                            CurrentAnimation?.Cleanup();

                            ILEDCubeAnimation animation;
                            if (!_animationHighPriorityQueue.TryDequeue(out animation)
                                && !_animationNormalPriorityQueue.TryDequeue(out animation)
                                && !_animationLowPriorityQueue.TryDequeue(out animation))
                            {
                                animation = GetAutoScheduledAnimation();
                            }

                            CurrentAnimation = animation;
                            Debug.WriteLine($"Starting new animation of type {CurrentAnimation.GetType()}");
                            CurrentAnimation.Prepare();

                            animationDuration = TimeSpan.Zero;
                            prefferedDuration = CurrentAnimation.PrefferedDuration;
                        }

                        if (!CurrentAnimation.IsStopping)
                        {
                            var anyHighPriorityAnimationQueued = _animationHighPriorityQueue.Any();
                            var allowedTimeToStop = GetAllowedTimeToStop();

                            if (anyHighPriorityAnimationQueued)
                            {
                                var remainingDuration = prefferedDuration - animationDuration;
                                prefferedDuration = (allowedTimeToStop < remainingDuration) ? allowedTimeToStop : prefferedDuration;
                            }

                            if (animationDuration + allowedTimeToStop >= prefferedDuration)
                            {
                                Debug.WriteLine($"Requesting stop of current animation within {allowedTimeToStop.TotalSeconds} seconds");
                                CurrentAnimation.RequestStop(allowedTimeToStop);
                            }
                        }

                        //await Task.Delay(_updateCooldown);
                        var elapsedTime = stopwatch.Elapsed;
                        if (elapsedTime < _updateCooldown)
                        {
                            await Task.Delay(_updateCooldown - elapsedTime).ConfigureAwait(true);
                            elapsedTime = stopwatch.Elapsed;
                        }

                        animationDuration = animationDuration.Add(elapsedTime);

                        stopwatch.Restart();

                        CurrentAnimation.Update(_cube, elapsedTime);

                        await _cube.DrawAsync().ConfigureAwait(true);
                    }

                    lock (_animationThreadLock)
                    {
                        _animationThreadCompletionSource.SetResult(true);
                        _animationThreadCompletionSource = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        private struct AnimationQueueItem
        {
            public ILEDCubeAnimation Animation { get; set; }
            public AnimationPriority Priority { get; set; }
        }
    }
}