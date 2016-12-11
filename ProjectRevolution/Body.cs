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
        protected Vector2 spritePosition;
        protected double mass; // enhet: kilogram
        protected string name;
        protected internal int radius; // Texturens radie, enhet: pixlar
        protected Texture2D texture;
        // Stjärnor behöver inte en egen klass och definieras därför endast genom denna bool.
        // Om man däremot skapar en planet falsifieras denna variabel i konstrukorn.
        protected bool isStar = true;
        

        // Använd en planet som referenspunkt för att få fram meter per positionsenhet.
        // Alltså (planetens avstånd från solen i enheter)/(planetens avstånd från stolen i meter)
        // Planet: Nepunus
        public static double scaleMultiplier = Game1.referenceDistanceInMeters / Game1.referenceDistanceInUnits;

        public static double timeSpeed;

        // Newtons konstant, gäller för alla kroppar med massa
        // enhet: Nm^2/kg^2
        protected double gravConstant = 6.67408 * Math.Pow(10, -11);

        public Vector2 Position { get { return position; } }
        public Vector2 SpritePosition { get { return spritePosition; } }
        public double Mass { get { return mass; } }
        public Texture2D Texture { get { return texture; } }
        public string Name { get { return name; } set { this.name = value; } }
        public double Radius { get { return radius; } }
        public bool IsStar { get { return isStar; } }
        public double ScaleMultiplier { get { return scaleMultiplier; } }
        public double TimeSpeed { get { return timeSpeed; } }
        

        // Denna konstruktor används för stjärnor
        public Body(double mass, string name, Texture2D texture, GraphicsDeviceManager graphics)
        {
            this.mass = mass;
            this.radius = texture.Width / 2;
            this.name = name;
            this.texture = texture;

            // Sätter stjärnan i mitten av skärmen genom att ta skärmstorleken och dela på två
            this.position = Game1.GetCenter(graphics);
            this.spritePosition = Vector2.Subtract(position, new Vector2(radius));
        }

        // Statisk Overload-funktion för när man behöver veta distansen mellan en viss punkt och en kropp.
        // enhet: positioner (INTE meter)
        public static double DetermineDistance(Body body, Body otherBody)
        {
            double distance = Vector2.Distance(body.position, otherBody.position);
            return distance;
        }

        public static bool DetectCollision(List<Body> bodies)
        {
            // Kollar om det finns några planeter som är tillräckligt nära för att kollidera
            foreach (Body body in bodies)
            {
                foreach (Body otherBody in bodies)
                {
                    if (body != otherBody)
                    {
                        double collisionDistance = 142984;
                        double distance = Body.DetermineDistance(body, otherBody) * scaleMultiplier;
                        if (distance < collisionDistance)

                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void UpdateTimeSpeed(int speed)
        {
            double defaultValue = 0.25 * Math.Pow(10, 6);
            if (speed == 0)
            {
                return;
            }
            else if (speed == 1)
            {
                Body.timeSpeed = defaultValue;
            }
            else
            {
                Body.timeSpeed = defaultValue * (3 * speed);
            }
        }
    }
}
