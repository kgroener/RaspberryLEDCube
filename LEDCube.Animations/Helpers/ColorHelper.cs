using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LEDCube.Animations.Helpers
{
    public static class ColorHelper
    {
        public static Color DimColor(Color baseColor, Color actualColor, double factor)
        {
            return Color.FromArgb(
                    (int)(actualColor.R - (baseColor.R * factor)).Clip(0, 255),
                    (int)(actualColor.G - (baseColor.G * factor)).Clip(0, 255),
                    (int)(actualColor.B - (baseColor.B * factor)).Clip(0, 255));
        }

        public static double GetHueValue(Color color)
        {
            float min = Math.Min(Math.Min(color.R, color.G), color.B);
            float max = Math.Max(Math.Max(color.R, color.G), color.B);

            if (min == max)
            {
                return 0;
            }

            double hue;
            if (max == color.R)
            {
                hue = (color.G - color.B) / (max - min);
            }
            else if (max == color.G)
            {
                hue = 2d + ((color.B - color.R) / (max - min));
            }
            else
            {
                hue = 4d + ((color.R - color.G) / (max - min));
            }

            hue *= 60;
            if (hue < 0)
            {
                hue = hue + 360;
            }

            return hue;
        }

        public static Color HSVToColor(double h, double S, double V)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - (S * f));
                double tv = V * (1 - (S * (1 - f)));
                switch (i)
                {
                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;

                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;

                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            return Color.FromArgb(255, Clamp((R * 255.0)), Clamp((G * 255.0)), Clamp((B * 255.0)));
        }

        public static Color ShiftHue(Color color, double degrees)
        {
            double U = Math.Cos(degrees * Math.PI / 180);
            double W = Math.Sin(degrees * Math.PI / 180);

            return Color.FromArgb(
             Clamp(
               ((.299 + (.701 * U) + (.168 * W)) * color.R)
             + ((.587 - (.587 * U) + (.330 * W)) * color.G)
             + ((.114 - (.114 * U) - (.497 * W)) * color.B)),
             Clamp(
               ((.299 - (.299 * U) - (.328 * W)) * color.R)
             + ((.587 + (.413 * U) + (.035 * W)) * color.G)
             + ((.114 - (.114 * U) + (.292 * W)) * color.B)),
             Clamp(
               ((.299 - (.3 * U) + (1.25 * W)) * color.R)
             + ((.587 - (.588 * U) - (1.05 * W)) * color.G)
             + ((.114 + (.886 * U) - (.203 * W)) * color.B)));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static int Clamp(double i)
        {
            if (i < 0)
            {
                return 0;
            }

            return i > 255d ? 255 : (int)i;
        }
    }
}