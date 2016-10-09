using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectRevolution
{
    class TextBox
    {
        protected bool selected;
        protected string text;
        protected SpriteFont font;
        protected Rectangle hitbox;

        public string Text { get { return text; } }
        public SpriteFont Font { get { return font; } }
        public Rectangle Hitbox { get { return hitbox; } }

        public TextBox (string txt, Point position, SpriteFont font)
        {
            this.text = txt;
            this.font = font;
            this.hitbox.X = position.X;
            this.hitbox.Y = position.Y;
            this.hitbox.Width = Convert.ToInt32(font.MeasureString(txt).X);
            this.hitbox.Height = Convert.ToInt32(font.MeasureString(txt).Y);
        }
    }
}
