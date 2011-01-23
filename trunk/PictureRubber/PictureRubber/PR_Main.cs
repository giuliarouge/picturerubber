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
        #region declaration of variables
        //constants
        /// <summary>
        /// width of our application
        /// </summary>
        private const int c_AppWidth = 640;

        /// <summary>
        /// height of our application
        /// </summary>
        private const int c_AppHeight = 480;
        
        /// <summary>
        /// The GraphicsDeviceManager
        /// </summary>
        public GraphicsDeviceManager m_Graphics;

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
        /// enumeration-type to specific the way, how the DynamicMouse-Shader works
        /// </summary>
        public enum RubberModus
        {
            Realtime,
            Path
        };

        /// <summary>
        /// variable which specify the current shader modus
        /// </summary>
        public RubberModus m_MouseShaderModus;

        /// <summary>
        /// instance of PR_GestureHandler to handle gesture input
        /// </summary>
        private PR_GestureHandler m_GestureHandler;

        /// <summary>
        /// The Intro
        /// </summary>
        private PR_Intro m_Intro;

        /// <summary>
        /// specify, if the intro will be played
        /// </summary>
        private bool m_PlayIntro;

        /// <summary>
        /// specify if an rendertarget is set or not
        /// </summary>
        private bool m_InitBlankTexture;

        /// <summary>
        /// specify if an rubbing-gesture ist running
        /// </summary>
        private bool m_IsGestureRunning;

        /// <summary>
        /// instance of menu
        /// </summary>
        private PR_Menu m_Menu;

        /// <summary>
        /// speficies, if there was a new rubbing-gesture
        /// </summary>
        private bool m_CreateMouseTexture;

        /// <summary>
        /// instance of PR_Nite to manage the Kinect
        /// </summary>
        private PR_Nite m_Nite;

        /// <summary>
        /// specify if the Kinect is connected or not
        /// </summary>
        public bool m_IsKinectConnected;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static PR_Main m_Object;

        #endregion

        /// <summary>
        /// Initializes a new Instance of PR_Main
        /// </summary>
        private PR_Main()
        {
            this.m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.m_PlayIntro = false;
            this.m_CreateMouseTexture = false;
            this.m_IsGestureRunning = false;
            this.m_InitBlankTexture = true;
            this.ShaderModus = RubberModus.Realtime;
        }        

        /// <summary>
        /// static function to get only one isntance of PR_Main
        /// </summary>
        /// <returns></returns>
        static public PR_Main GetInstance()
        {
            if (m_Object == null)
            {
                m_Object = new PR_Main();
            }
            return m_Object;
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
            this.m_IsKinectConnected = false;
            try
            {
                this.m_Nite = new PR_Nite();
                this.m_Nite.NiteInitialize();
                this.m_IsKinectConnected = true;
            }
            catch
            {
                this.m_Nite = null;
                System.Console.WriteLine("Bitte Kinect anschliessen");
            }

            this.m_Graphics.PreferredBackBufferHeight = c_AppHeight;
            this.m_Graphics.PreferredBackBufferWidth = c_AppWidth;
            this.m_Graphics.ApplyChanges();

            this.m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            this.m_InputManager = PR_InputManager.GetInstance();

            this.m_Menu = new PR_Menu(this.m_InputManager);
            this.ShowMenu = true;
            //create our pictures and save textures
            this.m_Pictures = new PR_Pictures("Images");

            this.m_Mouse = new PR_Mouse(this.m_InputManager);

            this.m_GestureHandler = new PR_GestureHandler();
         
            this.m_Intro = new PR_Intro("intro");
            if (this.m_PlayIntro)
            {
                this.m_Intro.Play();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            GC.Collect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="_gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime _gameTime)
        {
            this.m_InputManager.HandleInput(_gameTime);
            this.m_Menu.Update(_gameTime);
            if (!this.ShowMenu)
                this.m_Pictures.Update();
            this.m_GestureHandler.Update();

            base.Update(_gameTime);
        }

        /// <summary>
        /// time for animation between 0 and 360
        /// </summary>
        /// <param name="_waitingTime">wating time</param>
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
                return this.m_Menu.m_Visible;
            }
            set
            {
                this.m_Menu.m_Visible = value;
            }
        }

        public bool ShowOptions
        {
            get
            {
                return this.m_Menu.m_OptionsVisible;
            }
            set
            {
                this.m_Menu.m_OptionsVisible = value;
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
                if (this.ShowMenu)
                {
                    this.m_Menu.Draw(_gameTime);
                }
                else
                {
                    if (this.IsGesture)
                    {
                        //if there was a gesture recognized, delete the areas of the image
                        this.m_GestureHandler.ProcessAlphaFaderShader();
                        this.m_GestureHandler.ResetValues();
                    }
                    //draw our images
                    this.m_Pictures.Draw(_gameTime);

                    if (this.RunningGesture)
                    {
                        this.m_GestureHandler.Draw();
                    }    
                }

                this.m_Mouse.Draw(_gameTime);
            }
            base.Draw(_gameTime);
        }

        /// <summary>
        /// set or get the value which specify if someone tries to rubber
        /// </summary>
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

        /// <summary>
        /// get or set if a gesture is running
        /// </summary>
        public bool RunningGesture
        {
            get
            {
                return this.m_IsGestureRunning;
            }
            set
            {
                this.m_IsGestureRunning = value;
            }
        }

        /// <summary>
        /// get instance of PR_Mouse
        /// </summary>
        public PR_Mouse Mouse
        {
            get
            {
                return this.m_Mouse;
            }
        }

        /// <summary>
        /// get or set if we need to reset the modeltexture
        /// </summary>
        public bool InitBlankTexture
        {
            get
            {
                return this.m_InitBlankTexture;
            }
            set
            {
                this.m_InitBlankTexture = value;
            }
        }

        /// <summary>
        /// gets or sets the current shadermodus
        /// </summary>
        public RubberModus ShaderModus
        {
            get
            {
                return this.m_MouseShaderModus;
            }
            set
            {
                this.m_MouseShaderModus = value;
            }
        }

        /// <summary>
        /// unset kinect
        /// </summary>
        public void DeleteKinect()
        {
            if (this.m_Nite != null)
            {
                this.m_Nite.Stop();
            }
        }

        /// <summary>
        /// get instance of kinect class
        /// </summary>
        public PR_Nite Kinect
        {
            get
            {
                return this.m_Nite;
            }
        }

        /// <summary>
        /// get instance of PR_Pictures class
        /// </summary>
        public PR_Pictures Pictures
        {
            get
            {
                return this.m_Pictures;
            }
        }

        /// <summary>
        /// get instance of PR_GestureHandler class
        /// </summary>
        public PR_GestureHandler Gestures
        {
            get
            {
                return this.m_GestureHandler;
            }
        }

        public PR_Menu Menu
        {
            get
            {
                return this.m_Menu;
            }
        }
    }
}