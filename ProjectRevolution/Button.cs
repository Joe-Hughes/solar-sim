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
        private buttonBehavior behavior;

        public Point Location { get { return buttonArea.Location; } }
        public Rectangle ButtonArea { get { return buttonArea; } }
        public int Height { get { return buttonArea.Height; } }
        public int Width { get { return buttonArea.Width; } }
        public Texture2D Texture
        {
            get
            {
                if (activeTexture == texture1)
                {
                    activeTexture = texture2;
                    return texture1;
                }
                else
                {
                    activeTexture = texture1;
                    return texture2;
                }
            }
        }

        public enum buttonBehavior
        {
            Pause
        };

        public Button(Vector2 position, int width, int height, buttonBehavior behavior, Texture2D buttonTexture, 
            Texture2D toggleTexture=null)
        {
            this.buttonArea.Width = width;
            this.buttonArea.Height = height;
            this.buttonArea.Location = position.ToPoint();
            this.behavior = behavior;
            this.texture1 = buttonTexture;
            this.texture2 = buttonTexture;
            //if (toggleTexture != null)
            //{
            //    texture2 = toggleTexture;
            //}
        }

        // click for Pause behavior
        public bool CheckClick(MouseState mouse, bool mouseHold, bool pause)
        {
            if (!mouseHold && Game1.IsMouseInArea(mouse, buttonArea.Location, buttonArea.Height, buttonArea.Width))
            {
                pause = !pause;
            }
            return pause;
        }
    }
}
