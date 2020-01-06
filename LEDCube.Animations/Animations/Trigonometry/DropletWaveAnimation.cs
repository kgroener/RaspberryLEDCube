using LEDCube.Animations.Animations.Abstracts;
using LEDCube.Animations.Helpers;
using System;
using System.Drawing;
using System.Linq;

namespace LEDCube.Animations.Animations.Trigonometry
{
    public class DropletWaveAnimation : TrigonometryAnimation
    {
        private double _slowDownSpeed;
        private int _factor;

        public DropletWaveAnimation() : base()
        {
        }

        public override TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public override bool AutomaticSchedulingAllowed => true;

        protected override void PrepareInternal()
        {
            Speed = RandomNumber.GetRandomNumber(2, 5);
            _slowDownSpeed = 0;
            _factor = RandomNumber.GetRandomNumber(2, 4);
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
            var hue = (z + IterationValueX) * 100;
            var brightness = (Speed > 1 ? 0.4 : 0.4 * Speed);
            return ColorHelper.HSVToColor(hue, 1, brightness);
        }

        protected override double GetMissingAxisValue(double? x, double? y, double? z)
        {
            return (Math.Sin(
                IterationValueX
                + Math.Sqrt(
                    Math.Pow((x.Value - 0.5) * _factor, 2)
                    + Math.Pow((y.Value - 0.5) * _factor, 2)))
                + 1.0)
                / 2.0;
        }



        protected override void RequestStopInternal(TimeSpan timeout)
        {
            _slowDownSpeed = Speed / timeout.TotalSeconds;
        }

        protected override VariableAxis GetVariableAxis()
        {
            return VariableAxis.ZAxis;
        }
    }
}
