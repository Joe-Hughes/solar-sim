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
    class Planet : Body
    {
        private Vector2 velocity; // Hastigheten i enheter/updatering
        private double acceleration; // Enhet: meter/(sekund^2)
        private double force; // Enhet: newton
        private double speed; // Hastighet i SI-enheter alltså meter/sekund
        private double oldSpeed = 0; // Används för att beräkna delta-hastighet
        private Tail tail;

        public Vector2 Velocity { get { return velocity; } }
        public double Acceleration { get { return acceleration; } }
        public double Force { get { return force; } }
        public double Speed { get { return speed; } }
        public Tail Tail { get { return tail; } }

        public Planet(double mass, string name, double distanceFromStar, double positionAngle, double velocityAngle, 
            double initialVelocity, Texture2D texture, Texture2D tailTexture, Body star, GraphicsDevice graphicsDevice)
            : base(mass, name, texture, graphicsDevice)
        {
            this.isStar = false;
            tail = new Tail(this.name, texture.Width/2, tailTexture, 175, graphicsDevice); // Tar bort tails efter 340 grader

            // Tar en given vinkel och avstånd från stjärnan och placerar planeten på den platsen.
            Vector2 angleVector = AngleToVector(positionAngle);
            Console.WriteLine("AngleVector: " + angleVector);
            // Konverterar till meter och förlänger vektorn med korrekt skala givet distansen
            angleVector = Vector2.Multiply(angleVector, Convert.ToSingle((distanceFromStar * Math.Pow(10,9)) / scaleMultiplier));
            Console.WriteLine("Multiplied: " + angleVector);

            double posX = Game1.GetCenter(graphicsDevice).X - radius + angleVector.X;
            double posY = Game1.GetCenter(graphicsDevice).Y - radius + angleVector.Y;
            Vector2 initPosition = new Vector2(Convert.ToSingle(posX), Convert.ToSingle(posY));
            this.position = initPosition;

            // Skapar en vektor som har en riktning enligt velocityAngle och längd enligt initialVelocity
            Vector2 velocityVector = AngleToVector(velocityAngle);
            this.velocity = Vector2.Multiply(velocityVector, Convert.ToSingle((initialVelocity * 1000) / scaleMultiplier));
        }

        //Overload-funktion för att skapa en planet med given velocity och position istället för att beräkna med vinklar och distans från solen
        public Planet(double mass, string name, Vector2 position, Vector2 velocity,
            Texture2D texture, Body star, GraphicsDevice graphicsDevice)
            : base(mass, name, texture, graphicsDevice)
        {
            this.isStar = false;

            this.velocity = Vector2.Divide(velocity, (float)scaleMultiplier);

            double posX = Game1.GetCenter(graphicsDevice).X - radius + position.X;
            double posY = Game1.GetCenter(graphicsDevice).Y - radius + position.Y;
            this.position = new Vector2(Convert.ToSingle(posX), Convert.ToSingle(posY));
        }

        // Beräknar den resulterande vektorn av alla andra kroppars krafter på planeten och flytter den till en viss position
        public void updateVelocityAndPosition(List<Body> bodies, double totalSecondsSinceUpdate)
        {
            Vector2 velocityVector = new Vector2();

            foreach (Body otherBody in bodies)
            {
                if (otherBody != this)
                {

                    // radien behöver adderas på båda distanserna då positionen tas från det över vänstra hörnet av kroppen.
                    double xDistance = (otherBody.Position.X + otherBody.radius) - (this.position.X + otherBody.radius);
                    double yDistance = (otherBody.Position.Y + otherBody.radius) - (this.position.Y + otherBody.radius);

                    Vector2 direction = new Vector2(Convert.ToSingle(xDistance), Convert.ToSingle(yDistance));
                    direction.Normalize();

                    // gravitationslagen F = G * (m*M / r^2)
                    force = this.gravConstant * ((this.mass * otherBody.Mass) / Math.Pow(DetermineDistance(otherBody) * scaleMultiplier, 2));
                    // beräkna accelerationen genom a = F / m
                    double appliedAcceleration = force / this.mass;
                    // beräkna hastighet genom v = a * t
                    double appliedSpeed = appliedAcceleration * totalSecondsSinceUpdate * timeSpeed;

                    // konvertera till ordentlig skala
                    appliedSpeed = (appliedSpeed / scaleMultiplier);

                    // Beräknar vektorn i x- och y-led genom att multiplicera riktningen med hastigheten
                    Vector2 velocity = Vector2.Multiply(direction, new Vector2((float)appliedSpeed));

                    velocityVector = Vector2.Add(velocityVector, velocity);
                }
            }

            // När alla enskilda vektorer adderats ihop uppdateras velocity och positionen och beräknas utifrån den
            this.velocity += velocityVector;

            this.position.X += velocity.X * Convert.ToSingle(totalSecondsSinceUpdate * timeSpeed);
            this.position.Y += velocity.Y * Convert.ToSingle(totalSecondsSinceUpdate * timeSpeed);

            speed = Math.Sqrt(Math.Pow(velocity.X * scaleMultiplier, 2) + Math.Pow(velocity.Y * scaleMultiplier, 2));
            acceleration = (speed - oldSpeed) / totalSecondsSinceUpdate;
            oldSpeed = speed;
        }

        // Konverterar en vinkel angiven i grader och returnerar en vektor motsvarande den platsen i enhetscirkeln.
        public static Vector2 AngleToVector(double angle)
        {
            double radians = MathHelper.ToRadians(Convert.ToSingle(angle));
            return new Vector2((float)Math.Cos(radians), -(float)Math.Sin(radians));
        }

        // Konverterar en position i enhetscirkeln till motsvarande vinkel i grader.
        public static float VectorToAngle(Vector2 vector)
        {
            double degrees = Math.Atan2(-vector.Y, vector.X);
            //double degrees = MathHelper.ToDegrees(Convert.ToSingle(radians));
            // don't ask
            degrees = (degrees > 0 ? degrees : (2 * Math.PI + degrees)) * 360 / (2 * Math.PI);
            return Convert.ToSingle(degrees);
        }
    }
}