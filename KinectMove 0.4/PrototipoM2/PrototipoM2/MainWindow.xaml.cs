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
        double AnchoP;
        double AltoP;

        bool actclick = false;
        bool actenter = false;
        bool actesc2 = false;
        bool actEsc = false;
        bool actizq = false;
        bool actder = false;
        bool actclose = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            CenterWindowOnScreen();
            AltoP= System.Windows.SystemParameters.WorkArea.Height;
            AnchoP = System.Windows.SystemParameters.WorkArea.Width;
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
                mikinectD.SkeletonStream.Enable();
                mikinectD.SkeletonFrameReady += mikinectD_SkeletonFrameReady;
                mikinectD.ColorStream.Enable();
                mikinectD.ColorFrameReady += mikinectD_ColorFrameReady;
               
                App.Current.Exit += Current_Exit;
                
            }
        }
        void mikinectD_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame == null) return;
                byte[] datos = new byte[frame.PixelDataLength];
                frame.CopyPixelDataTo(datos);
               
                Cprofundidad.Source = BitmapSource.Create(frame.Width, frame.Height, 96, 96,
                    PixelFormats.Bgr32, null, datos, frame.Width * frame.BytesPerPixel);
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
            splash.Visibility = Visibility.Hidden;
            ClickZona.Visibility = Visibility.Hidden;
            EnterZona.Visibility = Visibility.Hidden;
            MouseZona.Visibility = Visibility.Hidden;
            MouseZona1.Visibility = Visibility.Hidden;
            EscZona.Visibility = Visibility.Hidden;
            IZQzona.Visibility = Visibility.Hidden;
            DERzona.Visibility = Visibility.Hidden;
            CerrarZona.Visibility = Visibility.Hidden;
            ZonaEsc2.Visibility = Visibility.Hidden;
            ZonaUP.Visibility = Visibility.Hidden;
            ZonaTOP.Visibility = Visibility.Hidden;
            string msgcap = "Acercate al area";
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
                    agregarELlipce(esqaux.Joints[JointType.HandRight], 30, 0);
                    agregarLinea(esqaux.Joints[JointType.ShoulderCenter], esqaux.Joints[JointType.ShoulderRight], false);
                    agregarLinea(esqaux.Joints[JointType.ShoulderRight], esqaux.Joints[JointType.ElbowRight], false);
                    moverMouse(esqaux);
                    // Columna Vertebral
                    agregarLinea(esqaux.Joints[JointType.Head], esqaux.Joints[JointType.ShoulderCenter],false);
                    agregarLinea(esqaux.Joints[JointType.ShoulderCenter], esqaux.Joints[JointType.Spine], false);
                    agregarELlipce(esqaux.Joints[JointType.Head], 50, 0);
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
                    //Joindraw(esqaux);  //dibuja articulaciones demanda de recursos
                   
                }
                label.Content = msgcap;
            }
            
        }/*
        void Joindraw(Skeleton esqaux) {
            agregarELlipce(esqaux.Joints[JointType.Spine], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.ElbowRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.WristRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.ShoulderCenter], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.ShoulderRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.ElbowRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.HipCenter], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.HipLeft], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.KneeLeft], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.FootLeft], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.HipCenter], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.HipRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.KneeRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.AnkleRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.AnkleLeft], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.FootRight], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.ShoulderLeft], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.ElbowLeft], 15, 2);
            agregarELlipce(esqaux.Joints[JointType.WristLeft], 15, 2);
        }*/
        int scroll(Skeleton esqaux)
        {
            if (esqaux.Joints[JointType.HandLeft].Position.X < esqaux.Joints[JointType.HipCenter].Position.X && esqaux.Joints[JointType.HandLeft].Position.X >= esqaux.Joints[JointType.HipCenter].Position.X-.25)
            {
                if (esqaux.Joints[JointType.HandLeft].Position.Y > esqaux.Joints[JointType.Spine].Position.Y+.10 && esqaux.Joints[JointType.HandLeft].Position.Y < esqaux.Joints[JointType.ShoulderCenter].Position.Y+.1)
                {
                    
                    agregarLinea(esqaux.Joints[JointType.WristLeft], esqaux.Joints[JointType.HandLeft], true);
                    agregarELlipce(esqaux.Joints[JointType.HandLeft], 30, 1);

                        if (esqaux.Joints[JointType.HandLeft].Position.Y > (esqaux.Joints[JointType.Spine].Position.Y + .10 + esqaux.Joints[JointType.ShoulderCenter].Position.Y+.1) / 2)
                        { registro.Text = registro.Text + "\nFLECHA ARRIBA"; ZonaUP.Visibility = Visibility.Visible; System.Windows.Forms.SendKeys.SendWait("{UP}"); }
                        else
                        { registro.Text = registro.Text + "\nFLECHA ABAJO"; ZonaTOP.Visibility = Visibility.Visible; System.Windows.Forms.SendKeys.SendWait("{DOWN}"); }
                         return 1;
                }
            }
           return 0;
        }
        int izq(Skeleton esqaux)
        {
            if (esqaux.Joints[JointType.HandRight].Position.X < esqaux.Joints[JointType.HipCenter].Position.X && esqaux.Joints[JointType.HandRight].Position.Y< esqaux.Joints[JointType.Spine].Position.Y+.05)
            {
                DERzona.Visibility = Visibility.Visible;
                agregarLinea(esqaux.Joints[JointType.ElbowRight], esqaux.Joints[JointType.WristRight], true);
                agregarLinea(esqaux.Joints[JointType.WristRight], esqaux.Joints[JointType.HandRight], true);
                agregarELlipce(esqaux.Joints[JointType.HandRight], 30, 1);

                if (actizq == false) { registro.Text = registro.Text + "\n<-FLECHA IZQUIERDA"; System.Windows.Forms.SendKeys.SendWait("{LEFT}"); actizq = true; }
                return 1;

            }
            else { actclose = false; actizq = false; }
            return 0;
        }
       int der(Skeleton esqaux)
       {
           if (esqaux.Joints[JointType.HandLeft].Position.X >= esqaux.Joints[JointType.HipCenter].Position.X && esqaux.Joints[JointType.HandLeft].Position.Y < esqaux.Joints[JointType.Spine].Position.Y+.05)
           {
               IZQzona.Visibility = Visibility.Visible;
               agregarLinea(esqaux.Joints[JointType.ElbowLeft], esqaux.Joints[JointType.WristLeft], true);
               agregarLinea(esqaux.Joints[JointType.WristLeft], esqaux.Joints[JointType.HandLeft], true);
               agregarELlipce(esqaux.Joints[JointType.HandLeft], 30, 1);
               if (actder == false) { registro.Text = registro.Text + "\n->FLECHA DERECHO"; System.Windows.Forms.SendKeys.SendWait("{RIGHT}"); actder = true; }
               return 1;

           }
           else { actclose = false; actder = false; }
           return 0;
       }
       void click(Skeleton esqaux)
       {
           if (esqaux.Joints[JointType.HandLeft].Position.X < esqaux.Joints[JointType.HipCenter].Position.X - .30 && esqaux.Joints[JointType.HandLeft].Position.X > esqaux.Joints[JointType.HipCenter].Position.X - .41 && esqaux.Joints[JointType.ElbowLeft].Position.X < esqaux.Joints[JointType.Spine].Position.X - .13 && esqaux.Joints[JointType.ElbowLeft].Position.X > esqaux.Joints[JointType.Spine].Position.X - .30)
           {
               ClickZona.Visibility = Visibility.Visible;
               if (actclick == false) { registro.Text = registro.Text + "\nCLIC IZQUIERDO"; actclick = true; }

           }
           else { actclick = false; }
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
                agregarELlipce(esqaux.Joints[JointType.HandRight], 30, 1);
                if (actEsc == false) { registro.Text = registro.Text + "\nCLIC DERECHO";System.Windows.Forms.SendKeys.SendWait("+{F10}"); actEsc = true; }

            }
            else { actEsc = false; }
        }
        int esc2(Skeleton esqaux)
        {
            if (esqaux.Joints[JointType.HandLeft].Position.X < esqaux.Joints[JointType.HipCenter].Position.X - .40 && esqaux.Joints[JointType.ShoulderCenter].Position.Y< esqaux.Joints[JointType.HandLeft].Position.Y)
            {
                drawBrazoizq(esqaux, true);
                ZonaEsc2.Visibility = Visibility.Visible;
                if (actesc2 == false)
                {
                    registro.Text = registro.Text+"\nESC"; System.Windows.Forms.SendKeys.SendWait("{ESC}"); actesc2 = true;
                }
                return 1;

            }
            else { drawBrazoizq(esqaux, false); actesc2 = false; return 0; }

        }
        int Enter(Skeleton esqaux) {
            if (esqaux.Joints[JointType.HandLeft].Position.X < esqaux.Joints[JointType.HipCenter].Position.X - .50 && esqaux.Joints[JointType.HandLeft].Position.X > esqaux.Joints[JointType.HipCenter].Position.X - .65 && esqaux.Joints[JointType.ShoulderLeft].Position.Y > esqaux.Joints[JointType.HandLeft].Position.Y)
            {
                drawBrazoizq(esqaux,true);
                EnterZona.Visibility = Visibility.Visible;
                if (actenter == false) {
                    registro.Text = registro.Text + "\nENTER"; System.Windows.Forms.SendKeys.SendWait("{Enter}"); actenter = true;
                }
                return 1;

            }
            else { drawBrazoizq(esqaux, false); actenter = false; return 0; }
 
        }
        void drawBrazoizq(Skeleton esqaux,bool act)
        {
            int aux;
            agregarLinea(esqaux.Joints[JointType.ShoulderCenter], esqaux.Joints[JointType.ShoulderLeft], act);
            agregarLinea(esqaux.Joints[JointType.ShoulderLeft], esqaux.Joints[JointType.ElbowLeft], act);
            agregarLinea(esqaux.Joints[JointType.ElbowLeft], esqaux.Joints[JointType.WristLeft], act);
            agregarLinea(esqaux.Joints[JointType.WristLeft], esqaux.Joints[JointType.HandLeft], act);
            if (act == false) { aux = 0; }
            else { aux = 1; }
            agregarELlipce(esqaux.Joints[JointType.HandLeft], 30,aux);
        }
        void moverMouse(Skeleton esqaux)
        {
            double x,y;
            click(esqaux);
            if (esqaux.Joints[JointType.HandRight].Position.Y -.10> esqaux.Joints[JointType.Spine].Position.Y)
            {
                MouseZona.Visibility = Visibility.Visible;
                MouseZona1.Visibility = Visibility.Visible;
                x = (AnchoP * esqaux.Joints[JointType.HandRight].Position.X) / .65;
                if (actclick == true) { MouseZona.Visibility = Visibility.Hidden; }
                y = AltoP - ((AltoP * esqaux.Joints[JointType.HandRight].Position.Y) / esqaux.Joints[JointType.Head].Position.Y);
                KinectMouseController.KinectMouseMethods.SendMouseInput
                (Convert.ToInt32(x), Convert.ToInt32(2*y), (int)SystemInformation.PrimaryMonitorSize.Width, (int)SystemInformation.PrimaryMonitorSize.Height, actclick);
                agregarLinea(esqaux.Joints[JointType.ElbowRight], esqaux.Joints[JointType.WristRight], true);
                agregarLinea(esqaux.Joints[JointType.WristRight], esqaux.Joints[JointType.HandRight], true);
                agregarELlipce(esqaux.Joints[JointType.HandRight], 30,1);
               
            }
            else
            {
                ESC(esqaux);// clic derecho
                if (izq(esqaux) == 1 && der(esqaux) == 1 && actclose == false) {
                    IZQzona.Visibility = Visibility.Hidden;
                    DERzona.Visibility = Visibility.Hidden;
                    CerrarZona.Visibility = Visibility.Visible;
                    registro.Text = registro.Text + "\n(X)CERRAR";
                    actclose = true; System.Windows.Forms.SendKeys.SendWait("%{F4}"); }
            }
            // invertido
            /*System.Windows.Forms.Cursor.Position = new System.Drawing.Point( Convert.ToInt32(x), Convert.ToInt32(y)); //Mover mouse*/
            if (actclick == false && (Enter(esqaux) == 1 || der(esqaux) == 1 || esc2(esqaux) == 1 || scroll(esqaux)==1)) { return; }
                     drawBrazoizq(esqaux,actclick);
        }      
        void agregarELlipce(Joint j1, int H,int act)
        {
            Ellipse p = new Ellipse();
            if (act == 0) { p.Fill = new SolidColorBrush(Colors.Orange); }
            else if (act == 1) { p.Fill = new SolidColorBrush(Colors.Blue); }
            else { p.Fill = new SolidColorBrush(Colors.Green); }
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
            if (act == false) { lineaHueso.Stroke = new SolidColorBrush(Colors.Orange);  }
            else { lineaHueso.Stroke = new SolidColorBrush(Colors.Blue);  }
            lineaHueso.StrokeThickness = 6;
            ColorImagePoint j1P = mikinectD.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X1 = j1P.X;
            lineaHueso.Y1 = j1P.Y;
            ColorImagePoint j2P = mikinectD.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X2 = j2P.X;
            lineaHueso.Y2 = j2P.Y;
            canvasEsqueleto.Children.Add(lineaHueso);
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

    }
}
