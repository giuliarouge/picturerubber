using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    class PR_InputManager
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

        public MouseState GetMouseState()
        {
            return this.m_ActualMouseState;
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
            if (this.m_ActualKeyboardState.IsKeyDown(Keys.B) && this.m_LastKeyboardState.IsKeyUp(Keys.B))
            {
                this.m_Root.shader = true;
            }
            if (this.m_ActualKeyboardState.IsKeyDown(Keys.V))
            {
                this.m_Root.shader = false;
            }

            if (this.m_ActualKeyboardState.IsKeyDown(Keys.R) && this.m_LastKeyboardState.IsKeyUp(Keys.R))
            {
                this.m_Root.DeletePicture();
            }

            this.m_LastKeyboardState = this.m_ActualKeyboardState;
            this.m_LastMouseState = this.m_ActualMouseState;
        }
    }
}
