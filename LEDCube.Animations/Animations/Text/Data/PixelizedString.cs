using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static LEDCube.Animations.Animations.Text.Data.PixelizedCharacter;

namespace LEDCube.Animations.Animations.Text.Data
{
    public class PixelizedString : IEnumerable<PixelizedCharacter>
    {
        private readonly string _text;

        public PixelizedString(IEnumerable<PixelizedCharacter> characters)
        {
            Characters = characters;
        }

        public PixelizedString(string text, bool compress = true) : this(text, Color.FromArgb(255, 0, 0), compress)
        {
        }

        public PixelizedString(string text, Color color, bool compress = true)
        {
            _text = text;
            var characters = new List<PixelizedCharacter>();
            foreach (var c in text)
            {
                characters.Add(new PixelizedCharacter(c, color, compress));
            }

            Characters = characters;
        }

        public IEnumerable<PixelizedCharacter> Characters { get; }
        public IEnumerable<PixelColumn> Columns => Characters.SelectMany(c => c.Columns);

        public PixelizedString Concat(params PixelizedString[] other)
        {
            return new PixelizedString(this.Characters.Concat(other.SelectMany(s => s.Characters)));
        }

        public IEnumerator<PixelizedCharacter> GetEnumerator()
        {
            return Characters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Characters.GetEnumerator();
        }

        public void SetColor(Color color)
        {
            foreach (var column in Columns)
            {
                column.Color = color;
            }
        }

        public override string ToString()
        {
            return _text;
        }
    }
}