using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    class PR_OptionsMenu
    {
        /// <summary>
        /// the root pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// the options frame
        /// </summary>
        private Texture2D m_OptionsFrame;

        /// <summary>
        /// The Menu Buttons
        /// </summary>
        private PR_MenuEntry[] m_MenuEntrys;

        private int[] m_SelectedIndex;
        
        /// <summary>
        /// the input manager
        /// </summary>
        private PR_InputManager m_InputManager;

        /// <summary>
        /// Flag whether visible or not
        /// </summary>
        public bool m_Visible;

        /// <summary>
        /// waiting Delay
        /// </summary>
        private int m_Delay = 120;

        /// <summary>
        /// current waiting delay
        /// </summary>
        private int m_CurrentDelay;

        /// <summary>
        /// Initializes a new instance of PR_MainMenu
        /// </summary>
        /// <param name="_root">the root pointer</param>
        /// <param name="_input">the input manager</param>
        public PR_OptionsMenu()
        {
            this.m_Visible = true;
            this.m_CurrentDelay = 0;
            
            this.m_Root = PR_Main.GetInstance();
            this.m_InputManager = PR_InputManager.GetInstance();

            this.m_OptionsFrame = this.m_Root.Content.Load<Texture2D>("menu\\options_frame");

            this.m_MenuEntrys = new PR_MenuEntry[8];
            //0 - Resolution 640x480
            //1 - Resolution 800x600
            //2 - Resolution 1024x768
            //3 - Fenstermodus
            //4 - Vollbild
            //5 - EchtzeitModus
            //6 - PfadModus
            //7 - Zurueck
            this.m_SelectedIndex = new int[8];
            this.m_SelectedIndex.Initialize();

            this.m_MenuEntrys[0] = new PR_MenuEntry("menu\\buttons\\options\\res_640",new Point(346,296));
            this.m_MenuEntrys[1] = new PR_MenuEntry("menu\\buttons\\options\\res_800", new Point(655, 296));
            this.m_MenuEntrys[2] = new PR_MenuEntry("menu\\buttons\\options\\res_1024", new Point(964, 296));
            this.m_MenuEntrys[3] = new PR_MenuEntry("menu\\buttons\\options\\window", new Point(496, 446));
            this.m_MenuEntrys[4] = new PR_MenuEntry("menu\\buttons\\options\\fullscreen", new Point(805, 446));
            this.m_MenuEntrys[5] = new PR_MenuEntry("menu\\buttons\\options\\realtime", new Point(496, 596));
            this.m_MenuEntrys[6] = new PR_MenuEntry("menu\\buttons\\options\\path", new Point(805, 596));
            this.m_MenuEntrys[7] = new PR_MenuEntry("menu\\buttons\\options\\back", new Point(546, 746));
        }

        /// <summary>
        /// Updates the Menu if visible
        /// </summary>
        /// <param name="_gameTime">the current gametime</param>
        public void Update(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                //if (this.m_CurrentDelay >= this.m_Delay)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (this.m_SelectedIndex[i] >= 360)
                        {
                            this.m_InputManager.HandleOptionsInput(i);
                            this.ClearUpCounter();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears the counter m_SelectedIndex
        /// </summary>
        private void ClearUpCounter()
        {
            for (int i = 0; i < 8; i++)
            {
                this.m_SelectedIndex[i] = 0;
            }
            this.m_Delay = 0;
        }

        /// <summary>
        /// Draws the menu if visible
        /// </summary>
        /// <param name="_gameTime">the current gametime</param>
        public void Draw(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                Vector2 mousePosition = this.m_InputManager.GetMousePosition();

                this.m_Root.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Rectangle rec = new Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width, this.m_Root.GraphicsDevice.Viewport.Height);

                this.m_Root.m_SpriteBatch.Draw(m_OptionsFrame, rec, Microsoft.Xna.Framework.Color.White);
                bool intersects = false;
                for (int i = 0; i < this.m_MenuEntrys.Length; ++i)
                {
                    if (m_MenuEntrys[i].Intersects(mousePosition))
                    {
                        intersects = true;
                        if (this.m_SelectedIndex[i] < 360)
                        {
                            this.m_MenuEntrys[i].Draw(1);
                            this.m_SelectedIndex[i] += 3;
                        }
                        else
                        {
                            this.m_MenuEntrys[i].Draw(2);
                            this.m_CurrentDelay++;
                        }
                        this.m_Root.SetMouseWaitingTime(this.m_SelectedIndex[i]);
                    }
                    else
                    {
                        this.m_MenuEntrys[i].Draw(0);
                    }
                }
                if (!intersects)
                {
                    this.ClearUpCounter();
                }
                    
                this.m_Root.m_SpriteBatch.End();
            }
        }

        public void RescaleElements()
        {
            for (int i = 0; i < this.m_MenuEntrys.Length; ++i)
            {
                this.m_MenuEntrys[i].RescaleButton();
            }
        }
    }
}