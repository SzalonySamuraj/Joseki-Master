using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;
using Baduk;

namespace JosekiMaster
{
    class WPFBadukBoardRenderer
    {
        public BadukBoard Board;

        Polygon BoardPolygon;
        Rectangle KORectangle;
        Rectangle HintRectangle;
        List<Line> lines;
        List<Ellipse> hoshi_ellipces;
        List<Ellipse> mark_ellipces;
        List<Rectangle> stone_rectangles;

        public bool BoardVisible = true;

        public int cell_size;
        public int margin;
        public int pos_x, pos_y;
        public int Width, Height;

        public int ClientWidth, ClientHeight;

        public int Hint_X, Hint_Y, Hint_Color;

        public double ScaleFactor = 1.0;
        public bool ClickSuccesfull = false;

        public bool ShowHint;

        public WPFBadukBoardRenderer()
        {
            KORectangle = new Rectangle();
            HintRectangle = new Rectangle();
            lines = new List<Line>();
            hoshi_ellipces = new List<Ellipse>();
            mark_ellipces = new List<Ellipse>();
            stone_rectangles = new List<Rectangle>();
            ShowHint = false;
            cell_size = 45;
            margin = 20;
            ClientWidth = 1600;
            ClientHeight = 900;
            Hint_X = -1;
            Hint_Y = -1;
            Hint_Color = -1;
        }

        public void InitBoard(BadukBoard pBoard)
        {
            Board = pBoard;
            CalculateMetrics();
        }

        public void UpdateRenderObjects()
        {
            lines.Clear();
            hoshi_ellipces.Clear();
            mark_ellipces.Clear();
            stone_rectangles.Clear();
            KORectangle = new Rectangle();
            BoardPolygon = new Polygon();

            if (BoardVisible)
            {
                string fullfilepath = System.Reflection.Assembly.GetEntryAssembly().Location;
                string[] parts = fullfilepath.Split('\\');
                string parentfolder = "";
                int numparts = parts.Count();
                for (int i = 0; i < numparts - 1; i++)
                {
                    parentfolder += parts[i];
                    parentfolder += "/";
                }

                Color WoodenColor = Color.FromRgb(247, 216, 137);
                SolidColorBrush WoodenBrush = new SolidColorBrush(WoodenColor);

                BoardPolygon = new Polygon();
                BoardPolygon.Stroke = System.Windows.Media.Brushes.Black;
                BoardPolygon.Fill = WoodenBrush;
                System.Windows.Point Point1 = new System.Windows.Point(pos_x, pos_y);
                System.Windows.Point Point2 = new System.Windows.Point(pos_x + Width, pos_y);
                System.Windows.Point Point3 = new System.Windows.Point(pos_x + Width, pos_y + Height);
                System.Windows.Point Point4 = new System.Windows.Point(pos_x, pos_y + Height);
                PointCollection myPointCollection = new PointCollection();
                myPointCollection.Add(Point1);
                myPointCollection.Add(Point2);
                myPointCollection.Add(Point3);
                myPointCollection.Add(Point4);
                BoardPolygon.Points = myPointCollection;



                //Init vertical lines
                for (int i = 0; i < Board.size_x; i++)
                {
                    Line CurrentLine = new Line();
                    CurrentLine.Stroke = System.Windows.Media.Brushes.Black;
                    CurrentLine.StrokeThickness = 1;
                    CurrentLine.X1 = pos_x + margin + i * cell_size;
                    CurrentLine.X2 = pos_x + margin + i * cell_size;
                    CurrentLine.Y1 = pos_y + margin;
                    CurrentLine.Y2 = pos_y + margin + cell_size * (Board.size_y - 1);
                    CurrentLine.SnapsToDevicePixels = true;
                    CurrentLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                    lines.Add(CurrentLine);
                }

                for (int i = 0; i < Board.size_y; i++)
                {
                    Line CurrentLine = new Line();
                    CurrentLine.Stroke = System.Windows.Media.Brushes.Black;
                    CurrentLine.StrokeThickness = 1;
                    CurrentLine.X1 = pos_x + margin;
                    CurrentLine.X2 = pos_x + margin + cell_size * (Board.size_x - 1);
                    CurrentLine.Y1 = pos_y + margin + i * cell_size;
                    CurrentLine.Y2 = pos_y + margin + i * cell_size;
                    CurrentLine.SnapsToDevicePixels = true;
                    CurrentLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                    lines.Add(CurrentLine);
                }



                //Init Hoshi points on 19 board
                if (Board.size_x == 19 && Board.size_y == 19)
                {
                    int[] hoshi_coords = new int[] { 3, 3, 3, 9, 3, 15, 9, 3, 9, 9, 9, 15, 15, 3, 15, 9, 15, 15 };

                    for (int i = 0; i < 9; i++)
                    {
                        Ellipse myEllipse = new Ellipse();
                        myEllipse.Stroke = System.Windows.Media.Brushes.Black;
                        myEllipse.Fill = System.Windows.Media.Brushes.Black;
                        myEllipse.Width = 7;
                        myEllipse.Height = 7;
                        int hoshi_left = pos_x + margin + hoshi_coords[2 * i] * cell_size - 3;
                        int hoshi_top = pos_y + margin + hoshi_coords[2 * i + 1] * cell_size - 3;
                        myEllipse.Margin = new Thickness(hoshi_left, hoshi_top, 0, 0);
                        hoshi_ellipces.Add(myEllipse);
                    }
                }

                //Init marks


                for (int i = 0; i < Board.marks.Count; i++)
                {
                    Ellipse MarkEllipse = new Ellipse();
                    MarkEllipse.Stroke = System.Windows.Media.Brushes.Black;
                    MarkEllipse.Fill = System.Windows.Media.Brushes.Yellow;
                    MarkEllipse.Width = 7;
                    MarkEllipse.Height = 7;
                    int mark_left = pos_x + margin + Board.marks[i].X * cell_size - 3;
                    int mark_top = pos_y + margin + Board.marks[i].Y * cell_size - 3;
                    MarkEllipse.Margin = new Thickness(mark_left, mark_top, 0, 0);
                    mark_ellipces.Add(MarkEllipse);
                }

                //Render KO mark
                if (Board.ko_X != -1 && Board.ko_Y != -1)
                {
                    KORectangle = new Rectangle();
                    KORectangle.Height = 19;
                    KORectangle.Width = 19;
                    SolidColorBrush blackBrush = new SolidColorBrush();
                    blackBrush.Color = Colors.Black;
                    KORectangle.StrokeThickness = 1;
                    KORectangle.Stroke = blackBrush;
                    int ko_left = pos_x + margin + Board.ko_X * cell_size - 9;
                    int ko_top = pos_y + margin + Board.ko_Y * cell_size - 9;
                    KORectangle.Margin = new Thickness(ko_left, ko_top, 0, 0);
                }
                else
                {
                    KORectangle = new Rectangle();
                }

                ImageBrush BlackStoneBrush = new ImageBrush();
                BlackStoneBrush.ImageSource = new BitmapImage(new Uri(parentfolder + "src/img/stone_black.png", UriKind.Absolute));
                ImageBrush WhiteStoneBrush = new ImageBrush();
                WhiteStoneBrush.ImageSource = new BitmapImage(new Uri(parentfolder + "src/img/stone_white.png", UriKind.Absolute));

                for (int i = 0; i < Board.stones.Count; i++)
                {
                    Rectangle StoneRectangle = new Rectangle();
                    StoneRectangle.Height = cell_size;
                    StoneRectangle.Width = cell_size;
                    if (Board.stones[i].Color == 0)
                    {
                        StoneRectangle.Fill = BlackStoneBrush;
                    }
                    else
                    {
                        StoneRectangle.Fill = WhiteStoneBrush;
                    }
                    int stone_x = pos_x + margin + Board.stones[i].X * cell_size - cell_size / 2;
                    int stone_y = pos_y + margin + Board.stones[i].Y * cell_size - cell_size / 2;
                    StoneRectangle.Margin = new Thickness(stone_x, stone_y, 0, 0);
                    stone_rectangles.Add(StoneRectangle);
                }

                if (Hint_X != -1 && Hint_Y != -1 && (Hint_Color == 0 || Hint_Color == 1))
                {
                    HintRectangle = new Rectangle();
                    HintRectangle.Height = (int)(cell_size * 0.688);
                    HintRectangle.Width = (int)(cell_size * 0.688);
                    HintRectangle.Opacity = 0.7;
                    if (Hint_Color == 0)
                    {
                        HintRectangle.Fill = BlackStoneBrush;
                    }
                    else
                    {
                        HintRectangle.Fill = WhiteStoneBrush;
                    }
                    int hint_left = pos_x + margin + Hint_X * cell_size - (int)(15 * ScaleFactor);
                    int hint_top = pos_y + margin + Hint_Y * cell_size - (int)(15 * ScaleFactor);
                    HintRectangle.Margin = new Thickness(hint_left, hint_top, 0, 0);
                    stone_rectangles.Add(HintRectangle);
                }
                else
                {
                    ShowHint = false;
                }
            }
        }

        public void Render(Panel BoardPanel)
        {
            BoardPanel.Children.Clear();
            UpdateRenderObjects();
            BoardPanel.Children.Add(BoardPolygon);
            for (int i = 0; i < lines.Count; i++)
            {
                BoardPanel.Children.Add(lines[i]);
            }
            for (int i = 0; i < hoshi_ellipces.Count; i++)
            {
                BoardPanel.Children.Add(hoshi_ellipces[i]);
            }
            for (int i = 0; i < stone_rectangles.Count; i++)
            {
                BoardPanel.Children.Add(stone_rectangles[i]);
            }
            for (int i = 0; i < mark_ellipces.Count; i++)
            {
                BoardPanel.Children.Add(mark_ellipces[i]);
            }
            BoardPanel.Children.Add(KORectangle);
            if(ShowHint)
            {
                BoardPanel.Children.Add(HintRectangle);
            }
        }

        public void SetHint(int pColor, int pX, int pY)
        {
            bool CoordsValidated = Board.ValidateCoords(pX, pY);
            if (CoordsValidated == false)
            {
                return;
            }

            if (pColor == 0 || pColor == 1 || pColor == -1)
            {
                Hint_X = pX;
                Hint_Y = pY;
                Hint_Color = pColor;
            }
            else
            {
                ClearHint();
            }
        }

        public void ClearHint()
        {
            Hint_X = -1;
            Hint_Y = -1;
            Hint_Color = -1;
        }

        public void CalculateMetrics()
        {
            Width = (Board.size_x - 1) * cell_size + 2 * margin;
            Height = (Board.size_y - 1) * cell_size + 2 * margin;
            pos_x = (ClientWidth - Width) / 2;
            pos_y = (ClientHeight - Height) / 2 + 12;
        }

        public void SetResolution(int pScreenWidth, int pScreenHeight)
        {
            ClientWidth = pScreenWidth;
            ClientHeight = pScreenHeight;
            CalculateMetrics();
        }
    }
}
