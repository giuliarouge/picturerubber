using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    /// <summary>
    /// this class will manage realtime rendering of rubber-effect
    /// </summary>
    public class PR_Renderer
    {
        //constants
        /// <summary>When HLSL rendering the texture coordinates have a little shifting. This value corrects this.
        /// Note: Unit in pixels.
        /// Note: code from Frank Nagl, SBIP, http://code.google.com/p/sbip/ </summary>
        internal const byte Adjustment = 4;

        /// <summary>
        /// alpha-effect to make parts of an image invisible
        /// </summary>
        private Effect m_Effect;

        /// <summary>
        /// shader technique
        /// </summary>
        private string m_Technique;

        /// <summary>
        /// instance of graphics
        /// </summary>
        private GraphicsDevice m_Graphics;

        /// <summary>
        /// instance of main class
        /// </summary>
        private PR_Main m_Main;

        /// <summary>
        /// render target for shader
        /// </summary>
        private RenderTarget2D m_RenderTarget;

        /// <summary>
        /// vertex for drawing the rendertarget
        /// </summary>
        private VertexPositionTexture[] quadVex;

        /// <summary>
        /// ids
        /// </summary>
        private short[] quadIdx;

        /// <summary>
        /// initializes a new instance of PR_Renderer
        /// </summary>
        /// <param name="_effect">name of effect-file</param>
        /// <param name="_technique">name of technique used in the effect-file</param>
        /// <param name="_graphics">instance of GRaphicsDevice</param>
        /// <param name="_main">instance of main-class</param>
        public PR_Renderer(string _effect, string _technique, GraphicsDevice _graphics)
        {
            this.m_Technique = _technique;
            this.m_Graphics = _graphics;
            this.m_Main = PR_Main.GetInstance();

            this.m_Effect = this.m_Main.Content.Load<Effect>(_effect);
            if (this.m_Effect == null)
            {
                throw new Exception("Could not load effectfile " + _effect);
            }
            this.m_Effect.CurrentTechnique = this.m_Effect.Techniques[this.m_Technique];

            this.InitQuad();
        }

        /// <summary>
        /// Init the quad vertex
        /// Note: code from Frank Nagl, SBIP, http://code.google.com/p/sbip/
        /// </summary>
        private void InitQuad()
        {
            //init quad
            quadVex = new VertexPositionTexture[4];
            quadVex[0] = new VertexPositionTexture(-Vector3.UnitX - Vector3.UnitY, Vector2.UnitY);
            quadVex[1] = new VertexPositionTexture(Vector3.UnitX - Vector3.UnitY, Vector2.One);
            quadVex[2] = new VertexPositionTexture(Vector3.UnitX + Vector3.UnitY, Vector2.UnitX);
            quadVex[3] = new VertexPositionTexture(-Vector3.UnitX + Vector3.UnitY, Vector2.Zero);

            quadIdx = new short[] { 0, 3, 1, 2 };
        }

        /// <summary>
        /// with this function we will pass our alphafader-shader
        /// </summary>
        /// <param name="_texture">texture on which a specific region will be manupulated</param>
        /// <param param name="_mouseTexture">black/white texture of mouse to calculate rubbing-area</param>
        /// <param name="_aplhaAmount">amount of alphablending 0 == all, 1 == nothing</param>
        /// <param name="_compareTexture">texture to compare</param>
        /// <returns>the new texture</returns>
        public void ApplyFilter(ref Texture2D _texture, Texture2D _mouseTexture,  int _alphaAmount, Texture2D _compareTexture = null)
        {
            //initialize rendertarget
            this.m_RenderTarget = new RenderTarget2D(
                this.m_Graphics,
                _texture.Width,
                _texture.Height,
                false,
                this.m_Graphics.DisplayMode.Format,
                DepthFormat.Depth24Stencil8);
            //set the rendertarget to our texture
            this.m_Graphics.SetRenderTarget(this.m_RenderTarget);
            this.m_Graphics.Clear(Microsoft.Xna.Framework.Color.Transparent);
            Vector2 delta = new Vector2(1.0f /_texture.Width, 1.0f / _texture.Height);

            //process shader
            //apply variables
            this.m_Effect.Parameters["imageTexture"].SetValue(_texture);
            this.m_Effect.Parameters["mouseTexture"].SetValue(_mouseTexture);
            this.m_Effect.Parameters["alphaAmount"].SetValue(_alphaAmount);
            this.m_Effect.Parameters["adjustment"].SetValue(Adjustment);
            this.m_Effect.Parameters["delta"].SetValue(delta);
            this.m_Effect.Parameters["textureIndex"].SetValue(0);

            if (_compareTexture != null)
            {
                this.m_Effect.Parameters["compareTexture"].SetValue(_compareTexture);
                this.m_Effect.Parameters["textureIndex"].SetValue(1);
            }

            this.DrawPrmitives(); 

            //clear rendertarget and return new texture
            this.m_Graphics.SetRenderTarget(null);

            _texture = this.m_RenderTarget;
            this.m_RenderTarget = null;
        }

        /// <summary>
        /// create a black/white texture of rubbing area
        /// </summary>
        /// <param name="_texture">output texture</param>
        /// <param name="_mouseTexture">ingame mouse texture</param>
        /// <param name="_actualPosition"></param>
        /// <param name="_startPosition"></param>
        public void CreateMouseTexture(ref Texture2D _texture, Texture2D _mouseTexture, Vector2 _mousePosition)
        {            
            Vector2 delta = new Vector2(1.0f / _texture.Width, 1.0f / _texture.Height);

            //calculate variabls for shader
            Vector2 topLeft, bottomRight, dimensions;
            //coordinates of topleft corner
            topLeft = new Vector2(
                _mousePosition.X - _mouseTexture.Width / 2,
                _mousePosition.Y - _mouseTexture.Height / 2);
            topLeft.X /= _texture.Width;
            topLeft.Y /= _texture.Height;

            //coordinates of bottomright corner
            bottomRight = new Vector2(
                _mousePosition.X + _mouseTexture.Width / 2,
                _mousePosition.Y + _mouseTexture.Height / 2);
            bottomRight.X /= _texture.Width;
            bottomRight.Y /= _texture.Height;

            //dimensions of the texture
            dimensions = new Vector2(
                _mouseTexture.Width,
                _mouseTexture.Height);

            //process shader
            //apply variables
            this.m_Effect.Parameters["backgroundTexture"].SetValue(_texture);
            this.m_Effect.Parameters["mouseTexture"].SetValue(_mouseTexture);
            this.m_Effect.Parameters["adjustment"].SetValue(Adjustment);
            this.m_Effect.Parameters["delta"].SetValue(delta);
            this.m_Effect.Parameters["topLeft"].SetValue(topLeft);
            this.m_Effect.Parameters["bottomRight"].SetValue(bottomRight);
            this.m_Effect.Parameters["dimensions"].SetValue(dimensions);

            this.DrawPrmitives();        
        }

        /// <summary>
        /// set actual rendertarget
        /// </summary>
        /// <param name="_texture">textureinformation for rendertarget</param>
        public void SetRenderTarget(Texture2D _texture)
        {
            //initialize rendertarget
            this.m_RenderTarget = new RenderTarget2D(
                this.m_Graphics,
                _texture.Width,
                _texture.Height,
                false,this.m_Graphics.DisplayMode.Format,
                DepthFormat.Depth24Stencil8);

            //set the rendertarget to our texture
            this.m_Graphics.SetRenderTarget(this.m_RenderTarget);
            this.m_Graphics.Clear(Microsoft.Xna.Framework.Color.Transparent);
        }

        /// <summary>
        /// reset rendertarget to null
        /// </summary>
        /// <param name="_texture">texture to save the new texture</param>
        public void ResetRenderTarget(ref Texture2D _texture)
        {
            //clear rendertarget and return new texture
            this.m_Graphics.SetRenderTarget(null);

            _texture = this.m_RenderTarget;
            this.m_RenderTarget = null;
        }

        /// <summary>
        /// draw the texture
        /// </summary>
        private void DrawPrmitives()
        {
            foreach (EffectPass pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //code from Frank Nagl, SBIP, http://code.google.com/p/sbip/
                this.m_Graphics.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    quadVex, 0, 4,
                    quadIdx, 0, 2);
            }
        }
    }
}
