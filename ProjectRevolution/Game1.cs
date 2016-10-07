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
        Rectangle pauseBtn = new Rectangle(700, 526, 100, 50);
        List<Body> bodies = new List<Body>();
        List<Planet> planets = new List<Planet>();
        bool mouseHold = false;
        bool shiftMouseHold = false;
        Vector2 initialPos;
        Vector2 dragVector;
        Dictionary<Planet, List<Vector2>> spriteCache = new Dictionary<Planet, List<Vector2>>();
        int spriteCacheSize = 900;
        bool pause = false;
        SpriteFont arial;
        Rectangle menuBackground = new Rectangle(700, 0, 324, 576);
        Body selected;
        bool isSelected = false;

        double d = 0.02;
        double u = 0.01;

        double oldTotalUpdateTime = 0.01;
        double oldTotalDrawTime = 0.02;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;    // unlocks framerate
            graphics.SynchronizeWithVerticalRetrace = false;    // disables Vsync
            graphics.PreferredBackBufferWidth = 1024;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 576;   // set this value to the desired height of your window
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;

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
            planetSprite = this.Content.Load<Texture2D>(@"CIRCLE");
            starSprite = this.Content.Load<Texture2D>(@"STAR");
            tailSprite = this.Content.Load<Texture2D>(@"TAIL");
            menuSprite = this.Content.Load<Texture2D>(@"MENU");
            markerSprite = this.Content.Load<Texture2D>(@"MARKER");
            txtBoxSprite = this.Content.Load<Texture2D>(@"TXTBOX");
            pauseBtnSprite = this.Content.Load<Texture2D>(@"PauseBtn");
            playBtnSprite = this.Content.Load<Texture2D>(@"PlayBtn");
            arial = this.Content.Load<SpriteFont>("StandardArial");

            // Create all the bodies in the system; 
            Body sun = new Body(1.99 * Math.Pow(10, 30), 8, graphics.GraphicsDevice, "Sun", starSprite);
            Planet earth = new Planet(5.93 * Math.Pow(10, 24), 8, graphics.GraphicsDevice, new Vector2(0, -140), "Earth", planetSprite, sun, new Vector2(20000, 0));
            //Planet planet2 = new Planet(5.93 * Math.Pow(10, 24), 8, graphics.GraphicsDevice, new Vector2(0, 140), "planet2", planetSprite, sun, new Vector2(-20000, 0));

            bodies.Add(sun);
            bodies.Add(earth);
            //bodies.Add(planet2);
            //bodies.Add(planet3);
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
            if (u >= 0.01)
            { 
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                MouseState mouse = Mouse.GetState();
                if (mouse.LeftButton == ButtonState.Pressed)
                { 
                    if(mouseHold == false && IsMouseInArea(mouse, pauseBtn.Location, pauseBtn.Height, pauseBtn.Width))
                    {
                        if(pause)
                        {
                            pause = false;
                        }
                        else
                        {
                            pause = true;
                        }
                    }

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
                                initialPos = mouse.Position.ToVector2();
                                shiftMouseHold = true;
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

                        initialPos.X = initialPos.X - Body.GetCenter(graphics.GraphicsDevice).X;
                        initialPos.Y = initialPos.Y - Body.GetCenter(graphics.GraphicsDevice).Y;

                        Planet spwnObject = new Planet(5.93 * Math.Pow(10, 24), 8, graphics.GraphicsDevice,
                            initialPos, "Planet" + bodies.Count.ToString(), planetSprite, bodies[0], shootVector);
                        bodies.Add(spwnObject);
                        planets.Add(spwnObject);
                        spriteCache.Add(spwnObject, new List<Vector2>());
                    }
                    mouseHold = false;
                }

                if (!pause)
                {
                    foreach (Planet planet in planets)
                    {
                        planet.updateVelocityAndPosition(bodies, IrlTotalUpdateTime(gameTime));
                    }
                }
                Console.WriteLine("Update: " + gameTime.ElapsedGameTime.TotalSeconds.ToString() + "   :   " + IrlTotalUpdateTime(gameTime));
                base.Update(gameTime);
                oldTotalUpdateTime = gameTime.TotalGameTime.TotalSeconds;
                u = 0;
            }
            u += gameTime.ElapsedGameTime.TotalSeconds;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            if (d >= 0.05)
            {
                //MakeDaPictures
                graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                foreach (Body body in bodies)
                {
                    if (!body.IsStar)   //If the body is a star then it skips the drawing of tail since it is supposed to be stationary
                    {
                        Planet planet = body as Planet;
                        spriteCache[planet].Add(body.Position);
                        if (spriteCache[planet].Count >= spriteCacheSize)
                        {
                            spriteCache[planet].RemoveAt(0);
                        }
                        foreach (Vector2 position in spriteCache[planet])
                        {
                            spriteBatch.Draw(tailSprite, position);
                        }
                    }

                    //Draws thebody itself
                    spriteBatch.Draw(body.Texture, body.Position);  

                    //Followed by the marker ontop of the body's sprite id it has been selected
                    if (body == selected)
                    {
                        spriteBatch.Draw(markerSprite, new Vector2(body.Position.X - 3, body.Position.Y - 3));
                    }
                }

                //Draws the menu's background color
                spriteBatch.Draw(menuSprite, null, menuBackground);

                //Draw pause button
                if(pause)
                {
                    spriteBatch.Draw(playBtnSprite, null, pauseBtn);
                }
                else
                {
                    spriteBatch.Draw(pauseBtnSprite, null, pauseBtn);
                }
                

                //Draws the debug counter in the top left corner
                spriteBatch.DrawString(arial, "FPS:" + Convert.ToInt32(1 / IrlTotalDrawTime(gameTime)), new Vector2(0, 0), new Color(new Vector3(233, 0, 0)));
                double updateTime = IrlTotalUpdateTime(gameTime);
                if (updateTime != 0)    //Temporary
                    spriteBatch.DrawString(arial, "UPS:" + Convert.ToInt32(1 / updateTime), new Vector2(0, 13), new Color(new Vector3(233, 0, 0)));
                
                //If it's selected the planets info gets drawn ontop of the menu's background sprite
                if (isSelected)
                {
                    spriteBatch.Draw(selected.Texture, new Vector2(720, 10));
                    spriteBatch.DrawString(arial, selected.Name, new Vector2(770, 10), new Color(new Vector3(0, 0, 0)));
                    if (!selected.IsStar)
                    {
                        //Acc still does not work properly
                        Planet planet = selected as Planet;
                        spriteBatch.DrawString(arial, "Distance from sun: " + (selected.DetermineDistance(bodies[0]) * Body.scaleMultiplier), new Vector2(720, 70), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Velocity: " + planet.Speed, new Vector2(720, 110), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Acceleration: " + planet.Acceleration, new Vector2(720, 150), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Force: " + planet.Force, new Vector2(720, 190), new Color(new Vector3(0, 0, 0)));
                    }
                }
                spriteBatch.End();
                base.Draw(gameTime);

                oldTotalDrawTime = gameTime.TotalGameTime.TotalSeconds;
                d = 0;
            }
            d += gameTime.ElapsedGameTime.TotalSeconds;
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

        public bool IsMouseInArea(MouseState mousestate, Point position, double Height, double Width)
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
