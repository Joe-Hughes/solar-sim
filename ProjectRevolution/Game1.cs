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
        List<Body> bodies = new List<Body>();
        List<Planet> planets = new List<Planet>();
        bool mouseHold = false;
        Vector2 initialPos;
        Vector2 dragVector;
        Dictionary<Planet, List<Vector2>> spriteCache = new Dictionary<Planet, List<Vector2>>();
        int spriteCacheSize = 900;
        bool pause = false;
        SpriteFont arial;
        Rectangle menuBackground = new Rectangle(700, 0, 324, 576);
        Body selected;
        bool isSelected = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
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
            arial = this.Content.Load<SpriteFont>("StandardArial");

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
                            initialPos = mouse.Position.ToVector2();
                            mouseHold = true;
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

                    Planet rngObject = new Planet(5.93 * Math.Pow(10, 25), 8, graphics.GraphicsDevice,
                        initialPos, "Planet" + bodies.Count.ToString(), planetSprite, bodies[0], shootVector);
                    bodies.Add(rngObject);
                    planets.Add(rngObject);
                    spriteCache.Add(rngObject, new List<Vector2>());
                }

                foreach (Planet planet in planets)
                {
                    planet.updateVelocityAndPosition(bodies, gameTime);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //MakeDaPictures
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            foreach (Body body in bodies)
            {
                if (!body.IsStar)
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
                
                spriteBatch.Draw(body.Texture, body.Position);
                if (body == selected)
                {
                    spriteBatch.Draw(markerSprite, body.Position);
                }
            }
            spriteBatch.Draw(menuSprite, menuBackground, new Color(new Vector3(1, 1, 1)));

            if (isSelected)
            {
                spriteBatch.Draw(selected.Texture, new Vector2(720, 10));
                spriteBatch.DrawString(arial, selected.Name, new Vector2(770, 10), new Color(new Vector3(0, 0, 0)));
                if (!selected.IsStar)
                {
                    Planet planet = selected as Planet;
                    spriteBatch.DrawString(arial, "Distance from sun: " + (selected.DetermineDistance(bodies[0]) * Body.scaleMultiplier), new Vector2(720, 70), new Color(new Vector3(0, 0, 0)));
                    spriteBatch.DrawString(arial, "Velocity: " + planet.Speed, new Vector2(720, 110), new Color(new Vector3(0, 0, 0)));
                    spriteBatch.DrawString(arial, "Acceleration: " + planet.Acceleration, new Vector2(720, 150), new Color(new Vector3(0, 0, 0)));
                    spriteBatch.DrawString(arial, "Force: " + planet.Force, new Vector2(720, 190), new Color(new Vector3(0, 0, 0)));
                }
            }

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
