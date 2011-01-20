using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    class PR_MainMenu
    {
        /// <summary>
        /// the root pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// the menu frame
        /// </summary>
        private Texture2D m_MenuFrame;

        /// <summary>
        /// the glowing rubber
        /// </summary>
        private Texture2D m_rubber_glow;

        /// <summary>
        /// Array holding the menu elements
        /// </summary>
        private PR_MenuEntry[] m_MenuEntrys;

        /// <summary>
        /// the input manager
        /// </summary>
        private PR_InputManager m_InputManager;

        /// <summary>
        /// Flag whether visible or not
        /// </summary>
        public bool m_Visible;

        /// <summary>
        /// Counter for the Control of the Buttons, holding on a button will increase the correspondating element
        /// </summary>
        private int[] m_SelectedIndex;

        /// <summary>
        /// waiting Delay
        /// </summary>
        private int m_Delay = 120;

        /// <summary>
        /// current waiting delay
        /// </summary>
        private int m_CurrentDelay;

        private float m_BlinkValue;
        
        /// <summary>
        /// Initializes a new instance of PR_MainMenu
        /// </summary>
        /// <param name="_root">the root pointer</param>
        /// <param name="_input">the input manager</param>
        public PR_MainMenu(PR_Main _root, PR_InputManager _input)
        {
            this.m_Visible = true;
            this.m_CurrentDelay = 0;
            this.m_SelectedIndex = new int[3];
            this.m_SelectedIndex[0] = 0;
            this.m_SelectedIndex[1] = 0;
            this.m_SelectedIndex[2] = 0;

            this.m_BlinkValue = 0;
            this.m_Root = _root;
            this.m_InputManager = _input;
            
            this.m_MenuFrame = this.m_Root.Content.Load<Texture2D>("menu\\menu_frame_rainy");
            this.m_rubber_glow = this.m_Root.Content.Load<Texture2D>("menu\\rubber_glow");

            this.m_MenuEntrys = new PR_MenuEntry[3];
            this.m_MenuEntrys[0] = new PR_MenuEntry("menu\\buttons\\main\\starten",new Point(400,352));
            this.m_MenuEntrys[1] = new PR_MenuEntry("menu\\buttons\\main\\optionen",new Point(400,492));
            this.m_MenuEntrys[2] = new PR_MenuEntry("menu\\buttons\\main\\beenden",new Point(400,632));
        }

        /// <summary>
        /// Updates the Menu if visible
        /// </summary>
        /// <param name="_gameTime">the actual gametime</param>
        public void Update(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                if (this.m_CurrentDelay >= this.m_Delay)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (this.m_SelectedIndex[i] >= 360)
                        {
                            this.m_InputManager.HandleMenuInput(i);
                            this.ClearUpCounter();
                            break;
                        }
                    }
                }
                temp++;
                temp = temp % 360;
                this.m_BlinkValue = (((float)Math.Sin(MathHelper.ToRadians(temp)))+1) / 2.0f;
            }
        }

        /// <summary>
        /// Clears the counter m_SelectedIndex
        /// </summary>
        private void ClearUpCounter()
        {
            for (int i = 0; i < 3; i++)
            {
                this.m_SelectedIndex[i] = 0;
            }
            this.m_Delay = 0;
        }
        int temp = 0;

        /// <summary>
        /// Draws the menu if visible
        /// </summary>
        /// <param name="_gameTime">the actual gametime</param>
        public void Draw(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                Vector2 mousePosition = this.m_InputManager.GetMousePosition();

                this.m_Root.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Rectangle rec = new Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width, this.m_Root.GraphicsDevice.Viewport.Height);

                this.m_Root.m_SpriteBatch.Draw(m_MenuFrame, rec, Microsoft.Xna.Framework.Color.White);
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

        private bool Intersects(Vector2 _position, Rectangle _rectangle)
        {
            return _rectangle.Intersects(new Rectangle((int)_position.X,(int)_position.Y,1,1));
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
