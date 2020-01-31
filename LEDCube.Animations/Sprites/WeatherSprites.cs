using LEDCube.Animations.Mathematics;
using LEDCube.Animations.Sprites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.Sprites
{
    internal class WeatherSprites
    {
        private const bool _ = false;

        private const bool X = true;

        private static readonly ReadOnlyDictionary<string, Sprite3D> Sprites = new ReadOnlyDictionary<string, Sprite3D>(new Dictionary<string, Sprite3D>()
        {
            { "Cloud", new Sprite3D(new bool[,,]
                {
                    {
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                    {
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,X,X,X,_,},
                        {_,_,X,X,X,X,X,_,},
                        {_,_,X,X,X,X,X,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                    {
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,X,X,X,_,},
                        {_,X,X,X,X,X,X,X,},
                        {_,X,X,X,X,X,X,X,},
                        {_,_,X,X,X,X,X,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                    {
                        {_,_,_,_,X,X,_,_,},
                        {_,X,X,X,X,X,X,_,},
                        {X,X,X,X,X,X,X,X,},
                        {X,X,X,X,X,X,X,X,},
                        {_,X,X,X,X,X,X,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                    {
                        {_,_,_,_,X,X,_,_,},
                        {_,X,X,X,X,X,X,_,},
                        {X,X,X,X,X,X,X,X,},
                        {X,X,X,X,X,X,X,X,},
                        {_,X,X,X,X,X,X,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                    {
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,X,X,X,_,},
                        {_,X,X,X,X,X,X,X,},
                        {_,X,X,X,X,X,X,X,},
                        {_,_,X,X,X,X,X,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                    {
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,X,X,X,_,},
                        {_,_,X,X,X,X,X,_,},
                        {_,_,X,X,X,X,X,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                    {
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                        {_,_,_,_,_,_,_,_,},
                    },
                }, Color.FromArgb(40,40,40))
            }
        });

        public static Sprite3D GetSprite(string spriteID)
        {
            return Sprites[spriteID];
        }
    }
}