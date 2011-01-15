using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedNite;
using System.Diagnostics;
using System.Threading;

namespace PictureRubber
{
    class PR_Nite
    {
        private XnMOpenNIContext m_Context;
        private XnMSessionManager m_SessionManager;
        private XnMPointDenoiser m_PointDenoiser;

        public void NiteInitialize()
        {
            m_Context = new XnMOpenNIContext();
            this.shouldRun = true;
            m_Context.Init();
 
            m_PointDenoiser = new XnMPointDenoiser();
            m_PointDenoiser.PrimaryPointCreate += new EventHandler<PrimaryPointCreateEventArgs>(sessionManager_PrimaryPointCreate);
            m_PointDenoiser.PrimaryPointUpdate += new EventHandler<HandPointContextEventArgs>(sessionManager_PrimaryPointUpdate);
            m_PointDenoiser.PrimaryPointDestroy += new EventHandler<PointDestroyEventArgs>(sessionManager_PrimaryPointDestroy);

            m_SessionManager = new XnMSessionManager(m_Context, "Wave", "Wave");     
            m_SessionManager.AddListener(m_PointDenoiser);

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

        void sessionManager_PrimaryPointCreate(object sender, PrimaryPointCreateEventArgs e)
        {
            float x = e.HPC.Position.X;
            float y = e.HPC.Position.Y;
            float z = e.HPC.Position.Z;
            Trace.WriteLine("StartCoord(" + x + y + z + ")");
        }

        void sessionManager_PrimaryPointUpdate(object sender, HandPointContextEventArgs e)
        {
            float x = e.HPC.Position.X;
            float y = e.HPC.Position.Y;
            float z = e.HPC.Position.Z;
            Trace.WriteLine("Coord(" + x + y + z + ")");
            //Update Pointer Position
        }

        void sessionManager_PrimaryPointDestroy(object sender, PointDestroyEventArgs e)
        {
            Trace.WriteLine("~ID: " + e.ID);
        }

        private void track_hands()
        {
        }

        private bool shouldRun = false;
        private Thread readerThread;
        // private double precision = 0.025;
    }
}