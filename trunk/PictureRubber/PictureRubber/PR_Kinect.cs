//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// This library is part of CL NUI SDK
// It allows the use of Microsoft Kinect cameras in your own applications
//
// For updates and file downloads go to: http://codelaboratories.com/get/kinect
//
// Copyright 2010 (c) Code Laboratories, Inc.  All rights reserved.
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;
using System.Diagnostics;
using CLNUIDeviceIntegration;
using Microsoft.Xna.Framework.Graphics;

namespace PictureRubber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PR_Kinect : Window
    {
        /// <summary>
        /// The Motor Pointer
        /// </summary>
        private IntPtr m_Motor = IntPtr.Zero;

        /// <summary>
        /// Important for the capturing
        /// </summary>
        private DispatcherTimer m_AccelerometerTimer;

        /// <summary>
        /// The Camera Pointer
        /// </summary>
        private IntPtr m_Camera = IntPtr.Zero;

        /// <summary>
        /// The Colored Kinect Image
        /// </summary>
        private NUIImage m_ColorImage;

        /// <summary>
        /// The Depth Kinect Image
        /// </summary>
        private NUIImage m_DepthImage;

        /// <summary>
        /// The Capture Thread
        /// </summary>
        private System.Threading.Thread m_CaptureThread;

        /// <summary>
        /// Flag whether the thread has to capture or not
        /// </summary>
        private bool m_Running;

        /// <summary>
        /// Númber of Kinect Devices
        /// </summary>
        private int m_DeviceCount;

        /// <summary>
        /// The Kinect Serial
        /// </summary>
        private string m_DeviceSerial;

        /// <summary>
        /// The root Pointer
        /// </summary>
        private PR_Main m_Root;

        /// <summary>
        /// Texture that is shown in the debug mode if no Kinect was found
        /// </summary>
        private Texture2D m_OfflineTexture;

        /// <summary>
        /// Initializes a new Instance of the PR_Kinect Class
        /// </summary>
        /// <param name="_root">the root pointer</param>
        public PR_Kinect(PR_Main _root)
        {
            this.m_Root = _root;
            this.m_OfflineTexture = this.m_Root.Content.Load<Texture2D>("kinect");
            this.m_DeviceSerial = "";
            try
            {
                this.m_DeviceCount = CLNUIDevice.GetDeviceCount();
                this.m_DeviceSerial = CLNUIDevice.GetDeviceSerial(0);
            }
            catch 
            {
                this.m_DeviceCount = 0;
            }
            if (this.m_DeviceCount != 0)
            {
                try
                {
                    this.m_Motor = CLNUIDevice.CreateMotor(this.m_DeviceSerial);
                    this.m_Camera = CLNUIDevice.CreateCamera(this.m_DeviceSerial);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Environment.Exit(0);
                }

                CLNUIDevice.SetMotorPosition(this.m_Motor, 0);
                CLNUIDevice.SetMotorLED(this.m_Motor, (byte)1);

                this.m_ColorImage = new NUIImage(640, 480);

                this.m_DepthImage = new NUIImage(640, 480);

                this.m_AccelerometerTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal, (EventHandler)delegate(object sender, EventArgs e)
                {
                    short _x = 0, _y = 0, _z = 0;
                    CLNUIDevice.GetMotorAccelerometer(this.m_Motor, ref _x, ref _y, ref _z);
                }, Dispatcher);
                this.m_AccelerometerTimer.Start();

                this.m_Running = true;
                this.m_CaptureThread = new Thread(delegate()
                {
                    //                 Trace.WriteLine(string.Format("Camera {0:X}", camera.ToInt32()));
                    if (CLNUIDevice.StartCamera(this.m_Camera))
                    {
                        while (this.m_Running)
                        {
                            CLNUIDevice.GetCameraColorFrameRGB32(this.m_Camera, this.m_ColorImage.ImageData, 500);
                            CLNUIDevice.GetCameraDepthFrameRGB32(this.m_Camera, this.m_DepthImage.ImageData, 0);
                            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                            {
                                this.m_ColorImage.Invalidate();
                                this.m_DepthImage.Invalidate();
                            });
                        }
                        CLNUIDevice.StopCamera(this.m_Camera);
                    }
                });
                this.m_CaptureThread.IsBackground = true;
                this.m_CaptureThread.Start();
            }
            this.Hide();
        }

        /// <summary>
        /// Frees the Kinect
        /// </summary>
        public void DeleteKinect()
        {
            if (this.m_DeviceCount != 0)
            {
                this.m_AccelerometerTimer.Stop();
                CLNUIDevice.SetMotorLED(this.m_Motor, 0);
                if (this.m_Motor != IntPtr.Zero) CLNUIDevice.DestroyMotor(this.m_Motor);
                if (this.m_CaptureThread != null)
                {
                    this.m_Running = false;
                    this.m_CaptureThread.Join(100);
                }
                if (this.m_Camera != IntPtr.Zero) CLNUIDevice.DestroyCamera(this.m_Camera);
            }
        }

        /// <summary>
        /// Konvertiert ein System.Windows.Controls.Image in ein System.Drawing.Image
        /// Quelle: http://www.mycsharp.de/wbb2/thread.php?threadid=91456
        /// </summary>
        /// <param name="image">zu konvertierendes System.Windows.Controls.Image </param>
        /// <returns>konvertiertes System.Drawing.Image</returns>
        public static System.Drawing.Image ConvertWpfImageToImage(System.Windows.Controls.Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image", "Image darf nicht null sein.");

            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
            encoder.Save(ms);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            return img;

        }

        /// <summary>
        /// Converts the DepthImage to a XNA Texcture
        /// </summary>
        /// <returns>XNA Texture2D</returns>
        public Texture2D GetDepthImageTexture()
        {
            if (this.m_DeviceCount == 0)
            {
                return this.m_OfflineTexture;
            }
            else
            {
                Image img = new Image();
                img.Source = this.m_DepthImage.BitmapSource;
                System.Drawing.Image image = ConvertWpfImageToImage(img);
                System.Drawing.Bitmap bit = new System.Drawing.Bitmap(image, image.Size);
                Texture2D tex;
                using (System.IO.MemoryStream s = new System.IO.MemoryStream())
                {
                    image.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                    s.Seek(0, System.IO.SeekOrigin.Begin); //must do this, or error is thrown in next line
                    tex = Texture2D.FromStream(this.m_Root.GraphicsDevice, s);
                }
                return tex;
            }
        }

        /// <summary>
        /// Gets the DepthImage
        /// </summary>
        /// <returns>The Kinect Depth Image</returns>
        public Image GetDepthImage()
        {
            Image img = new Image();
            img.Source = this.m_DepthImage.BitmapSource;
            return img;
        }
    }
}

