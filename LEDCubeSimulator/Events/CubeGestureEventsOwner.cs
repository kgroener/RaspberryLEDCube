using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LEDCube.Simulator.WPF.Events
{
    class CubeGestureEventsOwner : CubeGestureEvents
    {
        private bool _leftMouseButtonDragBusy;
        private bool _rightMouseButtonDragBusy;

        private Point _leftMouseButtonDragStartPosition;
        private Point _rightMouseButtonDragStartPosition;

        private object _dragLock = new object();
        private readonly FrameworkElement _element;

        public CubeGestureEventsOwner(FrameworkElement element)
        {
            _element = element;
        }

        private Vector GetDragVelocity(Point a, Point b)
        {
            var dragWidth = (b.X - a.X);
            var dragHeight = (b.Y - a.Y);

            return new Vector()
            {
                X = dragWidth / _element.ActualWidth,
                Y = dragHeight / _element.ActualHeight
            };
        }

        public void HandleMouseMovedEvent(MouseEventArgs e)
        {
            lock (_dragLock)
            {
                var currentPosition = e.GetPosition(_element);

                var leftButtonState = e.LeftButton;
                var rightButtonState = e.RightButton;

                if (!_leftMouseButtonDragBusy && e.LeftButton == MouseButtonState.Pressed)
                {
                    _leftMouseButtonDragBusy = true;
                    _leftMouseButtonDragStartPosition = currentPosition;
                }
                else if (_leftMouseButtonDragBusy && e.LeftButton == MouseButtonState.Released)
                {
                    _leftMouseButtonDragBusy = false;

                    RaiseCubeDragEvent(new CubeDragGestureEventArgs()
                    {
                        MouseButton = MouseButton.Left,
                        DragStartPosition = _leftMouseButtonDragStartPosition,
                        DragCurrentPosition = currentPosition,
                        DragVelocity = GetDragVelocity(_leftMouseButtonDragStartPosition, currentPosition),
                        DraggingStopped = true,
                    });
                }

                if (!_rightMouseButtonDragBusy && e.RightButton == MouseButtonState.Pressed)
                {
                    _rightMouseButtonDragBusy = true;
                    _rightMouseButtonDragStartPosition = currentPosition;
                }
                else if (_rightMouseButtonDragBusy && e.RightButton == MouseButtonState.Released)
                {
                    _rightMouseButtonDragBusy = false;

                    RaiseCubeDragEvent(new CubeDragGestureEventArgs()
                    {
                        MouseButton = MouseButton.Right,
                        DragStartPosition = _rightMouseButtonDragStartPosition,
                        DragCurrentPosition = currentPosition,
                        DragVelocity = GetDragVelocity(_rightMouseButtonDragStartPosition, currentPosition),
                        DraggingStopped = true,
                    });
                }


                if (_leftMouseButtonDragBusy)
                {
                    RaiseCubeDragEvent(new CubeDragGestureEventArgs()
                    {
                        MouseButton = MouseButton.Left,
                        DragStartPosition = _leftMouseButtonDragStartPosition,
                        DragCurrentPosition = currentPosition,
                        DragVelocity = GetDragVelocity(_leftMouseButtonDragStartPosition, currentPosition),
                        DraggingStopped = false,
                    });
                }

                if (_rightMouseButtonDragBusy)
                {
                    RaiseCubeDragEvent(new CubeDragGestureEventArgs()
                    {
                        MouseButton = MouseButton.Right,
                        DragStartPosition = _rightMouseButtonDragStartPosition,
                        DragCurrentPosition = currentPosition,
                        DragVelocity = GetDragVelocity(_rightMouseButtonDragStartPosition, currentPosition),
                        DraggingStopped = false,
                    });
                }
            }
        }

        internal void StopDrag(Point currentPosition)
        {
            if (_leftMouseButtonDragBusy)
            {
                _leftMouseButtonDragBusy = false;

                RaiseCubeDragEvent(new CubeDragGestureEventArgs()
                {
                    MouseButton = MouseButton.Left,
                    DragStartPosition = _leftMouseButtonDragStartPosition,
                    DragCurrentPosition = currentPosition,
                    DragVelocity = GetDragVelocity(_leftMouseButtonDragStartPosition, currentPosition),
                    DraggingStopped = true,
                });
            }

            if (_rightMouseButtonDragBusy)
            {
                _rightMouseButtonDragBusy = false;

                RaiseCubeDragEvent(new CubeDragGestureEventArgs()
                {
                    MouseButton = MouseButton.Right,
                    DragStartPosition = _rightMouseButtonDragStartPosition,
                    DragCurrentPosition = currentPosition,
                    DragVelocity = GetDragVelocity(_rightMouseButtonDragStartPosition, currentPosition),
                    DraggingStopped = true,
                });
            }
        }

        public void HandleMouseScrollEvent(MouseWheelEventArgs e)
        {
            var zoomFactor = e.Delta < 0 ? e.Delta / 120d : 120d / e.Delta;
            var position = e.GetPosition(_element);
            var positionXscaled = position.X / _element.ActualWidth;
            var positionYscaled = position.Y / _element.ActualHeight;

            RaiseCubeZoomEvent(new CubeZoomGestureEventArgs()
            {
                ZoomFactor = zoomFactor,
                ZoomPosition = position,
                ZoomPositionScaled = new Point(positionXscaled, positionYscaled)
            });
        }
    }
}
