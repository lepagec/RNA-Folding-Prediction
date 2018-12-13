using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace RNA_Folding
{
    //This class is the used to get the Input String
    public partial class DialogWindow : Window
    {
        //Use this to interact with the MainWindow
        private MainWindow mainWindow;
        //The string must be between these 2 values, inclusive
        private int minLength = 10, maxLength = 100;
        public DialogWindow(MainWindow mw)
        {
            InitializeComponent();
            mainWindow = mw;
        }

        //What happens when the Run button is clicked
        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            //Check content if it only has A,U,G,C in it
            String s = RNA_Box.Text.ToUpper();
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
                mainWindow.SetRNAString(s);
                this.Close();
            }
            else if (s.Length > maxLength)
                MessageBox.Show("Error, length cannot be greater than "+maxLength, "Length Error");
            else if (s.Length < minLength)
                MessageBox.Show("Error, length cannot be less than "+minLength, "Length Error");
            else
                MessageBox.Show("Error, text must contain only A, U, G, and C", "Sequence Error");
        }

        private void Upload_Button_Click(object sender, RoutedEventArgs e)
        {
            //Logic to open a window to search for a file
            String path = "";
            //Logic to open a window to search for a file
            OpenFileDialog file = new OpenFileDialog();
            //Filters to find .txt files
            file.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (file.ShowDialog() == true)
            {
                path = file.FileName;
            }

            //Read the file and take everything
            if (path != "")
            {
                StreamReader reader = new StreamReader(path);
                String line = reader.ReadLine();
                RNA_Box.Text = line;
            }
        }

        //Shows the number of characters currently in the textbox
        private void RNA_Box_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Character_Count.Text = "Count: " + RNA_Box.Text.Length;
        }
    }
}
