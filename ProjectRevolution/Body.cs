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
    // Basklass för stjärnor och planeter
    class Body
    {
        protected Vector2 position; // enhet: positioner (inte meter eller pixlar)
        protected double mass; // enhet: kilogram
        protected string name;
        protected internal double radius; // enhet: meter
        protected Texture2D texture;
        // Stjärnor behöver inte en egen klass och definieras därför endast genom denna bool.
        // Om man däremot skapar en planet falsifieras denna variabel i konstrukorn.
        protected bool isStar = true;
        // Newtons konstant, gäller för alla kroppar med massa
        // enhet: Nm^2/kg^2
        public static double scaleMultiplier = 1.06857 * Math.Pow(10, 9); // TODO: generera i kod istället för att använda ett arbiträrt nummer
        protected double timeSpeed = 840000;

        protected double gravConstant = 6.67408 * Math.Pow(10, -11);

        public Vector2 Position { get { return position; } }
        public double Mass { get { return mass; } }
        public Texture2D Texture { get { return texture; } }
        public string Name { get { return name; } }
        public double Radius { get { return radius; } }
        public bool IsStar { get { return isStar; } }

        // Denna konstruktor används för stjärnor
        public Body(double mass, double radius, GraphicsDevice graphicsDevice, string name, Texture2D texture)
        {
            this.mass = mass;
            this.radius = radius;
            this.name = name;
            this.texture = texture;

            // Sätter stjärnan i mitten av skärmen genom att ta skärmstorleken och dela på två
            this.position.X = Convert.ToSingle(GetCenter(graphicsDevice).X - radius);
            this.position.Y = Convert.ToSingle(GetCenter(graphicsDevice).Y - radius);
        }

        // returnerar distansen mellan denna och en annan kropp genom Pytagoras sats
        // enhet: positioner (INTE meter)
        public double DetermineDistance(Body otherBody)
        {
            double radius1 = this.radius;
            double radius2 = otherBody.radius;

            // radien behöver adderas på båda distanserna då positionen tas från det över vänstra hörnet av kroppen.
            double xDistance = (otherBody.position.X + radius2) - (this.position.X + radius1);
            double yDistance = (otherBody.position.Y + radius2) - (this.position.Y + radius1);

            // c = sqrt(a^2 + b^2)
            double hypotenuse = (Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2)));
            
            // om distansen hade angets i meter
            //hypotenuse *= scaleMultiplier;

            return hypotenuse;
        }

        public static Vector2 GetCenter(GraphicsDevice graphicsDevice)
        {
            Point window = graphicsDevice.PresentationParameters.Bounds.Center;
            window.X -= 162;
            Vector2 center = window.ToVector2();

            return center;
        }
    }
}
