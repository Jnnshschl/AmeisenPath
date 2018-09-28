using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using AmeisenPathCore;
using AmeisenPathCore.Objects;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace AmeisenPathGUI
{
    public partial class MainWindow : Window
    {
        NodePosition startPos;
        NodePosition endPos;

        List<Node> pathToGo = new List<Node>();

        Node[,] map;

        // Tile size, maybe a bit hacky but hey it works :D
        private int tileSize = 5;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetMap();
        }

        private void ResetMap()
        {
            Random rnd = new Random();
            startPos = new NodePosition(rnd.Next(0, (int)canvasPath.ActualWidth / tileSize), rnd.Next(0, (int)canvasPath.ActualHeight / tileSize));
            endPos = new NodePosition(rnd.Next(0, (int)canvasPath.ActualWidth / tileSize), rnd.Next(0, (int)canvasPath.ActualHeight / tileSize));
            pathToGo = new List<Node>();

            map = new Node[(int)canvasPath.ActualWidth / tileSize, (int)canvasPath.ActualHeight / tileSize];

            Stopwatch sw = new Stopwatch();
            sw.Start();
            FillMap();
            sw.Stop();

            long generationmillis = sw.ElapsedMilliseconds;

            sw.Reset();
            sw.Start();
            DrawMap();
            sw.Stop();

            labelTimeGridGen.Content = $"MapGeneration: {generationmillis}ms\nMapDrawing: {sw.ElapsedMilliseconds}ms";
        }

        private void DrawMap()
        {
            SolidColorBrush blockedBrush = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            SolidColorBrush startBrush = new SolidColorBrush(Colors.Lime);
            SolidColorBrush endBrush = new SolidColorBrush(Colors.Red);

            canvasPath.Children.Clear();

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                for (int x = 0; x < map.GetLength(0); x++)
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        SolidColorBrush selectedBrush = null;

                        if (map[x, y].IsBlocked)
                            selectedBrush = blockedBrush;
                        else if (x == endPos.X && y == endPos.Y)
                            selectedBrush = endBrush;
                        else if (x == startPos.X && y == startPos.Y)
                            selectedBrush = startBrush;
                        else continue;

                        if (checkboxDrawCosts.IsChecked == true)
                        {
                            if (map[x, y].GCost != 0)
                                DrawText((tileSize * x) + 4, (tileSize * y), "G: " + map[x, y].GCost, Colors.White, tileSize / 3);
                            if (map[x, y].HCost != 0)
                                DrawText((tileSize * x) + 4, (tileSize * y) + 30, "H: " + map[x, y].HCost, Colors.White, tileSize / 3);
                            if (map[x, y].FCost != 0)
                                DrawText((tileSize * x) + 4, (tileSize * y) + 14, "F: " + map[x, y].FCost, Colors.White, tileSize / 3);
                        }

                        dc.DrawRectangle(
                            selectedBrush,
                            null,
                            new Rect((tileSize * x), (tileSize * y), tileSize, tileSize));
                    }
                canvasPath.Children.Add(new VisualHost { Visual = dv });
            }
        }

        private void DrawText(double x, double y, string text, Color color, double fontSize)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                FontSize = (int)fontSize,
                Foreground = new SolidColorBrush(color)
            };
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvasPath.Children.Add(textBlock);
        }

        private void FillMap()
        {
            Random rnd = new Random();
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    bool blockIt = rnd.Next(0, 4) == 3 ? true : false;

                    if (x == startPos.X && y == startPos.Y) blockIt = false;
                    if (x == endPos.X && y == endPos.Y) blockIt = false;

                    map[x, y] = new Node(new NodePosition(x, y), blockIt);
                }
            }
        }

        private void ButtonFindPath_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            pathToGo = AmeisenPath.FindPathAStar(map, startPos, endPos, (bool)checkboxDiagonal.IsChecked, false, 0.0);
            sw.Stop();

            long pathfindingmillis = sw.ElapsedMilliseconds;

            sw.Reset();
            sw.Start();
            DrawPath();
            sw.Stop();

            labelTimePathGen.Content = $"PathFinding: {pathfindingmillis}ms\nMapDrawing: {sw.ElapsedMilliseconds}ms";
        }

        private void DrawPath()
        {
            SolidColorBrush orangeBrush = new SolidColorBrush(Colors.Orange);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                for (int x = 0; x < map.GetLength(0); x++)
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        SolidColorBrush selectedBrush = null;
                        
                        if (pathToGo != null)
                        {
                            foreach (Node n in pathToGo)
                            {
                                if (x == n.Position.X && y == n.Position.Y)
                                {
                                    selectedBrush = orangeBrush;
                                    break;
                                }
                            }
                        }

                        dc.DrawRectangle(
                            selectedBrush,
                            null,
                            new Rect((tileSize * x), (tileSize * y), tileSize, tileSize));
                    }
                canvasPath.Children.Add(new VisualHost { Visual = dv });
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                tileSize = (int)sliderTileSize.Value;
                labelTileSize.Content = $"TileSize: {tileSize}";
            }
            catch { }
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();

            if (o.ShowDialog() == true)
            {
            }
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            if (s.ShowDialog() == true)
            {
            }
        }
    }
}
