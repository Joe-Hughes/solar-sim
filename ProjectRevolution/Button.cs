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
        private ButtonBehavior behavior;

        public Point Location { get { return buttonArea.Location; } }
        public Rectangle ButtonArea { get { return buttonArea; } }
        public int Height { get { return buttonArea.Height; } }
        public int Width { get { return buttonArea.Width; } }
        public Texture2D Texture { get { return activeTexture; } }

        public enum ButtonBehavior
        {
            Pause
        };

        public Button(Vector2 position, int width, int height, ButtonBehavior behavior, Texture2D buttonTexture, 
            Texture2D toggleTexture=null)
        {
            this.buttonArea.Width = width;
            this.buttonArea.Height = height;
            this.buttonArea.Location = position.ToPoint();
            this.behavior = behavior;
            this.texture1 = buttonTexture;
            if (toggleTexture != null)
            {
                texture2 = toggleTexture;
            }
            this.activeTexture = texture1;
        }

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

        // click for Pause behavior
        public bool CheckClickPause(MouseState mouse, bool mouseHold, bool pause)
        {
            if (!mouseHold && Game1.IsMouseInArea(mouse, buttonArea.Location, buttonArea.Height, buttonArea.Width))
            {
                pause = !pause;
                ToggleTexture();
            }
            return pause;
        }

        //Zoom button behavior
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
                    referenceDistanceInMeters = 3000 * Math.Pow(10, 9);
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
            }
            return isZoomedOut;
        }
    }
}
