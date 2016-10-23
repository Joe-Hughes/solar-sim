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

        public Menu (Body body, Body sun, GraphicsDeviceManager graphics, SpriteFont font)
        {
            menuBackground = new Rectangle(graphics.PreferredBackBufferWidth - 324, 0, 324, graphics.PreferredBackBufferHeight);
            horizontalTextPosition = graphics.PreferredBackBufferWidth - menuBackground.Width + 10;
            this.body = body;
            this.sun = sun;
            this.font = font;

            Planet planet = body as Planet;

            this.txtBoxName = new TextBox(planet.Name, new Point(horizontalTextPosition, 10), font);
            this.txtBoxDis = new TextBox((planet.DetermineDistance(sun) * planet.ScaleMultiplier).ToString(), new Point(horizontalTextPosition, 70), font);
            this.txtBoxVel = new TextBox(planet.Speed.ToString(), new Point(horizontalTextPosition, 110), font);
            this.txtBoxAcc = new TextBox(planet.Acceleration.ToString(), new Point(horizontalTextPosition, 150), font);
            this.txtBoxForce = new TextBox(planet.Force.ToString(), new Point(horizontalTextPosition, 190), font);

            this.txtBoxes = new List<TextBox> { this.txtBoxName, this.txtBoxDis, this.txtBoxVel, this.txtBoxAcc, this.txtBoxForce};  
        }
         public Menu (Body body, GraphicsDeviceManager graphics, SpriteFont font)
        {
            this.txtBoxName = new TextBox(body.Name, new Point(horizontalTextPosition, 10), font);
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
            if (!body.IsStar)
            {

                Planet planet = body as Planet;

                txtBoxName.Text = planet.Name;
                txtBoxDis.Text = (planet.DetermineDistance(sun) * planet.ScaleMultiplier).ToString();
                txtBoxVel.Text = planet.Speed.ToString();
                txtBoxAcc.Text = planet.Acceleration.ToString();
                txtBoxForce.Text = planet.Force.ToString();
            }
            else
            {
                txtBoxName.Text = body.Name;
            }
        }

        public void PushChanges()
        {
            if(body.IsStar)
            {
                Planet planet = body as Planet;

                planet.Name = txtBoxName.Text;
            }
        }
    }
}
