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
        private PR_Main m_Root;
        private Texture2D m_Background;
        private Texture2D m_MenuFrame;
        private Texture2D m_LeftLine;
        private Texture2D m_RightLine;
        private Texture2D m_Rubber;
        private Texture2D[] m_StartenButton;
        private Texture2D[] m_OptionenButton;
        private Texture2D[] m_BeendenButton;
        private Rectangle m_StartButtonRect;
        private Rectangle m_OptionenButtonRect;
        private Rectangle m_BeendenButtonRect;
        private PR_InputManager m_InputManager;

        public bool m_Visible;
        private int[] m_SelectedIndex;
        private byte m_Delay = 10;
        private byte m_CurrentDelay;

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
            this.m_Background = this.m_Root.Content.Load<Texture2D>("menu\\background");
            this.m_MenuFrame = this.m_Root.Content.Load<Texture2D>("menu\\menu_frame");
            this.m_LeftLine = this.m_Root.Content.Load<Texture2D>("menu\\left_line");
            this.m_RightLine = this.m_Root.Content.Load<Texture2D>("menu\\right_line");
            this.m_Rubber = this.m_Root.Content.Load<Texture2D>("menu\\rubber");
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

        public void Update(GameTime _gameTime)
        {
            if (this.m_CurrentDelay >= this.m_Delay)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (this.m_SelectedIndex[i] == 360)
                    {
                        this.m_InputManager.HandleMenuInput(i);
                        this.ClearUpCounter();
                        break;
                        
                    }
                }
            }
        }

        private void ClearUpCounter()
        {
            for (int i = 0; i < 3; i++)
            {
                this.m_SelectedIndex[i] = 0;
            }
            this.m_Delay = 0;
        }

        public void Draw(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                Vector2 mousePosition = this.m_InputManager.GetMousePosition();
                this.m_Root.m_SpriteBatch.Begin();
                Rectangle rec = new Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width, this.m_Root.GraphicsDevice.Viewport.Height);
                this.m_Root.m_SpriteBatch.Draw(m_Background, rec, Microsoft.Xna.Framework.Color.White);
                this.m_Root.m_SpriteBatch.Draw(m_LeftLine, rec, Microsoft.Xna.Framework.Color.White);
                this.m_Root.m_SpriteBatch.Draw(m_RightLine, rec, Microsoft.Xna.Framework.Color.White);
                this.m_Root.m_SpriteBatch.Draw(m_Rubber, rec, Microsoft.Xna.Framework.Color.White);
                this.m_Root.m_SpriteBatch.Draw(m_MenuFrame, rec, Microsoft.Xna.Framework.Color.White);
                bool intersects = false;
                if (Intersects(mousePosition, m_StartButtonRect))
                {
                    intersects = true;
                    if (this.m_SelectedIndex[0] < 360)
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_StartenButton[1], m_StartButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_SelectedIndex[0]+=2;
                        this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[0]);
                    }
                    else
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_StartenButton[2], m_StartButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_CurrentDelay++;
                        this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[0]);
                    }
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
                        this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[1]);
                    }
                    else
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_OptionenButton[2], m_OptionenButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_CurrentDelay++;
                        this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[1]);
                    }
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
                        this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[2]);
                    }
                    else
                    {
                        this.m_Root.m_SpriteBatch.Draw(m_BeendenButton[2], m_BeendenButtonRect, Microsoft.Xna.Framework.Color.White);
                        this.m_CurrentDelay++;
                        this.m_Root.SetMouseWaitingTime((int)this.m_SelectedIndex[2]);
                    }
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
            bool intersects = false;
            if (_rectangle.Intersects(new Rectangle((int)_position.X,(int)_position.Y,1,1)))
            {
                intersects = true;
            }
            return intersects;
        }
    }
}
