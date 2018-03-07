using LEDCube.Animations.Animations;
using LEDCube.Animations.Controllers;
using LEDCube.Animations.Enums;
using LEDCube.Simulator.WPF.Cube;
using LEDCube.Simulator.WPF.Events;
using LEDCube.Simulator.WPF.MVVM;
using System.Diagnostics;

namespace LEDCube.Simulator.WPF.ViewModels
{
    class LEDCubeViewModel : ObservableObject
    {
        private const double DRAG_ROTATION_FACTOR = 200;

        private readonly LEDCubeGeometryGroup _cube;
        private readonly LEDCubeController _cubeController;
        private readonly AnimationController _animationController;
        private double _horizontalRotationAngle;
        private double _verticalRotationAngle;
        private double _horizontalDraggingAngle;
        private double _verticalDraggingAngle;
        private double _zoomScale;

        public LEDCubeViewModel(CubeGestureEvents cubeGestureEvents)
        {
            _cube = new LEDCubeGeometryGroup(8, 8, 8);
            _cubeController = new LEDCubeController(_cube);
            _animationController = new AnimationController(_cubeController);

            ZoomScale = 1;
            cubeGestureEvents.Dragged += HandleCubeDraggedEvent;
            cubeGestureEvents.Zoom += HandleCubeZoomEvent;

            _animationController.Start();

#if DEBUG
            _animationController.RequestAnimation<GameOfLifeAnimation>(AnimationPriority.High);
#endif
        }

        private void HandleCubeZoomEvent(object sender, CubeZoomGestureEventArgs e)
        {
            //ZoomScale *= e.ZoomFactor;
        }

        private void HandleCubeDraggedEvent(object sender, CubeDragGestureEventArgs e)
        {
            Debug.WriteLine($"Drag from {e.DragStartPosition} to {e.DragCurrentPosition}. Velocity: {e.DragVelocity}");


            _horizontalDraggingAngle = e.DragVelocity.X * -DRAG_ROTATION_FACTOR;
            _verticalDraggingAngle = e.DragVelocity.Y * -DRAG_ROTATION_FACTOR;

            if (e.DraggingStopped)
            {
                _horizontalRotationAngle += _horizontalDraggingAngle;
                _verticalRotationAngle += _verticalDraggingAngle;
                _horizontalDraggingAngle = 0;
                _verticalDraggingAngle = 0;

                Debug.WriteLine("Drag stopped");
            }

            RaisePropertyChanged(nameof(HorizontalRotationAngle));
            RaisePropertyChanged(nameof(VerticalRotationAngle));
        }

        public LEDCubeGeometryGroup Cube
        {
            get
            {
                return _cube;
            }
        }

        public double ZoomScale
        {
            get
            {
                return _zoomScale;
            }
            set
            {
                if (value != _zoomScale)
                {
                    _zoomScale = value;
                    RaisePropertyChanged(nameof(ZoomScale));
                }
            }
        }

        public double HorizontalRotationAngle
        {
            get
            {
                return _horizontalRotationAngle + _horizontalDraggingAngle;
            }
        }

        public double VerticalRotationAngle
        {
            get
            {
                return _verticalRotationAngle + _verticalDraggingAngle;
            }
        }
    }
}
