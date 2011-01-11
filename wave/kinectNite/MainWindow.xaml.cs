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
using ManagedNite;
using System.Threading;
using System.IO;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System.Windows.Media.Animation;

namespace kinectNite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XnMSessionManager sessionManager;
        XnMOpenNIContext context;
        XnMPushDetector pushpoint;
        XnMPointDenoiser pointdenoise;
        private int count = 1;

     
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            AvailableHands = new ObservableCollection<Hand>();
            context = new XnMOpenNIContext();
            this.shouldRun=true;
            HandDetected = true;

            context.Init();

            pushpoint = new XnMPushDetector();
             pushpoint.Push += new EventHandler<PushDetectorEventArgs>(pushpoint_Push);
        
            pointdenoise = new XnMPointDenoiser();
            pointdenoise.PrimaryPointCreate += new EventHandler<PrimaryPointCreateEventArgs>(detectPoint_PrimaryPointCreate);
            pointdenoise.PrimaryPointUpdate += new EventHandler<HandPointContextEventArgs>(detectPoint_PrimaryPointUpdate);
            pointdenoise.PrimaryPointDestroy += new EventHandler<PointDestroyEventArgs>(detectPoint_PrimaryPointDestroy);
           
            //Session
            sessionManager = new XnMSessionManager(context, "Wave", "RaiseHand");
            sessionManager.AddListener(pointdenoise);
            sessionManager.AddListener(pushpoint);
            sessionManager.FocusStartDetected += new EventHandler<FocusStartEventArgs>(sessionManager_FocusStartDetected);
            sessionManager.SessionStarted += new EventHandler<PointEventArgs>(sessionManager_SessionStarted);
           sessionManager.SessionEnded += new EventHandler(sessionManager_SessionEnded);
            
            //Start Working
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();


            //this is to animate the background
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
         
        
        }
   

        private void ReaderThread()
        {
            while (this.shouldRun)
            {
                uint rc = context.Update();
                if (rc == 0)
                    sessionManager.Update(context);
            }
        }
     
        #region Window Events
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {

            Point mousePos = Mouse.GetPosition(imgImage);
            mouse = GetPos(mouse, mousePos, speed);
            if (oldpos == mouse)
                IsMouseMoving = false;
            else
                IsMouseMoving = true;

            AnimateBackground();

            oldpos = mouse;
        }
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                MaximizeMinimize();
            }
        }
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.shouldRun = false;
            this.context.Close();
            this.context.Dispose();
            base.OnClosing(e);
        } 
        #endregion
      
        #region Push

        void pushpoint_Push(object sender, PushDetectorEventArgs e)
        {
            Dispatcher.Invoke(
                             System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(
                 delegate()
                 {
                    // MaximizeMinimize();
                     CursorHandler.SendMouseLeftClick(oldpos);
                 }));
        }

        
        #endregion

        #region Hands

        void detectPoint_PrimaryPointDestroy(object sender, PointDestroyEventArgs e)
        {
            int id = (int)e.ID;

            Dispatcher.Invoke(
                             System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(
                 delegate()
                 {
                     if (AvailableHands.Count > 0)
                     {
                         HandDetected = false;
                         ReFocus = true;
                         Hand currHand = AvailableHands.FirstOrDefault(h => h.ID == id);
                         AvailableHands.Remove(currHand);
                     }
                 }));
        }

        void detectPoint_PrimaryPointCreate(object sender, PrimaryPointCreateEventArgs e)
        {

            float x = e.HPC.Position.X;
            float y = e.HPC.Position.Y;
            int id = e.HPC.ID;
            Dispatcher.Invoke(
                               System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(
                   delegate()
                   {
                       ReFocus = false;

                       Hand newHand = new Hand
                       {
                           ID = id,
                           Left = (((lstHands.ActualWidth) / 2) - 30) + x,
                           Top = (((lstHands.ActualHeight) / 2) - 30) + (y * -1)
                       };

                       AvailableHands.Add(newHand);
                       count++;
                   }));
        }

        void detectPoint_PrimaryPointUpdate(object sender, HandPointContextEventArgs e)
        {
            float x = e.HPC.Position.X;
            float y = e.HPC.Position.Y;
            var cursorX = 0;
            var cursorY = 0;

            Dispatcher.Invoke(
                               System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(
                   delegate()
                   {
                       Hand currHand = AvailableHands.First(h => h.ID == e.HPC.ID);
                       currHand.Left = (((lstHands.ActualWidth) / 2) - 30) + x;
                       currHand.Top = (((lstHands.ActualHeight) / 2) - 30) + (y * -1);
                       cursorX = (int)((System.Windows.SystemParameters.PrimaryScreenWidth / 2) + x);
                       cursorY = (int)((System.Windows.SystemParameters.PrimaryScreenHeight / 2) + (y * -1));
                      
                   }

               ));

            CursorHandler.SetCursorPos(cursorX, cursorY);
        } 
        #endregion

        #region Session 
        void sessionManager_FocusStartDetected(object sender, FocusStartEventArgs e)
        {

            //string x = e.m_focusString;
            //Dispatcher.Invoke(
            //                   System.Windows.Threading.DispatcherPriority.Normal,
            //     new Action(
            //       delegate()
            //       {
            //           //HandDetected = false;
            //           //Hand newHand = new Hand
            //           //{
            //           //    ID = count.ToString(),
            //           //    Left = (((lstHands.ActualWidth) / 2) - 30) +  e.m_focusPoint.X,
            //           //    Top = (((lstHands.ActualHeight) / 2) - 30) + (e.m_focusPoint.Y * -1)
            //           //};

            //           //AvailableHands.Add(newHand);
            //           //count++;

            //       }

            //   ));
            //   c = new XnMPointControl()
            //   sessionManager.TrackPoint(e.m_focusPoint);
        }

        void sessionManager_SessionEnded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                delegate()
                {
                    HandDetected = true;
                    ReFocus = false;
                }));
        }

        void sessionManager_SessionStarted(object sender, PointEventArgs e)
        {

            Dispatcher.Invoke(
                              System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                  delegate()
                  {
                      HandDetected = false;
                  }));
        } 
        #endregion

        #region Properties
        public ObservableCollection<Hand> AvailableHands
        {
            get { return (ObservableCollection<Hand>)GetValue(AvailableHandsProperty); }
            set { SetValue(AvailableHandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvailableHands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailableHandsProperty =
            DependencyProperty.Register("AvailableHands", typeof(ObservableCollection<Hand>), typeof(MainWindow), new UIPropertyMetadata(null));


        public bool HandDetected
        {
            get { return (bool)GetValue(HandDetectedProperty); }
            set { SetValue(HandDetectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HandDetected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandDetectedProperty =
            DependencyProperty.Register("HandDetected", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));


        public bool ReFocus
        {
            get { return (bool)GetValue(ReFocusProperty); }
            set { SetValue(ReFocusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReFocusProperty =
            DependencyProperty.Register("ReFocus", typeof(bool), typeof(MainWindow), new UIPropertyMetadata(false));

        private bool shouldRun = false;
        private Thread readerThread;
        private double precision = 0.025;
        private double speed = 0.2;
        private Point mouse = new Point();
        private Point oldpos = new Point(0, 0);
        private bool IsMouseMoving = false;
        private int frequency = 40;
        private double magnitude = 0;
        #endregion

        #region Helpers
        private void MaximizeMinimize()
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Minimized;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }
        private void AnimateBackground()
        {

            if (IsMouseMoving && !HandDetected && !ReFocus)
            {
                myRipple.Center = new Point((1 / imgImage.ActualWidth) * mouse.X, (1 / imgImage.ActualHeight) * mouse.Y);

                if (frequency == 0)
                {
                    frequency = 40;
                    magnitude = 0;
                }
                if (frequency == 30)
                    magnitude = 0.02;
                myRipple.Magnitude = magnitude;
                myRipple.Frequency = frequency;
                frequency--;

            }
        }
        Point GetPos(Point pt, Point target, double speed)
        {
            double xdif = target.X - pt.X;
            double ydif = target.Y - pt.Y;
            if (xdif >= -precision && xdif <= precision) pt.X = target.X;
            else pt.X += (target.X - pt.X) * speed;

            if (ydif >= -precision && ydif <= precision) pt.Y = target.Y;
            else pt.Y += (target.Y - pt.Y) * speed;
            return pt;
        } 
        #endregion

        

    
    }

    public class Hand : ViewModelBase
    {

        private int _id = 0;

        public int ID
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                var oldValue = _id;
                _id = value;


                // Update bindings, no broadcast
                RaisePropertyChanged("ID");

            }
        }

        private double _top = 0;

        public double Top
        {
            get
            {
                return _top;
            }

            set
            {
                if (_top == value)
                {
                    return;
                }

                var oldValue = _top;
                _top = value;


                // Update bindings, no broadcast
                RaisePropertyChanged("Top");

            }
        }

        private double _left = 0;

        public double Left
        {
            get
            {
                return _left;
            }

            set
            {
                if (_left == value)
                {
                    return;
                }

                var oldValue = _left;
                _left = value;


                // Update bindings, no broadcast
                RaisePropertyChanged("Left");

            }
        }

    }
}
