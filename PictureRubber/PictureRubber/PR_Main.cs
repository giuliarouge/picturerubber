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
        //constants
        /// <summary>
        /// specify the alphaamount for alphafader-shader
        /// </summary>
        private const int c_AlphaAmount = 100;

        /// <summary>
        /// width of our application
        /// </summary>
        private const int c_AppWidth = 640;

        /// <summary>
        /// height of our application
        /// </summary>
        private const int c_AppHeight = 480;

        /// <summary>
        /// alpha value for path-modus
        /// </summary>
        private const int c_PathAlpha = 255 / 5;

        /// <summary>
        /// the gap a user needs to go forward to rub areas (in mm)
        /// </summary>
        private const int c_Gap = 200;

        /// <summary>
        /// minimal distance of a user to the kinect
        /// </summary>
        private const int c_MinimalDistance = 1000;

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
        /// copy of our image-textures
        /// </summary>
        private Texture2D[] m_PictureTextures;

        /// <summary>
        /// amount of textures
        /// </summary>
        private int m_TextureCount;

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
        /// texture to generate rubber-areas
        /// </summary>
        private Texture2D m_ModelTexture;

        /// <summary>
        /// real texture of the mouse seen on the screen
        /// </summary>
        private Texture2D m_RealMouseTexture;

        /// <summary>
        /// initial Texture for Mouse-Texture Shader
        /// </summary>
        private Texture2D m_BlankTexture;

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
            this.m_PictureTextures = this.m_Pictures.getTextures();
            this.m_TextureCount = this.m_PictureTextures.Count();

            this.m_Mouse = new PR_Mouse(this.m_InputManager);

            //create textures for shader and rubbing-araes
            this.m_BlankTexture = new Texture2D(
                this.m_Graphics.GraphicsDevice,
                this.m_Graphics.GraphicsDevice.Viewport.Width,
                this.m_Graphics.GraphicsDevice.Viewport.Height);
            this.m_ModelTexture = this.m_BlankTexture;
            this.m_RealMouseTexture = this.m_Mouse.GetMouseTexture();

            //initialize renderer
            this.m_RubberRenderer = new PR_Renderer("AlphaFader", "AlphaFader", this.m_Graphics.GraphicsDevice);
            this.m_MouseTextureRenderer = new PR_Renderer("DynamicMouse", "DynamicMouse", this.m_Graphics.GraphicsDevice);            
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

            if ((this.IsGesture && this.m_MouseShaderModus == RubberModus.Realtime) ||
                (this.RunningGesture && this.m_MouseShaderModus == RubberModus.Path))
            {
                if (this.m_InitBlankTexture && this.m_MouseShaderModus == RubberModus.Path)
                {
                    //draw a path, where the user wants to delete something of the image
                    this.m_ModelTexture = this.m_BlankTexture;
                    this.m_InitBlankTexture = false;
                }
                else if (this.m_MouseShaderModus == RubberModus.Realtime)
                {
                    //reset m_BlankTexture every update-call
                    this.m_ModelTexture = this.m_BlankTexture;
                }
                //process DynamicMouse-Shader to calculate the texture
                this.m_MouseTextureRenderer.SetRenderTarget(this.m_ModelTexture);
                this.m_MouseTextureRenderer.CreateMouseTexture(ref this.m_ModelTexture,
                    this.m_RealMouseTexture,
                    this.m_InputManager.GetMousePosition());
                this.m_MouseTextureRenderer.ResetRenderTarget(ref this.m_ModelTexture);
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
                        this.ProcessAlphaFaderShader();
                        this.ResetValues();
                    }
                    //draw our images
                    this.m_Pictures.Draw(_gameTime);

                    if (this.RunningGesture)
                    {
                        //draw a path, where the user wants to delete something of the image
                        this.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        this.m_SpriteBatch.Draw(this.m_ModelTexture,
                            new Rectangle(0, 0, this.m_ModelTexture.Width, this.m_ModelTexture.Height),
                            new Color(0, 0, 0, c_PathAlpha));
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
            if (this.m_IsKinectConnected && this.m_MouseShaderModus == RubberModus.Realtime)
            {
                int index = this.CalculateTextureIndex();
                if (index < this.m_TextureCount)
                {
                    if (index == this.m_TextureCount - 1)
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[index], this.m_ModelTexture, c_AlphaAmount);
                    }
                    else
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[index], this.m_ModelTexture, c_AlphaAmount, this.m_PictureTextures[index + 1]);
                    }
                }
            }
            else
            {
                for (int i = 1; i < this.m_TextureCount; ++i)
                {
                    if (i == this.m_TextureCount - 1)
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[i], this.m_ModelTexture, c_AlphaAmount);
                    }
                    else
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[i], this.m_ModelTexture, c_AlphaAmount, this.m_PictureTextures[i + 1]);
                    }
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
            if (this.m_MouseShaderModus == RubberModus.Path)
            {
                this.m_InitBlankTexture = true;
                this.RunningGesture = false;
            }
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
            if (this.m_Nite != null)
            {
                this.m_Nite.Stop();
            }
        }

        /// <summary>
        /// calculate the texture index a user is actuallz working on
        /// </summary>
        /// <returns></returns>
        private int CalculateTextureIndex()
        {
            //initial z value == distance from kinect
            int ZValue = 2000 - c_Gap;//this.m_Nite.GetInitialZValue()
            //actual z value
            int ActualZValue = this.ActualZ - c_MinimalDistance;
            int Area = (ZValue - c_MinimalDistance) / (this.m_TextureCount-1);
            //return the index of the texture a user is working on
            if (ActualZValue % Area == 0)
            {
                return ActualZValue / Area;
            }
            return (ActualZValue / Area) + 1;
        }

        /// <summary>
        /// get actual z-value from kinect
        /// </summary>
        public int ActualZ
        {
            get
            {
                return this.m_Nite.ActualZ;
            }
        }

        /// <summary>
        /// get the z-value where a user is able to delete ares in realtime-mode
        /// </summary>
        public int InitialZValue
        {
            get
            {
                return 2000 - c_Gap;
            }
        }
    }
}