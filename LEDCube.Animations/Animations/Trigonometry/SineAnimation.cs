using LEDCube.Animations.Animations.Abstracts;
using LEDCube.Animations.Helpers;
using System;
using System.Drawing;
using System.Linq;

namespace LEDCube.Animations.Animations.Trigonometry
{
    public class SineAnimation : TrigonometryAnimation
    {
        private double _slowDownSpeed;
        private int _factor;
        private VariableAxis _axis;

        public SineAnimation() : base()
        {
            _axis = VariableAxis.YAxis;
        }

        public override TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public override bool AutomaticSchedulingAllowed => true;

        protected override void PrepareInternal()
        {
            Speed = RandomNumber.GetRandomNumber(2, 5);
            _slowDownSpeed = 0;
            _factor = RandomNumber.GetRandomNumber(2, 4);
            _axis = RandomNumber.GetRandomEnumValue<VariableAxis>();
        }

        protected override void CleanupInternal()
        {
        }

        protected override void UpdateInternal(TimeSpan updateInterval)
        {
            Speed -= _slowDownSpeed * updateInterval.TotalSeconds;
        }

        protected override Color GenerateColor(double x, double y, double z)
        {
            var hue = (x + y + z + IterationValueX) * 100;
            var brightness = (Speed > 1 ? 0.4 : 0.4 * Speed);
            return ColorHelper.HSVToColor(hue, 1, brightness);
        }

        protected override double GetMissingAxisValue(double? x, double? y, double? z)
        {
            double a, b;

            switch (_axis)
            {
                case VariableAxis.XAxis:
                    a = z.Value;
                    b = y.Value;
                    break;
                case VariableAxis.YAxis:
                    a = x.Value;
                    b = z.Value;
                    break;
                case VariableAxis.ZAxis:
                    a = x.Value;
                    b = y.Value;
                    break;
                default:
                    throw new InvalidOperationException("Invalid axis");
            }

            return (Math.Sin((_factor * a * b) + IterationValueX) + 1) / 2.0;
        }

        

        protected override void RequestStopInternal(TimeSpan timeout)
        {
            _slowDownSpeed = Speed / timeout.TotalSeconds;
        }

        protected override VariableAxis GetVariableAxis()
        {
            return _axis;
        }
    }
}
