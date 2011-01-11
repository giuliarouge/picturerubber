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
        /// The Renderer for rubbing-areas
        /// </summary>
        private PR_Renderer m_RubberRenderer;

        /// <summary>
        /// The Renderer to calculate the black/white texture
        /// </summary>
        private PR_Renderer m_MouseTextureRenderer;

        /// <summary>
        /// The Intro
        /// </summary>
        private PR_Intro m_Intro;

        /// <summary>
        /// specifics, if the intro will be played
        /// </summary>
        private bool m_PlayIntro;

        /// <summary>
        /// The Build Modes
        /// </summary>
        public enum Modus
        {
            Debug,
            Release
        };

        private PR_MainMenu m_MainMenu;

        private Texture2D test;

        /// <summary>
        /// speficis, if there was a new rubbing-gesture
        /// </summary>
        private bool m_CreateMouseTexture;

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
            this.m_PlayIntro = false;
            this.m_CreateMouseTexture = false;
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
            m_Graphics.PreferredBackBufferHeight = 768;
            if (this.m_Modus == Modus.Debug)
            {
                m_Graphics.PreferredBackBufferWidth = 1280;
            }
            else
            {
                m_Graphics.PreferredBackBufferWidth = 1024;
            }
            m_Graphics.ApplyChanges();
            this.m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            this.m_Kinect = new PR_Kinect(this);
            
            m_InputManager = new PR_InputManager(this,this.m_Kinect);

            m_MainMenu = new PR_MainMenu(this, this.m_InputManager);
            
            this.m_Pictures = new PR_Pictures(this, "Images",this.m_Kinect);
            this.m_Mouse = new PR_Mouse(this, this.m_InputManager);
            this.m_RubberRenderer = new PR_Renderer("AlphaFader", "AlphaFader", this.m_Graphics.GraphicsDevice, this);
            this.m_MouseTextureRenderer = new PR_Renderer("DynamicMouse", "DynamicMouse", this.m_Graphics.GraphicsDevice, this);
            this.m_Intro = new PR_Intro(this, "intro");
            if (this.m_PlayIntro)
            {
                this.m_Intro.Play();
            }
            this.test = Content.Load<Texture2D>("test");
            //this.m_Graphics.IsFullScreen = true;
            //this.m_Graphics.ApplyChanges();
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
            this.m_MainMenu.Update(_gameTime);
            base.Update(_gameTime);
        }

        public void SetMouseWaitingTime(int _waitingTime)
        {
            this.m_Mouse.SetWaitingTime(_waitingTime);
        }

        /// <summary>
        /// get and set the flag to show menu oder game
        /// </summary>
        public bool ShowMenu
        {
            get
            {
                return this.m_MainMenu.m_Visible;
            }
            set
            {
                this.m_MainMenu.m_Visible = value;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="_gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime _gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (this.m_Intro.isActive())
            {
                this.m_Intro.Draw(_gameTime);
            }
            else
            {
                if (this.m_CreateMouseTexture && !this.ShowMenu)
                {
                    //create mouse-texture for rubbing-areas
                    Texture2D blankTexture = new Texture2D(this.m_Graphics.GraphicsDevice, this.m_Graphics.GraphicsDevice.Viewport.Width, this.m_Graphics.GraphicsDevice.Viewport.Height);
                    Texture2D mouseTexture = this.m_Mouse.GetMouseTexture();
                    List<Vector2> positions = this.m_Mouse.GetMousePositions();
                    //set rendertarger
                    this.m_MouseTextureRenderer.SetRenderTarget(ref blankTexture);
                    //calculate rubbing-areas with DynamicMouse-shader
                    foreach(Vector2 position in positions)
                    {
                        this.m_MouseTextureRenderer.CreateMouseTexture(ref blankTexture, mouseTexture, position);
                    }
                    this.m_MouseTextureRenderer.ResetRenderTarget(ref blankTexture);

                    //delete areas AlphaFader-Shader
                    Texture2D[] texture = this.m_Pictures.getTextures();
                    for (int i = 1; i < this.m_Pictures.getTextureCount(); i++)
                    {
                        if (i == this.m_Pictures.getTextureCount() - 1)
                        {
                            this.m_RubberRenderer.ApplyFilter(ref texture[i], blankTexture, 100);
                        }
                        else
                        {
                            this.m_RubberRenderer.ApplyFilter(ref texture[i], blankTexture, 100, texture[i + 1]);
                        }
                    }
                    this.m_CreateMouseTexture = false;
                    this.m_Mouse.ResetMousePositions();
                }
                if (this.m_MainMenu.m_Visible)
                {
                    this.m_MainMenu.Draw(_gameTime);
                }
                else
                {
                    this.m_Pictures.Draw(_gameTime);
                }
                this.m_Mouse.Draw(_gameTime);
            }
            base.Draw(_gameTime);
        }

        public bool IsGesture
        {
            get
            {
                return this.m_CreateMouseTexture;
            }
            set
            {
                this.m_CreateMouseTexture = value;
            }
        }

        public PR_Mouse Mouse
        {
            get
            {
                return this.m_Mouse;
            }
        }
    }
}
