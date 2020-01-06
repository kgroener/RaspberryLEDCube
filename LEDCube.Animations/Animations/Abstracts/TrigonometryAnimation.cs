using LEDCube.CanonicalSchema.Contract;
using System;
using System.Drawing;

namespace LEDCube.Animations.Animations.Abstracts
{
    public abstract class TrigonometryAnimation : ILEDCubeAnimation
    {
        public enum VariableAxis
        {
            XAxis,
            YAxis,
            ZAxis
        };

        internal TrigonometryAnimation()
        {
        }

        public bool IsFinished { get; protected set; }
        public bool IsStopping { get; private set; }
        public bool IsFinite => false;
        public abstract TimeSpan PrefferedDuration { get; }
        public abstract bool AutomaticSchedulingAllowed { get; }

        public void RequestStop(TimeSpan timeout)
        {
            RequestStopInternal(timeout);
            IsStopping = true;
        }

        protected abstract void RequestStopInternal(TimeSpan timeout);
        protected abstract void UpdateInternal(TimeSpan updateInterval);

        protected abstract VariableAxis GetVariableAxis();

        public void Update(ILEDCube cube, TimeSpan updateInterval)
        {
            if (cube == null)
            {
                throw new ArgumentNullException(nameof(cube));
            }

            cube.Clear();

            UpdateInternal(updateInterval);

            //Double samples for antialising
            double dX = 1.0 / (2 * cube.ResolutionX);
            double dY = 1.0 / (2 * cube.ResolutionY);
            double dZ = 1.0 / (2 * cube.ResolutionZ);

            switch (GetVariableAxis())
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
            IsStopping = false;
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
