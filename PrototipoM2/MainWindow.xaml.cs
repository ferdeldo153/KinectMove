using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Windows.Forms;
using KinectMouseController;

namespace PrototipoM2
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor mikinectD;
        /*byte[] datosColor = null;
        WriteableBitmap colorImagenBitmap = null;*/
        // colores
        
        double AnchoP;
        double AltoP;

        bool actclick = false;
        bool actenter = false;
        bool actEsc = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            CenterWindowOnScreen();
            AltoP= System.Windows.SystemParameters.WorkArea.Height;
            AnchoP = System.Windows.SystemParameters.WorkArea.Width;
            Reso.Text ="Resolucion: "+ string.Format("{0:f0}", AnchoP)+" X "+ string.Format("{0:f0}",AltoP);
            Uri iconUri = new Uri("icon.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            if (KinectSensor.KinectSensors.Count == 0)
            {
                App.Current.Shutdown();
            }
            else
            {
                mikinectD = KinectSensor.KinectSensors.FirstOrDefault();
                mikinectD.Start();
                mikinectD.DepthStream.Enable();
                mikinectD.DepthFrameReady += mikinectD_DepthFrameReady;
               /*mikinectD.ColorStream.Enable();
               mikinectD.ColorFrameReady += mikinectD_ColorFrameReady;*/
                mikinectD.SkeletonStream.Enable();
                mikinectD.SkeletonFrameReady += mikinectD_SkeletonFrameReady;
                App.Current.Exit += Current_Exit;
                
            }
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
           if(mikinectD!=null){
               mikinectD.Stop();
               mikinectD = null;
           }
        }

        void mikinectD_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            canvasEsqueleto.Children.Clear();
            ClickZona.Visibility = Visibility.Hidden;
            EnterZona.Visibility = Visibility.Hidden;
            MouseZona.Visibility = Visibility.Hidden;
            EscZona.Visibility = Visibility.Hidden;
            string msgst = "No hay datos de esqueleto", msgcap = "Acercate al area", msgmd = "No hay datos de esqueleto", msgmi = "No hay datos de esqueleto";
            Skeleton[] esqueleto = null;
            using (SkeletonFrame frame = e.OpenSkeletonFrame()) {
                if (frame != null) {
                    esqueleto=new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(esqueleto);
                }
            }
            if (esqueleto == null) return;
            foreach (Skeleton esqaux in esqueleto) {
                if (esqaux.TrackingState == SkeletonTrackingState.Tracked) {
                    if (esqaux.ClippedEdges == 0) { msgcap = "Posicion adecuada"; }
                    else{
                        if ((esqaux.ClippedEdges & FrameEdges.Bottom) != 0)
                            msgcap = "Aceptable, Aléjate más";
                        if ((esqaux.ClippedEdges & FrameEdges.Top) != 0)
                            msgcap = "Aceptable, más abajo";
                        if ((esqaux.ClippedEdges & FrameEdges.Right) != 0)
                            msgcap = "Aceptable, más a la izquierda";
                        if ((esqaux.ClippedEdges & FrameEdges.Left) != 0)
                            msgcap = "Aceptable, más a la derecha";

                    }
                    agregarLinea(esqaux.Joints[JointType.ElbowRight], esqaux.Joints[JointType.WristRight], false);
                    agregarLinea(esqaux.Joints[JointType.WristRight], esqaux.Joints[JointType.HandRight], false);
                    agregarELlipce(esqaux.Joints[JointType.HandRight], 30, false);
                    agregarLinea(esqaux.Joints[JointType.ShoulderCenter], esqaux.Joints[JointType.ShoulderRight], false);
                    agregarLinea(esqaux.Joints[JointType.ShoulderRight], esqaux.Joints[JointType.ElbowRight], false);
                    moverMouse(esqaux);
                    // Columna Vertebral
                    agregarLinea(esqaux.Joints[JointType.Head], esqaux.Joints[JointType.ShoulderCenter],false);
                    agregarLinea(esqaux.Joints[JointType.ShoulderCenter], esqaux.Joints[JointType.Spine], false);
                    agregarELlipce(esqaux.Joints[JointType.Head], 50, false);
                    // Pierna Izquierda
                    agregarLinea(esqaux.Joints[JointType.Spine], esqaux.Joints[JointType.HipCenter], false);
                    agregarLinea(esqaux.Joints[JointType.HipCenter], esqaux.Joints[JointType.HipLeft], false);
                    agregarLinea(esqaux.Joints[JointType.HipLeft], esqaux.Joints[JointType.KneeLeft], false);
                    agregarLinea(esqaux.Joints[JointType.KneeLeft], esqaux.Joints[JointType.AnkleLeft], false);
                    agregarLinea(esqaux.Joints[JointType.AnkleLeft], esqaux.Joints[JointType.FootLeft], false);
                    // Pierna Derecha
                    agregarLinea(esqaux.Joints[JointType.HipCenter], esqaux.Joints[JointType.HipRight], false);
                    agregarLinea(esqaux.Joints[JointType.HipRight], esqaux.Joints[JointType.KneeRight], false);
                    agregarLinea(esqaux.Joints[JointType.KneeRight], esqaux.Joints[JointType.AnkleRight], false);
                    agregarLinea(esqaux.Joints[JointType.AnkleRight], esqaux.Joints[JointType.FootRight], false);
                    // Brazo Derecho

 
                    Joint jointhead = esqaux.Joints[JointType.Head];
                    Joint jointMD = esqaux.Joints[JointType.HandRight];
                    Joint jointMI = esqaux.Joints[JointType.HandLeft];
                    SkeletonPoint poshead = jointhead.Position;
                    SkeletonPoint posmd = jointMD.Position;
                    SkeletonPoint posmi = jointMI.Position;
                    msgst = string.Format("Cabeza: X:{0:0.000} Y:{0:0.000} Z:{0:0.000}", poshead.X, poshead.Y, poshead.Z);
                    msgmd = string.Format("M.Derecha: X:{0:0.000} Y:{0:0.000} Z:{0:0.000}", posmd.X, posmd.Y, posmd.Z);
                    msgmi = string.Format("M.Izquierda: X:{0:0.000} Y:{0:0.000} Z:{0:0.000}", posmi.X, posmi.Y, posmi.Z);
                   
                }
                status.Text = msgst;
                statusMD.Text = msgmd;
                statusMI.Text = msgmi;
               label.Content = msgcap;
            }
            
        }
        void ESC(Skeleton esqaux)
        {
            if (esqaux.Joints[JointType.HandRight].Position.X > esqaux.Joints[JointType.HipCenter].Position.X + .28 && esqaux.Joints[JointType.HandRight].Position.X > esqaux.Joints[JointType.HipCenter].Position.X + .38 && esqaux.Joints[JointType.ElbowRight].Position.X > esqaux.Joints[JointType.Spine].Position.X + .12 && esqaux.Joints[JointType.ElbowRight].Position.X > esqaux.Joints[JointType.Spine].Position.X + .28&&esqaux.Joints[JointType.HandRight].Position.Y + .09 < esqaux.Joints[JointType.Spine].Position.Y)
            {
                EscZona.Visibility = Visibility.Visible;
                agregarLinea(esqaux.Joints[JointType.ShoulderCenter], esqaux.Joints[JointType.ShoulderRight], true);
                agregarLinea(esqaux.Joints[JointType.ShoulderRight], esqaux.Joints[JointType.ElbowRight], true);
                agregarLinea(esqaux.Joints[JointType.ElbowRight], esqaux.Joints[JointType.WristRight], true);
                agregarLinea(esqaux.Joints[JointType.WristRight], esqaux.Joints[JointType.HandRight], true);
                agregarELlipce(esqaux.Joints[JointType.HandRight], 30, true);
                if (actEsc == false) { System.Windows.Forms.SendKeys.SendWait("{ESC}"); actEsc = true; }

            }
            else { actEsc = false; }
        }
        int Enter(Skeleton esqaux) {
            if (esqaux.Joints[JointType.HandLeft].Position.X < esqaux.Joints[JointType.HipCenter].Position.X - .50 && esqaux.Joints[JointType.HandLeft].Position.X > esqaux.Joints[JointType.HipCenter].Position.X - .65 && esqaux.Joints[JointType.ShoulderLeft].Position.Y > esqaux.Joints[JointType.HandLeft].Position.Y)
            {
                drawBrazoizq(esqaux,true);
                EnterZona.Visibility = Visibility.Visible;
                if (actenter == false) {
                    System.Windows.Forms.SendKeys.SendWait("{Enter}"); actenter = true; }
                return 1;

            }
            else { drawBrazoizq(esqaux, false); actenter = false; return 0; }
 
        }
        void drawBrazoizq(Skeleton esqaux,bool act)
        {
            agregarLinea(esqaux.Joints[JointType.ShoulderCenter], esqaux.Joints[JointType.ShoulderLeft], act);
            agregarLinea(esqaux.Joints[JointType.ShoulderLeft], esqaux.Joints[JointType.ElbowLeft], act);
            agregarLinea(esqaux.Joints[JointType.ElbowLeft], esqaux.Joints[JointType.WristLeft], act);
            agregarLinea(esqaux.Joints[JointType.WristLeft], esqaux.Joints[JointType.HandLeft], act);
            agregarELlipce(esqaux.Joints[JointType.HandLeft], 30, act);
        }
        void moverMouse(Skeleton esqaux)
        {
            double x,y;
            click(esqaux);
            if (esqaux.Joints[JointType.HandRight].Position.Y + .05 > esqaux.Joints[JointType.Spine].Position.Y)
            {
                MouseZona.Visibility = Visibility.Visible;
                x = (AnchoP * esqaux.Joints[JointType.HandRight].Position.X) / .65;
                y = AltoP - ((AltoP * esqaux.Joints[JointType.HandRight].Position.Y) / esqaux.Joints[JointType.Head].Position.Y);
                KinectMouseController.KinectMouseMethods.SendMouseInput
                (Convert.ToInt32(x), Convert.ToInt32(y), (int)SystemInformation.PrimaryMonitorSize.Width, (int)SystemInformation.PrimaryMonitorSize.Height, actclick);
                agregarLinea(esqaux.Joints[JointType.ElbowRight], esqaux.Joints[JointType.WristRight], true);
                agregarLinea(esqaux.Joints[JointType.WristRight], esqaux.Joints[JointType.HandRight], true);
                agregarELlipce(esqaux.Joints[JointType.HandRight], 30, true);
            }
            else
            {
                ESC(esqaux);
            }
            // invertido
            /*System.Windows.Forms.Cursor.Position = new System.Drawing.Point( Convert.ToInt32(x), Convert.ToInt32(y)); //Mover mouse*/

            if (actclick == false && Enter(esqaux) == 1) { return; }
                     drawBrazoizq(esqaux,actclick);
        }

        void click(Skeleton esqaux)
        {
            if (esqaux.Joints[JointType.HandLeft].Position.X < esqaux.Joints[JointType.HipCenter].Position.X - .30 && esqaux.Joints[JointType.HandLeft].Position.X > esqaux.Joints[JointType.HipCenter].Position.X - .41 && esqaux.Joints[JointType.ElbowLeft].Position.X < esqaux.Joints[JointType.Spine].Position.X - .13 && esqaux.Joints[JointType.ElbowLeft].Position.X > esqaux.Joints[JointType.Spine].Position.X - .30)
            {
                ClickZona.Visibility = Visibility.Visible;
                if (actclick == false) {actclick = true;}
            
            }
            else { actclick = false; }
        }
       
        void agregarELlipce(Joint j1, int H,bool act)
        {
            Ellipse p = new Ellipse();
            if (act == false) { p.Fill = new SolidColorBrush(Colors.Green); }
            else { p.Fill = new SolidColorBrush(Colors.Red);}
            p.Height = H; p.Width = H;
            CoordinateMapper map = mikinectD.CoordinateMapper;
            var j1p = map.MapSkeletonPointToColorPoint(j1.Position, mikinectD.ColorStream.Format);
            Canvas.SetLeft(p,j1p.X-p.Width/2);
            Canvas.SetTop(p, j1p.Y-p.Height/2);
            canvasEsqueleto.Children.Add(p);
        
        }

        void agregarLinea(Joint j1, Joint j2,bool act)
        {
            Line lineaHueso = new Line();
            if (act == false) { lineaHueso.Stroke = new SolidColorBrush(Colors.Green);  }
            else { lineaHueso.Stroke = new SolidColorBrush(Colors.Red);  }
            lineaHueso.StrokeThickness = 6;
            ColorImagePoint j1P = mikinectD.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X1 = j1P.X;
            lineaHueso.Y1 = j1P.Y;
            ColorImagePoint j2P = mikinectD.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X2 = j2P.X;
            lineaHueso.Y2 = j2P.Y;
            canvasEsqueleto.Children.Add(lineaHueso);
        }

        short[] Distancia = null;
        byte[] ColorImagenDistancia = null;
        WriteableBitmap bitmapImagenDistancia = null;

        void mikinectD_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame frame = e.OpenDepthImageFrame())
            {
                if (frame == null) return;
                if (Distancia == null)
                    Distancia = new short[frame.PixelDataLength];
                if (ColorImagenDistancia == null)
                    ColorImagenDistancia = new byte[frame.PixelDataLength * 4];

                frame.CopyPixelDataTo(Distancia);

                int postcolor = 0;
                for (int i = 0; i < frame.PixelDataLength; i++)
                {
                    int val = Distancia[i] >> 3;
                    if (val == mikinectD.DepthStream.UnknownDepth)
                    { //pixel no recolocido
                        ColorImagenDistancia[postcolor++] = 0; //azul
                        ColorImagenDistancia[postcolor++] = 0;//verde
                        ColorImagenDistancia[postcolor++] = 255;//rojo
                    }
                    else if (val == mikinectD.DepthStream.TooFarDepth)
                    { //pixeles lejos
                        ColorImagenDistancia[postcolor++] = 255; //azul
                        ColorImagenDistancia[postcolor++] = 0;//verde
                        ColorImagenDistancia[postcolor++] = 0;//rojo
                    }
                    else if (val == mikinectD.DepthStream.TooNearDepth)
                    { //pixeles correcta
                        ColorImagenDistancia[postcolor++] = 0; //azul
                        ColorImagenDistancia[postcolor++] = 255;//verde
                        ColorImagenDistancia[postcolor++] = 0;//rojo
                    }
                    else
                    {
                        byte byted = (byte)(255 - val >> 5);
                        ColorImagenDistancia[postcolor++] = byted;
                        ColorImagenDistancia[postcolor++] = byted;
                        ColorImagenDistancia[postcolor++] = byted;
                    }

                    postcolor++;
                }
                if (bitmapImagenDistancia == null)
                {
                    this.bitmapImagenDistancia = new WriteableBitmap(
                        frame.Width,
                        frame.Height, 96, 96, PixelFormats.Bgr32, null);
                    Cprofundidad.Source = bitmapImagenDistancia;
                }
                this.bitmapImagenDistancia.WritePixels(new Int32Rect(0, 0, frame.Width, frame.Height),
                    ColorImagenDistancia, frame.Width * 4, 0);
            }

        }
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        /*
        private void mikinectD_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {

            using (ColorImageFrame framesColor = e.OpenColorImageFrame())
            {
                if (framesColor == null) return;

                if (datosColor == null)
                    datosColor = new byte[framesColor.PixelDataLength];

                framesColor.CopyPixelDataTo(datosColor);

                if (colorImagenBitmap == null)
                {
                    this.colorImagenBitmap = new WriteableBitmap(
                        framesColor.Width,
                        framesColor.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);
                }

                this.colorImagenBitmap.WritePixels(
                    new Int32Rect(0, 0, framesColor.Width, framesColor.Height),
                    datosColor,
                    framesColor.Width * framesColor.BytesPerPixel,
                    0
                    );

                canvasEsqueleto.Background = new ImageBrush(colorImagenBitmap);
            }
        }*/

    }
}
