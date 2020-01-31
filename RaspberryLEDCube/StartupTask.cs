using LEDCube.Animations.Animations;
using LEDCube.Animations.Animations.Cubes;
using LEDCube.Animations.Animations.Text;
using LEDCube.Animations.Animations.Trigonometry;
using LEDCube.Animations.Animations.Weather;
using LEDCube.Animations.Controllers;
using LEDCube.Animations.Helpers;
using Microsoft.IoT.Lightning.Providers;
using RaspberryLEDCube.CubeControl.Controllers;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices;

namespace RaspberryLEDCube
{
    public sealed class StartupTask : IBackgroundTask
    {
        private AnimationController _animationController;
        private LEDCubeController _cubeController;
        private BackgroundTaskDeferral _deferral;
        private PSUController _psuController;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
            }

            _psuController = new PSUController(24, 23);
            await _psuController.InitializeAsync();

            var ledController = new LEDController(CanonicalSchema.Enums.ChipSelectLines.ChipSelectPin24);
            await ledController.InitializeAsync();

            _cubeController = new LEDCubeController(ledController);
            _cubeController.Initialize();

            _animationController = new AnimationController(_cubeController, TimeSpan.FromMilliseconds(20));

            await StartLEDCubeAsync();

            _animationController.RequestAnimation<DropletWaveAnimation>(LEDCube.Animations.Enums.AnimationPriority.Normal);
            _animationController.Start();
        }

        private void ShutdownLEDCube()
        {
            if (!_psuController.IsPowerOn())
            {
                throw new InvalidOperationException("LED cube is already shutdown.");
            }

            _psuController.TurnPowerOff();
        }

        private Task StartLEDCubeAsync()
        {
            if (_psuController.IsPowerOn())
            {
                throw new InvalidOperationException("LED cube is already started.");
            }

            _cubeController.Clear();
            _psuController.TurnPowerOn();
            return _cubeController.DrawAsync();
        }
    }
}