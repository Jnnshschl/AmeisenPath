using AmeisenPathCore;
using AmeisenPathCore.Objects;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AmeisenPathGUI
{
    public partial class MainWindow : Window
    {
        private NodePosition startPos;
        private NodePosition endPos;
        private List<Node> pathToGo = new List<Node>();
        private Node[,] map;

        // Tile size, maybe a bit hacky but hey it works :D
        private int tileSize = 5;

        private Brush blockedBrush = new SolidBrush(Color.FromArgb(255, 77, 77, 77));
        private Brush startBrush = new SolidBrush(Color.Lime);
        private Brush endBrush = new SolidBrush(Color.Red);
        private Brush orangeBrush = new SolidBrush(Color.Orange);
        private Brush blackBrush = new SolidBrush(Color.FromArgb(255, 50, 50, 50));
        private Brush selectedBrush = null;

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
            pathImage.Visibility = Visibility.Hidden;
            mapImage.Visibility = Visibility.Hidden;

            Random rnd = new Random();
            //startPos = new NodePosition(rnd.Next(0, (int)mapCanvas.ActualWidth / tileSize), rnd.Next(0, (int)mapCanvas.ActualHeight / tileSize));
            //endPos = new NodePosition(rnd.Next(0, (int)mapCanvas.ActualWidth / tileSize), rnd.Next(0, (int)mapCanvas.ActualHeight / tileSize));

            startPos = new NodePosition(0, 0);
            endPos = new NodePosition((int)mapCanvas.ActualWidth / tileSize - 1, (int)mapCanvas.ActualHeight / tileSize - 1);

            pathToGo = new List<Node>();

            map = new Node[(int)mapCanvas.ActualWidth / tileSize, (int)mapCanvas.ActualHeight / tileSize];

            Stopwatch sw = new Stopwatch();
            sw.Start();
            FillMap();
            sw.Stop();

            long generationmillis = sw.ElapsedMilliseconds;

            sw.Reset();
            sw.Start();
            RenderMap();
            sw.Stop();

            labelTimeGridGen.Content = $"MapGeneration: {generationmillis}ms\nMapDrawing: {sw.ElapsedMilliseconds}ms";
            labelMapInfo.Content = $"Map Width: {(int)mapCanvas.ActualWidth / tileSize}\nMap Height: {(int)mapCanvas.ActualHeight / tileSize}\nDistance: {(int)Math.Sqrt(Math.Pow(startPos.X - endPos.X, 2) + Math.Pow(startPos.Y - endPos.Y, 2))}";
        }

        private void FillMap()
        {
            Random rnd = new Random();
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    bool blockIt = rnd.Next(0, 4) == 3 ? true : false;

                    if (x == startPos.X && y == startPos.Y)
                    {
                        blockIt = false;
                    }

                    if (x == endPos.X && y == endPos.Y)
                    {
                        blockIt = false;
                    }

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
            RenderPath();
            sw.Stop();

            labelTimePathGen.Content = $"PathFinding: {pathfindingmillis}ms\nMapDrawing: {sw.ElapsedMilliseconds}ms";
        }

        private void RenderMap()
        {
            Bitmap bitmap = new Bitmap(map.GetLength(0) * tileSize, map.GetLength(1) * tileSize);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                RenderMap(graphics);
            }

            BitmapImage bitmapImageMap = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImageMap.BeginInit();
                bitmapImageMap.StreamSource = memory;
                bitmapImageMap.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImageMap.EndInit();
            }

            mapImage.Source = bitmapImageMap;
            mapImage.Visibility = Visibility.Visible;
        }

        private void RenderPath()
        {
            Bitmap bitmap = new Bitmap(map.GetLength(0) * tileSize, map.GetLength(1) * tileSize);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                RenderPath(graphics);
            }

            BitmapImage bitmapImagePath = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImagePath.BeginInit();
                bitmapImagePath.StreamSource = memory;
                bitmapImagePath.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImagePath.EndInit();
            }

            pathImage.Source = bitmapImagePath;
            pathImage.Visibility = Visibility.Visible;
        }

        private void RenderMap(Graphics graphics)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y].IsBlocked)
                    {
                        selectedBrush = blockedBrush;
                    }
                    else if (x == endPos.X && y == endPos.Y)
                    {
                        selectedBrush = endBrush;
                    }
                    else if (x == startPos.X && y == startPos.Y)
                    {
                        selectedBrush = startBrush;
                    }
                    else
                    {
                        continue;
                    }

                    graphics.FillRectangle(
                        selectedBrush,
                        new Rectangle(tileSize * x, tileSize * y, tileSize, tileSize));
                }
            }
        }

        private void RenderPath(Graphics graphics)
        {
            if (pathToGo != null)
            {
                int stepcount = 0;
                foreach (Node n in pathToGo)
                {
                    int x = n.Position.X;
                    int y = n.Position.Y;

                    if (endPos.X == n.Position.X && endPos.Y == n.Position.Y)
                    {
                        selectedBrush = endBrush;
                    }
                    else
                    {
                        selectedBrush = orangeBrush;
                    }

                    if (x == n.Position.X && y == n.Position.Y)
                    {
                        x *= tileSize;
                        y *= tileSize;

                        graphics.FillRectangle(
                            selectedBrush,
                            new Rectangle(x, y, tileSize, tileSize));
                        // Draw stepcount only if its visible
                        if (tileSize > 20 && checkboxDrawCosts.IsChecked == false)
                        {
                            graphics.DrawString(
                            $"{stepcount}",
                            new Font(System.Drawing.FontFamily.GenericSansSerif, tileSize / 3, System.Drawing.FontStyle.Regular),
                            blackBrush,
                            x,
                            y);
                        }

                        // Draw costs only if its visible
                        if (tileSize > 20 && checkboxDrawCosts.IsChecked == true)
                        {
                            graphics.DrawString(
                            $"F: {n.FCost}",
                            new Font(System.Drawing.FontFamily.GenericSansSerif, tileSize / 4, System.Drawing.FontStyle.Regular),
                            blackBrush,
                            x,
                            y + ((tileSize / 4) * 0));

                            graphics.DrawString(
                            $"G: {n.GCost}",
                            new Font(System.Drawing.FontFamily.GenericSansSerif, tileSize / 4, System.Drawing.FontStyle.Regular),
                            blackBrush,
                            x,
                            y + ((tileSize / 4) * 1) + 2);

                            graphics.DrawString(
                            $"H: {n.HCost}",
                            new Font(System.Drawing.FontFamily.GenericSansSerif, tileSize / 4, System.Drawing.FontStyle.Regular),
                            blackBrush,
                            x,
                            y + ((tileSize / 4) * 2) + 4);
                        }
                    }
                    stepcount++;
                }
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
