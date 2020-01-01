using RaspberryLEDCube.CanonicalSchema.Enums;
using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspberryLEDCube.CubeControl.Controllers
{
    public class PSUController
    {
        private GpioPin _powerOkPin;
        private readonly int _powerOkPinNumber;
        private GpioPin _powerPin;
        private readonly int _powerPinNumber;
        private readonly object _powerLock;
        private bool _initialized;

        public PSUController(int powerPin, int powerOkPin)
        {
            _powerPinNumber = powerPin;
            _powerOkPinNumber = powerOkPin;
            _powerLock = new object();
            _initialized = false;
        }

        public async Task InitializeAsync()
        {
            var gpio = await GpioController.GetDefaultAsync();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                throw new UnauthorizedAccessException("GPIO controller was allready in use by another application.");
            }

            lock (_powerLock)
            {
                _powerPin = gpio.OpenPin(_powerPinNumber);
                _powerPin.Write(GpioPinValue.Low);
                _powerPin.SetDriveMode(GpioPinDriveMode.Output);

                _powerOkPin = gpio.OpenPin(_powerOkPinNumber);
                //_powerPin.Write(GpioPinValue.High);
                _powerOkPin.SetDriveMode(GpioPinDriveMode.Input);

                _initialized = true;
            }
        }

        public bool IsPowerOn()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("PSU controller not yet initialized");
            }

            return _powerPin.Read() == GpioPinValue.High;
        }

        public bool IsPowerStable()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("PSU controller not yet initialized");
            }

            return _powerOkPin.Read() == GpioPinValue.High;
        }

        public void TurnPowerOn()
        {
            lock (_powerLock)
            {
                if (!_initialized)
                {
                    throw new InvalidOperationException("PSU controller not yet initialized");
                }

                _powerPin.Write(GpioPinValue.High);
                _powerOkPin.Write(GpioPinValue.High);
            }
        }

        public void TurnPowerOff()
        {
            lock (_powerLock)
            {
                if (!_initialized)
                {
                    throw new InvalidOperationException("PSU controller not yet initialized");
                }

                _powerPin.Write(GpioPinValue.Low);
                _powerOkPin.Write(GpioPinValue.Low);
            }
        }
    }
}
