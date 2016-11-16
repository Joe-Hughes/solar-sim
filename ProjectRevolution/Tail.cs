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
    // TODO dra linjer mellan punkterna för en jämnare svans
    class Tail
    {
        string planetName;
        int planetRadius;
        List<Vector2> tailPositions = new List<Vector2>();
        Texture2D tailSprite;
        // antalet grader runt stjärnan som man vill att det ska kvarstå en tail
        // Om null försvinner dem aldrig
        int? fadeDegree;
        Vector2 screenCenter;
        double lastAngleDifference;
        

        public Texture2D Texture { get { return tailSprite; } }

        public Tail(string planetName, int planetRadius, Texture2D sprite, int? fadeDegree, GraphicsDevice gd)
        {
            this.planetName = planetName;
            this.planetRadius = planetRadius;
            this.tailSprite = sprite;
            this.fadeDegree = fadeDegree;
            this.screenCenter = Game1.GetCenter(gd);
        }

        // Lägger till en planetens position i svansens historik
        public void AddTailPosition(Planet planet)
        {
            tailPositions.Add(planet.Position);
        }

        // Uppdaterar listan tailPositions och returnerar den
        public List<Vector2> GetTailPositions()
        {
            // Tar bort positioner som inte uppnär kravet för grad
            if (tailPositions.Count > 0)
            {
                for (int i = tailPositions.Count - 1; i >= 0; i--)
                {
                    if (isPositionOutOfBounds(tailPositions[i], i))
                    {
                        tailPositions.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return tailPositions;
        }

        // Kollar om en Vector2-position är inom det specifierade gradintervallet.
        // OBS: fungerar endast upp till 179 grader!!!! TODO: fixa det
        private bool isPositionOutOfBounds(Vector2 position, int iteration)
        {
            // Om ingen fadeDegree angetts tas svansen aldrig bort.
            // Hade varit bättre att ta bort efter 360 grader men funktionen är för nuvarande begränsad till 179.
            if (fadeDegree == null)
            {
                return false;
            }

            Vector2 radiusLength = new Vector2(planetRadius);
            // Planetens position i relation till skärmens mitt.
            Vector2 planetMiddlePosition = Vector2.Add(position, radiusLength) - screenCenter;
            // Positionen av den tidigast sparade svanspositionen i relation till skärmens mitt
            Vector2 comparisonMiddlePosition = Vector2.Add(tailPositions[0], radiusLength) - screenCenter;

            // Båda graderna beräknas trigonometriskt utifrån deras position runt stjärnan.
            float planetAngle = Planet.VectorToAngle(planetMiddlePosition);
            float comparisonAngle = Planet.VectorToAngle(comparisonMiddlePosition);

            // Beräknar den absoluta skillnaden mellan de två graderna.
            // Källa: http://gamedev.stackexchange.com/a/4472
            double angleDifference = Math.Ceiling(180 - Math.Abs(Math.Abs(comparisonAngle - planetAngle) - 180));

            // Om det är första gången funktionen körs likställs graderna då de beskriver samma plats.
            if (iteration == tailPositions.Count - 1)
            {
                lastAngleDifference = angleDifference;
            }

            if (lastAngleDifference > angleDifference)
            {
                return true;
            }

            if (angleDifference < fadeDegree)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}