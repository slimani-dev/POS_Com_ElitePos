using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS_Com_ElitPos
{
    public partial class MainWindow : Window
    {
        private const int BaudRate = 2400;
        private const string PortName = "COM2";
        public System.IO.Ports.SerialPort port;

        public MainWindow()
        {
            InitializeComponent();
            port = new System.IO.Ports.SerialPort(PortName, BaudRate)
            {
                Encoding = System.Text.Encoding.GetEncoding("Windows-1252")
            };

            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
                Close();
            }
        }

        private void Total_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Coreection.IsChecked == true)
            {
                port.WriteLine(" ");
            }
            else if (port.IsOpen)
            {
                string total = Total.Text.Trim().ToString();

                if (total.Contains(".")) total = total.PadLeft(9, ' ');
                else total = total.PadLeft(8, ' ');
                port.WriteLine(total);
            }
            else
            {
                MessageBox.Show("Error : Port not Open", "Error");
            }
        }

        private void NumOnly(object sender, TextCompositionEventArgs e)
        {
            if (Coreection.IsChecked == true)
            {
                e.Handled = !e.Text.Equals(" ");
            }
            else
            {
                Regex regex = new Regex("[^0-9.-]+");
                e.Handled = regex.IsMatch(e.Text);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (port.IsOpen)
            {
                if(Coreection.IsChecked == true)
                {
                    port.WriteLine("00000000");
                    Total.Text = "";
                }
                else
                {
                    port.Write("\x0C");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (port.IsOpen)
            {
                port.Write("\x0C");
                port.Close();
            }
            Close();
        }
    }
}
