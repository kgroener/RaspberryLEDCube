using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LEDCube.Simulator.WPF.Events
{
    class CubeDragGestureEventArgs : CubeGestureEventArgs
    {
        public CubeDragGestureEventArgs()
        {

        }

        public MouseButton MouseButton { get; set; }
        public Point DragStartPosition { get; set; }
        public Point DragCurrentPosition { get; set; }
        public Vector DragVelocity { get; set; }
        public bool DraggingStopped { get; set; }
    }
}
