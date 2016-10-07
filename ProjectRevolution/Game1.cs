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
        List<Body> bodies = new List<Body>();
        List<Planet> planets = new List<Planet>();
        bool mouseHold = false;
        Vector2 initialPos;
        Vector2 dragVector;
        Dictionary<Planet, List<Vector2>> spriteCache = new Dictionary<Planet, List<Vector2>>();
        int spriteCacheSize = 3000;
        bool pause = false;
        SpriteFont arial;
        Rectangle menuBackground;
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
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = 1366;  // Window width (1024)
            graphics.PreferredBackBufferHeight = 768;   // Window height (576)
            //graphics.ToggleFullScreen();
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
            planetSprite = this.Content.Load<Texture2D>(@"CIRCLE");
            starSprite = this.Content.Load<Texture2D>(@"STAR");
            tailSprite = this.Content.Load<Texture2D>(@"TAIL");
            menuSprite = this.Content.Load<Texture2D>(@"MENU");
            markerSprite = this.Content.Load<Texture2D>(@"MARKER");
            txtBoxSprite = this.Content.Load<Texture2D>(@"TXTBOX");
            arial = this.Content.Load<SpriteFont>("StandardArial");

            menuBackground = new Rectangle(graphics.PreferredBackBufferWidth - 324, 0, 324, graphics.PreferredBackBufferHeight);

            // Skapar kroppar och lägger in dem i systemet
            Body sun = new Body(1.9885 * Math.Pow(10, 30), "Sun", starSprite, graphics.GraphicsDevice);
            bodies.Add(sun);

            Planet mercury = new Planet(0.330 * Math.Pow(10, 24), "Mercury", 57.9, 90, 0, 47.4, planetSprite, sun, graphics.GraphicsDevice);
            bodies.Add(mercury);

            Planet earth = new Planet(5.9724 * Math.Pow(10, 24), "Earth", 149.6, 90, 0, 29.8, planetSprite, sun, graphics.GraphicsDevice);
            bodies.Add(earth);

            Planet mars = new Planet(0.64171 * Math.Pow(10, 24), "Mars", 227.9, 90, 0, 24.1, planetSprite, sun, graphics.GraphicsDevice);
            bodies.Add(mars);

            Planet jupiter = new Planet(1898 * Math.Pow(10, 24), "Jupiter", 778.6, 90, 0, 13.1, planetSprite, sun, graphics.GraphicsDevice);
            bodies.Add(jupiter);

            Planet saturn = new Planet(568 * Math.Pow(10, 24), "Saturn", 1433.5, 90, 0, 9.7, planetSprite, sun, graphics.GraphicsDevice);
            bodies.Add(saturn);

            Planet uranus = new Planet(86.8 * Math.Pow(10, 24), "Uranus", 2872.5, 90, 0, 6.8, planetSprite, sun, graphics.GraphicsDevice);
            bodies.Add(uranus);

            Planet neptune = new Planet(102 * Math.Pow(10, 24), "Neptune", 4495.1, 90, 0, 5.4, planetSprite, sun, graphics.GraphicsDevice);
            bodies.Add(neptune);

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
                if (!pause)
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        {
                            if (mouseHold)
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
                    }
                    else if (mouseHold == true && mouse.LeftButton == ButtonState.Released)
                    {
                        mouseHold = false;
                        Vector2 shootVector = new Vector2(dragVector.X * 100, dragVector.Y * 100);

                        initialPos.X = initialPos.X - Body.GetCenter(graphics.GraphicsDevice).X;
                        initialPos.Y = initialPos.Y - Body.GetCenter(graphics.GraphicsDevice).Y;

                        double mass = bodies[1].Mass; // Jordens massa
                        string name = "Planet" + bodies.Count.ToString();

                        Planet rngObject = new Planet(mass, name, initialPos, shootVector, planetSprite, bodies[0], graphics.GraphicsDevice);
                        Console.WriteLine("Planet added at: " + rngObject.Position);
                        bodies.Add(rngObject);
                        planets.Add(rngObject);
                        spriteCache.Add(rngObject, new List<Vector2>());
                    }

                    foreach (Planet planet in planets)
                    {
                        planet.updateVelocityAndPosition(bodies, IrlTotalUpdateTime(gameTime));
                    }
                }
                //Console.WriteLine("Update: " + gameTime.ElapsedGameTime.TotalSeconds.ToString() + "   :   " + IrlTotalUpdateTime(gameTime));
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
            if (d >= 0.0167)
            {
                //MakeDaPictures
                graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                foreach (Body body in bodies)
                {
                    if (!body.IsStar)
                    {
                        Planet planet = body as Planet;
                        spriteCache[planet].Add(planet.Position);
                        // DEBUG
                        //if (planet.Name == "Neptune")
                        //{
                        //    Vector2 center = Body.GetCenter(graphics.GraphicsDevice);
                        //    Console.WriteLine(planet.Name + " X: " + (planet.Position.X - center.X + planet.radius) + " Y: " + (planet.Position.Y - center.Y + planet.radius));
                        //}
                        if (spriteCache[planet].Count >= spriteCacheSize)
                        {
                            spriteCache[planet].RemoveAt(0);
                        }
                        foreach (Vector2 position in spriteCache[planet])
                        {
                            spriteBatch.Draw(tailSprite, position);
                        }
                    }

                    spriteBatch.Draw(body.Texture, body.Position);
                    if (body == selected)
                    {
                        spriteBatch.Draw(markerSprite, new Vector2(body.Position.X - 3, body.Position.Y - 3));
                    }
                }
                spriteBatch.Draw(menuSprite, null, menuBackground);
                spriteBatch.DrawString(arial, "FPS:" + Convert.ToInt32(1 / IrlTotalDrawTime(gameTime)), new Vector2(0, 0), new Color(new Vector3(233, 0, 0)));
                double updateTime = IrlTotalUpdateTime(gameTime);
                if (updateTime != 0)
                    spriteBatch.DrawString(arial, "UPS:" + Convert.ToInt32(1 / updateTime), new Vector2(0, 13), new Color(new Vector3(233, 0, 0)));
                

                if (isSelected)
                {
                    // Den horisontella positionen där text skrivs ut i menyn
                    float horizontalTextPosition = graphics.PreferredBackBufferWidth - menuBackground.Width + 10;
                    spriteBatch.Draw(selected.Texture, new Vector2(horizontalTextPosition, 10));
                    spriteBatch.DrawString(arial, selected.Name, new Vector2(horizontalTextPosition + 30, 10), new Color(new Vector3(0, 0, 0)));
                    if (!selected.IsStar)
                    {
                        //Acc still does not work properly
                        Planet planet = selected as Planet;
                        spriteBatch.DrawString(arial, "Distance from sun: " + (selected.DetermineDistance(bodies[0]) * Body.scaleMultiplier), new Vector2(horizontalTextPosition, 70), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Velocity: " + planet.Speed, new Vector2(horizontalTextPosition, 110), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Acceleration: " + planet.Acceleration, new Vector2(horizontalTextPosition, 150), new Color(new Vector3(0, 0, 0)));
                        spriteBatch.DrawString(arial, "Force: " + planet.Force, new Vector2(horizontalTextPosition, 190), new Color(new Vector3(0, 0, 0)));
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
            return gametime.TotalGameTime.TotalSeconds - oldTotalUpdateTime;
        }

        public double IrlTotalDrawTime(GameTime gametime)
        {
            return gametime.TotalGameTime.TotalSeconds - oldTotalDrawTime;
        }
    }
}
