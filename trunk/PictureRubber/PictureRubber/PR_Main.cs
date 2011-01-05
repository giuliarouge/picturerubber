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
        private GraphicsDeviceManager m_Graphics;

        /// <summary>
        /// The Spritebatch
        /// </summary>
        public SpriteBatch m_SpriteBatch;

        /// <summary>
        /// The Input Manager
        /// </summary>
        private PR_InputManager m_InputManager;

        /// <summary>
        /// The Pictures
        /// </summary>
        private PR_Pictures m_Pictures;

        /// <summary>
        /// The Mouse
        /// </summary>
        private PR_Mouse m_Mouse;

        /// <summary>
        /// The Kinect
        /// </summary>
        public PR_Kinect m_Kinect;

        /// <summary>
        /// The Renderer
        /// </summary>
        private PR_Renderer m_Renderer;

        /// <summary>
        /// The Build Modes
        /// </summary>
        public enum Modus
        {
            Debug,
            Release
        };

        private Texture2D test;
        public bool shader;

        public void DeletePicture()
        {
            this.m_Pictures.DeleteLastPicture();
        }


        /// <summary>
        /// Variable for Build Mode Selection
        /// </summary>
        public Modus m_Modus;
        
        /// <summary>
        /// Initializes a new Instance of PR_Main
        /// </summary>
        public PR_Main()
        {
            this.m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.m_Modus = Modus.Release;
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
        [STAThread()]
        protected override void LoadContent()
        {
            m_Graphics.PreferredBackBufferHeight = 480;
            if (this.m_Modus == Modus.Debug)
            {
                m_Graphics.PreferredBackBufferWidth = 1280;
            }
            else
            {
                m_Graphics.PreferredBackBufferWidth = 640;
            }
            m_Graphics.ApplyChanges();
            this.m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            this.m_Kinect = new PR_Kinect(this);
            
            m_InputManager = new PR_InputManager(this,this.m_Kinect);

            
            
            this.m_Pictures = new PR_Pictures(this, "Images",this.m_Kinect);
            this.m_Mouse = new PR_Mouse(this, this.m_InputManager);
            this.m_Renderer = new PR_Renderer("AlphaFader", "AlphaFader", this.m_Graphics.GraphicsDevice, this);
            this.test = Content.Load<Texture2D>("test");
            this.shader = false;
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

            if (this.shader)
            {
                Texture2D texture = this.m_Pictures.getFirstTexture();
                this.m_Pictures.setFirstTexture(
                    this.m_Renderer.ApplyFilter(texture, this.test, 100));
                this.shader = false;
            }

            this.m_Pictures.Draw(_gameTime);
            this.m_Mouse.Draw(_gameTime);
            base.Draw(_gameTime);
        }
    }
}
