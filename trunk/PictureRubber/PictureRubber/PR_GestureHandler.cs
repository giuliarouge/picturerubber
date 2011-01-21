﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PictureRubber
{
    public class PR_GestureHandler
    {
        #region declaration of variables
        //constants
        /// <summary>
        /// specify the alphaamount for alphafader-shader
        /// </summary>
        private const int c_AlphaAmount = 100;

        /// <summary>
        /// the gap a user needs to go forward to rub areas (in mm)
        /// </summary>
        private const int c_Gap = 200;

        /// <summary>
        /// minimal distance of a user to the kinect
        /// </summary>
        private const int c_MinimalDistance = 1000;

        /// <summary>
        /// alpha value for path-modus
        /// </summary>
        private const int c_PathAlpha = 255 / 5;

        /// <summary>
        /// instance of main class
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// The Renderer for rubbing-areas
        /// </summary>
        private PR_Renderer m_RubberRenderer;

        /// <summary>
        /// The Renderer to calculate the black/white texture
        /// </summary>
        private PR_Renderer m_MouseTextureRenderer;

        /// <summary>
        /// texture to generate rubber-areas
        /// </summary>
        private Texture2D m_ModelTexture;

        /// <summary>
        /// initial Texture for Mouse-Texture Shader
        /// </summary>
        private Texture2D m_BlankTexture;

        /// <summary>
        /// real texture of the mouse seen on the screen
        /// </summary>
        private Texture2D m_RealMouseTexture;

        /// <summary>
        /// copy of our image-textures
        /// </summary>
        private Texture2D[] m_PictureTextures;

        /// <summary>
        /// amount of textures
        /// </summary>
        private int m_TextureCount;

        #endregion

        /// <summary>
        /// constructor of PR_GestureHandler
        /// </summary>
        public PR_GestureHandler()
        {
            this.m_Root = PR_Main.GetInstance();
            //initialize renderer
            this.m_RubberRenderer = new PR_Renderer("AlphaFader", "AlphaFader", this.m_Root.GraphicsDevice);
            this.m_MouseTextureRenderer = new PR_Renderer("DynamicMouse", "DynamicMouse", this.m_Root.GraphicsDevice);
            //create textures for shader and rubbing-araes
            this.m_BlankTexture = new Texture2D(
                this.m_Root.GraphicsDevice,
                this.m_Root.GraphicsDevice.Viewport.Width,
                this.m_Root.GraphicsDevice.Viewport.Height);
            this.m_ModelTexture = this.m_BlankTexture;
            this.m_PictureTextures = this.m_Root.Pictures.getTextures();
            this.m_TextureCount = this.m_PictureTextures.Count();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public void Update()
        {
            if ((this.m_Root.IsGesture && this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime) ||
                (this.m_Root.RunningGesture && this.m_Root.ShaderModus == PR_Main.RubberModus.Path))
            {
                if (this.m_Root.InitBlankTexture && this.m_Root.ShaderModus == PR_Main.RubberModus.Path)
                {
                    //draw a path, where the user wants to delete something of the image
                    this.m_ModelTexture = this.m_BlankTexture;
                    this.m_Root.InitBlankTexture = false;
                }
                else if (this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime)
                {
                    //reset m_BlankTexture every update-call
                    this.m_ModelTexture = this.m_BlankTexture;
                }
                //process DynamicMouse-Shader to calculate the texture
                this.m_MouseTextureRenderer.SetRenderTarget(this.m_ModelTexture);
                this.m_MouseTextureRenderer.CreateMouseTexture(ref this.m_ModelTexture,
                    this.m_Root.Mouse.GetMouseTexture(),
                    PR_InputManager.GetInstance().GetMousePosition());
                this.m_MouseTextureRenderer.ResetRenderTarget(ref this.m_ModelTexture);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public void Draw()
        {
            //draw a path, where the user wants to delete something of the image
            this.m_Root.m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            this.m_Root.m_SpriteBatch.Draw(this.m_ModelTexture,
                new Rectangle(0, 0, this.m_ModelTexture.Width, this.m_ModelTexture.Height),
                new Color(0, 0, 0, c_PathAlpha));
            this.m_Root.m_SpriteBatch.End();
        }

        /// <summary>
        /// process the alphafader shader to clean the ares
        /// </summary>
        public void ProcessAlphaFaderShader()
        {
            //delete areas AlphaFader-Shader
            if (this.m_Root.m_IsKinectConnected && this.m_Root.ShaderModus == PR_Main.RubberModus.Realtime)
            {
                int index = this.CalculateTextureIndex();
                if (index >= 0 && index < this.m_TextureCount)
                {
                    if (index == this.m_TextureCount - 1)
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[index], this.m_ModelTexture, c_AlphaAmount);
                    }
                    else
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[index], this.m_ModelTexture, c_AlphaAmount, this.m_PictureTextures[index + 1]);
                    }
                }
            }
            else
            {
                for (int i = 1; i < this.m_TextureCount; ++i)
                {
                    if (i == this.m_TextureCount - 1)
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[i], this.m_ModelTexture, c_AlphaAmount);
                    }
                    else
                    {
                        this.m_RubberRenderer.ApplyFilter(ref this.m_PictureTextures[i], this.m_ModelTexture, c_AlphaAmount, this.m_PictureTextures[i + 1]);
                    }
                }
            }
        }

        /// <summary>
        /// reset values and call the garbage collector
        /// </summary>
        public void ResetValues()
        {
            //reset values
            this.m_Root.IsGesture = false;
            if (this.m_Root.ShaderModus == PR_Main.RubberModus.Path)
            {
                this.m_Root.InitBlankTexture = true;
                this.m_Root.RunningGesture = false;
            }
            GC.Collect();
        }

        /// <summary>
        /// calculate the texture index a user is actually working on
        /// </summary>
        /// <returns></returns>
        private int CalculateTextureIndex()
        {
            //initial z value == distance from kinect
            int ZValue = 2000 - c_Gap;//this.m_Nite.GetInitialZValue()
            //actual z value
            int ActualZValue = this.ActualZ - c_MinimalDistance;
            int Area = (ZValue - c_MinimalDistance) / (this.m_TextureCount - 1);
            //return the index of the texture a user is working on
            if (ActualZValue % Area == 0)
            {
                return ActualZValue / Area;
            }
            return (ActualZValue / Area) + 1;
        }

        /// <summary>
        /// get actual z-value from kinect
        /// </summary>
        public int ActualZ
        {
            get
            {
                return this.m_Root.Kinect.ActualZ;
            }
        }

        /// <summary>
        /// get the z-value where a user is able to delete ares in realtime-mode
        /// </summary>
        public int InitialZValue
        {
            get
            {
                //return this.m_Root.Kinect.InitialZValue;
                return 2000 - c_Gap;
            }
        }

        /// <summary>
        /// if the kinect lost the hand we will reset the gesture if the game is in Path-Modus
        /// </summary>
        public void ResetGesture()
        {
            this.m_ModelTexture = this.m_BlankTexture;
            this.m_Root.RunningGesture = false;
        }

        public void RescaleElements()
        {
            this.m_BlankTexture = new Texture2D(
                this.m_Root.GraphicsDevice,
                this.m_Root.GraphicsDevice.Viewport.Width,
                this.m_Root.GraphicsDevice.Viewport.Height);
            this.m_ModelTexture = this.m_BlankTexture;
        }
    }
}