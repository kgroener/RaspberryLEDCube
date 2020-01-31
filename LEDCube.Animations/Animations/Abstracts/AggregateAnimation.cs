using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using LEDCube.Animations.Models;

namespace LEDCube.Animations.Animations.Abstracts
{
    public abstract class AggregateAnimation : ILEDCubeAnimation
    {
        private IDictionary<ILEDCubeAnimation, VirtualCube> _animations;

        public abstract bool AutomaticSchedulingAllowed { get; }
        public virtual bool IsFinished => _animations.Keys.All(a => a.IsFinished);

        public abstract bool IsFinite { get; }
        public virtual bool IsStopping => _animations.Keys.All(a => a.IsStopping);
        public virtual TimeSpan PrefferedDuration => _animations.Keys.Max(a => a.PrefferedDuration);

        public void Cleanup()
        {
            foreach (var animation in _animations)
            {
                animation.Key.Cleanup();
                animation.Value.Clear();
            }

            _animations = null;
        }

        public void Prepare()
        {
            _animations = GetAnimations().ToDictionary(k => k, v => new VirtualCube());
            foreach (var animation in _animations.Keys)
            {
                animation.Prepare();
            }
        }

        public void RequestStop(TimeSpan timeout)
        {
            foreach (var animation in _animations.Keys)
            {
                animation.RequestStop(timeout);
            }
        }

        public void Update(ILEDCube cube, TimeSpan updateInterval)
        {
            cube.Clear();
            foreach (var animation in _animations)
            {
                if (!animation.Value.HasBeenInitialized)
                {
                    animation.Value.Initialize(cube);
                }

                animation.Key.Update(animation.Value, updateInterval);
                animation.Value.WriteToCube(cube);
            }
        }

        protected abstract IEnumerable<ILEDCubeAnimation> GetAnimations();
    }
}