using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using AmeisenPathLib;
using AmeisenPathLib.objects;

namespace AmeisenPathGUI
{
    public partial class MainWindow : Window
    {
        NodePosition startPos;
        NodePosition endPos;

        List<Node> pathToGo = new List<Node>();

        Node[,] map;

        // Tile size, maybe a bit hacky but hey it works :D
        private const int FACTOR = 50;

        public MainWindow()
        {
            InitializeComponent();

            ResetMap();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetMap();
        }

        private void ResetMap()
        {
            Random rnd = new Random();
            startPos = new NodePosition(rnd.Next(0, (int)canvasPath.ActualWidth / FACTOR), rnd.Next(0, (int)canvasPath.ActualHeight / FACTOR));
            endPos = new NodePosition(rnd.Next(0, (int)canvasPath.ActualWidth / FACTOR), rnd.Next(0, (int)canvasPath.ActualHeight / FACTOR));
            pathToGo = new List<Node>();

            map = new Node[(int)canvasPath.ActualWidth / FACTOR, (int)canvasPath.ActualHeight / FACTOR];
            FillMap();
            DrawMap();
        }

        private void DrawMap()
        {
            canvasPath.Children.Clear();
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Stroke = new SolidColorBrush(Colors.Black),
                        Width = FACTOR,
                        Height = FACTOR,
                        StrokeThickness = 1
                    };

                    if (pathToGo != null)
                        foreach (Node n in pathToGo)
                            if (x == n.Position.X && y == n.Position.Y)
                                rect.Fill = new SolidColorBrush(Colors.Orange);

                    if (map[x, y].IsBlocked)
                        rect.Fill = new SolidColorBrush(Colors.Black);

                    if (x == endPos.X && y == endPos.Y)
                        rect.Fill = new SolidColorBrush(Colors.Red);

                    if (x == startPos.X && y == startPos.Y)
                        rect.Fill = new SolidColorBrush(Colors.Lime);


                    Canvas.SetLeft(rect, (FACTOR * x));
                    Canvas.SetTop(rect, (FACTOR * y));

                    // Debug Output
                    if (map[x, y].GCost != 0)
                        DrawText((FACTOR * x) + 4, (FACTOR * y), "G: " + map[x, y].GCost, Colors.White);
                    if (map[x, y].HCost != 0)
                        DrawText((FACTOR * x) + 4, (FACTOR * y) + 30, "H: " + map[x, y].HCost, Colors.White);
                    if (map[x, y].FCost != 0)
                        DrawText((FACTOR * x) + 16, (FACTOR * y) + 14, "F: " + map[x, y].FCost, Colors.White);

                    canvasPath.Children.Add(rect);
                }
        }

        private void DrawText(double x, double y, string text, Color color)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
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
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    bool blockIt = rnd.Next(0, 4) == 3 ? true : false;

                    if (x == startPos.X && x == endPos.X) blockIt = false;
                    if (y == startPos.Y && y == endPos.Y) blockIt = false;

                    map[x, y] = new Node(new NodePosition(x, y), blockIt);
                }
        }

        private void ButtonFindPath_Click(object sender, RoutedEventArgs e)
        {
            pathToGo = AmeisenPath.FindPathAStar(map, startPos, endPos);

            DrawMap();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
