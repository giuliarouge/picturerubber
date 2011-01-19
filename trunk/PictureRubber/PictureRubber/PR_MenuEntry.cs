using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    class PR_MenuEntry
    {
        private PR_Main m_Root;
        private Texture2D[] m_Textures;
        private Rectangle m_ButtonRectangle;

        public PR_MenuEntry(string _buttonPath, Point _position)
        {
            this.m_Root = PR_Main.GetInstance();
            this.m_Textures = new Texture2D[3];
            this.m_Textures[0] = this.m_Root.Content.Load<Texture2D>(_buttonPath + "_normal");
            this.m_Textures[1] = this.m_Root.Content.Load<Texture2D>(_buttonPath + "_over");
            this.m_Textures[2] = this.m_Root.Content.Load<Texture2D>(_buttonPath + "_pressed");
            float value = this.m_Root.GraphicsDevice.Viewport.Width / 1600f;
            this.m_ButtonRectangle = new Rectangle(
                (int)(_position.X * value), (int)(_position.Y * value), (int)(this.m_Textures[0].Width * value), (int)(this.m_Textures[0].Height * value));
        }

        public bool Intersects(Vector2 _position)
        {
            return m_ButtonRectangle.Intersects(new Rectangle((int)_position.X, (int)_position.Y, 1, 1));
        }

        public void Draw(int _index)
        {
            this.m_Root.m_SpriteBatch.Draw(this.m_Textures[_index], this.m_ButtonRectangle, Microsoft.Xna.Framework.Color.White);
        }
    }
}
