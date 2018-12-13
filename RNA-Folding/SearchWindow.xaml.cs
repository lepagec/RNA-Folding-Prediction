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
using System.Windows.Shapes;

namespace RNA_Folding
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        //Use this to interact with the MainWindow
        private MainWindow mainWindow;
        //The string must be between these 2 values, inclusive
        private int minLength = 3, maxLength = 500;

        public SearchWindow(MainWindow mw)
        {
            mainWindow = mw;
            InitializeComponent();
        }

        //What happens when the Run button is clicked
        private void Search_For_Button_Click(object sender, RoutedEventArgs e)
        {
            //Check content if it only has A,U,G,C in it
            String s = Search_Box.Text.ToUpper();
            Boolean b = true;
            foreach (char c in s)
            {
                if (!c.Equals('A') && !c.Equals('U') && !c.Equals('G') && !c.Equals('C'))
                    b = false;
            }
            //
            if (b && s.Length >= minLength && s.Length <= maxLength)
            {
                //Send s to mainWindow
                mainWindow.SetSearchString(s);
                this.Close();
            }
            else if (s.Length > maxLength)
                MessageBox.Show("Error, length cannot be greater than " + maxLength, "Length Error");
            else if (s.Length < minLength)
                MessageBox.Show("Error, length cannot be less than " + minLength, "Length Error");
            else
                MessageBox.Show("Error, text must contain only A, U, G, and C", "Sequence Error");
        }

        //Shows the number of characters currently in the textbox
        private void Search_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            Character_Count.Content = "Count: " + Search_Box.Text.Length;
        }
    }
}
