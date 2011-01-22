using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    public class PR_Mouse
    {
        /// <summary>
        /// the root pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// The Mouse Teture
        /// </summary>
        private Texture2D m_Texture;

        /// <summary>
        /// texture which will be drawn on the screen
        /// </summary>
        private Texture2D m_ScaledTexture;

        /// <summary>
        /// The Waiting Part Texture
        /// </summary>
        private Texture2D m_WaitingTexture;

        /// <summary>
        /// The Input Manager
        /// </summary>
        private PR_InputManager m_InputManager;

        /// <summary>
        /// Holds the current Waiting Time
        /// </summary>
        private int m_WaitingTime;

        /// <summary>
        /// sclaing factor for mouse-texture
        /// </summary>
        private float m_ScalingValue;

        private SpriteFont m_Font;

        /// <summary>
        /// initializes mouse-class
        /// </summary>
        /// <param name="_root">instance of mainclass</param>
        /// <param name="_input">instance of inputmanager</param>
        public PR_Mouse(PR_InputManager _input)
        {
            this.m_Root = PR_Main.GetInstance();
            this.m_InputManager = _input;
            this.m_ScalingValue = this.m_Root.GraphicsDevice.Viewport.Width / 1600f;
            try
            {
                this.m_Texture = this.m_Root.Content.Load<Texture2D>("mouse");
                this.m_WaitingTexture = this.m_Root.Content.Load<Texture2D>("waiting_logo_part_rainy");
                this.m_Font = this.m_Root.Content.Load<SpriteFont>("Arial");
                this.m_Root.IsMouseVisible = false;
                this.RescaleTexture();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                this.m_Root.IsMouseVisible = true;
            }
        }

        /// <summary>
        /// Sets the Waiting Time for the animation direction
        /// </summary>
        /// <param name="_waitingTime">the time, given in angles</param>
        public void SetWaitingTime(int _waitingTime)
        {
            this.m_WaitingTime = _waitingTime;
        }

        /// <summary>
        /// draw method for mouse
        /// </summary>
        /// <param name="_gameTime">current gametime</param>
        public void Draw(GameTime _gameTime)
        {
            if (!this.m_Root.IsMouseVisible)
            {
                Vector2 MousePosition = new Vector2(this.m_InputManager.GetCurrentMouseState().X, this.m_InputManager.GetCurrentMouseState().Y);
                this.m_Root.m_SpriteBatch.Begin();
                Rectangle rec = new Rectangle(
                    (int)MousePosition.X - this.m_ScaledTexture.Width / 2,
                    (int)MousePosition.Y - this.m_ScaledTexture.Height / 2, 
                    this.m_ScaledTexture.Width,
                    this.m_ScaledTexture.Height);
                //draw mouse-texture and texture number
                this.m_Root.m_SpriteBatch.Draw(this.m_ScaledTexture, rec, Microsoft.Xna.Framework.Color.White);
                if (!this.m_Root.ShowMenu && this.m_Root.m_IsKinectConnected && this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime)
                {
                    int index = this.m_Root.Gestures.TextureIndex;
                    String text;
                    if (index > 0 && index < this.m_Root.Gestures.TextureCount)
                    {
                        text = index.ToString();
                    }
                    else
                    {
                        text = "-";
                    }
                    MousePosition -= (this.m_Font.MeasureString(text) / 2);
                    MousePosition.Y += 15;
                    this.m_Root.m_SpriteBatch.DrawString(this.m_Font,
                        text,
                        MousePosition,
                        Microsoft.Xna.Framework.Color.Green,
                        0.0f,
                        Vector2.Zero,
                        this.m_ScalingValue,
                        SpriteEffects.None,
                        0);
                }

                for (int i = 0; i < this.m_WaitingTime / 45; ++i)
                {
                    float angle = MathHelper.ToRadians(45.0f * i);
                    this.m_Root.m_SpriteBatch.Draw(this.m_WaitingTexture,
                        new Vector2(this.m_InputManager.GetCurrentMouseState().X, this.m_InputManager.GetCurrentMouseState().Y),
                        null,
                        Color.White,
                        angle,
                        new Vector2(this.m_WaitingTexture.Width / 2, this.m_WaitingTexture.Height / 2),
                        this.m_ScalingValue,
                        SpriteEffects.None,
                        0f);
                }

                this.m_WaitingTime = 0;
                this.m_Root.m_SpriteBatch.End();
            }
        }

        /// <summary>
        /// returns the mouse-texture
        /// </summary>
        /// <returns>m_Texture</returns>
        public Texture2D GetMouseTexture()
        {
            return this.m_ScaledTexture;
        }

        /// <summary>
        /// rescale mouse texture
        /// </summary>
        public void RescaleTexture()
        {
            this.m_ScalingValue = this.m_Root.GraphicsDevice.Viewport.Width / 1600f;
            GraphicsDevice graphics = this.m_Root.GraphicsDevice;
            int width = (int)(this.m_Texture.Width * this.m_ScalingValue);
            int height = (int)(this.m_Texture.Height * this.m_ScalingValue);

            //XNA 3.1 Code from here http://forums.create.msdn.com/forums/t/24124.aspx
            // Create the Render Target to draw the scaled Texture to 
            RenderTarget2D renderTarget = new RenderTarget2D(graphics,
                width,
                height,
                false,
                graphics.DisplayMode.Format,
                DepthFormat.Depth24Stencil8);

            graphics.SetRenderTarget(renderTarget);

            // Clear the scene 
            graphics.Clear(Color.Transparent);

            // Create the new SpriteBatch that will be used to scale the Texture 
            SpriteBatch spriteBatch = new SpriteBatch(graphics);

            // Draw the scaled Texture 
            spriteBatch.Begin();
            spriteBatch.Draw(this.m_Texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();

            // Restore the given Graphics Device's Render Target 
            graphics.SetRenderTarget(null);

            // Set the Texture To Return to the scaled Texture 
            this.m_ScaledTexture = renderTarget;
            renderTarget = null;
            GC.Collect();
        }
    }
}
