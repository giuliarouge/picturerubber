using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    public class PR_OverlayButtons
    {
        private PR_Main m_Root;

        private Texture2D m_WaveAdvice;
        private Texture2D m_MenuButton;
        private Texture2D m_ResetButton;
        private Rectangle m_WaveRect;
        private Rectangle m_MenuRect;
        private Rectangle m_ResetRect;
        private PR_Nite m_Nite;

        private int m_MenuAlpha;
        private int m_ResetAlpha;
        private int m_WaitingTime;
        private int m_Active;

        public PR_OverlayButtons()
        {
            this.m_Root = PR_Main.GetInstance();
            this.m_MenuButton = this.m_Root.Content.Load<Texture2D>("menu\\menu");
            this.m_ResetButton = this.m_Root.Content.Load<Texture2D>("menu\\refresh");
            this.m_WaveAdvice = this.m_Root.Content.Load<Texture2D>("menu\\wave");
            this.m_Nite = this.m_Root.Kinect;

            this.RescaleButtons();
            this.m_MenuAlpha = 0;
            this.m_ResetAlpha = 0;
            this.ResetCounter();
            this.m_Active = 0;
        }

        /// <summary>
        /// Clears the counter m_WatingTime
        /// </summary>
        private void ResetCounter()
        {
            this.m_WaitingTime = 0;
        }

        public void Update()
        {
            Vector2 MousePosition = PR_InputManager.GetInstance().GetMousePosition();
            bool intersects = false;
            if (Intersects(MousePosition, this.m_MenuRect))
            {
                intersects = true;
                this.m_MenuAlpha = 255;
                this.m_WaitingTime += 3;
                this.m_Active = 0;
            }
            else
            {
                this.m_MenuAlpha = 51;
            }
            if (Intersects(MousePosition, this.m_ResetRect))
            {
                intersects = true;
                this.m_ResetAlpha = 255;
                this.m_WaitingTime += 3;
                this.m_Active = 1;
            }
            else
            {
                this.m_ResetAlpha = 51;
            }

            if (!intersects)
            {
                ResetCounter();
            }
            this.m_Root.Mouse.SetWaitingTime(m_WaitingTime);
            if (m_WaitingTime >= 360)
            {
                PR_InputManager.GetInstance().HandleOverlayInput(this.m_Active);
            }
        }

        public bool Intersects(Vector2 _position, Rectangle _rec)
        {
            if (!this.m_Root.IsGesture && !this.m_Root.RunningGesture)
            {
                return _rec.Intersects(new Rectangle((int)_position.X, (int)_position.Y, 1, 1));
            }
            return false;
        }

        public void RescaleButtons()
        {
            float value = this.m_Root.GraphicsDevice.Viewport.Width / 1600f;
            Point ResetPosition = new Point(0, 0);
            Point MenuPosition = new Point(1600 - this.m_MenuButton.Width, 0);
            Point WavePosition = new Point(800-this.m_WaveAdvice.Width/2, 200);
            this.m_ResetRect = new Rectangle(
                (int)(ResetPosition.X * value), (int)(ResetPosition.Y * value), (int)(this.m_ResetButton.Width * value), (int)(this.m_ResetButton.Height * value));
            this.m_MenuRect = new Rectangle(
                (int)(MenuPosition.X * value), (int)(MenuPosition.Y * value), (int)(this.m_MenuButton.Width * value), (int)(this.m_MenuButton.Height * value));
            this.m_WaveRect = new Rectangle(
                (int)(WavePosition.X * value), (int)(WavePosition.Y * value), (int)(this.m_WaveAdvice.Width * value), (int)(this.m_WaveAdvice.Height * value));
        }

        public void Draw()
        {
            this.m_Root.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            if (this.m_Root.m_IsKinectConnected && this.m_Nite.is_HandRecognized)
            {
                this.m_Root.m_SpriteBatch.Draw(this.m_MenuButton, this.m_MenuRect, new Color(255, 255, 255, this.m_MenuAlpha));
                this.m_Root.m_SpriteBatch.Draw(this.m_ResetButton, this.m_ResetRect, new Color(255, 255, 255, this.m_ResetAlpha));
            }
            else if (this.m_Root.m_IsKinectConnected)
            {
                this.m_Root.m_SpriteBatch.Draw(this.m_WaveAdvice, m_WaveRect, new Color(255, 255, 255, 200));
            }
            else
            {
                this.m_Root.m_SpriteBatch.Draw(this.m_MenuButton, this.m_MenuRect, new Color(255, 255, 255, this.m_MenuAlpha));
                this.m_Root.m_SpriteBatch.Draw(this.m_ResetButton, this.m_ResetRect, new Color(255, 255, 255, this.m_ResetAlpha));
            }
            this.m_Root.m_SpriteBatch.End();
        }
    }
}
