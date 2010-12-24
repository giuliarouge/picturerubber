using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PictureRubber
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PR_Main : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// The GraphicsDeviceManager
        /// </summary>
        GraphicsDeviceManager m_Graphics;

        /// <summary>
        /// The Spritebatch
        /// </summary>
        SpriteBatch m_SpriteBatch;

        /// <summary>
        /// The Input Manager
        /// </summary>
        PR_InputManager m_InputManager;

        /// <summary>
        /// Initializes a new Instance of PR_Main
        /// </summary>
        public PR_Main()
        {
            this.m_Graphics = new GraphicsDeviceManager(this);
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
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            this.m_InputManager = new PR_InputManager(this);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="_gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime _gameTime)
        {
            this.m_InputManager.HandleInput(_gameTime);                       

            base.Update(_gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="_gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime _gameTime)
        {
            GraphicsDevice.Clear(Color.Black);



            base.Draw(_gameTime);
        }
    }
}
