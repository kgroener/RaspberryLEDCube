using LEDCube.CanonicalSchema.Contract;
using System;
using System.Drawing;

namespace LEDCube.Animations.Animations.Abstracts
{
    public abstract class GeomitryAnimation : ILEDCubeAnimation
    {
        internal enum VariableAxis
        {
            XAxis,
            YAxis,
            ZAxis
        };

        private readonly VariableAxis _axis;
        private Random _random;
        private bool _isStopping;

        internal GeomitryAnimation(VariableAxis axis)
        {
            _axis = axis;
            _random = new Random();
        }

        public bool IsFinished { get; protected set; }
        public bool IsStopping => _isStopping;
        public bool IsFinite => false;
        public abstract TimeSpan PrefferedDuration { get; }
        public abstract bool AutomaticSchedulingAllowed { get; }

        public void RequestStop(TimeSpan timeout)
        {
            RequestStopInternal(timeout);
            _isStopping = true;
        }

        internal abstract void RequestStopInternal(TimeSpan timeout);

        public void Update(ILEDCubeController cube, TimeSpan updateInterval)
        {
            cube.Clear();

            //Double samples for antialising
            double dX = 1.0 / (2 * cube.ResolutionX);
            double dY = 1.0 / (2 * cube.ResolutionY);
            double dZ = 1.0 / (2 * cube.ResolutionZ);

            switch (_axis)
            {
                case VariableAxis.XAxis:
                    for (double y = -dY; y <= 1 + dY; y += dY)
                    {
                        for (double z = -dZ; z <= 1 + dZ; z += dZ)
                        {
                            double x = GetMissingAxisValue(null, y, z);
                            cube.SetLEDColor(x, y, z, GenerateColor(x, y, z));
                        }
                    }
                    break;
                case VariableAxis.YAxis:
                    for (double x = -dX; x <= 1 + dX; x += dX)
                    {
                        for (double z = -dZ; z <= 1 + dZ; z += dZ)
                        {
                            double y = GetMissingAxisValue(x, null, z);
                            cube.SetLEDColor(x, y, z, GenerateColor(x, y, z));
                        }
                    }
                    break;
                case VariableAxis.ZAxis:
                    for (double x = -dX; x <= 1 + dX; x += dX)
                    {
                        for (double y = -dY; y <= 1 + dY; y += dY)
                        {
                            double z = GetMissingAxisValue(x, y, null);
                            cube.SetLEDColor(x, y, z, GenerateColor(x, y, z));
                        }
                    }
                    break;
            };

            IterationValueX += updateInterval.TotalSeconds * Speed;
        }

        public double Speed { get; protected set; }
        protected double IterationValueX { get; private set; }
        protected abstract Color GenerateColor(double x, double y, double z);
        protected abstract double GetMissingAxisValue(double? x, double? y, double? z);

        public virtual void Prepare()
        {
            _isStopping = false;
            IsFinished = false;
            Speed = 1;
            IterationValueX = 0;

            PrepareInternal();
        }

        protected abstract void PrepareInternal();

        public void Cleanup()
        {
            CleanupInternal();
        }

        protected abstract void CleanupInternal();
    }
}
