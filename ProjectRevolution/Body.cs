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
        protected internal double radius; // Texturens radie, enhet: pixlar
        protected Texture2D texture;
        // Stjärnor behöver inte en egen klass och definieras därför endast genom denna bool.
        // Om man däremot skapar en planet falsifieras denna variabel i konstrukorn.
        protected bool isStar = true;

        protected Menu menu;
        

        // Använd jorden som referenspunkt för att få fram meter per positionsenhet. Alltså (Neptunus avstånd från solen i enheter)/(Neptunus avstånd från stolen i meter)
        private static double referenceDistanceInUnits = 360;
        private static double referenceDistanceInMeters = 4495.1 * Math.Pow(10, 9);
        public static double scaleMultiplier = referenceDistanceInMeters / referenceDistanceInUnits;
        protected double timeSpeed = 5000000;

        // Newtons konstant, gäller för alla kroppar med massa
        // enhet: Nm^2/kg^2
        protected double gravConstant = 6.67408 * Math.Pow(10, -11);

        public Vector2 Position { get { return position; } }
        public double Mass { get { return mass; } }
        public Texture2D Texture { get { return texture; } }
        public string Name { get { return name; } set { this.name = value; } }
        public double Radius { get { return radius; } }
        public bool IsStar { get { return isStar; } }
        public double ScaleMultiplier { get { return scaleMultiplier; } }
        public Menu Menu { get { return menu; } }
        

        // Denna konstruktor används för stjärnor
        public Body(double mass, string name, Texture2D texture, GraphicsDeviceManager graphics, SpriteFont font)
        {
            this.mass = mass;
            this.radius = texture.Width / 2;
            this.name = name;
            this.texture = texture;
            this.menu = new Menu(this, graphics, font);

            // Sätter stjärnan i mitten av skärmen genom att ta skärmstorleken och dela på två
            this.position.X = Convert.ToSingle(GetCenter(graphics.GraphicsDevice).X - radius);
            this.position.Y = Convert.ToSingle(GetCenter(graphics.GraphicsDevice).Y - radius);
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
        // Statisk Overload-funktion för när man behöver veta distansen mellan en viss punkt och en kropp.
        public static double DetermineDistance(Vector2 position,Body otherBody)
        {
            // radien behöver adderas på distansen då positionen tas från det över vänstra hörnet av kroppen.
            double xDistance = (otherBody.position.X + otherBody.radius) - position.X;
            double yDistance = (otherBody.position.Y + otherBody.radius) - position.Y;

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
