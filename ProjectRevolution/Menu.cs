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
    class Menu
    {
        private List<TextBox> txtBoxes;

        private TextBox txtBoxName;
        private TextBox txtBoxDis;
        private TextBox txtBoxVel;
        private TextBox txtBoxAcc;
        private TextBox txtBoxForce;

        private TextBox selected;

        private Rectangle menuBackground;
        private int horizontalTextPosition;
        private Body body;
        private Body sun;
        private SpriteFont font;

        public List<TextBox> TxtBoxes { get { return txtBoxes; } }

        public TextBox TxtBoxName { get { return txtBoxName; } set { txtBoxName = value; } }
        public TextBox TxtBoxDis { get { return txtBoxDis; } set { txtBoxDis = value; } }
        public TextBox TxtBoxVel { get { return txtBoxVel; } set { txtBoxVel = value; } }
        public TextBox TxtBoxAcc { get { return txtBoxAcc; } set { txtBoxAcc = value; } }
        public TextBox TxtBoxForce { get { return txtBoxForce; } set { txtBoxName = value; } }
        public TextBox Selected { get { return selected; } set { selected = value; } }
        public Body Body { get { return body; } set { this.body = value; } }

        public Menu(Body sun, GraphicsDeviceManager graphics, SpriteFont font)
        {
            menuBackground = new Rectangle(graphics.PreferredBackBufferWidth - 324, 0, 324, graphics.PreferredBackBufferHeight);
            horizontalTextPosition = graphics.PreferredBackBufferWidth - menuBackground.Width + 10;
            this.sun = sun;
            this.font = font;

            Planet planet = body as Planet;

            this.txtBoxName = new TextBox("", new Point(horizontalTextPosition, 10), font, false);
            this.txtBoxDis = new TextBox("", new Point(horizontalTextPosition, 70), font, false);
            this.txtBoxVel = new TextBox("", new Point(horizontalTextPosition, 110), font, true);
            this.txtBoxAcc = new TextBox("", new Point(horizontalTextPosition, 150), font, true);
            this.txtBoxForce = new TextBox("", new Point(horizontalTextPosition, 190), font, false);

            this.txtBoxes = new List<TextBox> { this.txtBoxName, this.txtBoxDis, this.txtBoxVel, this.txtBoxAcc, this.txtBoxForce };
        }

        public void DrawStrings(SpriteBatch spriteBatch)
        {
            foreach(TextBox txtBox in txtBoxes)
            {
                spriteBatch.DrawString(font, txtBox.Text, txtBox.Hitbox.Location.ToVector2(), Color.Black);
            }
        }

        public void UpdateValues()
        {
            if (!this.body.IsStar)
            {

                Planet planet = body as Planet;

                txtBoxName.Text = planet.Name;
                txtBoxDis.Text = Math.Round((planet.DetermineDistance(sun) * planet.ScaleMultiplier)).ToString();
                txtBoxVel.Text = Math.Round(planet.Speed).ToString();
                txtBoxAcc.Text = Math.Round(planet.Acceleration).ToString();
                txtBoxForce.Text = Math.Round(planet.Force).ToString();
            }
            else
            {
                txtBoxName.Text = body.Name;
            }
        }

        public void PushChanges()
        {
            if(!this.body.IsStar)
            {
                Planet planet = body as Planet;

                planet.Name = txtBoxName.Text;
            }
        }
    }
}
