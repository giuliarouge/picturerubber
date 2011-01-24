using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace PictureRubber
{
    class PR_VideoBackground
    {
        /// <summary>
        /// The root Pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// The Video Player
        /// </summary>
        private VideoPlayer m_Player;

        /// <summary>
        /// The Video
        /// </summary>
        private Video m_Video;

        /// <summary>
        /// Creates a new Instance from PR_Intro
        /// </summary>
        /// <param name="_root"></param>
        public PR_VideoBackground(string _videoPath)
        {
            this.m_Root = PR_Main.GetInstance();
            this.m_Player = new VideoPlayer();
            this.m_Video = this.m_Root.Content.Load<Video>(_videoPath);
            this.m_Player.IsLooped = true;
            this.m_Player.Play(this.m_Video);
        }

        public void Play()
        {
            this.m_Player.Play(this.m_Video);
        }

        public void Draw(GameTime _gameTime)
        {
            if (this.m_Player.State == MediaState.Playing)
            {
                this.m_Root.m_SpriteBatch.Begin();
                this.m_Root.m_SpriteBatch.Draw(this.m_Player.GetTexture(), new Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width, this.m_Root.GraphicsDevice.Viewport.Height), Color.White);
                this.m_Root.m_SpriteBatch.End();
            }
        }

        public bool isActive()
        {
            return this.m_Player.State == MediaState.Playing;
        }
    }
}
