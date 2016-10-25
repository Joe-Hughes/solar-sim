﻿using System;
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
        Button pauseButton;
        SpriteFont arial;

        List<Body> bodies = new List<Body>();
        List<Planet> planets = new List<Planet>();

        public static double referenceDistanceInUnits;
        public static double referenceDistanceInMeters;

        bool mouseHold = false;
        bool shiftMouseHold = false;
        Vector2 initialPos;
        Vector2 dragVector;

        bool firstUpdate = true;

        double drawFrequency;
        double updateFrequency;

        bool pause = false;
        Rectangle menuBackground;
        Body selected;
        bool isSelected = false;

        double oldTotalUpdateTime;
        double oldTotalDrawTime;

        // Programvariabler
        double preferedFPS = 60;
        double preferedUPS = 2 * 60;

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
            int windowWidth = 1366;
            int windowHeight = 768;

            IsFixedTimeStep = false;    // Låser upp framerate från 30/50/60FPS
            graphics.SynchronizeWithVerticalRetrace = false;    // Stänger av Vsync
            graphics.PreferredBackBufferWidth = windowWidth;   // Spelrutans bredd i pixlar
            graphics.PreferredBackBufferHeight = windowHeight;   // Spelrutans höjd i pixlar

            // Om skärmen är 1366x768, spela i fullskärm, annars, centralisera rutan på skärmen
            if (displayHeight <= 768 && displayWidth <= 1366)
            {
                graphics.IsFullScreen = false;
                this.Window.Position = new Point((displayWidth - windowWidth) / 2, (displayHeight - windowHeight) / 2);
            }
            else
            {
                this.Window.Position = new Point((displayWidth - windowWidth) / 2, (displayHeight - windowHeight) / 2);
            }

            graphics.PreferMultiSampling = true;    // Förminskar pixelering på icke-raka linjer
            graphics.ApplyChanges();
            this.IsMouseVisible = true;

            // Använd en planet som referensvärden för att få fram meter per positionsenhet.
            // Alltså (planetens avstånd från solen i enheter)/(planetens avstånd från stolen i meter)
            // Planet: Nepunus
            referenceDistanceInUnits = (graphics.PreferredBackBufferHeight / 2) - 10;
            referenceDistanceInMeters = 1433.5 * Math.Pow(10, 9);

            drawFrequency = 1 / preferedFPS; // brukade vara 0.02
            updateFrequency = 1 / preferedUPS; // brukade vara 0.01

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
            planetSprite = this.Content.Load<Texture2D>(@"CIRCLE");
            starSprite = this.Content.Load<Texture2D>(@"STAR");
            tailSprite = this.Content.Load<Texture2D>(@"TAIL");
            menuSprite = this.Content.Load<Texture2D>(@"MENU");
            markerSprite = this.Content.Load<Texture2D>(@"MARKER");
            txtBoxSprite = this.Content.Load<Texture2D>(@"TXTBOX");
            pauseBtnSprite = this.Content.Load<Texture2D>(@"PauseBtn");
            playBtnSprite = this.Content.Load<Texture2D>(@"PlayBtn");
            arial = this.Content.Load<SpriteFont>("StandardArial");

            // Ritar grundläggande UI-element
            int menuWidth = 324;
            menuBackground = new Rectangle(graphics.PreferredBackBufferWidth - menuWidth, 0, menuWidth, graphics.PreferredBackBufferHeight);
            pauseButton = new Button(new Vector2(graphics.PreferredBackBufferWidth - menuBackground.Width,
                graphics.PreferredBackBufferHeight - 50), 100, 50, Button.ButtonBehavior.Pause, pauseBtnSprite, playBtnSprite);

            // Skapar kroppar och lägger in dem i systemet
            Body sun = new Body(1.9885 * Math.Pow(10, 30), "Sun", starSprite, graphics.GraphicsDevice);
            bodies.Add(sun);

            Planet mercury = new Planet(0.330 * Math.Pow(10, 24), "Mercury", 57.9, 90, 0, 47.4, planetSprite, tailSprite, sun, graphics.GraphicsDevice);
            bodies.Add(mercury);

            Planet earth = new Planet(5.9724 * Math.Pow(10, 24), "Earth", 149.6, 90, 0, 29.8, planetSprite, tailSprite, sun, graphics.GraphicsDevice);
            bodies.Add(earth);

            Planet mars = new Planet(0.64171 * Math.Pow(10, 24), "Mars", 227.9, 90, 0, 24.1, planetSprite, tailSprite, sun, graphics.GraphicsDevice);
            bodies.Add(mars);

            Planet jupiter = new Planet(1898 * Math.Pow(10, 24), "Jupiter", 778.6, 90, 0, 13.1, planetSprite, tailSprite, sun, graphics.GraphicsDevice);
            bodies.Add(jupiter);

            Planet saturn = new Planet(568 * Math.Pow(10, 24), "Saturn", 1433.5, 90, 0, 9.7, planetSprite, tailSprite, sun, graphics.GraphicsDevice);
            bodies.Add(saturn);

            Planet uranus = new Planet(86.8 * Math.Pow(10, 24), "Uranus", 2872.5, 90, 0, 6.8, planetSprite, tailSprite, sun, graphics.GraphicsDevice);
            bodies.Add(uranus);

            Planet neptune = new Planet(102 * Math.Pow(10, 24), "Neptune", 4495.1, 90, 0, 5.4, planetSprite, tailSprite, sun, graphics.GraphicsDevice);
            bodies.Add(neptune);

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
            // Om det har gått tillräckligt med tid sedan förra RIKTIGA uppdateringen, uppdatera igen
            if (updateFrequency >= (1/preferedUPS))
            { 
                // Stäng ner programmet om man trycker på tillbaka eller Escape
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                MouseState mouse = Mouse.GetState();
                // 
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // Kollar om man tryckt på pausknappen och sparar sedan resultatet i "paus"-variabeln
                    pause = pauseButton.CheckClick(mouse, mouseHold, pause);

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    {
                        if (!pause)
                        {
                            if (shiftMouseHold)
                            {
                                dragVector = new Vector2(initialPos.X - mouse.Position.ToVector2().X,
                                    initialPos.Y - mouse.Position.ToVector2().Y);
                            }
                            else
                            {
                                // Ser till att musen inte befinner sig utanför rutan när man droppar in planeter
                                int x = mouse.Position.X;
                                int y = mouse.Position.Y;
                                if (x > 0 && x < (graphics.PreferredBackBufferWidth - menuBackground.Width) &&
                                    y > 0 && y < graphics.PreferredBackBufferHeight)
                                {
                                    initialPos = mouse.Position.ToVector2();
                                    mouseHold = true;
                                    Console.WriteLine("Mouse: " + initialPos);
                                }
                            }
                        }
                    }

                    else
                    {
                        foreach (Body body in bodies)
                        {
                            if ((mouse.Position.X - body.Position.X < 16 && mouse.Position.X - body.Position.X > 0)
                                && (mouse.Position.Y - body.Position.Y < 16 && mouse.Position.Y - body.Position.Y > 0))
                            {
                                selected = body;
                                isSelected = true;
                            }
                        }
                    }  
                    mouseHold = true;
                }

                else if (mouse.LeftButton == ButtonState.Released)
                {
                    if (shiftMouseHold == true)
                    {
                        shiftMouseHold = false;
                        Vector2 shootVector = new Vector2(dragVector.X * 100, dragVector.Y * 100);

                        initialPos.X = initialPos.X - Game1.GetCenter(graphics.GraphicsDevice).X;
                        initialPos.Y = initialPos.Y - Game1.GetCenter(graphics.GraphicsDevice).Y;

                        double mass = bodies[1].Mass; // Jordens massa
                        string name = "Planet" + bodies.Count.ToString();

                        Planet spwnObject = new Planet(mass, name, initialPos, shootVector, planetSprite, bodies[0], graphics.GraphicsDevice);
                        Console.WriteLine("Planet added at: " + spwnObject.Position);
                        bodies.Add(spwnObject);
                        planets.Add(spwnObject);
                    }
                    mouseHold = false;
                }

                // Om spelet inte är pausat, uppdatera planeternas positioner och värden
                if (!pause)
                {
                    foreach (Planet planet in planets)
                    {
                        planet.updateVelocityAndPosition(bodies, IrlTotalUpdateTime(gameTime));
                        planet.Tail.AddTailPosition(planet);
                    }
                }
                
                //Console.WriteLine("Update: " + gameTime.ElapsedGameTime.TotalSeconds.ToString() + "   :   " + IrlTotalUpdateTime(gameTime));
                
                base.Update(gameTime);
                oldTotalUpdateTime = gameTime.TotalGameTime.TotalSeconds;
                updateFrequency = 0;
            }
            updateFrequency += gameTime.ElapsedGameTime.TotalSeconds;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (drawFrequency >= (1/preferedFPS))
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
                        // DEBUG
                        //if (planet.Name == "Neptune")
                        //{
                        //    Vector2 center = Game1.GetCenter(graphics.GraphicsDevice);
                        //    Console.WriteLine(planet.Name + " X: " + (planet.Position.X - center.X + planet.radius) + " Y: " + (planet.Position.Y - center.Y + planet.radius));
                        //}
                        foreach (Vector2 position in tail.GetTailPositions())
                        {
                            spriteBatch.Draw(tail.Texture, position);
                        }
                    }

                    // Ritar själva kroppen
                    spriteBatch.Draw(body.Texture, body.Position);  

                    // Ritar markören om kroppe är markerad
                    if (body == selected)
                    {
                        spriteBatch.Draw(markerSprite, new Vector2(body.Position.X - 3, body.Position.Y - 3));
                    }
                }

                // Ritar menyns bakgrundsfärg
                spriteBatch.Draw(menuSprite, null, menuBackground);

                // Ritar pausknappen
                spriteBatch.Draw(pauseButton.Texture, pauseButton.Location.ToVector2());

                // Ritar debugmätarna för FPS och UPS
                spriteBatch.DrawString(arial, "FPS:" + Convert.ToInt32(1 / IrlTotalDrawTime(gameTime)), new Vector2(0, 0), new Color(new Vector3(233, 0, 0)));
                double updateTime = IrlTotalUpdateTime(gameTime);
                if (updateTime != 0)    //Temporary
                    spriteBatch.DrawString(arial, "UPS:" + Convert.ToInt32(1 / updateTime), new Vector2(0, 13), new Color(new Vector3(233, 0, 0)));
                
                // Fyller menyn med information om den valda planeten
                if (isSelected)
                {
                    // Den horisontella positionen där text skrivs ut i menyn
                    float horizontalTextPosition = graphics.PreferredBackBufferWidth - menuBackground.Width + 10;
                    spriteBatch.Draw(selected.Texture, new Vector2(horizontalTextPosition, 10));
                    spriteBatch.DrawString(arial, selected.Name, new Vector2(horizontalTextPosition + 30, 10), new Color(new Vector3(0, 0, 0)));

                    if (!selected.IsStar)
                    {
                        Planet planet = selected as Planet;
                        spriteBatch.DrawString(arial, "Distance from sun: " + (selected.DetermineDistance(bodies[0]) * Body.scaleMultiplier), new Vector2(horizontalTextPosition, 70), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Velocity: " + planet.Speed, new Vector2(horizontalTextPosition, 110), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Acceleration: " + planet.Acceleration, new Vector2(horizontalTextPosition, 150), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Force: " + planet.Force, new Vector2(horizontalTextPosition, 190), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Position: " + planet.Position, new Vector2(horizontalTextPosition, 230), new Color(new Vector3(0, 0, 0)));
                    }
                }
                spriteBatch.End();
                base.Draw(gameTime);

                oldTotalDrawTime = gameTime.TotalGameTime.TotalSeconds;
                drawFrequency = 0;
            }
            drawFrequency += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static Vector2 GetCenter(GraphicsDevice graphicsDevice)
        {
            Point window = graphicsDevice.PresentationParameters.Bounds.Center;
            window.X -= 324 / 2;
            Vector2 center = window.ToVector2();

            return center;
        }

        public double IrlTotalUpdateTime(GameTime gametime)
        {
            double lol = gametime.TotalGameTime.TotalSeconds - oldTotalUpdateTime;
            return gametime.TotalGameTime.TotalSeconds - oldTotalUpdateTime;
        }

        public double IrlTotalDrawTime(GameTime gametime)
        {
            return gametime.TotalGameTime.TotalSeconds - oldTotalDrawTime;
        }

        public static bool IsMouseInArea(MouseState mousestate, Point position, double Height, double Width)
        {
            if(mousestate.Position.X > position.X && mousestate.Position.X < position.X + Width 
                && mousestate.Position.Y > position.Y && mousestate.Position.Y < position.Y + Height)
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
