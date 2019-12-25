using LEDCube.Simulator.WPF.Events;
using LEDCube.Simulator.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LEDCube.Simulator.WPF.Views
{
    /// <summary>
    /// Interaction logic for LEDCubeView.xaml
    /// </summary>
    public partial class LEDCubeView : UserControl
    {
        private readonly CubeGestureEventsOwner _cubeGestureEvents;

        public LEDCubeView()
        {
            InitializeComponent();

            _cubeGestureEvents = new CubeGestureEventsOwner(CubeViewPort3D);

            DataContext = new LEDCubeViewModel(_cubeGestureEvents);
        }

        private void HandleCubeMouseMove(object sender, MouseEventArgs e) =>  _cubeGestureEvents.HandleMouseMovedEvent(e);
        private void HandleCubeMouseWheel(object sender, MouseWheelEventArgs e) => _cubeGestureEvents.HandleMouseScrollEvent(e);

        private void HandleCubeMouseLeave(object sender, MouseEventArgs e) => _cubeGestureEvents.StopDrag(e.GetPosition(CubeViewPort3D));
        private void HandleCubeStylusLeave(object sender, StylusEventArgs e) => _cubeGestureEvents.StopDrag(e.GetPosition(CubeViewPort3D));

        private void HandleCubeMouseUp(object sender, MouseButtonEventArgs e) => _cubeGestureEvents.StopDrag(e.GetPosition(CubeViewPort3D));
    }
}
