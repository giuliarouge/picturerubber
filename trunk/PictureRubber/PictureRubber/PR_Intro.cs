using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

namespace PictureRubber
{
    class PR_Intro
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

        private int m_Alpha;

        private Texture2D m_BlendTexture;

        /// <summary>
        /// Creates a new Instance from PR_Intro
        /// </summary>
        /// <param name="_root"></param>
        public PR_Intro(string _videoPath)
        {
            this.m_Root = PR_Main.GetInstance();
            this.m_Player = new VideoPlayer();
            this.m_Video = this.m_Root.Content.Load<Video>(_videoPath);
            this.m_BlendTexture = this.m_Root.Content.Load<Texture2D>("black");
            this.m_Alpha = 255;
        }

        public void Play()
        {
            this.m_Player.Play(this.m_Video);
        }

        public void Draw(GameTime _gameTime)
        {
            if (this.m_Player.PlayPosition >= this.m_Video.Duration - new TimeSpan(0, 0, 2))
            {
                if (this.m_Alpha < 255)
                    this.m_Alpha+=2;
            }
            else
            {
                if (this.m_Alpha > 0)
                    this.m_Alpha-=2;
            }
            if (this.m_Player.State == MediaState.Playing)
            {
                
                this.m_Root.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                this.m_Root.m_SpriteBatch.Draw(this.m_Player.GetTexture(),new Rectangle(0,0,this.m_Root.GraphicsDevice.Viewport.Width,this.m_Root.GraphicsDevice.Viewport.Height),Color.White);
                this.m_Root.m_SpriteBatch.Draw(this.m_BlendTexture, new Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width, this.m_Root.GraphicsDevice.Viewport.Height), new Color(255,255,255,this.m_Alpha));
                this.m_Root.m_SpriteBatch.End();
            }
        }

        public bool isActive()
        {
            return this.m_Player.State == MediaState.Playing;
        }
    }
}
