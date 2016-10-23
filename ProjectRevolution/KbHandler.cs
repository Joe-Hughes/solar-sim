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
    class KbHandler
    {
        private Keys[] lastPressedKeys;

        public KbHandler()
        {
            lastPressedKeys = new Keys[0];
        }

        public void Update(Menu menu)
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            //check if any of the previous update's keys are no longer pressed
            foreach (Keys key in lastPressedKeys)
            {
                if (!pressedKeys.Contains(key))
                    OnKeyUp(key);
            }

            //check if the currently pressed keys were already pressed
            foreach (Keys key in pressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                    OnKeyDown(key, menu);
            }

            //save the currently pressed keys so we can compare on the next update
            lastPressedKeys = pressedKeys;
        }

        private void OnKeyDown(Keys key, Menu menu)
        {
            if (key == Keys.Back)
            {
                menu.Selected.Text = menu.Selected.Text.Substring(0, menu.Selected.Text.Length - 1);
            }
            else if (key == Keys.Enter)
            {
                menu.PushChanges();
            }
            else
            {
                menu.Selected.Text += key.ToString();
            }
        }

        private void OnKeyUp(Keys key)
        {
            //do stuff
        }
    }
}
