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

        private TextBox txtBoxDesName;
        private TextBox txtBoxDesDis;
        private TextBox txtBoxDesVel;
        private TextBox txtBoxDesAcc;
        private TextBox txtBoxCentriAcc;

        private TextBox selected;

        private Rectangle menuBackground;
        private int horizontalTextPosition;
        private Body body;
        private Body sun;
        private SpriteFont font;
        private TimeSpan realTimeElapsed;
        private TimeSpan simulationTimeElapsed;

        public List<TextBox> TxtBoxes { get { return txtBoxes; } }

        public TextBox TxtBoxName { get { return txtBoxName; } set { txtBoxName = value; } }
        public TextBox TxtBoxDis { get { return txtBoxDis; } set { txtBoxDis = value; } }
        public TextBox TxtBoxVel { get { return txtBoxVel; } set { txtBoxVel = value; } }
        public TextBox TxtBoxAcc { get { return txtBoxAcc; } set { txtBoxAcc = value; } }
        
        public TextBox TxtBoxDesName { get { return txtBoxDesName; } set { txtBoxDesName = value; } }
        public TextBox TxtBoxDesDis { get { return txtBoxDesDis; } set { txtBoxDesDis = value; } }
        public TextBox TxtBoxDesVel { get { return txtBoxDesVel; } set { txtBoxDesVel = value; } }
        public TextBox TxtBoxDesAcc { get { return txtBoxDesAcc; } set { txtBoxDesAcc = value; } }
        public TextBox TxtTBoxCentriAcc { get { return txtBoxCentriAcc; } set { txtBoxCentriAcc = value; } }

        public TextBox Selected { get { return selected; } set { selected = value; } }
        public Body Body { get { return body; } set { this.body = value; } }

        public Menu(Body sun, GraphicsDeviceManager graphics, SpriteFont font, TimeSpan timeElapsed)
        {
            menuBackground = new Rectangle(graphics.PreferredBackBufferWidth - 324, 0, 324, graphics.PreferredBackBufferHeight);
            horizontalTextPosition = graphics.PreferredBackBufferWidth - menuBackground.Width + 10;
            this.sun = sun;
            this.font = font;

            Planet planet = body as Planet;

            this.txtBoxDesName = new TextBox("Namn: ", new Point(horizontalTextPosition, 10), font, false);
            this.txtBoxDesDis = new TextBox("Distans till solen: ", new Point(horizontalTextPosition, 70), font, false);
            this.txtBoxDesVel = new TextBox("Hastighet: ", new Point(horizontalTextPosition, 110), font, false);
            this.txtBoxDesAcc = new TextBox("Acceleration: ", new Point(horizontalTextPosition, 150), font, false);

            this.txtBoxName = new TextBox("", new Point(horizontalTextPosition + Convert.ToInt32(font.MeasureString(txtBoxDesName.Text).X), 10), font, false);
            this.txtBoxDis = new TextBox("", new Point(horizontalTextPosition + Convert.ToInt32(font.MeasureString(txtBoxDesDis.Text).X), 70), font, false);
            this.txtBoxVel = new TextBox("", new Point(horizontalTextPosition + Convert.ToInt32(font.MeasureString(txtBoxDesVel.Text).X), 110), font, true);
            this.txtBoxAcc = new TextBox("", new Point(horizontalTextPosition + Convert.ToInt32(font.MeasureString(txtBoxDesAcc.Text).X), 150), font, false);
            this.txtBoxCentriAcc = new TextBox("", new Point(horizontalTextPosition, 190), font, true); // TODO CHANGE THIS

            this.txtBoxes = new List<TextBox>
            {
                this.txtBoxName,
                this.txtBoxDis,
                this.txtBoxVel,
                this.txtBoxAcc,
                this.txtBoxDesName,
                this.txtBoxDesDis,
                this.txtBoxDesVel,
                this.txtBoxDesAcc
            };
            
            realTimeElapsed = timeElapsed;
            simulationTimeElapsed = TimeSpan.FromMilliseconds(realTimeElapsed.TotalMilliseconds * sun.TimeSpeed);
        }

        public void DrawStrings(SpriteBatch spriteBatch, Body body)
        {
            if (body != null)
            {
                if (body.IsStar)
                {
                    spriteBatch.DrawString(font, txtBoxName.Text, txtBoxName.Hitbox.Location.ToVector2(), Color.Black);
                    spriteBatch.DrawString(font, txtBoxDesName.Text, txtBoxDesName.Hitbox.Location.ToVector2(), Color.Black);
                }

                else
                {
                    foreach (TextBox txtBox in txtBoxes)
                    {
                        spriteBatch.DrawString(font, txtBox.Text, txtBox.Hitbox.Location.ToVector2(), Color.Black);
                    }
                }
            }
        }

        public void UpdateValues()
        {
            if (!this.body.IsStar)
            {
                Planet planet = body as Planet;

                txtBoxName.Text = planet.Name;
                txtBoxDesName.Text = "Namn: ";
                txtBoxDesDis.Text = "Distans till solen: ";
                txtBoxDesVel.Text = "Hastighet: ";
                txtBoxDesAcc.Text = "Acceleration: ";

                txtBoxDis.Text = Math.Round((Body.DetermineDistance(planet, sun) * planet.ScaleMultiplier * 6.68469 * Math.Pow(10, -12)), 3).ToString() + " AU";
                txtBoxVel.Text = Math.Round(planet.Speed).ToString() + " m/s";
                txtBoxAcc.Text = Math.Round(planet.Acceleration).ToString() + " m/s^2";
                txtBoxCentriAcc.Text = (Math.Pow(planet.Acceleration, 2) / Body.DetermineDistance(planet, sun)).ToString() + " m/s^2";
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
