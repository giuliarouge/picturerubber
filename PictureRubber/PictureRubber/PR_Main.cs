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
using System.IO;

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
        /// The Renderer for rubbing-areas
        /// </summary>
        private PR_Renderer m_RubberRenderer;

        /// <summary>
        /// The Renderer to calculate the black/white texture
        /// </summary>
        private PR_Renderer m_MouseTextureRenderer;

        /// <summary>
        /// initial Texture for Mouse-Texture Shader
        /// </summary>
        private Texture2D m_MouseTexture;

        /// <summary>
        /// real texture of the mouse seen on the screen
        /// </summary>
        private Texture2D m_RealMouseTexture;

        /// <summary>
        /// texture to generate rubber-areas
        /// </summary>
        private Texture2D m_BlankTexture;

        /// <summary>
        /// specify the alphaamount for alphafader-shader
        /// </summary>
        private const int m_AlphaAmount = 100;

        /// <summary>
        /// enumeration-type to specific the way, how the DynamicMouse-Shader works
        /// </summary>
        public enum RubberModus
        {
            Realtime,
            Path
        };

        /// <summary>
        /// variable which specify the actual shader modus
        /// </summary>
        private RubberModus m_MouseShaderModus;

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
        /// speficis, if there was a new rubbing-gesture
        /// </summary>
        private bool m_CreateMouseTexture;

        /// <summary>
        /// 
        /// </summary>
        private PR_Nite m_Nite;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static PR_Main m_Object;

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
            this.ShaderModus = RubberModus.Path;
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
            try
            {
                this.m_Nite = new PR_Nite();
                this.m_Nite.NiteInitialize();
            }
            catch
            {
                System.Console.WriteLine("Bitte Kinect anschliessen");
            }

            this.m_Graphics.PreferredBackBufferHeight = 480;
            this.m_Graphics.PreferredBackBufferWidth = 640;

            this.m_Graphics.ApplyChanges();
            this.m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            this.m_InputManager = PR_InputManager.GetInstance();

            this.m_Menu = new PR_Menu(this.m_InputManager);
            this.ShowMenu = true;
            this.m_Pictures = new PR_Pictures("Images");
            this.m_Mouse = new PR_Mouse(this.m_InputManager);

            //initialize renderer
            this.m_RubberRenderer = new PR_Renderer("AlphaFader", "AlphaFader", this.m_Graphics.GraphicsDevice);
            this.m_MouseTextureRenderer = new PR_Renderer("DynamicMouse", "DynamicMouse", this.m_Graphics.GraphicsDevice);
            //create textures for shader and rubbing-araes
            this.m_MouseTexture = new Texture2D(
                this.m_Graphics.GraphicsDevice,
                this.m_Graphics.GraphicsDevice.Viewport.Width,
                this.m_Graphics.GraphicsDevice.Viewport.Height);
            this.m_BlankTexture = this.m_MouseTexture;
            this.m_RealMouseTexture = this.m_Mouse.GetMouseTexture();

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

            if (this.IsGesture && this.m_MouseShaderModus == RubberModus.Realtime ||
                this.RunningGesture && this.m_MouseShaderModus == RubberModus.Path)
            {
                if (this.m_InitBlankTexture && this.m_MouseShaderModus == RubberModus.Path)
                {
                    //draw a path, where the user wants to delete something of the image
                    this.m_BlankTexture = this.m_MouseTexture;
                    this.m_InitBlankTexture = false;
                }
                else if (this.m_MouseShaderModus == RubberModus.Realtime)
                {
                    //reset m_BlankTexture every update-call
                    this.m_BlankTexture = this.m_MouseTexture;
                }
                //process DynamicMouse-Shader to calculate the texture
                this.m_MouseTextureRenderer.SetRenderTarget(this.m_BlankTexture);
                this.m_MouseTextureRenderer.CreateMouseTexture(ref this.m_BlankTexture,
                    this.m_RealMouseTexture,
                    this.m_Mouse.GetMousePositions().ElementAt(this.m_Mouse.GetMousePositions().Count - 1));
                this.m_MouseTextureRenderer.ResetRenderTarget(ref this.m_BlankTexture);
            }

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
                        if (this.m_MouseShaderModus == RubberModus.Path)
                        {
                            this.m_InitBlankTexture = true;
                            this.RunningGesture = false;
                        }
                        this.ProcessAlphaFaderShader();
                        this.ResetValues();
                    }

                    this.m_Pictures.Draw(_gameTime);
                    if (this.RunningGesture)
                    {
                        //draw a path, where the user wants to delete something of the image
                        this.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        this.m_SpriteBatch.Draw(this.m_BlankTexture,
                            new Rectangle(0, 0, this.m_BlankTexture.Width, this.m_BlankTexture.Height),
                            new Color(0, 0, 0, 255 / 5));
                        this.m_SpriteBatch.End();
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
        /// process the alphafader shader to clean the ares
        /// </summary>
        private void ProcessAlphaFaderShader()
        {
            //delete areas AlphaFader-Shader
            Texture2D[] texture = this.m_Pictures.getTextures();
            for (int i = 1; i < texture.Count(); i++)
            {
                if (i == texture.Count() - 1)
                {
                    this.m_RubberRenderer.ApplyFilter(ref texture[i], this.m_BlankTexture, m_AlphaAmount);
                }
                else
                {
                    this.m_RubberRenderer.ApplyFilter(ref texture[i], this.m_BlankTexture, m_AlphaAmount, texture[i + 1]);
                }
            }
        }

        /// <summary>
        /// reset values and call the garbage collector
        /// </summary>
        private void ResetValues()
        {
            //reset values
            this.IsGesture = false;
            this.m_Mouse.ResetMousePositions();
            GC.Collect();
        }

        /// <summary>
        /// gets or sets the actual shadermodus
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
            this.m_Nite.Stop();
        }
    }
}