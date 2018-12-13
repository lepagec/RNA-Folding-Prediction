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
    /// Interaction logic for BaseWindow.xaml
    /// </summary>
    public partial class BaseWindow : Window
    {
        private MainWindow mw;
        Base myBase;
        private Char myType;
        public BaseWindow(MainWindow m, Base b)
        {
            InitializeComponent();
            mw = m;
            myBase = b;
            myType = myBase.GetBaseType();
            this.Base_ID_Label.Content = "Base ID: " + myBase.GetID();
            switch (myBase.GetBaseType())
            {
                case 'A':
                    {
                        A_Radio.IsChecked = true;
                        Base_Type_Label.Content = "Base Type: " + myBase.GetBaseType();
                        break;
                    }
                case 'U':
                    {
                        U_Radio.IsChecked = true;
                        Base_Type_Label.Content = "Base Type: " + myBase.GetBaseType();
                        break;
                    }
                case 'G':
                    {
                        G_Radio.IsChecked = true;
                        Base_Type_Label.Content = "Base Type: " + myBase.GetBaseType();
                        break;
                    }
                case 'C':
                    {
                        C_Radio.IsChecked = true;
                        Base_Type_Label.Content = "Base Type: " + myBase.GetBaseType();
                        break;
                    }
                default:
                {
                    break;
                }
            }
        }

        private void Change_Base_Button_Click(object sender, RoutedEventArgs e)
        {
            if (A_Radio.IsChecked == true)
            {
                if (myType != 'A')
                {
                    mw.ChangeBaseType('A');
                    this.Close();
                }
            }
            else if (U_Radio.IsChecked == true)
            {
                if (myType != 'U')
                {
                    mw.ChangeBaseType('U');
                    this.Close();
                }
            }
            else if (G_Radio.IsChecked == true)
            {
                if (myType != 'G')
                {
                    mw.ChangeBaseType('G');
                    this.Close();
                }
            }
            else if (C_Radio.IsChecked == true)
            {
                if (myType != 'C')
                {
                    mw.ChangeBaseType('C');
                    this.Close();
                }
            }
        }
    }
}
