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
                m_Texture = this.m_Root.Content.Load<Texture2D>("mouse");
                m_WaitingTexture = this.m_Root.Content.Load<Texture2D>("waiting_logo_part_rainy");
                this.RescaleTexture();
                this.m_Root.IsMouseVisible = false;
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
        /// <param name="_gameTime">actual gametime</param>
        public void Draw(GameTime _gameTime)
        {
            if (!this.m_Root.IsMouseVisible)
            {
                this.m_Root.m_SpriteBatch.Begin();
                Rectangle rec = new Rectangle(
                    this.m_InputManager.GetActualMouseState().X - this.m_Texture.Width / 2,
                    this.m_InputManager.GetActualMouseState().Y - this.m_Texture.Height / 2, 
                    this.m_Texture.Width,
                    this.m_Texture.Height);
                this.m_Root.m_SpriteBatch.Draw(m_Texture, rec, Microsoft.Xna.Framework.Color.White);
                
                for (int i = 0; i < this.m_WaitingTime / 45; i++)
                {
                    float angle = MathHelper.ToRadians(45.0f * i);
                    this.m_Root.m_SpriteBatch.Draw(m_WaitingTexture,
                        new Vector2(this.m_InputManager.GetActualMouseState().X, this.m_InputManager.GetActualMouseState().Y),
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
