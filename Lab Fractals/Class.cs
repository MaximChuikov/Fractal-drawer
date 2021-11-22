using Lab_Fractals;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Fractals
{
    class Class
    {
        const double PXL = 0.1;
        const double EPS = 0.1;
        const double PI5 = 0.628318530717959;

        public void NewtonPool(int width, int height, Action<SystemBrush, int, int> draw)
        {
            int W = width;
            int H = height;
            int X0 = W / 2;
            int Y0 = H / 2;

            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                {
                    double x = (i - X0) * PXL;
                    double y = (j - Y0) * PXL;
                    var z = new Complex(x, y);
                    if (x != y)
                    {
                        Complex t;
                        do
                        {
                            t = z;
                            z = 0.8 * z + 0.2 * Complex.Pow(z, -4);
                        }
                        while (Complex.Abs(z - t) >= EPS);
                        SystemBrush col = SystemBrush.Orange;
                        switch ((int)(z.Phase / PI5))
                        {
                            case 0:
                                col = (SystemBrush)3;
                                break;
                            case 1:
                            case 2:
                                col = (SystemBrush)8;
                                break;
                            case 3:
                            case 4:
                                col = (SystemBrush)13;
                                break;
                            case -3:
                            case -4:
                                col = (SystemBrush)18;
                                break;
                            case -1:
                            case -2:
                                col = (SystemBrush)23;
                                break;
                        }
                        draw(col, i, j);
                    }
                }
        }
    }
    class LSystemFractal
    {
        readonly Canvas canvas;
        readonly Random rand = new();
        public LSystemFractal(Canvas canvas) => this.canvas = canvas;
        public void MainCode(string axiom, uint iterations, double angle, double startAngle, double dist, 
                             Dictionary<char, string> rules, bool isColored, int? speed)
        {
            Point start = new(canvas.ActualWidth * 0.5, canvas.ActualHeight * 0.5);
            DownFeather(3, start, Brushes.Black, out Polyline graphic);

            var polylines = new List<Polyline> { graphic };

            SystemBrush color = SystemBrush.Gold;
            Action chColor = () => GetBrush.AddBrush(ref color, 1);

            Point last = start;
            Action<Point> move;

            Action startDraw = () =>
            {
                if (polylines[polylines.Count - 1] == null)
                {
                    polylines.Add(new Polyline());
                    graphic = polylines[polylines.Count - 1];
                    DownFeather(3, last, Brushes.Black, out graphic);
                }
            };

            Ellipse pointer = new Ellipse { Width = 6, Height = 6, Fill = Brushes.Brown };
            canvas.Children.Add(pointer);
            Action<Point> movePointer = (Point to) =>
            {
                var aniX = new DoubleAnimation { From = last.X - pointer.ActualWidth / 2, To = to.X - pointer.ActualWidth / 2, Duration = TimeSpan.FromMilliseconds(speed ?? 0) };
                var aniY = new DoubleAnimation { From = last.Y - pointer.ActualHeight / 2, To = to.Y - pointer.ActualHeight / 2, Duration = TimeSpan.FromMilliseconds(speed ?? 0) };
                pointer.BeginAnimation(Canvas.LeftProperty, aniX);
                pointer.BeginAnimation(Canvas.TopProperty, aniY);
            };

            if (isColored)
                move = delegate (Point to)
                {
                    startDraw();
                    DrawLine(last, to, 3, GetBrush.Get(color));
                    last = to;
                };
            else
            {
                move = (Point to) =>
                {
                    startDraw();
                    MoveTo(graphic, to);
                    last = to;
                };
            }

            Action<Point> fly = delegate (Point to)
            {
                if (polylines[polylines.Count - 1] != null)
                    polylines.Add(null);
                last = to;
            };

            var stack = new Stack<Tuple<Point, double>>();
            Action<Point, double> save = (Point pos, double ang) => stack.Push(Tuple.Create(pos, ang));
            Func<Tuple<Point, double>> load = () => stack.Pop();

            Action removePointer = () => canvas.Children.Remove(pointer);

            LSystem(start,
                    iterations,
                    rules,
                    axiom,
                    new Tuple<double, double>(dist, angle),
                    startAngle,
                    move,
                    fly,
                    save,
                    load,
                    movePointer,
                    removePointer,
                    chColor,
                    speed ?? 0);
        }
        public async void LSystem(Point currPosition, uint deep, Dictionary<char, string> rules, string axioma, 
                            Tuple<double, double> incr_distance_angle, double currAngle, Action<Point> move,
                            Action<Point> fly, Action<Point, double> save, Func<Tuple<Point, double>> load,
                            Action<Point> movePointer, Action removePointer, Action changeColor = null, int timeDelay = 0)
        {
            axioma = TransformAxioma(0, deep, axioma);

            foreach (var c in axioma)
            {
                switch (c)
                {
                    case 'F':
                        currPosition = GetPoint(currPosition, currAngle, incr_distance_angle.Item1);
                        movePointer(currPosition);
                        await Task.Delay(timeDelay);
                        move(currPosition);
                        break;
                    case 'f':
                        currPosition = GetPoint(currPosition, currAngle, incr_distance_angle.Item1);
                        movePointer(currPosition);
                        await Task.Delay(timeDelay);
                        fly(currPosition);
                        break;
                    case '+':
                        currAngle -= incr_distance_angle.Item2;
                        currAngle %= 360;
                        changeColor?.Invoke();
                        break;
                    case '-':
                        currAngle += incr_distance_angle.Item2;
                        currAngle %= 360;
                        changeColor?.Invoke();
                        break;
                    case '[':
                        save(currPosition, currAngle);
                        break;
                    case ']':
                        var l = load();
                        currPosition = l.Item1;
                        currAngle = l.Item2;
                        fly(currPosition);
                        break;
                }
            }
            removePointer();

            //Вспомогательный метод
            string TransformAxioma(int me, uint deep, string axioma)
            {
                var str = new StringBuilder();
                foreach (var c in axioma)
                    if (rules.ContainsKey(c))
                        str.Append(rules[c]);
                    else
                        str.Append(c);
                if (me < deep)
                    return TransformAxioma(me + 1, deep, str.ToString());
                else
                    return str.ToString();
            }
        }
        private Point GetPoint(Point from, double ang, double radius)
        {
            return new Point(from.X + Math.Round(Math.Sin(Math.PI * (90 - ang) / 180d), 4) * radius, from.Y + Math.Round(Math.Sin(Math.PI * ang / 180d), 4) * radius);
        }
        void DownFeather(double stroke, Point from, SolidColorBrush br, out Polyline graphic)
        {
            //var blur = new BlurEffect { Radius = 10 };
            graphic = new Polyline
            {
                Stroke = br,
                StrokeThickness = stroke,
                //Effect = blur
            };
            canvas.Children.Add(graphic);
            graphic.Points.Add(from);
        }
        void DrawLine(Point from, Point to, double stroke, SolidColorBrush br)
        {
            canvas.Children.Add(new Line
            {
                StrokeThickness = stroke,
                X1 = from.X,
                Y1 = from.Y,
                X2 = to.X,
                Y2 = to.Y,
                Stroke = br,
                //Effect = new BlurEffect { Radius = 2 } 
            });
        }
        Point GetLastPoint(Polyline graphic) => graphic.Points[graphic.Points.Count - 1];
        void MoveTo(Polyline graphic, Point to)
        {
            graphic.Points.Add(to);
        }
        void DrawFigure(Point center, double rotate, double incAng, double radius, SolidColorBrush b)
        {
            //var blur = new BlurEffect();
            //blur.Radius = 20;
            var graphic = new Polyline
            {
                Fill = b,
                //Opacity = 0.5,
                //Effect = blur
            };
            canvas.Children.Add(graphic);
            double currAngle = 0;
            while (currAngle < 360)
            {
                graphic.Points.Add(new Point(center.X + Math.Round(Math.Sin(Math.PI * (90 - currAngle - rotate) / 180d), 4) * radius, center.Y + Math.Round(Math.Sin(Math.PI * (currAngle + rotate) / 180d), 4) * radius));
                currAngle += incAng;
            }
        }
        void DrawCycle(Point center, double rotate, double incAng, double radius, double thickness, SolidColorBrush b)
        {
            var blur = new BlurEffect();
            blur.Radius = 6 + 8 * rand.NextDouble();
            var graphic = new Polyline
            {
                StrokeThickness = thickness,
                Stroke = b,
                Effect = blur
            };
            canvas.Children.Add(graphic);
            double currAngle = 0;
            while (currAngle <= 360)
            {
                graphic.Points.Add(new Point(center.X + Math.Round(Math.Sin(Math.PI * (90 - currAngle - rotate) / 180d), 4) * radius, center.Y + Math.Round(Math.Sin(Math.PI * (currAngle + rotate) / 180d), 4) * radius));
                currAngle += incAng;
            }
        }
        void DrawLinesFromOrigin(Point origin, double rotate, double incAng, double radius, double thickness, SolidColorBrush b)
        {
            double currAngle = 0;
            while (currAngle <= 360)
            {
                //var blur = new BlurEffect();
                //blur.Radius = 2 + 4 * rand.NextDouble();
                var graphic = new Polyline { StrokeThickness = thickness, Stroke = b };
                canvas.Children.Add(graphic);
                graphic.Points.Add(origin);
                graphic.Points.Add(new Point(origin.X + Math.Round(Math.Sin(Math.PI * (90 - currAngle - rotate) / 180d), 4) * radius, origin.Y + Math.Round(Math.Sin(Math.PI * (currAngle + rotate) / 180d), 4) * radius));
                currAngle += incAng;
            }
        }
    }
}

