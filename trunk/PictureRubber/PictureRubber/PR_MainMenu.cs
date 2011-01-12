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
        /// Array holding textures of the start button
        /// </summary>
        private Texture2D[] m_StartenButton;

        /// <summary>
        /// Array holding textures of the options button
        /// </summary>
        private Texture2D[] m_OptionenButton;

        /// <summary>
        /// Array holding textures of the close button
        /// </summary>
        private Texture2D[] m_BeendenButton;

        /// <summary>
        /// Rectangle representing the start button
        /// </summary>
        private Rectangle m_StartButtonRect;

        /// <summary>
        /// Rectangle representing the options button
        /// </summary>
        private Rectangle m_OptionenButtonRect;

        /// <summary>
        /// Rectangle representing the close button
        /// </summary>
        private Rectangle m_BeendenButtonRect;

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
            
            this.m_Root = _root;
            this.m_InputManager = _input;
            
            this.m_MenuFrame = this.m_Root.Content.Load<Texture2D>("menu\\menu_frame");
            
            this.m_BeendenButton = new Texture2D[3];
            this.m_OptionenButton = new Texture2D[3];
            this.m_StartenButton = new Texture2D[3];

            this.m_BeendenButton[0] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\beenden_normal");
            this.m_BeendenButton[1] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\beenden_over");
            this.m_BeendenButton[2] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\beenden_pressed");

            this.m_OptionenButton[0] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\optionen_normal");
            this.m_OptionenButton[1] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\optionen_over");
            this.m_OptionenButton[2] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\optionen_pressed");

            this.m_StartenButton[0] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\starten_normal");
            this.m_StartenButton[1] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\starten_over");
            this.m_StartenButton[2] = this.m_Root.Content.Load<Texture2D>("menu\\buttons\\starten_pressed");

            float value = this.m_Root.GraphicsDevice.Viewport.Width / 1600f;

            this.m_StartButtonRect = new Rectangle((int)(196 * value), (int)(552 * value), (int)(this.m_StartenButton[0].Width*value), (int)(this.m_StartenButton[0].Height*value));
            this.m_OptionenButtonRect = new Rectangle((int)(196 * value), (int)(692 * value), (int)(this.m_StartenButton[0].Width * value), (int)(this.m_StartenButton[0].Height * value));
            this.m_BeendenButtonRect = new Rectangle((int)(196 * value), (int)(832 * value), (int)(this.m_StartenButton[0].Width * value), (int)(this.m_StartenButton[0].Height * value));
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
                if (Intersects(mousePosition, m_StartButtonRect))
                {
                    intersects = true;
                    if (this.m_SelectedIndex[0] < 360)
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_StartenButton[1], m_StartButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_SelectedIndex[0]+=2;
                    }
                    else
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_StartenButton[2], m_StartButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_CurrentDelay++;
                    }
                    this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[0]);
                }
                else
                {
                    this.m_Root.m_SpriteBatch.Draw(m_StartenButton[0], m_StartButtonRect, Microsoft.Xna.Framework.Color.White);
                }
                if (Intersects(mousePosition, m_OptionenButtonRect))
                {
                    intersects = true;
                    if (this.m_SelectedIndex[1] < 360)
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_OptionenButton[1], m_OptionenButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_SelectedIndex[1]+=2;
                    }
                    else
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_OptionenButton[2], m_OptionenButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_CurrentDelay++;
                    }
                    this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[1]);
                }
                else
                {
                    this.m_Root.m_SpriteBatch.Draw(m_OptionenButton[0], m_OptionenButtonRect, Microsoft.Xna.Framework.Color.White);
                }
                if (Intersects(mousePosition, m_BeendenButtonRect))
                {
                    intersects = true;
                    if (this.m_SelectedIndex[2] < 360)
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_BeendenButton[1], m_BeendenButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_SelectedIndex[2]+=2;
                    }
                    else
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_BeendenButton[2], m_BeendenButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_CurrentDelay++;
                    }
                    this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[2]);
                }
                else
                {
                    this.m_Root.m_SpriteBatch.Draw(m_BeendenButton[0], m_BeendenButtonRect, Microsoft.Xna.Framework.Color.White);
                }
                if (intersects == false)
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
    }
}
