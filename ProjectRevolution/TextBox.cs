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
    public class TextBox
    {
        //alla egenskaper av en textBox
        protected bool selected;
        protected string text;
        protected SpriteFont font;
        protected Rectangle hitbox;
        protected bool edit;

        public bool Selected { get { return selected; } set { selected = value; } }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                this.hitbox.Width = Convert.ToInt32(font.MeasureString(text).X);
                this.hitbox.Height = Convert.ToInt32(font.MeasureString(text).Y);
            }
        }
        public SpriteFont Font { get { return font; } }
        public Rectangle Hitbox { get { return hitbox; } }
        public bool Edit { get { return edit; } }

        public TextBox (string txt, Point position, SpriteFont font, bool edit)
        {
            this.text = txt;
            this.font = font;
            this.hitbox.X = position.X;
            this.hitbox.Y = position.Y;
            this.hitbox.Width = Convert.ToInt32(font.MeasureString(txt).X);
            this.hitbox.Height = Convert.ToInt32(font.MeasureString(txt).Y);
            this.edit = edit;
        }
    }
}
