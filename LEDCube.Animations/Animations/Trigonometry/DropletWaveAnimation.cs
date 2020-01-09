using LEDCube.Animations.Animations.Trigonometry.Abstracts;
using LEDCube.Animations.Helpers;
using System;
using System.Drawing;
using System.Linq;

namespace LEDCube.Animations.Animations.Trigonometry
{
    public class DropletWaveAnimation : TrigonometryAnimation
    {
        private int _factor;
        private double _slowDownSpeed;

        public DropletWaveAnimation() : base()
        {
        }

        public override bool AutomaticSchedulingAllowed => true;
        public override TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        protected override void CleanupInternal()
        {
        }

        protected override Color GenerateColor(double x, double y, double z)
        {
            var hue = (y + IterationValueX) * 100;
            var brightness = (Speed > 1 ? 0.4 : 0.4 * Speed);
            return ColorHelper.HSVToColor(hue, 1, brightness);
        }

        protected override double GetMissingAxisValue(double? x, double? y, double? z)
        {
            return (Math.Sin(
                IterationValueX
                + Math.Sqrt(
                    Math.Pow((x.Value - 0.5) * _factor, 2)
                    + Math.Pow((z.Value - 0.5) * _factor, 2)))
                + 1.0)
                / 2.0;
        }

        protected override VariableAxis GetVariableAxis()
        {
            return VariableAxis.YAxis;
        }

        protected override void PrepareInternal()
        {
            Speed = RandomNumber.GetRandomNumber(2, 5);
            _slowDownSpeed = 0;
            _factor = RandomNumber.GetRandomNumber(2, 4);
        }

        protected override void RequestStopInternal(TimeSpan timeout)
        {
            _slowDownSpeed = Speed / timeout.TotalSeconds;
        }

        protected override void UpdateInternal(TimeSpan updateInterval)
        {
            Speed -= _slowDownSpeed * updateInterval.TotalSeconds;
        }
    }
}