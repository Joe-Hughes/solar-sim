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

        public bool CheckClickZoom(MouseState mouse, bool mouseHold, bool isZoomedOut , double referenceDistanceInUnits)
        {
            if (!mouseHold && Game1.IsMouseInArea(mouse, buttonArea.Location, buttonArea.Height, buttonArea.Width))
            {
                double referenceDistanceInMeters;

                if (isZoomedOut)
                {
                    referenceDistanceInMeters = 227.9 * Math.Pow(10, 9);
                    Body.scaleMultiplier = referenceDistanceInMeters / referenceDistanceInUnits;
                    isZoomedOut = false;
                }
                else
                {
                    referenceDistanceInMeters = 4433.5 * Math.Pow(10, 9);
                    Body.scaleMultiplier = referenceDistanceInMeters / referenceDistanceInUnits;
                    isZoomedOut = true;
                }
            }
            return isZoomedOut;
        }
    }
}
