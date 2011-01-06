using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using xn;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Security;


namespace kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.KeyDown += new System.Windows.Input.KeyEventHandler(MainWindow_KeyDown);
          //Initialize context
            context = new Context(SAMPLE_XML_FILE);
           
          //Initialize Generators
            this.gestures = context.FindExistingNode(NodeType.Gesture) as GestureGenerator;
            this.hands = context.FindExistingNode(NodeType.Hands) as HandsGenerator;
            this.images = context.FindExistingNode(NodeType.Image) as ImageGenerator;
        
            if (this.hands == null)
            {
                throw new Exception("Viewer must have a hands node!");
            }

            if (this.images == null)
            {
                throw new Exception("Viewer must have a images node!");
            }

            if (this.gestures == null)
            {
                throw new Exception("Viewer must have a gestures node!");
            }

            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (this.depth == null)
            {
                throw new Exception("Viewer must have a depth node!");
            }

            //Add gestures we want to captures
             this.gestures.AddGesture("Wave"); 
            //Subscribe to the event  when a gesture is recognized
            this.gestures.GestureRecognized += new GestureGenerator.GestureRecognizedHandler(gestures_GestureRecognized);
          //Subscribe to events related to the hand tracking
            this.hands.HandCreate += new HandsGenerator.HandCreateHandler(hands_HandCreate);
            this.hands.HandUpdate += new HandsGenerator.HandUpdateHandler(hands_HandUpdate);
            this.hands.HandDestroy += new HandsGenerator.HandDestroyHandler(hands_HandDestroy);
         //Star capturing gestures
            this.gestures.StartGenerating();

            this.histogram = new int[this.depth.GetDeviceMaxDepth()];
            MapOutputMode mapMode = this.depth.GetMapOutputMode();
           
            //Init our writablebitmaps
            InitializeRenderBitmap((int)mapMode.nXRes, (int)mapMode.nYRes);
        
            this.shouldRun = true;
            AvailableHands = new ObservableCollection<Hand>();
        
            //Start working
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();

   
        }

        private unsafe void ReaderThread()
        {
            DepthMetaData depthMD = new DepthMetaData();
            ImageMetaData imgMD = new ImageMetaData();

            while (this.shouldRun)
            {
                try
                {
                    //updated or generators
                    this.context.WaitAndUpdateAll();

                }
                catch (Exception)
                {
                }

                //get metadata
                this.images.GetMetaData(imgMD);
                this.depth.GetMetaData(depthMD);

                //calculate histogram
                CalcHist(depthMD);

                lock (this)
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        // write pixes from the depth histogram array to the writablebitmap
                               m_writeableBitmapDepth.Lock();

                               ushort* pDepth = (ushort*)this.depth.GetDepthMapPtr().ToPointer();
                               for (int y = 0; y < depthMD.YRes; ++y)
                               {
                                   byte* pDest = (byte*)m_writeableBitmapDepth.BackBuffer.ToPointer() + y * m_writeableBitmapDepth.BackBufferStride;
                                   for (int x = 0; x < depthMD.XRes; ++x, ++pDepth, pDest += 3)
                                   {
                                       byte pixel = (byte)this.histogram[*pDepth];
                                       pDest[0] = 0;
                                       pDest[1] = pixel;
                                       pDest[2] = pixel;
                                   }
                               }
                               //Say what are changed (ALL)
                               m_writeableBitmapDepth.AddDirtyRect(new Int32Rect(0, 0,(int)depthMD.XRes, (int)depthMD.YRes));
                               m_writeableBitmapDepth.Unlock();

                               m_writeableBitmapImage.Lock();
                               //Just write the pixels to the bitpam from the image metada, (best way?)
                               m_writeableBitmapImage.WritePixels(new Int32Rect(0,0,(int)imgMD.XRes, (int)imgMD.YRes), imgMD.ImageMapPtr, (int)imgMD.DataSize, m_writeableBitmapImage.BackBufferStride);
                               m_writeableBitmapImage.Unlock();
             
           });

                }
            }
        }
      

        #region Gestures events

        void gestures_GestureRecognized(ProductionNode node, string strGesture, ref Point3D idPosition, ref Point3D endPosition)
        {
           
            switch (strGesture) 
            { 
                    //Start tracking this hand
                case "Wave": 
                         hands.StartTracking(ref idPosition); 
                    
                    break; 
                default: 
                    break; 
            } 

        }
        #endregion

        #region Hands events

        void hands_HandCreate(ProductionNode node, uint id, ref Point3D position, float fTime)
        {
            float x = position.X;
            float y = position.Y;
            Dispatcher.Invoke(
                               System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(
                   delegate()
                   {
                       Hand newHand = new Hand
                       {
                           ID = id.ToString(),
                           Left = (((lstHands.ActualWidth) / 2) - 30) + x,
                           Top = (((lstHands.ActualHeight) / 2) - 30) + (y * -1)
                       };

                       AvailableHands.Add(newHand);

                   }

               ));
        }


        void hands_HandDestroy(ProductionNode node, uint id, float fTime)
        {
            Dispatcher.Invoke(
                             System.Windows.Threading.DispatcherPriority.Normal,
               new Action(
                 delegate()
                 {

                     if (AvailableHands != null && AvailableHands.Count > 0)
                     {
                         AvailableHands.Remove(AvailableHands.First(h => h.ID == id.ToString()));
                     }
                 }));
        }


        void hands_HandUpdate(ProductionNode node, uint id, ref Point3D position, float fTime)
        {

            float x = position.X;
            float y = position.Y;
            var cursorX = 0;
            var cursorY = 0; 

            Dispatcher.Invoke(
                               System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(
                   delegate()
                   {
                       Hand currHand = AvailableHands.First(h => h.ID == id.ToString());
                       currHand.Left = (((lstHands.ActualWidth) / 2) - 30) + x;
                       currHand.Top = (((lstHands.ActualHeight) / 2) - 30) + (y * -1);
                       cursorX = (int)currHand.Left;
                       cursorY = (int)currHand.Top; 

                   }
               
               ));

         //  CursorHandler.SetCursorPos(cursorX, cursorY); 

        }

        
        #endregion

        #region Helpers

        private unsafe void CalcHist(DepthMetaData depthMD)
        {
            // reset
            for (int i = 0; i < this.histogram.Length; ++i)
                this.histogram[i] = 0;

            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    ushort depthVal = *pDepth;
                    if (depthVal != 0)
                    {
                        this.histogram[depthVal]++;
                        points++;
                    }
                }
            }

            for (int i = 1; i < this.histogram.Length; i++)
            {
                this.histogram[i] += this.histogram[i - 1];
            }

            if (points > 0)
            {
                for (int i = 1; i < this.histogram.Length; i++)
                {
                    this.histogram[i] = (int)(256 * (1.0f - (this.histogram[i] / (float)points)));
                }
            }
        }

        private void InitializeRenderBitmap(int width, int height)
        {
            m_writeableBitmapDepth = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
            m_writeableBitmapImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgb24, null);
            imgDepth.Source = m_writeableBitmapDepth;
            imgImage.Source = m_writeableBitmapImage;
        }


        #endregion
        
        #region Window Events
        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //  if (e. == 27)
        //    {
        //        Close();
        //    }
            base.OnKeyDown(e);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.context.Shutdown();
            
            this.shouldRun = false;
            this.readerThread.Join();
            this.context.Dispose();
            base.OnClosing(e);

        }

        #endregion

        #region Properties
        private readonly string SAMPLE_XML_FILE = @"openni.xml";
        private WriteableBitmap m_writeableBitmapDepth;
        private WriteableBitmap m_writeableBitmapImage;
        private Context context;
        private DepthGenerator depth;
        private HandsGenerator hands;
        private GestureGenerator gestures;
        private ImageGenerator images;
        private Thread readerThread;
        private bool shouldRun;
        private int[] histogram;
  


        public ObservableCollection<Hand> AvailableHands
        {
            get { return (ObservableCollection<Hand>)GetValue(AvailableHandsProperty); }
            set { SetValue(AvailableHandsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvailableHands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailableHandsProperty =
            DependencyProperty.Register("AvailableHands", typeof(ObservableCollection<Hand>), typeof(MainWindow), new UIPropertyMetadata(null));
        
        #endregion

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination,IntPtr Source, int Length); 



    }

    public class Hand : ViewModelBase
    {
        
        private string _id = "";

        public string ID
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
