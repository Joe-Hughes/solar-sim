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
        

        public Texture2D Texture { get { return tailSprite; } }

        public Tail(string planetName, int planetRadius, Texture2D sprite, int? fadeDegree, GraphicsDevice gd)
        {
            this.planetName = planetName;
            this.planetRadius = planetRadius;
            this.tailSprite = sprite;
            this.fadeDegree = fadeDegree;
            this.screenCenter = Game1.GetCenter(gd);
        }

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
                    if (isPositionOutOfBounds(tailPositions[i]))
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
        private bool isPositionOutOfBounds(Vector2 position)
        {
            if (fadeDegree == null)
            {
                return false;
            }
            Vector2 radiusLength = new Vector2(planetRadius);
            Vector2 planetMiddlePosition = Vector2.Add(position, radiusLength) - screenCenter;
            Vector2 comparisonMiddlePosition = Vector2.Add(tailPositions[0], radiusLength) - screenCenter;
            double planetAngle = Math.Abs(Math.Round((double)Planet.VectorToAngle(planetMiddlePosition)));
            double comparisonAngle = Math.Abs(Math.Round((double)Planet.VectorToAngle(comparisonMiddlePosition)));

            if (comparisonAngle - planetAngle <= fadeDegree)
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
