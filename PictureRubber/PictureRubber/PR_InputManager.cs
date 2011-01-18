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
        /// The actual Keyboard State
        /// </summary>
        private KeyboardState m_ActualKeyboardState;

        /// <summary>
        /// The actual Mouse State
        /// </summary>
        private MouseState m_ActualMouseState;

        /// <summary>
        /// kinect gesture
        /// </summary>
        private bool m_KinectGestureRecognized;

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

        static public PR_InputManager GetInstance()
        {
            if (m_Object == null)
            {
                m_Object = new PR_InputManager();
            }
            return m_Object;
        }

        /// <summary>
        /// get actual mousestate
        /// </summary>
        /// <returns>m_ActualMouseState</returns>
        public MouseState GetActualMouseState()
        {
            return this.m_ActualMouseState;
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
            this.m_ActualKeyboardState = Keyboard.GetState();
            this.m_ActualMouseState = Mouse.GetState();

            //Exiting of Program
            if (this.m_ActualKeyboardState.IsKeyDown(Keys.Escape))
            {
                this.m_Root.DeleteKinect();
                this.m_Root.Exit();
            }

            //manually start game
            if (this.m_ActualKeyboardState.IsKeyDown(Keys.S) && this.m_LastKeyboardState.IsKeyUp(Keys.S))
            {
                this.m_Root.ShowMenu = false;
            }

            //rubbing gesture (start)
            if ((this.GetMousePosition().X >= 0 &&
                this.GetMousePosition().X <= this.m_Root.GraphicsDevice.Viewport.Width &&
                this.GetMousePosition().Y >= 0 &&
                this.GetMousePosition().Y <= this.m_Root.GraphicsDevice.Viewport.Height) &&
                ((this.m_ActualMouseState.LeftButton == ButtonState.Pressed && !this.m_Root.ShowMenu) ||
                this.m_GestureState == GestureState.While))
            {
                if (this.m_Root.ShaderModus == PR_Main.RubberModus.Path)
                {
                    this.m_Root.RunningGesture = true;
                }
                else if (this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime)
                {
                    this.m_Root.IsGesture = true;
                }
                this.m_Root.Mouse.MousePosition = this.GetMousePosition();
            }

            //rubbing gesture (end)
            if ((this.m_ActualMouseState.LeftButton == ButtonState.Released &&
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

            this.m_LastKeyboardState = this.m_ActualKeyboardState;
            this.m_LastMouseState = this.m_ActualMouseState;
        }

        public void HandleMenuInput(int _index)
        {
            //do something
            switch (_index)
            {
                case 0:
                    this.m_Root.ShowMenu = false;
                    break;
                case 2:
                    this.m_Root.DeleteKinect();
                    this.m_Root.Exit();
                    break;
            }
        }

        /// <summary>
        /// get actual mouse-position on the screen
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePosition()
        {
            return new Vector2(this.m_ActualMouseState.X, this.m_ActualMouseState.Y);
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
