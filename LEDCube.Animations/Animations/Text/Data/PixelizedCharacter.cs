using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static LEDCube.Animations.Animations.Text.Data.PixelCharacters;

namespace LEDCube.Animations.Animations.Text.Data
{
    public class PixelizedCharacter
    {
        public readonly static PixelColumn EmptyColumn = new PixelColumn(new[] { false, false, false, false, false, false, false, false }, Color.FromArgb(0, 0, 0));

        internal PixelizedCharacter(char c, bool compress = false) : this(c, Color.FromArgb(255, 0, 0), compress)
        {
        }

        internal PixelizedCharacter(char c, Color color, bool compress = false)
        {
            if (!Characters.ContainsKey(c))
            {
                Columns = Enumerable.Empty<PixelColumn>();
            }
            else
            {
                var columns = new List<PixelColumn>();
                var pixels = Characters[c];
                for (int x = compress ? 2 : 0; x < 8; x++)
                {
                    var column = new List<bool>();
                    for (int y = 0; y < 8; y++)
                    {
                        column.Add(pixels[y, x]);
                    }
                    columns.Add(new PixelColumn(column, color));
                }

                Columns = columns;
            }
        }

        public IEnumerable<PixelColumn> Columns { get; }

        public void SetColor(Color color)
        {
            foreach (var column in Columns)
            {
                column.Color = color;
            }
        }

        public class PixelColumn
        {
            internal PixelColumn(IEnumerable<bool> column, Color color)
            {
                Pixels = column;
                Color = color;
            }

            public Color Color { get; set; }
            public IEnumerable<bool> Pixels { get; }
        }
    }
}