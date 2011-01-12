using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    /// <summary>
    /// The Menu Container
    /// </summary>
    class PR_Menu
    {
        /// <summary>
        /// the root Pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// The Pointer to the main menu
        /// </summary>
        private PR_MainMenu m_MainMenu;

        /// <summary>
        /// the Pointer to the Backgorund
        /// </summary>
        private PR_MenuBackground m_Background;

        /// <summary>
        /// holds the pointer to the input manager
        /// </summary>
        private PR_InputManager m_InputManager;

        /// <summary>
        /// Flag if visible or not
        /// </summary>
        public bool m_Visible;

        /// <summary>
        /// Initializes a new instance of PR_Menu
        /// </summary>
        /// <param name="_root">the root pointer</param>
        /// <param name="_input">the input manager</param>
        public PR_Menu(PR_Main _root, PR_InputManager _input)
        {
            this.m_Root = _root;
            this.m_InputManager = _input;
            this.m_Background = new PR_MenuBackground(this.m_Root);
            this.m_MainMenu = new PR_MainMenu(this.m_Root, this.m_InputManager);
        }

        /// <summary>
        /// Updates the menu if visible
        /// </summary>
        /// <param name="_gameTime">the actual GameTime</param>
        public void Update(GameTime _gameTime)
        {
            if (this.m_Visible && !this.m_MainMenu.m_Visible)
            {
                this.m_MainMenu.m_Visible = true;
            }
            if (this.m_MainMenu.m_Visible)
            {
                this.m_MainMenu.Update(_gameTime);
            }
            m_Background.Update(_gameTime);
        }

        /// <summary>
        /// Draws the Menu if Visible
        /// </summary>
        /// <param name="_gameTime">the actual gameTime</param>
        public void Draw(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                this.m_Background.Draw(_gameTime);
                if (this.m_MainMenu.m_Visible)
                {
                    this.m_MainMenu.Draw(_gameTime);
                }
            }
        }
    }
}
