using LEDCube.Animations.Animations.Abstracts;
using System;
using System.Drawing;

namespace LEDCube.Animations.Animations
{
    public class SineAnimation : GeomitryAnimation
    {
        public SineAnimation() : base(VariableAxis.YAxis)
        {
        }

        public override TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public override bool AutomaticSchedulingAllowed => true;

        protected override void CleanupInternal()
        {

        }

        protected override Color GenerateColor(double x, double y, double z)
        {
            return Color.Red;
        }

        protected override double GetMissingAxisValue(double? x, double? y, double? z)
        {
            return (Math.Sin(2 * z.Value * x.Value + IterationValueX) + 1) / 2.0;
        }

        protected override void PrepareInternal()
        {
            Speed = 5;
        }

        internal override void RequestStopInternal(TimeSpan timeout)
        {
        }
    }
}
