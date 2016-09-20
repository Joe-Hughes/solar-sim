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
        private double distance; // från stjärna/centrum
        private Vector2 velocity;

        private Vector2 Velocity { get { return velocity; } }
        private double DistanceFromStar { get { return distance; } }

        public Planet(double mass, double radius, GraphicsDevice graphicsDevice, Vector2 positionOffset,
            string name, Texture2D texture, Body star, Vector2 initialVelocity)
            : base(mass, radius, graphicsDevice, name, texture)
        {
            this.isStar = false;
            this.velocity.X = Convert.ToSingle((initialVelocity.X / scaleMultiplier) * timeSpeed);
            this.velocity.Y = Convert.ToSingle((initialVelocity.Y / scaleMultiplier) * timeSpeed);
            this.distance = DetermineDistance(star);

            double posX = GetCenter(graphicsDevice).X - radius + (positionOffset.X);
            double posY = GetCenter(graphicsDevice).Y - radius + (positionOffset.Y);
            this.position = new Vector2(Convert.ToSingle(posX), Convert.ToSingle(posY));
            
            
        }

        // Beräknar den resulterande vektorn av alla andra kroppars krafter på planeten och flytter den till en viss position
        public void updateVelocityAndPosition(List<Body> bodies)
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

                    // TODO prova att ta bort scale multiplier, bör ha samma resultat då man räknar i positioner istället för meter.

                    // gravitationslagen F = G * (m*M / r^2)
                    double force = this.gravConstant * ((this.mass * otherBody.Mass) / Math.Pow(DetermineDistance(otherBody) * scaleMultiplier, 2));
                    // beräkna accelerationen genom a = F / m
                    double acceleration = force / this.mass;
                    // beräkna hastighet genom v = a * t
                    double speed = acceleration * timeSpeed;

                    // konvertera till ordentlig skala
                    speed = (speed / scaleMultiplier) * timeSpeed;

                    // Beräknar vektorn i x- och y-led genom att multiplicera riktningen med hastigheten
                    float xVelocity = Convert.ToSingle(direction.X * speed);
                    float yVelocity = Convert.ToSingle(direction.Y * speed);

                    velocityVector.X += xVelocity;
                    velocityVector.Y += yVelocity;
                }
            }

            // När alla enskilda vektorer adderats ihop uppdateras velocity och positionen beräknas utifrån den
            this.velocity += velocityVector;

            this.position.X += velocity.X;
            this.position.Y += velocity.Y;

            // GAMMAL REFERENSKOD

            //Vector2 direct = new Vector2(Convert.ToSingle(disX), Convert.ToSingle(disY));
            //direct.Normalize();

            //// G * (m*M / distance^2) - enhet: F
            //// dela med massa - enhet: acceleration
            //// Gångra med tid - enhet: hastighet
            //double vel = ((((((body2mass * bodymass) / Math.Pow(distance, 2)) * G) / bodymass) * timeSpeed) / (1.68571 * Math.Pow(10, 8))) * timeSpeed;
            //float velx = body.GetVelocity().X + Convert.ToSingle(direct.X * vel);
            //float vely = body.GetVelocity().Y + Convert.ToSingle(direct.Y * vel);
            //Vector2 velVect = new Vector2(velx, vely);

            //float posx = body.GetPosition().X + velx;
            //float posy = body.GetPosition().Y + vely;
            //Vector2 posVect = new Vector2(posx, posy);

            //body.SetVelocity(velVect);
            //body.SetPosition(posVect);
        }
    }
}
