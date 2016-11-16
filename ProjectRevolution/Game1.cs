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
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D planetSprite;
        Texture2D starSprite;
        Texture2D tailSprite;
        Texture2D menuSprite;
        Texture2D markerSprite;
        Texture2D txtBoxSprite;
        Texture2D pauseBtnSprite;
        Texture2D playBtnSprite;
        Rectangle pauseBtn;

        List<Body> bodies = new List<Body>();
        List<Planet> planets = new List<Planet>();

        bool mouseHold = false;
        //bool shiftMouseHold = false;
        //Vector2 initialPos;
        //Vector2 dragVector;

        bool takeKeyboardInput = false;
        Dictionary<Planet, List<Vector2>> spriteCache = new Dictionary<Planet, List<Vector2>>();
        int spriteCacheSize = 3000;
        bool pause = false;

        SpriteFont arial;
        Rectangle menuBackground;
        Menu menu;
        //TextBox selectedTxtBox;

        Body selectedBody;
        bool isSelectedBody = false;

        KbHandler kbHandler = new KbHandler();

        double oldTotalUpdateTime = 0;
        double oldTotalDrawTime = 0;
        int wait = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;    // unlocks framerate
            graphics.SynchronizeWithVerticalRetrace = false;    // disables Vsync
            graphics.PreferredBackBufferWidth = 1366;  // set this value to the desired width of your window. Standard: 1366
            graphics.PreferredBackBufferHeight = 700;   // set this value to the desired height of your window. Standard: 768
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            menuBackground = new Rectangle(graphics.PreferredBackBufferWidth - 324, 0, 324, graphics.PreferredBackBufferHeight);
            pauseBtn = new Rectangle(graphics.PreferredBackBufferWidth - 324, graphics.PreferredBackBufferHeight - 50, 100, 50);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            // Förminskar pixelering på icke-raka linjer
            graphics.PreferMultiSampling = true;

            //this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f/60.0f);
            //graphics.GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.One;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loads all the sprites needed
            starSprite = this.Content.Load<Texture2D>(@"STAR");
            planetSprite = this.Content.Load<Texture2D>(@"earth");
            Texture2D earthSprite = this.Content.Load<Texture2D>(@"earth");
            Texture2D marsSprite = this.Content.Load<Texture2D>(@"mars");
            Texture2D mercurySprite = this.Content.Load<Texture2D>(@"mercury");
            Texture2D jupiterSprite = this.Content.Load<Texture2D>(@"jupiter");
            Texture2D neptunusSprite = this.Content.Load<Texture2D>(@"neptunus");
            Texture2D saturnusSprite = this.Content.Load<Texture2D>(@"saturnus");
            Texture2D uranusSprite = this.Content.Load<Texture2D>(@"uranus");

            tailSprite = this.Content.Load<Texture2D>(@"TAIL");
            menuSprite = this.Content.Load<Texture2D>(@"MENU");
            markerSprite = this.Content.Load<Texture2D>(@"MARKER");
            txtBoxSprite = this.Content.Load<Texture2D>(@"TXTBOX");
            pauseBtnSprite = this.Content.Load<Texture2D>(@"PauseBtn");
            playBtnSprite = this.Content.Load<Texture2D>(@"PlayBtn");
            arial = this.Content.Load<SpriteFont>("StandardArial");

            

            // Skapar kroppar och lägger in dem i systemet
            Body sun = new Body(1.9885 * Math.Pow(10, 30), "Sun", starSprite, graphics);
            bodies.Add(sun);

            //Planet mercury = new Planet(0.330 * Math.Pow(10, 24), "Mercury", 57.9, 90, 0, 47.4, mercurySprite, sun, graphics);
            //bodies.Add(mercury);

            //Planet earth = new Planet(5.9724 * Math.Pow(10, 24), "Earth", 149.6, 90, 0, 29.8, earthSprite, sun, graphics);
            //bodies.Add(earth);

            //Planet mars = new Planet(0.64171 * Math.Pow(10, 24), "Mars", 227.9, 90, 0, 24.1, marsSprite, sun, graphics);
            //bodies.Add(mars);

            Planet jupiter = new Planet(1898 * Math.Pow(10, 24), "Jupiter", 778.6, 90, 0, 13.1, jupiterSprite, sun, graphics);
            bodies.Add(jupiter);

            Planet saturn = new Planet(568 * Math.Pow(10, 24), "Saturn", 1433.5, 90, 0, 9.7, saturnusSprite, sun, graphics);
            bodies.Add(saturn);

            Planet uranus = new Planet(86.8 * Math.Pow(10, 24), "Uranus", 2872.5, 90, 0, 6.8, uranusSprite, sun, graphics);
            bodies.Add(uranus);

            Planet neptune = new Planet(102 * Math.Pow(10, 24), "Neptune", 4495.1, 90, 0, 5.4, neptunusSprite, sun, graphics);
            bodies.Add(neptune);

            menu = new Menu(sun, graphics, arial);

            foreach (Body body in bodies)
            {
                if (!body.IsStar)
                {
                    Planet planet = body as Planet;
                    planets.Add(planet);
                    spriteCache.Add(planet, new List<Vector2>());
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (IrlTotalUpdateTime(gameTime) > 0.01)
            { 
               if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                KeyboardState keyboard = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();
                if (takeKeyboardInput)
                {
                    kbHandler.Update(menu);
                }
                if (mouse.LeftButton == ButtonState.Pressed)
                { 
                    if(mouseHold == false)
                    {
                        if (IsMouseInArea(mouse, new Point(pauseBtn.Location.X, pauseBtn.Location.Y), pauseBtn.Height, pauseBtn.Width))
                        {
                            pause = !pause;
                            if (isSelectedBody)
                            {
                                menu.UpdateValues();
                            }
                            takeKeyboardInput = false;
                        }

                        else if(isSelectedBody)
                        {
                            foreach (TextBox textBox in menu.TxtBoxes)
                            {
                                if (IsMouseInArea(mouse, textBox.Hitbox.Location, textBox.Hitbox.Height, textBox.Hitbox.Width))
                                {
                                    pause = true;
                                    takeKeyboardInput = true;
                                    textBox.Text = "";
                                    menu.Selected = textBox;
                                }
                            }
                        }
                    }

                    

                    foreach (Body body in bodies)
                    {
                        if (IsMouseInArea(mouse, body.Position.ToPoint(), body.radius * 2, body.radius * 2))
                        {
                            selectedBody = body;
                            isSelectedBody = true;
                            menu.Body = body;
                        }
                    }
                    mouseHold = true;
                }

                else if (mouse.LeftButton == ButtonState.Released)
                {

                    mouseHold = false;
                }

                if (!pause)
                {
                    foreach (Planet planet in planets)
                    {
                        planet.updateVelocityAndPosition(bodies, IrlTotalUpdateTime(gameTime));
                    }
                }

                base.Update(gameTime);
                oldTotalUpdateTime = gameTime.TotalGameTime.TotalSeconds;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (IrlTotalDrawTime(gameTime) > 0.0167)
            {
                //MakeDaPictures
                graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                foreach (Body body in bodies)
                {
                    if (!body.IsStar)   //If the body is a star then it skips the drawing of tail since it is supposed to be stationary
                    {
                        Planet planet = body as Planet;
                        //spriteCache[planet].Add(planet.Position);
                        spriteCache[planet].Add(new Vector2(Convert.ToSingle(planet.Position.X + body.radius - 2), Convert.ToSingle(planet.Position.Y + body.radius - 2)));
                        if (spriteCache[planet].Count >= spriteCacheSize)
                        {
                            spriteCache[planet].RemoveAt(0);
                        }
                        foreach (Vector2 position in spriteCache[planet])
                        {
                            spriteBatch.Draw(tailSprite, position);
                        }
                    }

                    //Draws the body itself
                    spriteBatch.Draw(body.Texture, body.Position);

                    //Followed by the marker ontop of the body's sprite id it has been selected
                    if (body == selectedBody)
                    {
                        spriteBatch.Draw(markerSprite, new Vector2(Convert.ToSingle(body.Position.X + body.radius - 11), Convert.ToSingle(body.Position.Y + body.radius - 11)));
                    }
                }

                //Draws the menu's background color
                spriteBatch.Draw(menuSprite, null, menuBackground);

                //Draw pause button
                if (pause)
                {
                    spriteBatch.Draw(playBtnSprite, null, pauseBtn);
                }
                else
                {
                    spriteBatch.Draw(pauseBtnSprite, null, pauseBtn);
                }


                //Draws the debug counter in the top left corner
                spriteBatch.DrawString(arial, "FPS:" + Convert.ToInt32(1 / IrlTotalDrawTime(gameTime)), new Vector2(0, 0), new Color(new Vector3(233, 0, 0)));
                //if (IrlTotalUpdateTime(gameTime) != 0)    //Temporary
                //    spriteBatch.DrawString(arial, "UPS:" + Convert.ToInt32(1 / IrlTotalUpdateTime(gameTime)), new Vector2(0, 13), new Color(new Vector3(233, 0, 0)));

                //If it's selected the planets info gets drawn ontop of the menu's background sprite
                if (wait >= 5)
                {
                    wait = 0;
                    if (isSelectedBody)
                    {
                        if (!pause)
                        {
                            menu.UpdateValues();
                        }
                    }
                }
                else
                {
                    wait++;
                }
                menu.DrawStrings(spriteBatch);
                spriteBatch.End();
                oldTotalDrawTime = gameTime.TotalGameTime.TotalSeconds;
                base.Draw(gameTime);
            }
        }

        public double IrlTotalUpdateTime(GameTime gametime)
        {
            return gametime.TotalGameTime.TotalSeconds - oldTotalUpdateTime;
        }

        public double IrlTotalDrawTime(GameTime gametime)
        {
            return gametime.TotalGameTime.TotalSeconds - oldTotalDrawTime;
        }

        public bool IsMouseInArea(MouseState mousestate, Point position, double Height, double Width)
        {
            return mousestate.X > position.X && mousestate.X < position.X + Width
                && mousestate.Y > position.Y && mousestate.Y < position.Y + Height;
        }

        public Keys readKeyBoard()
        {
            return Keyboard.GetState().GetPressedKeys()[0];
        }
    }
}






























//if (keyboard.IsKeyDown(Keys.LeftShift))
//{
//    if (!pause)
//    {
//        if (shiftMouseHold)
//        {
//            dragVector = new Vector2(initialPos.X - mouse.Position.ToVector2().X,
//                initialPos.Y - mouse.Position.ToVector2().Y);
//        }
//        else
//        {
//            // Ser till att musen inte befinner sig utanför rutan när man droppar in planeter
//            if (IsMouseInArea(mouse, new Point(0, 0), graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth - menuBackground.Width))
//            {
//                initialPos = mouse.Position.ToVector2();
//                mouseHold = true;
//                Console.WriteLine("Mouse: " + initialPos);
//            }
//        }
//    }
//}

//else
//{
//    foreach (Body body in bodies)
//    {
//        if ((mouse.Position.X - body.Position.X < body.radius * 2 && mouse.Position.X - body.Position.X > 0)
//            && (mouse.Position.Y - body.Position.Y < body.radius * 2 && mouse.Position.Y - body.Position.Y > 0))
//        {
//            selectedBody = body;
//            isSelectedBody = true;
//            menu.Body = body;
//        }
//    }
//}






//    if (shiftMouseHold == true)
//    {
//        shiftMouseHold = false;
//        Vector2 shootVector = new Vector2(dragVector.X * 100, dragVector.Y * 100);

//        initialPos.X = initialPos.X - Body.GetCenter(graphics.GraphicsDevice).X;
//        initialPos.Y = initialPos.Y - Body.GetCenter(graphics.GraphicsDevice).Y;

//        double mass = bodies[1].Mass; // Jordens massa
//        string name = "Planet" + bodies.Count.ToString();

//        Planet spwnObject = new Planet(mass, name, initialPos, shootVector, planetSprite, bodies[0], graphics);
//        Console.WriteLine("Planet added at: " + spwnObject.Position);
//        bodies.Add(spwnObject);
//        planets.Add(spwnObject);
//        spriteCache.Add(spwnObject, new List<Vector2>());
//    }