using LEDCube.Simulator.WPF.MVVM;
using System.Windows.Media;

namespace LEDCube.Simulator.WPF.Cube
{
    public class LEDGeometryData : ObservableObject
    {
        private Brush _ledColor;

        public LEDGeometryData(int offsetX, int offsetY, int offsetZ)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            OffsetZ = offsetZ;
            LEDColor = new SolidColorBrush(Colors.Gray);
        }

        public int OffsetX { get; }
        public int OffsetY { get; }
        public int OffsetZ { get; }

        public Brush LEDColor
        {
            get
            {
                return _ledColor;
            }
            set
            {
                if (value != _ledColor)
                {
                    _ledColor = value;
                    RaisePropertyChanged(nameof(LEDColor));
                }
            }
        }
    }
}
