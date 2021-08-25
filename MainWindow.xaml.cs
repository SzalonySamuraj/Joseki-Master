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
using Baduk;

namespace JosekiMaster
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JosekiMasterScene Scene = new JosekiMasterScene();
        DispatcherTimer MakeNextMoveTimer = new DispatcherTimer();
        DispatcherTimer RepeatJosekiTimer = new DispatcherTimer();
        BitmapImage ImageLizaNeutral, ImageLizaHappy, ImageLizaAngry;

        string RootPath;
        bool isFullscreen = false;
        int OldW = 1600, OldH = 900;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitRootPath();

            ImageLizaNeutral = new BitmapImage(new Uri(RootPath + "/src/img/liza/liza-neutral.png", UriKind.Absolute));
            ImageLizaHappy = new BitmapImage(new Uri(RootPath + "/src/img/liza/liza-happy.png", UriKind.Absolute));
            ImageLizaAngry = new BitmapImage(new Uri(RootPath + "/src/img/liza/liza-angry.png", UriKind.Absolute));

            MakeNextMoveTimer.Tick += new EventHandler(MakeNextMoveTimer_Tick);
            MakeNextMoveTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            MakeNextMoveTimer.Stop();

            RepeatJosekiTimer.Tick += new EventHandler(RepeatJosekiTimer_Tick);
            RepeatJosekiTimer.Interval = new TimeSpan(0, 0, 0, 0, 1500);
            RepeatJosekiTimer.Stop();
            Scene.InitTimers(MakeNextMoveTimer, RepeatJosekiTimer);

            Render();
        }

        private void InitRootPath()
        {
            string fullfilepath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string[] parts = fullfilepath.Split('\\');
            RootPath = "";
            int numparts = parts.Count();
            for (int i = 0; i < numparts - 1; i++)
            {
                RootPath += parts[i];
                RootPath += "/";
            }
        }

        void Render()
        {
            Scene.BoardRenderer.BoardVisible = Scene.BoardVisible;
            Scene.BoardRenderer.UpdateRenderObjects();
            Scene.BoardRenderer.Render(BoardCanvas);
            if(Scene.ResultPositionDemo)
            {
                ClickToStartImage.Visibility = Visibility.Visible;
            }
            else
            {
                ClickToStartImage.Visibility = Visibility.Hidden;
            }
            switch(Scene.CharacterEmotion)
            {
                case "liza-neutral":
                    ImageLiza.Source = ImageLizaNeutral;
                    break;
                case "liza-happy":
                    ImageLiza.Source = ImageLizaHappy;
                    break;
                case "liza-angry":
                    ImageLiza.Source = ImageLizaAngry;
                    break;
            }
            TopLeftTextblock.Text = Scene.TopLeftText;
        }

        private void SetFulscreen(bool param)
        {
            if(param == true)
            {
                double ClientWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double ClientHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

                OldW = Scene.ClientWidth;
                OldH = Scene.ClientHeight;

                if (    (ClientWidth == 1920 && ClientHeight == 1080) ||
                        (ClientWidth == 1600 && ClientHeight == 900) ||
                        (ClientWidth == 1280 && ClientHeight == 720) ||
                        (ClientWidth == 1024 && ClientHeight == 576) ||
                        (ClientWidth == 854 && ClientHeight == 480))
                {
                    isFullscreen = true;

                    SetResolution((int)ClientWidth, (int)ClientHeight);
                    Render();

                    Visibility = Visibility.Collapsed;
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    SizeToContent = SizeToContent.Manual;
                    WindowState = WindowState.Maximized;
                    Topmost = true;
                    Visibility = Visibility.Visible;
                }
            }
            else
            {
                SetResolution(OldW, OldH);
                Render();

                isFullscreen = false;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanMinimize;
                SizeToContent = SizeToContent.WidthAndHeight;
                WindowState = WindowState.Normal;
                Topmost = false;
            }
        }

        private void MakeNextMoveTimer_Tick(object sender, EventArgs e)
        {
            MakeNextMoveTimer.Stop();
            Scene.MakeNextMove();
            if(!Scene.JosekiFinished)
            {
                Scene.BoardEnabled = true;
            }
            Render();
        }

        private void RepeatJosekiTimer_Tick(object sender, EventArgs e)
        {
            RepeatJosekiTimer.Stop();
            Scene.RepeatJoseki();
            Render();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            int x = (int)p.X;
            int y = (int)p.Y;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Scene.ShiftPressed = true;
            }
            else
            {
                Scene.ShiftPressed = false;
            }
            Scene.LeftClick(x, y);            
            Render();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Scene.OpenJosekiCollection();
            Render();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O)
            {
                Scene.OpenJosekiCollection();
                Render();
            }
            if (e.Key == Key.L)
            {
                if(Scene.State != "learning")
                {
                    Scene.SetState("learning");
                    Render();
                }
            }
            if (e.Key == Key.T)
            {
                if (Scene.State != "training")
                {
                    Scene.SetState("training");
                    Render();
                }
            }
            if (e.Key == Key.E)
            {
                if (Scene.State != "editor-overview")
                {
                    Scene.SetState("editor-overview");
                    Render();
                }
            }

            if (e.Key == Key.I)
            {
                if (Scene.State == "editor-overview")
                {
                    Scene.EditorInsertNewJoseki();
                    Render();
                }
            }

            if (e.Key == Key.Right)
            {
                Scene.EditorGoNext();
                Render();
            }
            if(e.Key == Key.Left)
            {
                Scene.EditorGoPrevious();
                Render();
            }
            if (e.Key == Key.Enter)
            {
                if (Scene.State == "editor-starting-stones")
                {
                    Scene.EditorProcessToMoves();
                    Render();
                }
                else if (Scene.State == "editor-writing-moves")
                {
                    Scene.EditorSaveCurrentJoseki();
                    Render();
                }
            }
            if (e.Key == Key.Space)
            {
                if(Scene.State == "learning")
                {
                    Scene.IncrementIndex();
                    Scene.ShowResultPosition();
                    Render();
                }
            }
            if(e.Key == Key.Delete)
            {
                if (Scene.State == "editor-overview")
                {
                    Scene.EditorDeleteCurrentJoseki();
                    Render();
                }
            }
            if(e.Key == Key.Tab)
            {
                if(MenuGeneral.Visibility == Visibility.Visible)
                {
                    MenuGeneral.Visibility = Visibility.Hidden;
                }
                else
                {
                    MenuGeneral.Visibility = Visibility.Visible;
                }
            }
            if (e.Key == Key.F)
            {
                SetFulscreen(!isFullscreen);
                Render();
            }
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Scene.State == "training")
            {
                if (Scene.ResultPositionDemo == false)
                {
                    if (Scene.BoardEnabled || Scene.LastMoveCorrect == false)
                    {
                        Scene.ResetPosition();
                        if (!Scene.PlayerFirstMove)
                        {
                            Scene.MakeNextMove();
                        }
                    }
                }
                else
                {
                    Scene.ResultPositionDemo = false;
                    Scene.ResetPosition();
                    if (!Scene.PlayerFirstMove)
                    {
                        Scene.MakeNextMove();
                    }
                }
                Render();
            }
            if(Scene.State == "learning")
            {
                if (Scene.ResultPositionDemo == false)
                {
                    if (Scene.BoardEnabled || Scene.LastMoveCorrect == false)
                    {
                        Scene.ResetPosition();                        
                    }
                }
                else
                {
                    Scene.ResultPositionDemo = false;
                    Scene.ResetPosition();
                }
                Render();
            }
            if (Scene.State == "editor-starting-stones" || Scene.State == "editor-writing-moves")
            {
                Scene.ResetPosition();
                Render();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Scene.SetState("learning");
            Render();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Scene.SetState("editor-overview");
            Render();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Scene.SetState("training");
            Render();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            string targetURL = @"http://skillplay.pro";
            System.Diagnostics.Process.Start(targetURL);
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            string targetURL = @"http://vk.com/skillplay";
            System.Diagnostics.Process.Start(targetURL);
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            string targetURL = @"https://patreon.com/skillplay";
            System.Diagnostics.Process.Start(targetURL);
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            string targetURL = @"https://discord.gg/4RFAZsf";
            System.Diagnostics.Process.Start(targetURL);
        }

        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            SetResolution(1280, 720);
            Render();
        }

        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            SetResolution(1600, 900);
            Render();
        }

        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            SetResolution(1920, 1080);
            Render();
        }

        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {
            SetResolution(1024, 576);
            Render();
        }

        private void MenuItem_Click_13(object sender, RoutedEventArgs e)
        {
            SetResolution(854, 480);
            Render();
        }

        private void MenuItem_Click_14(object sender, RoutedEventArgs e)
        {
            SetFulscreen(!isFullscreen);
            Render();
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            Scene.SetState("about");
            Render();
        }

        private void SetResolution(int pX, int pY)
        {
            Scene.SetResolution(pX, pY);
            ImageBackground.Width = pX;
            ImageBackground.Height = pY;
            ImageLiza.Width = Scene.CharacterWidth;
            ImageLiza.Height = Scene.CharacterHeight;
            TopLeftTextblock.FontSize = Scene.FontSize;
            Render();
        }
    }
}
