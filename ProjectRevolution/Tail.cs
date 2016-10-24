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
        List<Vector2> tailPositions = new List<Vector2>();
        Texture2D tailSprite;
        int? fadeDegree;

        public Tail(Texture2D sprite, int? fadeDegree)
        {
            this.tailSprite = sprite;
            this.fadeDegree = fadeDegree;
        }

        public void AddTailPosition(Planet planet)
        {
            tailPositions.Add(planet.Position);
        }
        public List<Vector2> GetTailPositions()
        {

        }
        private bool isPositionWithinBounds(Vector2 position)
        {
            if (fadeDegree == null)
            {
                return true;
            }
            float planetAngle = Planet.VectorToAngle(tailPositions.Last());
            float comparisonAngle = Planet.VectorToAngle(position);
            if ((comparisonAngle - planetAngle) <= fadeDegree)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
