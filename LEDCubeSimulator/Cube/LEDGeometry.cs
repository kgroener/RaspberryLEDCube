using LEDCube.Simulator.WPF.Cube.Sphere3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDCube.Simulator.WPF.Cube
{
    internal class LEDGeometry : SphereGeometry3D
    {
        public LEDGeometry()
        {
            Radius = 1;
            Separators = 20;
        }
    }
}
