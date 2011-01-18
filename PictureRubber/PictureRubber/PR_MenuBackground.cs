using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    /// <summary>
    /// The Menu Background Class
    /// </summary>
    class PR_MenuBackground
    {
        /// <summary>
        /// the root pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// the background texture
        /// </summary>
        private Texture2D m_Background;

        /// <summary>
        /// the left photo strip
        /// </summary>
        private Texture2D m_LeftLine;

        /// <summary>
        /// the right photostrip
        /// </summary>
        private Texture2D m_RightLine;

        /// <summary>
        /// the rubber itself
        /// </summary>
        private Texture2D m_Rubber;

        /// <summary>
        /// Array holding the alpha values of the left and right photostrip
        /// </summary>
        private int[] m_Animations;

        /// <summary>
        /// The Delay after which the animation goes to the next step
        /// </summary>
        private int m_AnimationDelay;

        /// <summary>
        /// the actual animation point counter
        /// </summary>
        private int m_CurrentAnimationDelay;

        /// <summary>
        /// An Enumerator representing the animation status, whether Show or Hide
        /// </summary>
        private enum AnimationDirection
        {
            Show,
            Hide
        };

        /// <summary>
        /// the Variable for the enum
        /// </summary>
        private AnimationDirection m_AnimationDirection;

        /// <summary>
        /// Visibility of the menu
        /// </summary>
        public bool m_Visible;

        /// <summary>
        /// Initializes a new Instance of PR_MenuBackground
        /// </summary>
        /// <param name="_root">the root pointer</param>
        public PR_MenuBackground(PR_Main _root)
        {
            this.m_Root = _root;
            this.m_Visible = true;
            this.m_AnimationDelay = 255;
            this.m_CurrentAnimationDelay = 0;
            this.m_Animations = new int[2];
            this.m_Animations[0] = 0;
            this.m_Animations[1] = 0;
            this.m_AnimationDirection = AnimationDirection.Show;

            this.m_Background = this.m_Root.Content.Load<Texture2D>("menu\\background_rainy");
            this.m_LeftLine = this.m_Root.Content.Load<Texture2D>("menu\\left_line");
            this.m_RightLine = this.m_Root.Content.Load<Texture2D>("menu\\right_line");
            this.m_Rubber = this.m_Root.Content.Load<Texture2D>("menu\\rubber_glow");
        }

        /// <summary>
        /// Updates the MenuBackground Animation if the menu is visible
        /// </summary>
        /// <param name="_gameTime">The actual GameTime</param>
        public void Update(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                if (this.m_CurrentAnimationDelay < this.m_AnimationDelay)
                {
                    this.m_CurrentAnimationDelay++;
                }
                if (this.m_CurrentAnimationDelay == this.m_AnimationDelay)
                {
                    switch (this.m_AnimationDirection)
                    {
                        case AnimationDirection.Show:
                            if (this.m_Animations[1] == 0 && this.m_Animations[0] < 255)
                            {
                                this.m_Animations[0]++;
                            }
                            if (this.m_Animations[0] == 255 && this.m_Animations[1] < 255)
                            {
                                this.m_Animations[1]++;
                            }
                            if (this.m_Animations[0] == 255 && this.m_Animations[1] == 255)
                            {
                                this.m_CurrentAnimationDelay = 0;
                                this.m_AnimationDirection = AnimationDirection.Hide;
                            }
                            break;
                        case AnimationDirection.Hide:
                            if (this.m_Animations[0] == 255 && this.m_Animations[1] > 0)
                            {
                                this.m_Animations[1]--;
                            }
                            if (this.m_Animations[1] == 0 && this.m_Animations[0] > 0)
                            {
                                this.m_Animations[0]--;
                            }
                            if (this.m_Animations[0] == 0 && this.m_Animations[1] == 0)
                            {
                                this.m_CurrentAnimationDelay = 0;
                                this.m_AnimationDirection = AnimationDirection.Show;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the menu background if visible
        /// </summary>
        /// <param name="_gameTime">The actual GameTime</param>
        public void Draw(GameTime _gameTime)
        {
            if (this.m_Visible)
            {
                this.m_Root.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Rectangle rec = new Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width, this.m_Root.GraphicsDevice.Viewport.Height);
                this.m_Root.m_SpriteBatch.Draw(m_Background, rec, Microsoft.Xna.Framework.Color.White);
            
                //   this.m_Root.m_SpriteBatch.Draw(m_LeftLine, rec, new Microsoft.Xna.Framework.Color(255, 255, 255, this.m_Animations[1]));
                //   this.m_Root.m_SpriteBatch.Draw(m_RightLine, rec, new Microsoft.Xna.Framework.Color(255, 255, 255, this.m_Animations[0]));
                this.m_Root.m_SpriteBatch.End();
            }
        }
    }
}
