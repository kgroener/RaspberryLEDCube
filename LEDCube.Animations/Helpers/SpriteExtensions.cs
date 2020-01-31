using LEDCube.Animations.Mathematics;
using LEDCube.Animations.Models;
using LEDCube.Animations.Sprites;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LEDCube.Animations.Helpers
{
    internal static class SpriteExtensions
    {
        public static void DrawSprite(this ILEDCube cube, Sprite3D sprite, double scale = 1, Coordinate offset = null, RotationMatrix matrix = null)
        {
            Sprite3D.Draw(cube, sprite, scale, offset, matrix);
        }

        public static void DrawSpriteAbsolute(this ILEDCube cube, Sprite3D sprite, AbsoluteCoordinate offset = null)
        {
            Sprite3D.DrawAbsolute(cube, sprite, offset);
        }
    }
}