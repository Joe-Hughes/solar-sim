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

        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public double Acceleration { get { return acceleration; } }
        public double Force { get { return force; } }
        public double Speed
        {
            get { return Math.Sqrt(Math.Pow(velocity.X * scaleMultiplier, 2) + Math.Pow(velocity.Y * scaleMultiplier, 2)); }
            set
            {
                double V = value / scaleMultiplier;
                Vector2 normVelocity = this.velocity;
                normVelocity.Normalize();
                this.velocity.X = Convert.ToSingle(V) * normVelocity.X;
                this.velocity.Y = Convert.ToSingle(V) * normVelocity.Y;
            }
        }
        
        public Tail Tail { get { return tail; } }
        public Planet(double mass, string name, double distanceFromStar, double positionAngle, 
            double initialVelocity, Texture2D texture, Texture2D tailTexture, GraphicsDeviceManager graphicsDevice)
            : base(mass, name, texture, graphicsDevice)
        {
            this.isStar = false;
            tail = new Tail(this.name, texture.Width/2, tailTexture, 140, graphicsDevice); // Tar bort tails efter 140 grader

            // Tar en given vinkel och avstånd från stjärnan och placerar planeten på den platsen.
            Vector2 angleVector = AngleToVector(positionAngle);
            // Konverterar till meter och förlänger vektorn med korrekt skala givet distansen
            double distance = distanceFromStar * Math.Pow(10, 9) / scaleMultiplier;
            angleVector = Vector2.Multiply(angleVector, Convert.ToSingle(distance));

            // Positionerar planeten med hänsyn till solsystemets mittpunkt
            Vector2 initPosition = Vector2.Add(Game1.GetCenter(graphicsDevice), angleVector);
            this.position = initPosition;

            // Skapar en vektor som har en riktning enligt velocityAngle och längd enligt initialVelocity
            Vector2 velocityVector = AngleToVector(positionAngle - 90);
            this.velocity = Vector2.Multiply(velocityVector, Convert.ToSingle((initialVelocity * 1000) / scaleMultiplier));
        }

        public void UpdateVelocityAndPosition(List<Body> bodies, double totalSecondsSinceUpdate)
        {
            // Beräknar kraften som verkar från varje kropp.
            // Utifrån detta kan en hastighetsvektor beräknas med avseende på massan och tiden sedan den senaste uppdateringen
            // Dessa vektorer adderas sedan ihop för att beräkna vart planeten bör positioneras denna updatering
            Vector2 velocityVector = new Vector2();
            foreach (Body otherBody in bodies)
            {
                if (otherBody != this)
                {
                    Vector2 direction = Vector2.Subtract(otherBody.Position, this.position);
                    // Normaliserar riktningen till ett trigonometriskt värde i enhetscirkeln
                    direction.Normalize();

                    // gravitationslagen F = G * (m*M / r^2)
                    force = this.gravConstant * ((this.mass * otherBody.Mass) / Math.Pow(DetermineDistance(this, otherBody) * scaleMultiplier, 2));
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

            this.position = Vector2.Add(this.position, Vector2.Multiply(velocity, Convert.ToSingle(totalSecondsSinceUpdate * timeSpeed)));
            this.spritePosition = Vector2.Subtract(position, new Vector2(radius));

            speed = velocity.Length() * scaleMultiplier;
            acceleration = (speed - oldSpeed) / (totalSecondsSinceUpdate * timeSpeed);
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
            //don't ask
            degrees = (degrees > 0 ? degrees : (2 * Math.PI + degrees)) * 360 / (2 * Math.PI);
            return Convert.ToSingle(degrees);
        }
    }
}