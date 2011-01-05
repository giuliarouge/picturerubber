﻿using System;
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
    class PR_Pictures
    {
        /// <summary>
        /// The Root Pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// The Folder Path
        /// </summary>
        private string m_FolderPath;

        /// <summary>
        /// The Bitmaps, saved separatly for manipulation
        /// </summary>
        private Bitmap[] m_Pictures;

        /// <summary>
        /// The Textures
        /// </summary>
        private Texture2D[] m_Textures;

        private PR_Kinect m_Kinect;

        /// <summary>
        /// Initializes a new Instance of PR_Pictures
        /// </summary>
        public PR_Pictures(PR_Main _root, string _folderPath, PR_Kinect _kinect)
        {
            this.m_Root = _root;
            this.m_Kinect = _kinect;
            this.m_FolderPath = _folderPath;
            if (!this.LoadPictures())
            {
                //Fehler aufgetreten
                MessageBox.Show("Error while Loading Pictures");
            }
            else
            {
                this.m_Textures = new Texture2D[this.m_Pictures.Length];
                for (int i = 0; i < this.m_Pictures.Length; i++)
                {
                    this.m_Textures[i] = this.CreateTexture(this.m_Pictures[i]);
                }
            }
            
        }

        /// <summary>
        /// Creates a Texture2D from a bitmap
        /// </summary>
        /// <param name="picture">The Picture</param>
        /// <returns>The Texture</returns>
        public Texture2D CreateTexture(Bitmap _picture)
        {
            Texture2D tex;

            using (System.IO.MemoryStream s = new System.IO.MemoryStream())
            {
                _picture.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                s.Seek(0, System.IO.SeekOrigin.Begin); //must do this, or error is thrown in next line
                tex = Texture2D.FromStream(this.m_Root.GraphicsDevice, s);
            }
            return tex;
        }

        public void setFirstTexture(Texture2D _texture)
        {
            this.m_Textures[this.m_Pictures.Length-1] = _texture;
        }

        public Texture2D getFirstTexture()
        {
            return this.m_Textures[this.m_Pictures.Length-1];
        }


        /// <summary>
        /// Loads The Pictures
        /// </summary>
        /// <returns>The Success of Loading</returns>
        private bool LoadPictures()
        {
            bool success = true;
            
            DirectoryInfo dirInfo = new DirectoryInfo(this.m_FolderPath);
            if (dirInfo == null)
            {
                success = false;
            }
            else
            {
                FileInfo[] files = dirInfo.GetFiles();
                if (files == null)
                {
                    success = false;
                }

                this.m_Pictures = new Bitmap[files.Length];
                int i = 0;

                foreach (FileInfo fiOutput in files)
                {
                    try
                    {
                        Bitmap image = (Bitmap)Bitmap.FromFile(this.m_FolderPath + "\\" + fiOutput.Name);
                        this.m_Pictures[i] = image;
                        i++;
                    }
                    catch (System.OutOfMemoryException e)
                    {
                        System.Console.Out.WriteLine("OutOfMemoryException " + e.Message);
                        success = false;
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        System.Console.Out.WriteLine("FileNotFoundException" + e.Message);
                        success = false;
                    }
                    catch (System.ArgumentException e)
                    {
                        System.Console.Out.WriteLine("ArgumentException" + e.Message);
                        success = false;
                    }
                }
            }
            return success;
        }

        public void DeleteLastPicture()
        {
            this.m_Pictures[this.m_Pictures.Length - 1] = null;
            this.m_Textures[this.m_Textures.Length - 1] = null;
        }

        public void Draw(GameTime _gameTime)
        {
            this.m_Root.m_SpriteBatch.Begin();
            Microsoft.Xna.Framework.Rectangle rec;
            if (this.m_Root.m_Modus == PR_Main.Modus.Debug)
            {
                rec = new Microsoft.Xna.Framework.Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width/2, this.m_Root.GraphicsDevice.Viewport.Height);
                Microsoft.Xna.Framework.Rectangle depth;
                depth = new Microsoft.Xna.Framework.Rectangle(this.m_Root.GraphicsDevice.Viewport.Width / 2, 0, this.m_Root.GraphicsDevice.Viewport.Width / 2, this.m_Root.GraphicsDevice.Viewport.Height);
                this.m_Root.m_SpriteBatch.Draw(this.m_Kinect.GetDepthImageTexture(), depth, Microsoft.Xna.Framework.Color.White);
            }
            else
            {
                rec = new Microsoft.Xna.Framework.Rectangle(0, 0, this.m_Root.GraphicsDevice.Viewport.Width, this.m_Root.GraphicsDevice.Viewport.Height);
            }
            foreach (Texture2D tex in this.m_Textures)
            {
                if (tex != null)
                    this.m_Root.m_SpriteBatch.Draw(tex, rec, Microsoft.Xna.Framework.Color.White);
            }
            this.m_Root.m_SpriteBatch.End();
        }
    }
}
