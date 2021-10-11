using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Fractals;

namespace Lab_Fractals
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void DeleteRule(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (rulesList.Children.Count > 0)
                rulesList.Children.RemoveAt(rulesList.Children.Count - 1);
        }

        private void AddRule(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var ch = new TextBox { MinWidth = 20, MaxWidth = 50, FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, MaxLength = 1 };
            var lab = new Label { Content = "to ->" };
            var str = new TextBox { MinWidth = 30, MaxWidth = 120, FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            DockPanel dock = new();
            dock.Children.Add(ch);
            dock.Children.Add(lab);
            dock.Children.Add(str);
            rulesList.Children.Add(dock);
        }
        private void DrawFractal(object sender, RoutedEventArgs e)
        {
            try
            {
                canvas.Children.Clear();
                var ls = new LSystemFractal(canvas);
                var rules = new Dictionary<char, string>();
                foreach (var rule in rulesList.Children)
                {
                    DockPanel line = (DockPanel)rule;
                    TextBox ch = (TextBox)line.Children[0];
                    TextBox r = (TextBox)line.Children[2];
                    rules.Add(ch.Text[0], r.Text);
                }

                int? speed = null;
                if (animated.FindName("speed") != null)
                    speed = int.Parse((this.FindName("speed") as TextBox).Text);

                ls.MainCode(axiom.Text, uint.Parse(iterations.Text), double.Parse(angle.Text), double.Parse(startAngle.Text), double.Parse(distantion.Text), rules, isColored.IsChecked ?? false, speed);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void TextBox_PreviewTextInputInt(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            foreach (var c in e.Text)
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            e.Handled = false;
        }
        private void TextBox_PreviewTextInputDouble(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var text = ((TextBox)sender).Text + e.Text;
            bool havePoint = false;
            foreach (var c in text)
                if (!char.IsDigit(c))
                {
                    if (c == ',')
                        if (!havePoint)
                        {
                            havePoint = true;
                            continue;
                        }
                    e.Handled = true;
                    return;
                }
            e.Handled = false;
        }
        Point lastPoint;
        bool mousePosIsChecked = false;

        private void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mousePosIsChecked && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                RedrawCanvasPoints((Point)(e.GetPosition(canvas) - lastPoint));
                lastPoint = e.GetPosition(canvas);
            }
        }
        private void canvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            lastPoint = e.GetPosition(canvas);
            mousePosIsChecked = true;
        }
        private void canvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => mousePosIsChecked = false;
        private void canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lastPoint = e.GetPosition(canvas);
            mousePosIsChecked = true;
        }
        private void RedrawCanvasPoints(Point offset)
        {
            foreach (object obj in canvas.Children)
            {
                if (obj is Polyline pol)
                {
                    for (int i = 0; i < pol.Points.Count; i++)
                    {
                        pol.Points[i] = new Point(offset.X + pol.Points[i].X, offset.Y + pol.Points[i].Y);
                    }
                }
                else if (obj is Line l)
                {
                    l.X1 += offset.X;
                    l.X2 += offset.X;
                    l.Y1 += offset.Y;
                    l.Y2 += offset.Y;
                }
            }
        }
        private void isAnimated_Click(object sender, RoutedEventArgs e)
        {
            if (isAnimated.IsChecked ?? false)
            {
                var dock = new DockPanel { HorizontalAlignment = HorizontalAlignment.Left };
                var label = new Label { Content = "Animation speed" };
                var tb = new TextBox { Name = "speed" };
                tb.PreviewTextInput += TextBox_PreviewTextInputInt;
                dock.Children.Add(label);
                dock.Children.Add(tb);
                animated.Children.Add(dock);
                RegisterName(tb.Name, tb);
            }
            else
            {
                if (animated.Children.Count == 2)
                {
                    UnregisterName("speed");
                    animated.Children.RemoveAt(1);
                }
            }
        }
    }
}
