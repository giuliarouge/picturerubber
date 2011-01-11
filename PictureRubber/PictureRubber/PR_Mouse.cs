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
        private PR_Main m_Root;
        private Texture2D m_Texture;
        private Texture2D m_WaitingTexture;
        private PR_InputManager m_InputManager;
        private int m_WaitingTime;

        /// <summary>
        /// sclaing factor for mouse-texture
        /// </summary>
        private float m_ScalingValue;

        /// <summary>
        /// List of actual position of the mouse to generate a texture with rubbing areas
        /// </summary>
        private List<Vector2> m_MousePositions;

        /// <summary>
        /// initializes mouse-class
        /// </summary>
        /// <param name="_root">instance of mainclass</param>
        /// <param name="_input">instance of inputmanager</param>
        public PR_Mouse(PR_Main _root, PR_InputManager _input)
        {
            this.m_Root = _root;
            this.m_InputManager = _input;
            this.m_ScalingValue = this.m_Root.GraphicsDevice.Viewport.Width / 1600f;
            this.m_MousePositions = new List<Vector2>();
            try
            {
                m_Texture = this.m_Root.Content.Load<Texture2D>("mouse");
                m_WaitingTexture = this.m_Root.Content.Load<Texture2D>("waiting_logo_part");
                this.RescaleTexture();
                this.m_Root.IsMouseVisible = false;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                this.m_Root.IsMouseVisible = true;
            }
        }

        
        public void SetWaitingTime(int _waitingTime)
        {
            this.m_WaitingTime = _waitingTime;
        }

        /// <summary>
        /// draw method for mouse
        /// </summary>
        /// <param name="_gameTime">actual gametime</param>
        public void Draw(GameTime _gameTime)
        {
            if (!this.m_Root.IsMouseVisible)
            {
                this.m_Root.m_SpriteBatch.Begin();
                float value = this.m_Root.GraphicsDevice.Viewport.Width / 1600f;
                Rectangle rec = new Rectangle(
                    this.m_InputManager.GetMouseState().X - this.m_Texture.Width / 2, 
                    this.m_InputManager.GetMouseState().Y - this.m_Texture.Height / 2, 
                    this.m_Texture.Width,
                    this.m_Texture.Height);
                this.m_Root.m_SpriteBatch.Draw(m_Texture, rec, Microsoft.Xna.Framework.Color.White);

                for (int i = this.m_WaitingTime / 45; i > 0; i--)
                {
                    float angle = MathHelper.ToRadians(this.m_WaitingTime - 45.0f * i);
                    this.m_Root.m_SpriteBatch.Draw(m_WaitingTexture,
                        new Vector2(this.m_InputManager.GetMouseState().X, this.m_InputManager.GetMouseState().Y),
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
        /// add a new value to the list of mouse-positions
        /// </summary>
        public Vector2 MousePosition
        {
            set
            {
                this.m_MousePositions.Add(value);
            }
        }

        /// <summary>
        /// clear all mouse-positions
        /// </summary>
        public void ResetMousePositions()
        {
            this.m_MousePositions.Clear();
        }

        /// <summary>
        /// get the list of mouse-positions
        /// </summary>
        /// <returns>m_MousePositions</returns>
        public List<Vector2> GetMousePositions()
        {
            return this.m_MousePositions;
        }

        /// <summary>
        /// returns the mouse-texture
        /// </summary>
        /// <returns>m_Texture</returns>
        public Texture2D GetMouseTexture()
        {
            return this.m_Texture;
        }

        /// <summary>
        /// rescale mouse texture
        /// </summary>
        private void RescaleTexture()
        {
            GraphicsDevice graphics = this.m_Root.GraphicsDevice;
            int width = (int)(this.m_Texture.Width * this.m_ScalingValue);
            int height = (int)(this.m_Texture.Height * this.m_ScalingValue);
            //XNA 3.1 Code from here http://forums.create.msdn.com/forums/t/24124.aspx
            // Create the Render Target to draw the scaled Texture to 
            RenderTarget2D renderTarget = new RenderTarget2D(graphics, width, height, false, graphics.DisplayMode.Format, DepthFormat.Depth24Stencil8);

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
            this.m_Texture = renderTarget;
            renderTarget = null;
        }
    }
}
