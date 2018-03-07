using LEDCube.Animations.Enums;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LEDCube.Animations.Controllers
{
    public class AnimationController
    {
        private struct AnimationQueueItem
        {
            public AnimationPriority Priority { get; set; }
            public ILEDCubeAnimation Animation { get; set; }
        }

        private Task _animationThread;
        private readonly ILEDCubeController _cube;
        private readonly object _animationThreadLock;
        private readonly object _animationQueueLock;
        private TaskCompletionSource<bool> _animationThreadCompletionSource;
        private readonly Queue<AnimationQueueItem> _animationQueue;
        private readonly Random _random;

        private IEnumerable<ILEDCubeAnimation> Animations { get; }
        private ILEDCubeAnimation CurrentAnimation { get; set; }


        public AnimationController(ILEDCubeController cube)
        {
            _cube = cube;
            _random = new Random();
            _animationThreadLock = new object();
            _animationQueueLock = new object();
            _animationQueue = new Queue<AnimationQueueItem>();
            Animations = LoadAnimations();
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

        private Task RunAnimationThread()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    DateTime animationStartTime = DateTime.Now;
                    DateTime animationStopTime = DateTime.MaxValue;
                    stopwatch.Start();

                    while (_animationThreadCompletionSource == null)
                    {
                        lock (_animationQueueLock)
                        {
                            DateTime currentTime = DateTime.Now;
                            if (CurrentAnimation == null || CurrentAnimation.IsFinished || currentTime >= animationStopTime)
                            {
                                CurrentAnimation?.Cleanup();

                                CurrentAnimation = _animationQueue.Any() ? _animationQueue.Dequeue().Animation : GetAutoScheduledAnimation();
                                Console.WriteLine($"Starting new animation of type {CurrentAnimation.GetType()}");
                                CurrentAnimation.Prepare();

                                animationStartTime = currentTime;
                                animationStopTime = animationStartTime + CurrentAnimation.PrefferedDuration;
                            }


                            if (!CurrentAnimation.IsStopping)
                            {
                                var anyHighPriorityAnimationQueued = _animationQueue.Any(a => a.Priority == AnimationPriority.High);
                                var allowedTimeToStop = GetAllowedTimeToStop();

                                if (anyHighPriorityAnimationQueued)
                                {
                                    DateTime allowedEndTime = currentTime + allowedTimeToStop;
                                    animationStopTime = (allowedEndTime < animationStopTime) ? allowedEndTime : animationStopTime;
                                }

                                if (currentTime + allowedTimeToStop >= animationStopTime)
                                {
                                    Console.WriteLine($"Requesting stop of current animation within {allowedTimeToStop.TotalSeconds} seconds");
                                    CurrentAnimation.RequestStop(allowedTimeToStop);
                                }
                            }
                        }

                        await Task.Delay(5);

                        var elapsedTime = stopwatch.Elapsed;

                        Debug.WriteLine($"FPS: {1 / stopwatch.Elapsed.TotalSeconds}");

                        stopwatch.Restart();

                        CurrentAnimation.Update(_cube, elapsedTime);

                        //Debug.WriteLine($"Update took {stopwatch.Elapsed}");

                        await _cube.DrawAsync();

                        //Debug.WriteLine($"Draw + Update took {stopwatch.Elapsed}");

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

        private ILEDCubeAnimation GetAutoScheduledAnimation()
        {
            IEnumerable<ILEDCubeAnimation> autoSchedulableAnimations = Animations.Where(a => a.AutomaticSchedulingAllowed);
            return autoSchedulableAnimations.Skip(_random.Next(0, autoSchedulableAnimations.Count())).First();
        }

        private TimeSpan GetAllowedTimeToStop()
        {
            var numberOfHighPrioAnimationsQueued = _animationQueue.Count(a => a.Priority == AnimationPriority.High);
            var anyNormalPrioAnimationsQueued = _animationQueue.Any(a => a.Priority == AnimationPriority.Normal);

            double normalPrioAllowedTimeReductionPart = (anyNormalPrioAnimationsQueued ? 0.5 : 0);
            return TimeSpan.FromSeconds(5 / (1.0 + numberOfHighPrioAnimationsQueued + normalPrioAllowedTimeReductionPart));
        }

        public void RequestAnimation<T>(AnimationPriority priority) where T : ILEDCubeAnimation
        {
            _animationQueue.Enqueue(new AnimationQueueItem()
            {
                Animation = Animations.OfType<T>().Single(),
                Priority = priority
            });
        }

        private IEnumerable<ILEDCubeAnimation> LoadAnimations()
        {
            var animationType = typeof(ILEDCubeAnimation);
            return GetType().Assembly.ExportedTypes
                .Where(t => !t.IsAbstract)
                .Where(t => animationType.IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t) as ILEDCubeAnimation);
        }
    }
}
