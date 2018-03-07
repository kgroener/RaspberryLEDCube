using RaspberryLEDCube.CubeControl.Controllers;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RaspberryLEDCube
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            LEDCubeController controller = new LEDCubeController(8, 8, 8);

            await controller.InitializeAsync();

            controller.StartLEDCube();
            controller.Clear();

            while (true)
            {
                controller.Fill(new CanonicalSchema.Schema.Color3(124, 0, 0));
                controller.Draw();
                await Task.Delay(5000);

                controller.Fill(new CanonicalSchema.Schema.Color3(0, 124, 0));
                controller.Draw();
                await Task.Delay(5000);

                controller.Fill(new CanonicalSchema.Schema.Color3(0, 0, 124));
                controller.Draw();
                await Task.Delay(5000);
            }

            //_deferral.Complete();
        }
    }
}
