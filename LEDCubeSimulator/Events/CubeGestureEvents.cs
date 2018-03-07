using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LEDCube.Simulator.WPF.Events
{
    internal abstract class CubeGestureEvents
    {
        public event EventHandler<CubeDragGestureEventArgs> Dragged;
        public event EventHandler<CubeZoomGestureEventArgs> Zoom;

        protected void RaiseCubeDragEvent(CubeDragGestureEventArgs e) => Dragged?.Invoke(this, e);
        protected void RaiseCubeZoomEvent(CubeZoomGestureEventArgs e) => Zoom?.Invoke(this, e);

    }
}
