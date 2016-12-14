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
    class Button
    {
        private Rectangle buttonArea;
        private Texture2D texture1;
        private Texture2D texture2;
        private Texture2D activeTexture;
        private int simulationSpeed;

        public Point Location { get { return buttonArea.Location; } }
        public Rectangle ButtonArea { get { return buttonArea; } }
        public int Height { get { return buttonArea.Height; } }
        public int Width { get { return buttonArea.Width; } }
        public Texture2D Texture { get { return activeTexture; } }

        public Button(Vector2 position, int width, int height, Texture2D buttonTexture, 
            Texture2D toggleTexture=null)
        {
            this.buttonArea.Width = width;
            this.buttonArea.Height = height;
            this.buttonArea.Location = position.ToPoint();
            this.texture1 = buttonTexture;
            if (toggleTexture != null)
            {
                texture2 = toggleTexture;
            }
            this.activeTexture = texture1;
        }

        // Sekundär constructor för att skapa hastighetsknappar, lagrar vilken hastighet (0-3) knappen aktiverar.
        public Button(Vector2 position, int width, int height, int simulationSpeed, Texture2D buttonTexture,
    Texture2D toggleTexture = null)
        {
            this.buttonArea.Width = width;
            this.buttonArea.Height = height;
            this.buttonArea.Location = position.ToPoint();
            this.simulationSpeed = simulationSpeed;
            this.texture1 = buttonTexture;
            if (toggleTexture != null)
            {
                texture2 = toggleTexture;
            }
            this.activeTexture = texture1;
        }

        // Ger upphov till att byta textur efter att knappen har tryckts.
        // Om texture2 inte angivits byter inte knappen textur
        public void ToggleTexture()
        {
            if (texture2 != null)
            {
                if (activeTexture == texture1)
                {
                    activeTexture = texture2;
                }
                else
                {
                    activeTexture = texture1;
                }
            }
        }

        // Klick-metod för hastighet
        public int CheckClick(MouseState mouse, bool mouseHold, int currentSimulationSpeed)
        {
            if (!mouseHold && Game1.IsMouseInArea(mouse, buttonArea.Location, buttonArea.Height, buttonArea.Width))
            {
                return this.simulationSpeed;
            }
            else
            {
                return currentSimulationSpeed;
            }
        }

        // Klick-metod för zoom
        public bool CheckClickZoom(MouseState mouse, bool mouseHold, bool isZoomedOut , double referenceDistanceInUnits, List<Planet> planets, GraphicsDeviceManager graphicsDeviceManager)
        {
            //Kollar om mus är i arean av knappen
            if (!mouseHold && Game1.IsMouseInArea(mouse, buttonArea.Location, buttonArea.Height, buttonArea.Width))
            {
                double referenceDistanceInMeters;
                double newScaleMultiplier;
                Vector2 center = Game1.GetCenter(graphicsDeviceManager);

                //kollar om det är in eller ut zoomat när knappen trycks
                if (isZoomedOut)
                {
                    referenceDistanceInMeters = 227.9 * Math.Pow(10, 9);
                    newScaleMultiplier = referenceDistanceInMeters / referenceDistanceInUnits;
                    
                    //Går igenom alla planter och ändrar deras positioner och hastigheter med den nya skalan
                    foreach (Planet planet in planets)
                    {
                        float differenceX = planet.Position.X - center.X;
                        float differenceY = planet.Position.Y - center.Y;

                        planet.Velocity = new Vector2(Convert.ToSingle((planet.Velocity.X * Body.scaleMultiplier) / newScaleMultiplier),
                            Convert.ToSingle((planet.Velocity.Y * Body.scaleMultiplier) / newScaleMultiplier));
                        
                        planet.Position = new Vector2(Convert.ToSingle(center.X + (differenceX * Body.scaleMultiplier) / newScaleMultiplier), 
                            Convert.ToSingle(center.Y + (differenceY * Body.scaleMultiplier) / newScaleMultiplier));
                    }
                    isZoomedOut = false;
                }
                else
                {
                    referenceDistanceInMeters = 4433.5 * Math.Pow(10, 9);
                    newScaleMultiplier = referenceDistanceInMeters / referenceDistanceInUnits;
                    foreach (Planet planet in planets)
                    {
                        float differenceX = planet.Position.X - center.X;
                        float differenceY = planet.Position.Y - center.Y;

                        planet.Velocity = new Vector2(Convert.ToSingle((planet.Velocity.X * Body.scaleMultiplier) / newScaleMultiplier),
                            Convert.ToSingle((planet.Velocity.Y * Body.scaleMultiplier) / newScaleMultiplier));

                        planet.Position = new Vector2(Convert.ToSingle(center.X + (differenceX * Body.scaleMultiplier) / newScaleMultiplier),
                            Convert.ToSingle(center.Y + (differenceY * Body.scaleMultiplier) / newScaleMultiplier));
                    }
                    isZoomedOut = true;
                }
                Body.scaleMultiplier = newScaleMultiplier;
                foreach (Planet planet in planets)
                {
                    Tail.ClearTailPositions(planet);
                }
            }
            return isZoomedOut;
        }
    }
}
