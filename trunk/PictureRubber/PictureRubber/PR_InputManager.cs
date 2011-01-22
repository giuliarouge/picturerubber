using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    public class PR_InputManager
    {
        /// <summary>
        /// enum for status of kinect gesture
        /// </summary>
        public enum GestureState
        {
            Before,
            While,
            After
        };

        private GestureState m_GestureState;

        /// <summary>
        /// The Root Pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// The last Keyboard State
        /// </summary>
        private KeyboardState m_LastKeyboardState;

        /// <summary>
        /// The last Mouse State
        /// </summary>
        private MouseState m_LastMouseState;

        /// <summary>
        /// The current Keyboard State
        /// </summary>
        private KeyboardState m_CurrentKeyboardState;

        /// <summary>
        /// The current Mouse State
        /// </summary>
        private MouseState m_CurrentMouseState;

        /// <summary>
        /// object for singleton
        /// </summary>
        private static PR_InputManager m_Object;

        /// <summary>
        /// Initializes a new Instance of PR_InputManager
        /// </summary>
        /// <param name="_root">The Root Pointer</param>
        private PR_InputManager()
        {
            this.m_Root = PR_Main.GetInstance();
            this.m_GestureState = GestureState.Before;
        }

        /// <summary>
        /// static function to get only one isntance of PR_Main
        /// </summary>
        /// <returns></returns>
        static public PR_InputManager GetInstance()
        {
            if (m_Object == null)
            {
                m_Object = new PR_InputManager();
            }
            return m_Object;
        }

        /// <summary>
        /// get current mousestate
        /// </summary>
        /// <returns>m_currentMouseState</returns>
        public MouseState GetCurrentMouseState()
        {
            return this.m_CurrentMouseState;
        }

        /// <summary>
        /// get last mousestate
        /// </summary>
        /// <returns>m_LastMouseState</returns>
        public MouseState GetLastMouseState()
        {
            return this.m_LastMouseState;
        }
        
        public void HandleInput(GameTime _gameTime)
        {
            this.m_CurrentKeyboardState = Keyboard.GetState();
            this.m_CurrentMouseState = Mouse.GetState();

            //Exiting of Program
            if (this.m_CurrentKeyboardState.IsKeyDown(Keys.Escape))
            {
                this.m_Root.DeleteKinect();
                this.m_Root.Exit();
            }

            //manually start game
            if (this.m_CurrentKeyboardState.IsKeyDown(Keys.S) && this.m_LastKeyboardState.IsKeyUp(Keys.S))
            {
                this.m_Root.ShowMenu = false;
            }

            //rubbing gesture (start)
            if ((this.IsMouseInScreen()) &&
                ((this.m_CurrentMouseState.LeftButton == ButtonState.Pressed && !this.m_Root.ShowMenu) ||
                (this.m_GestureState == GestureState.While && !this.m_Root.ShowMenu) ||
                (this.m_Root.m_IsKinectConnected && this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime && this.m_Root.Gestures.currentZ < this.m_Root.Gestures.KinectDistance)))
            {
                if (this.m_Root.ShaderModus == PR_Main.RubberModus.Path)
                {
                    this.m_Root.RunningGesture = true;
                }
                else if (this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime)
                {
                    this.m_Root.IsGesture = true;
                }
            }

            //rubbing gesture (end)
            if ((this.IsMouseInScreen()) &&
                (this.m_CurrentMouseState.LeftButton == ButtonState.Released &&
                this.m_LastMouseState.LeftButton == ButtonState.Pressed) ||
                this.m_GestureState == GestureState.After)
            {
                this.m_GestureState = GestureState.Before;
                if (this.m_Root.ShaderModus == PR_Main.RubberModus.Path)
                {
                    this.m_Root.IsGesture = true;
                }
                else if (this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime)
                {
                    this.m_Root.IsGesture = false;
                }
            }
            //safe mouse and keyboard states
            this.m_LastKeyboardState = this.m_CurrentKeyboardState;
            this.m_LastMouseState = this.m_CurrentMouseState;
        }

        /// <summary>
        /// checks if the current mouse-position is inside the window
        /// </summary>
        /// <returns>true or false</returns>
        private bool IsMouseInScreen()
        {
            Vector2 MousePosition = this.GetMousePosition();
            return MousePosition.X >= 0 &&
                MousePosition.X <= this.m_Root.GraphicsDevice.Viewport.Width &&
                MousePosition.Y >= 0 &&
                MousePosition.Y <= this.m_Root.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// handle main-menu input
        /// </summary>
        /// <param name="_index">index of option</param>
        public void HandleMenuInput(int _index)
        {
            switch (_index)
            {
                case 0:
                    this.m_Root.ShowMenu = false;
                    break;
                case 1:
                    this.m_Root.ShowOptions = true;
                    break;
                case 2:
                    this.m_Root.DeleteKinect();
                    this.m_Root.Exit();
                    break;
            }
        }

        /// <summary>
        /// handle option-menu input
        /// </summary>
        /// <param name="_index">index of option</param>
        public void HandleOptionsInput(int _index)
        {
            switch (_index)
            {
                case 0:
                    //Resolution 640x480
                    this.m_Root.m_Graphics.PreferredBackBufferWidth = 640;
                    this.m_Root.m_Graphics.PreferredBackBufferHeight = 480;
                    this.m_Root.m_Graphics.ApplyChanges();
                    this.m_Root.Mouse.RescaleTexture();
                    this.m_Root.Menu.RescaleMenuElements();
                    this.m_Root.Gestures.RescaleElements();
                    break;
                case 1:
                    //Resolution 800x600
                    this.m_Root.m_Graphics.PreferredBackBufferWidth = 800;
                    this.m_Root.m_Graphics.PreferredBackBufferHeight = 600;
                    this.m_Root.m_Graphics.ApplyChanges();
                    this.m_Root.Mouse.RescaleTexture();
                    this.m_Root.Menu.RescaleMenuElements();
                    this.m_Root.Gestures.RescaleElements();
                    break;
                case 2:
                    //Resolution 1024x768
                    this.m_Root.m_Graphics.PreferredBackBufferWidth = 1024;
                    this.m_Root.m_Graphics.PreferredBackBufferHeight = 768;
                    this.m_Root.m_Graphics.ApplyChanges();
                    this.m_Root.Mouse.RescaleTexture();
                    this.m_Root.Menu.RescaleMenuElements();
                    this.m_Root.Gestures.RescaleElements();
                    break;
                case 3:
                    //Fenstermodus
                    if (this.m_Root.m_Graphics.IsFullScreen)
                    {
                        this.m_Root.m_Graphics.ToggleFullScreen();
                        this.m_Root.m_Graphics.ApplyChanges();
                        this.m_Root.Mouse.RescaleTexture();
                    }                        
                    break;
                case 4:
                    //VollbildModus
                    if (!this.m_Root.m_Graphics.IsFullScreen)
                    {
                        this.m_Root.m_Graphics.ToggleFullScreen();
                        this.m_Root.m_Graphics.ApplyChanges();
                        this.m_Root.Mouse.RescaleTexture();
                    }  
                    break;
                case 5:
                    //EchtzeitModus
                    this.m_Root.m_MouseShaderModus = PR_Main.RubberModus.Realtime;
                    break;
                case 6:
                    //PfadModus
                    this.m_Root.m_MouseShaderModus = PR_Main.RubberModus.Path;
                    break;
                case 7:
                    //Zurueck
                    this.m_Root.ShowOptions = false;
                    break;
            }
        }

        /// <summary>
        /// get current mouse-position on the screen
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePosition()
        {
            return new Vector2(this.m_CurrentMouseState.X, this.m_CurrentMouseState.Y);
        }

        /// <summary>
        /// set or get status of kinect gesture
        /// </summary>
        public void UpdateGesture()
        {
            switch (this.m_GestureState)
            {
                case GestureState.Before: 
                    this.m_GestureState = GestureState.While;
                    break;
                case GestureState.While:
                    this.m_GestureState = GestureState.After;
                    break;
            }
        }
    }
}
