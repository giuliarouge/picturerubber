using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedNite;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    class PR_Nite
    {
        private XnMOpenNIContext m_Context;
        private XnMSessionManager m_SessionManager;
        private XnMPointDenoiser m_PointDenoiser;
        private XnMPushDetector m_PushDetector;

        public void NiteInitialize()
        {
            this.shouldRun = true;

            m_Context = new XnMOpenNIContext();
            m_Context.Init();
 
            m_PointDenoiser = new XnMPointDenoiser();
            m_PointDenoiser.PrimaryPointCreate += new EventHandler<PrimaryPointCreateEventArgs>(sessionManager_PrimaryPointCreate);
            m_PointDenoiser.PrimaryPointUpdate += new EventHandler<HandPointContextEventArgs>(sessionManager_PrimaryPointUpdate);
            m_PointDenoiser.PrimaryPointDestroy += new EventHandler<PointDestroyEventArgs>(sessionManager_PrimaryPointDestroy);

            m_PushDetector = new XnMPushDetector();
            m_PushDetector.Push += new EventHandler<PushDetectorEventArgs>(pushDetector_Push);

            m_SessionManager = new XnMSessionManager(m_Context, "Wave", "Wave");     
            m_SessionManager.AddListener(m_PointDenoiser);
            m_SessionManager.AddListener(m_PushDetector);

            

            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
        }

        private void ReaderThread()
        {
            while (this.shouldRun)
            {
                uint rc = m_Context.Update();
                if (rc == 0)
                    m_SessionManager.Update(m_Context);
            }
        }

        public void Stop()
        {
            this.shouldRun = false;
            this.readerThread.Abort();
        }

        void sessionManager_PrimaryPointCreate(object sender, PrimaryPointCreateEventArgs e)
        {
            float x = e.HPC.Position.X;
            float y = e.HPC.Position.Y;
            float z = e.HPC.Position.Z;
            Trace.WriteLine("StartCoord(" + x + ";" + y + ";" + z + ")");
        }

        void sessionManager_PrimaryPointUpdate(object sender, HandPointContextEventArgs e)
        {
            int x = (int)((System.Windows.SystemParameters.PrimaryScreenWidth / 2 ) + e.HPC.Position.X);
            int y = (int)((System.Windows.SystemParameters.PrimaryScreenHeight / 2) + e.HPC.Position.Y * -1);
            float z = e.HPC.Position.Z;
            Trace.WriteLine("Hand(" + x + ";" + y + ";" + z + ")");
            //Update Pointer Position
            PR_Glove.SetCursorPos(x, y);
        }

        void sessionManager_PrimaryPointDestroy(object sender, PointDestroyEventArgs e)
        {
            Trace.WriteLine("~ID: " + e.ID);
        }

        void pushDetector_Push(object sender, PushDetectorEventArgs e)
        {
            Trace.WriteLine("Pushed!");
            // send LeftClick_Event
        }

        private bool shouldRun = false;
        private Thread readerThread;
    }
}