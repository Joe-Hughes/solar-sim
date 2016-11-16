﻿using System;
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
                txtBoxForce.Text = Math.Round(planet.Force, 2).ToString();
            }
            else
            {
                txtBoxName.Text = body.Name;
            }
        }

        public void PushChanges()
        {
            if (!this.body.IsStar)
            {
                Planet planet = body as Planet;

                planet.Speed = ConvertDisplayToDouble(txtBoxVel);
            }
        }

        //Konverterar från textrepresentationen av ett nummer (t.ex. 2,23E+4) till en double
        private double ConvertDisplayToDouble (TextBox txtBox)
        {
            string txt = txtBox.Text;

            double displayValue = 0;

            if (txt.Contains("E"))
            {
                String[] strValues = txt.Split('E');
                double[] numbValues = new double[2];
                if (double.TryParse(strValues[0], out numbValues[0]) && double.TryParse(strValues[1], out numbValues[1]))
                {
                    displayValue = numbValues[0] * Math.Pow(10, numbValues[1]);
                }
            }
            else
            {
                double.TryParse(txt, out displayValue);
            }
            return displayValue;
        }
    }
}
