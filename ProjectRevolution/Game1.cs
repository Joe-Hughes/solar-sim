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
        Texture2D Texture;
        List<Body> bodies = new List<Body>();
        List<Planet> planets = new List<Planet>();

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
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;

            Body sun = new Body(1.99 * Math.Pow(10, 30), 8, graphics.GraphicsDevice, "Sun", Texture);
            Planet earth = new Planet(5.93 * Math.Pow(10, 24), 8, graphics.GraphicsDevice, new Vector2(0, -140), "Earth", Texture, sun, new Vector2(30000, 0));
            //Planet planet2 = new Planet(5.93 * Math.Pow(10, 24), 16, new Vector2(392, 380), "planet2", Texture, sun, new Vector2(-30000, 0));


            //Body earth = new Body(5.23 * Math.Pow(10, 24), new Vector2(392, 92), new Vector2(1, 0), "earth");
            //Body sun = new Body(1.99 * Math.Pow(10, 30), new Vector2(392, 232), new Vector2(0, 0), "sun");
            //Body blah = new Body(1.99 * Math.Pow(10, 24), new Vector2(192, 232), new Vector2(0, -1), "blah");
            //Body bleh = new Body(1.99 * Math.Pow(10, 24), new Vector2(392, 400), new Vector2(-1, 0), "bleh");
            bodies.Add(sun);
            bodies.Add(earth);
            //bodies.Add(planet2);
            //bodies.Add(planet3);
            foreach (Body body in bodies)
            {
                if (!body.IsStar)
                {
                    planets.Add(body as Planet);
                }
            }

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
            Texture = this.Content.Load<Texture2D>(@"CIRCLE");
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

            foreach (Planet planet in planets)
            {
                planet.updateVelocityAndPosition(bodies);
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
            spriteBatch.Begin();
            foreach (Body body in bodies)
            {
                spriteBatch.Draw(Texture, body.Position);
            }
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
