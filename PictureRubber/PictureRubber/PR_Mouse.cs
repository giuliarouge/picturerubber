using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    class PR_Mouse
    {
        private PR_Main m_Root;
        private Texture2D m_Texture;
        private PR_InputManager m_InputManager;

        public PR_Mouse(PR_Main _root, PR_InputManager _input)
        {
            this.m_Root = _root;
            this.m_InputManager = _input;
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

        public void Draw(GameTime _gameTime)
        {
            if (!this.m_Root.IsMouseVisible)
            {
                this.m_Root.m_SpriteBatch.Begin();
                Microsoft.Xna.Framework.Rectangle rec = new Microsoft.Xna.Framework.Rectangle(this.m_InputManager.GetMouseState().X - this.m_Texture.Width / 2, this.m_InputManager.GetMouseState().Y - this.m_Texture.Height / 2, this.m_Texture.Width, this.m_Texture.Height);
                this.m_Root.m_SpriteBatch.Draw(m_Texture, rec, Microsoft.Xna.Framework.Color.White);
                this.m_Root.m_SpriteBatch.End();
            }
        }
    }
}
