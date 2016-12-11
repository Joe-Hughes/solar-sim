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
        TimeSpan realTimeElapsed;
        // Specifierar hur lång tid som spelet varit pausat så att man kan subtrahera det från TimeElapsed.
        TimeSpan totalPausedTime;

        Texture2D planetSprite;
        Texture2D starSprite;
        Texture2D tailSprite;
        Texture2D menuSprite;
        Texture2D markerSprite;
        Texture2D txtBoxSprite;
        Texture2D pauseBtnSprite;
        Texture2D playBtnSprite;
        Texture2D playBtn2Sprite;
        Texture2D playBtn3Sprite;

        Button pauseButton;
        Button playButton;
        Button playButton2;
        Button playButton3;
        SpriteFont arial;

        List<Body> bodies = new List<Body>();
        List<Planet> planets = new List<Planet>();

        public static double referenceDistanceInUnits;
        public static double referenceDistanceInMeters;

        bool mouseHold = false;

        double drawFrequency;
        double updateFrequency;

        int simulationSpeed = 1;

        Rectangle menuBackground;
        Menu menu;
        //TextBox selectedTxtBox;

        Body selectedBody;
        bool isSelectedBody = false;
        bool physicsBroken = false;
        bool promtedAboutCollision = false;

        KbHandler kbHandler = new KbHandler();
        bool takeKeyboardInput = false;

        double oldTotalUpdateTime = 0;
        double oldTotalDrawTime = 0;

        // Programvariabler
        double preferedFPS = 30;
        double preferedUPS = 4 * 60;
        int wait = 0;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            int displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            int windowWidth = 1320;
            int windowHeight = 695;

            IsFixedTimeStep = false;    // Låser upp framerate från 30/50/60FPS
            graphics.SynchronizeWithVerticalRetrace = false;    // Stänger av Vsync
            graphics.PreferredBackBufferWidth = windowWidth;   // Spelrutans bredd i pixlar
            graphics.PreferredBackBufferHeight = windowHeight;   // Spelrutans höjd i pixlar
            graphics.IsFullScreen = false;
            //this.Window.Position = new Point((displayWidth - windowWidth) / 2, (displayHeight - windowHeight) / 2);
            this.Window.Position = new Point(0, 0);

            graphics.PreferMultiSampling = true;    // Förminskar pixelering på icke-raka linjer
            graphics.ApplyChanges();
            this.IsMouseVisible = true;

            // Använd en planet som referensvärden för att få fram meter per positionsenhet.
            // Alltså (planetens avstånd från solen i enheter)/(planetens avstånd från stolen i meter)
            // Planet: 
            referenceDistanceInUnits = (graphics.PreferredBackBufferHeight / 2) - 10;
            referenceDistanceInMeters = 227.9 * Math.Pow(10, 9);

            drawFrequency = 1 / preferedFPS;
            updateFrequency = 1 / preferedUPS;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Laddar alla sprites från /Content mappen

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
            playBtn2Sprite = this.Content.Load<Texture2D>(@"PlayBtn2");
            playBtn3Sprite = this.Content.Load<Texture2D>(@"PlayBtn3");
            arial = this.Content.Load<SpriteFont>("StandardArial");

            // Ritar grundläggande UI-element
            int menuWidth = 324;
            menuBackground = new Rectangle(graphics.PreferredBackBufferWidth - menuWidth, 0, menuWidth, graphics.PreferredBackBufferHeight);

            pauseButton = new Button(new Vector2(graphics.PreferredBackBufferWidth - menuBackground.Width,
                graphics.PreferredBackBufferHeight - 50), 81, 50, 0, pauseBtnSprite);
            playButton = new Button(new Vector2(graphics.PreferredBackBufferWidth - (menuBackground.Width * 3/4),
                graphics.PreferredBackBufferHeight - 50), 81, 50, 1, playBtnSprite);
            playButton2 = new Button(new Vector2(graphics.PreferredBackBufferWidth - (menuBackground.Width * 2/4),
                graphics.PreferredBackBufferHeight - 50), 81, 50, 2, playBtn2Sprite);
            playButton3 = new Button(new Vector2(graphics.PreferredBackBufferWidth - (menuBackground.Width * 1/4),
                graphics.PreferredBackBufferHeight - 50), 81, 50, 3, playBtn3Sprite);

            int[] randomDegrees = new int[2];
            Random rng = new Random();
            int rngNumb;
            // Skapar kroppar och lägger in dem i systemet
            Body sun = new Body(1.9885 * Math.Pow(10, 30), "Sun", starSprite, graphics);
            bodies.Add(sun);
            rngNumb = rng.Next(0, 360);
            Planet mercury = new Planet(0.330 * Math.Pow(10, 24), "Mercury", 69.82, rngNumb, rngNumb - 90, 38.86, mercurySprite, tailSprite, sun, graphics);
            bodies.Add(mercury);
            rngNumb = rng.Next(0, 360);
            Planet earth = new Planet(5.9724 * Math.Pow(10, 24), "Earth", 149.6, rngNumb, rngNumb - 90, 29.8, earthSprite, tailSprite, sun, graphics);
            bodies.Add(earth);
            rngNumb = rng.Next(0, 360);
            Planet mars = new Planet(0.64171 * Math.Pow(10, 24), "Mars", 227.9, rngNumb, rngNumb - 90, 24.1, marsSprite, tailSprite, sun, graphics);
            bodies.Add(mars);
            rngNumb = rng.Next(0, 360);
            Planet jupiter = new Planet(1898 * Math.Pow(10, 24), "Jupiter", 778.6, rngNumb, rngNumb - 90, 13.1, jupiterSprite, tailSprite, sun, graphics);
            bodies.Add(jupiter);
            rngNumb = rng.Next(0, 360);
            Planet saturn = new Planet(568 * Math.Pow(10, 24), "Saturn", 1433.5, rngNumb, rngNumb - 90, 9.7, saturnusSprite, tailSprite, sun, graphics);
            bodies.Add(saturn);
            rngNumb = rng.Next(0, 360);
            Planet uranus = new Planet(86.8 * Math.Pow(10, 24), "Uranus", 2872.5, rngNumb, rngNumb - 90, 6.8, uranusSprite, tailSprite, sun, graphics);
            bodies.Add(uranus);
            rngNumb = rng.Next(0, 360);
            Planet neptune = new Planet(102 * Math.Pow(10, 24), "Neptune", 4495.1, rngNumb, rngNumb - 90, 5.4, neptunusSprite, tailSprite, sun, graphics);
            bodies.Add(neptune);

            menu = new Menu(sun, graphics, arial, realTimeElapsed);

            foreach (Body body in bodies)
            {
                if (!body.IsStar)
                {
                    Planet planet = body as Planet;
                    planets.Add(planet);
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
            Body.UpdateTimeSpeed(simulationSpeed);

            if (IrlTotalUpdateTime(gameTime) >= (1 / preferedUPS))
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
                    // Kollar om man tryckt på någon av tidsknapparna och byter i så fall hastighet
                    simulationSpeed = pauseButton.CheckClick(mouse, mouseHold, simulationSpeed);
                    simulationSpeed = playButton.CheckClick(mouse, mouseHold, simulationSpeed);
                    simulationSpeed = playButton2.CheckClick(mouse, mouseHold, simulationSpeed);
                    simulationSpeed = playButton3.CheckClick(mouse, mouseHold, simulationSpeed);
                    if (simulationSpeed > 0 && physicsBroken && !promtedAboutCollision)
                    {
                        promtedAboutCollision = true;
                    }

                    if (mouseHold == false)
                    { 
                        if (isSelectedBody)
                        {
                            foreach (TextBox textBox in menu.TxtBoxes)
                            {
                                if (IsMouseInArea(mouse, textBox.Hitbox.Location, textBox.Hitbox.Height, textBox.Hitbox.Width))
                                {
                                    simulationSpeed = 0;
                                    takeKeyboardInput = true;
                                    textBox.Text = "";
                                    menu.Selected = textBox;
                                }
                            }
                        }
                    }
                    foreach (Body body in bodies)
                    {
                        if (IsMouseInArea(mouse, body.SpritePosition.ToPoint(), body.radius * 2, body.radius * 2))
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

                // Om spelet inte är pausat, uppdatera planeternas positioner och värden
                if (simulationSpeed > 0)
                {
                    // Uppdaterar variabeln som håller koll på hur länge simulationen pågått (minus total paus tid)
                    realTimeElapsed = gameTime.TotalGameTime.Subtract(totalPausedTime);

                    foreach (Planet planet in planets)
                    {
                        // Kollar om någon av kropparna kolliderat med varandra.
                        // Promptar sedan användaren när det händer första gången och frågar om den vill fortsätta.
                        bool collisionDetected = false;
                        if (!physicsBroken)
                        {
                            if (Body.DetectCollision(bodies))
                            {
                                collisionDetected = true;
                                physicsBroken = true;
                                simulationSpeed = 0;
                            }
                        }

                        // Om det inte skett en kollision, uppdatera planeternas positioner igen.
                        if (!collisionDetected)
                        {
                            planet.UpdateVelocityAndPosition(bodies, IrlTotalUpdateTime(gameTime));
                            planet.Tail.AddTailPosition(planet);
                        }
                    }
                }
                else
                {
                    // om programmet är pausat håller denna variabel koll på hur länge.
                    totalPausedTime = gameTime.TotalGameTime.Subtract(realTimeElapsed);
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
            if (IrlTotalDrawTime(gameTime) >= (1 / preferedFPS))
            {
                //MakeDaPictures
                graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                foreach (Body body in bodies)
                {
                    // Ritar ut tails för alla planeter
                    if (!body.IsStar)
                    {
                        Planet planet = body as Planet;
                        Tail tail = planet.Tail;
                        foreach (Vector2 position in tail.GetTailPositions())
                        {
                            spriteBatch.Draw(tail.Texture, position);
                        }
                    }

                    // Ritar själva kroppen
                    spriteBatch.Draw(body.Texture, body.SpritePosition);

                    // Ritar markören om kroppen är markerad
                    if (body == selectedBody)
                    {
                        spriteBatch.Draw(markerSprite, Vector2.Subtract(body.Position, new Vector2(markerSprite.Width / 2)));

                    }
                }

                // Ritar menyns bakgrundsfärg
                spriteBatch.Draw(menuSprite, null, menuBackground);

                // Ritar tidsknapparna
                spriteBatch.Draw(pauseButton.Texture, pauseButton.Location.ToVector2());
                spriteBatch.Draw(playButton.Texture, playButton.Location.ToVector2());
                spriteBatch.Draw(playButton2.Texture, playButton2.Location.ToVector2());
                spriteBatch.Draw(playButton3.Texture, playButton3.Location.ToVector2());

                // Ritar debugmätarna för FPS och UPS
                spriteBatch.DrawString(arial, "FPS:" + Convert.ToInt32(1 / IrlTotalDrawTime(gameTime)), new Vector2(0, 0), new Color(new Vector3(233, 0, 0)));
                //if (IrlTotalUpdateTime(gameTime) != 0)    //Temporary
                //    spriteBatch.DrawString(arial, "UPS:" + Convert.ToInt32(1 / IrlTotalUpdateTime(gameTime)), new Vector2(0, 13), new Color(new Vector3(233, 0, 0)));

                //If it's selected the planets info gets drawn ontop of the menu's background sprite
                if (wait >= 5)
                {
                    wait = 0;
                    if (isSelectedBody)
                    {
                        if (simulationSpeed > 0)
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

                if (physicsBroken && !promtedAboutCollision)
                {
                    int width = 275;
                    int height = 75;
                    Vector2 centerScreen = GetCenter(graphics);
                    int xPosition = Convert.ToInt32(centerScreen.X - (width / 2));
                    int yPosition = Convert.ToInt32(centerScreen.Y - (height / 2));
                    Rectangle collisionPrompt = new Rectangle(xPosition, yPosition, width, height);
                    spriteBatch.Draw(menuSprite, null, collisionPrompt);

                    string[] collisionText = {
                        "WARNING: Kollision detekterad!",
                        "Simulationen agerar nu ofysikaliskt."
                        //"Tryck på Spela-knappen för att fortsätta"
                    };

                    for (int i = 0; i < collisionText.Length; i++)
                    {
                        Vector2 textPosition = Vector2.Add(collisionPrompt.Location.ToVector2(), new Vector2(14, 22 + 16 * i));
                        spriteBatch.DrawString(arial, collisionText[i], textPosition, Color.Black);
                    }
                }

                spriteBatch.End();
                oldTotalDrawTime = gameTime.TotalGameTime.TotalSeconds;
                base.Draw(gameTime);
            }
        }

        public static Vector2 GetCenter(GraphicsDeviceManager graphicsDevice)
        {
            Point window = graphicsDevice.GraphicsDevice.PresentationParameters.Bounds.Center;
            window.X -= 324 / 2;
            Vector2 center = window.ToVector2();

            return center;
        }

        public double IrlTotalUpdateTime(GameTime gametime)
        {
            return gametime.TotalGameTime.TotalSeconds - oldTotalUpdateTime;
        }

        public double IrlTotalDrawTime(GameTime gametime)
        {
            return gametime.TotalGameTime.TotalSeconds - oldTotalDrawTime;
        }

        public static bool IsMouseInArea(MouseState mousestate, Point position, double Height, double Width)
        {
            return mousestate.X > position.X && mousestate.X < position.X + Width
                && mousestate.Y > position.Y && mousestate.Y < position.Y + Height;
        }

        public Keys readKeyBoard()
        {
            return Keyboard.GetState().GetPressedKeys()[0];
        }

        public int[] GenerateRngStartingAngles ()
        {
            Random rng = new Random();
            int degrees = rng.Next(0, 320);
            int[] values = new int[2] { degrees, degrees - 90 };
            return values;
        }
    }
}


























// Code graveyard

// Om skärmen är 1366x768, spela i fullskärm, annars, centralisera rutan på skärmen
//if (false)
//{
//    graphics.IsFullScreen = false;
//    this.Window.Position = new Point((displayWidth - windowWidth) / 2, (displayHeight - windowHeight) / 2);
//}
//else
//{
//    this.Window.Position = new Point((displayWidth - windowWidth) / 2, (displayHeight - windowHeight) / 2);
//}

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