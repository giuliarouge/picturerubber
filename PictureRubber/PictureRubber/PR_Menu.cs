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
    public class PR_Menu
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

        private PR_OptionsMenu m_OptionsMenu;

        /// <summary>
        /// Flag if visible or not
        /// </summary>
        public bool m_Visible;

        /// <summary>
        /// Flag if the Options menu is shown, otherwise the main menu is shown
        /// </summary>
        public bool m_OptionsVisible;

        /// <summary>
        /// Initializes a new instance of PR_Menu
        /// </summary>
        /// <param name="_root">the root pointer</param>
        /// <param name="_input">the input manager</param>
        public PR_Menu(PR_InputManager _input)
        {
            this.m_Root = PR_Main.GetInstance();
            this.m_InputManager = _input;
            this.m_Background = new PR_MenuBackground(this.m_Root);
            this.m_MainMenu = new PR_MainMenu(this.m_Root, this.m_InputManager);
            this.m_OptionsMenu = new PR_OptionsMenu();
            this.m_OptionsVisible = false;
        }

        /// <summary>
        /// Updates the menu if visible
        /// </summary>
        /// <param name="_gameTime">the current GameTime</param>
        public void Update(GameTime _gameTime)
        {
            if (this.m_Visible && !this.m_OptionsVisible)
            {
                this.m_MainMenu.m_Visible = true;
            }
            else
            {
                this.m_MainMenu.m_Visible = false;
            }
            if (this.m_MainMenu.m_Visible)
            {
                this.m_MainMenu.Update(_gameTime);
            }
            if (this.m_OptionsMenu.m_Visible)
            {
                this.m_OptionsMenu.Update(_gameTime);
            }
            if (this.m_OptionsVisible)
            {
                this.m_OptionsMenu.m_Visible = true;
            }
            else
            {
                this.m_OptionsMenu.m_Visible = false;
            }
            m_Background.Update(_gameTime);
        }

        /// <summary>
        /// Draws the Menu if Visible
        /// </summary>
        /// <param name="_gameTime">the current gameTime</param>
        public void Draw(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                this.m_Background.Draw(_gameTime);
                if (this.m_MainMenu.m_Visible)
                {
                    this.m_MainMenu.Draw(_gameTime);
                }
                if (this.m_OptionsVisible)
                {
                    this.m_OptionsMenu.Draw(_gameTime);
                }
            }
        }

        public void RescaleMenuElements()
        {
            this.m_MainMenu.RescaleElements();
            this.m_OptionsMenu.RescaleElements();
        }
    }
}
