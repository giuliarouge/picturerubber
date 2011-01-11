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
        private PR_InputManager m_InputManager;

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
            this.m_MousePositions = new List<Vector2>();
            try
            {
                m_Texture = this.m_Root.Content.Load<Texture2D>("mouse");
                this.m_Root.IsMouseVisible = false;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                this.m_Root.IsMouseVisible = true;
            }
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
                    (int)(this.m_InputManager.GetMouseState().X - this.m_Texture.Width / 2 * value), 
                    (int)(this.m_InputManager.GetMouseState().Y - this.m_Texture.Height / 2 * value), 
                    (int)(this.m_Texture.Width * value),
                    (int)(this.m_Texture.Height * value));
                this.m_Root.m_SpriteBatch.Draw(m_Texture, rec, Microsoft.Xna.Framework.Color.White);
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
    }
}
