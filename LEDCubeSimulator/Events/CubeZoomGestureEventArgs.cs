using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LEDCube.Simulator.WPF.Events
{
    class CubeZoomGestureEventArgs : CubeGestureEventArgs
    {
        public CubeZoomGestureEventArgs()
        {

        }

        public Point ZoomPosition { get; set; }
        public double ZoomFactor { get; set; }
        public Point ZoomPositionScaled { get; set; }
    }
}
