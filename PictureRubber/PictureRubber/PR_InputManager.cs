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
        /// instance of Kinect class
        /// </summary>
        private PR_Kinect m_Kinect;

        /// <summary>
        /// Initializes a new Instance of PR_InputManager
        /// </summary>
        /// <param name="_root">The Root Pointer</param>
        public PR_InputManager(PR_Main _root, PR_Kinect _kinect)
        {
            this.m_Root = _root;
            this.m_Kinect = _kinect;
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
                this.m_Kinect.DeleteKinect();
                this.m_Root.Exit();
            }

            if (this.m_ActualKeyboardState.IsKeyDown(Keys.S) && this.m_LastKeyboardState.IsKeyUp(Keys.S))
            {
                this.m_Root.ShowMenu = false;
            }

            if (this.GetMousePosition().X >= 0 &&
                this.GetMousePosition().X <= this.m_Root.GraphicsDevice.Viewport.Width &&
                this.GetMousePosition().Y >= 0 &&
                this.GetMousePosition().Y <= this.m_Root.GraphicsDevice.Viewport.Height &&
                this.m_ActualMouseState.LeftButton == ButtonState.Pressed && !this.m_Root.ShowMenu)
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
            if (this.m_ActualMouseState.LeftButton == ButtonState.Released &&
                this.m_LastMouseState.LeftButton == ButtonState.Pressed)
            {
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
                    this.m_Kinect.DeleteKinect();
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
    }
}
