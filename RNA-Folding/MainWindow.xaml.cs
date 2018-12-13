using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;

namespace RNA_Folding
{
    //MainWindow which will display everything
    public partial class MainWindow : Window
    {
        private Main m;
        private String rnaString, searchString;
        private char newBaseType;
        //Size of the display, cannot be changed (i.e FINAL)
        private readonly int HEIGHT, WIDTH;
        //Radius of the circle, will be set to depend on RNA Length
        private int RADIUS = 20;

        public MainWindow()
        {
            InitializeComponent();
            HEIGHT = (int)Stack_Panel.Height;
            WIDTH = (int)Stack_Panel.Width;
            rnaString = "";
        }

        //When the Upload button is clicked
        private void Upload_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogWindow dw = new DialogWindow(this);
            dw.ShowDialog();
            dw.Close();
            //Send RNAString to Main
            //The larger the string, the smaller the radius
            RADIUS = 20;
            if (rnaString != "")
            {
                m = new Main(rnaString, this, RADIUS);
            }
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            //Will bring up another window just like Upload_Button
            searchString = "";
            SearchWindow sw = new SearchWindow(this);
            sw.ShowDialog();
            sw.Close();
            //Error handling
            if (m != null && searchString.Length <= rnaString.Length && searchString != "")
            {
                //Send the string to main to search for it and highlight them
                m.SearchForString(searchString);
            }
        }

        //Method to set the RNAString from the DialogWindow
        public void SetRNAString(String s)
        {
            rnaString = s;
        }

        public void ChangeBaseType(Char c)
        {
            newBaseType = c;
        }

        //This is how to get a Base on the Stack Panel
        private void Stack_Panel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Get the information of where the click happened
            System.Windows.Point p = e.GetPosition(this.Stack_Panel);
            int x = (int) p.X;
            int y = (int) p.Y;
            //MessageBox.Show("X: "+x+" Y:"+y);
            Base b;
            newBaseType = '\0';
            if (m != null)
            {
                b = m.SearchForBase(x - (RADIUS / 2), y - (RADIUS / 2), x, y);
                //MessageBox.Show("ID: " + b.GetID() + " Type: " + b.GetBaseType());
                if (b != null)
                {
                    BaseWindow bw = new BaseWindow(this, b);
                    bw.ShowDialog();
                    bw.Close();
                    if (!newBaseType.Equals('\0'))
                    {
                        StringBuilder str = new StringBuilder(rnaString);
                        str[b.GetID() - 1] = newBaseType;
                        rnaString = str.ToString();
                        m.ChangeBaseType(rnaString);

                    }
                }
            }
        }

        public void SetSearchString(String s)
        {
            searchString = s;
        }

        private void Export_String_Button_Click(object sender, RoutedEventArgs e)
        {
            if (rnaString != "")
            {
                try
                {
                    SaveFileDialog saveFile = new SaveFileDialog
                    {
                        Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
                    };
                    saveFile.ShowDialog();
                    if (saveFile.FileName != "")
                    {
                        StreamWriter writer = new StreamWriter(saveFile.OpenFile());
                        writer.WriteLine(rnaString);
                        writer.Dispose();
                        writer.Close();
                    }
                }
                catch (Exception error)
                {
                    //Nothing in the simulation
                }
            }
        }

        //Method to draw the bases, won't need change other than adding more graphics (i.e printing AUGC on the base)
        public void DrawBases(Base[] bases)
        {
            //Clear everthing on the display before doing anything
            Stack_Panel.Children.Clear();
            //Draw the lines to each base pair and next
            DrawLines(bases);
            foreach (Base b in bases)
            {
                Ellipse e = new Ellipse();
                Canvas root = new Canvas();
                System.Windows.Media.SolidColorBrush mySolidColorBrush = new System.Windows.Media.SolidColorBrush();
                //e.HorizontalAlignment = HorizontalAlignment.Left;
                //e.VerticalAlignment = VerticalAlignment.Center;
                //Will be a function of the number of elements in b
                e.Width = RADIUS;
                e.Height = RADIUS;
                switch (b.GetBaseType().ToString())
                {
                    //Red
                    case "A":
                        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                        break;
                    //Orange
                    case "U":
                        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 128, 0);
                        break;
                    //Green
                    case "G":
                        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 172, 0);
                        break;
                    //Blue
                    case "C":
                        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 174, 255);
                        break;
                    //Black
                    default:
                        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        break;
                }
                e.Fill = mySolidColorBrush;
                //e.StrokeThickness = 3;
                double thickness = 0;
                if (b.GetIsHighlighted())
                {
                    thickness = 2;
                }
                e.StrokeThickness = thickness;
                e.Stroke = System.Windows.Media.Brushes.Black;
                Canvas.SetLeft(e, b.GetX() - (RADIUS / 2));
                Canvas.SetTop(e, b.GetY() - (RADIUS / 2));
                root.Children.Add(e);

                //We will need to set the font size of the textblock based on RADIUS
                //This isn't perfect right now, but it displays it find right now
                TextBlock textBlock = new TextBlock();
                textBlock.Text = b.GetBaseType().ToString();
                textBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                Canvas.SetLeft(textBlock, b.GetX() + 5.7 - (e.Height / 2));
                Canvas.SetTop(textBlock, b.GetY() + 2.2 - (e.Width / 2));
                root.Children.Add(textBlock);

                //Everything is added to the canvas, which is added to the Stack Panel
                Stack_Panel.Children.Add(root);
            }
        }

        private void DrawLines(Base[] bases)
        {
            Pen dashPen = new Pen(Color.Black);
            dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            Pen linePen = new Pen(Color.Black);
            linePen.Width = 8.0F;
            Dictionary<Base, bool> dict = new Dictionary<Base, bool>();
            foreach (Base b in bases)
            {
                Canvas root = new Canvas();
                //Draw a line to the next base
                if (b.GetNext() != null)
                {
                    //Draw a line from b to b.Next
                    Line line = new Line();
                    line.Stroke = System.Windows.Media.Brushes.Black;
                    line.X1 = b.GetX();
                    line.Y1 = b.GetY();
                    line.X2 = b.GetNext().GetX();
                    line.Y2 = b.GetNext().GetY();
                    line.HorizontalAlignment = HorizontalAlignment.Left;
                    line.VerticalAlignment = VerticalAlignment.Center;
                    line.StrokeThickness = 2;
                    root.Children.Add(line);
                }

                //Draw a line to its pair
                if (b.GetBasePair() != null)
                {
                    if (!dict.ContainsKey(b))
                    {
                        dict.Add(b, true);
                        dict.Add(b.GetBasePair(), true);
                        Line line = new Line();
                        line.Stroke = System.Windows.Media.Brushes.Black;
                        line.X1 = b.GetX();
                        line.Y1 = b.GetY();
                        line.X2 = b.GetBasePair().GetX();
                        line.Y2 = b.GetBasePair().GetY();
                        line.HorizontalAlignment = HorizontalAlignment.Left;
                        line.VerticalAlignment = VerticalAlignment.Center;
                        line.StrokeThickness = 2;
                        System.Windows.Media.DoubleCollection dc = new System.Windows.Media.DoubleCollection
                        {
                            2, 2
                        };
                        line.StrokeDashArray = dc;
                        root.Children.Add(line);
                    }
                }
                Stack_Panel.Children.Add(root);
            }
        }
    }
}
