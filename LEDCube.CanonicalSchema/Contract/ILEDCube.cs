using System.Threading.Tasks;
using System.Drawing;

namespace LEDCube.CanonicalSchema.Contract
{
    public interface ILEDCube
    {
        /// <summary>
        /// Sets the color at x,y,z coordinates
        /// </summary>
        /// <param name="x">value between 0-1 for the x coordinate on the led cube</param>
        /// <param name="y">value between 0-1 for the y coordinate on the led cube</param>
        /// <param name="z">value between 0-1 for the z coordinate on the led cube</param>
        /// <param name="color">Color to give the LED</param>
        void SetLEDColor(double x, double y, double z, Color color);

        /// <summary>
        /// Sets the color at the absolute x,y,z coordinates
        /// </summary>
        /// <param name="x">value between 0-<see cref="ResolutionX"/> for the x coordinate on the led cube</param>
        /// <param name="y">value between 0-<see cref="ResolutionY"/> for the y coordinate on the led cube</param>
        /// <param name="z">value between 0-<see cref="ResolutionZ"/> for the z coordinate on the led cube</param>
        /// <param name="color">Color to give the LED</param>
        void SetLEDColorAbsolute(int x, int y, int z, Color color);

        /// <summary>
        /// Gets the color at x,y,z coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Color GetLEDColor(double x, double y, double z);

        /// <summary>
        /// Gets the color at the absolute x,y,z coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Color GetLEDColorAbsolute(int x, int y, int z);

        /// <summary>
        /// Sets all leds to the specified color
        /// </summary>
        /// <param name="fillColor"></param>
        /// <returns></returns>
        void Fill(Color fillColor);

        int ResolutionX { get; }
        int ResolutionY { get; }
        int ResolutionZ { get; }

        void Clear();
    }
}
