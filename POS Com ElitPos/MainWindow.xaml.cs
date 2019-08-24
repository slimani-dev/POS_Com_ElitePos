using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POS_Com_ElitPos
{
    public partial class MainWindow : Window
    {
        // setting up the port information - no need to give the user any select options or anything
        private const int BaudRate = 2400;
        private const string PortName = "COM2";
        public System.IO.Ports.SerialPort port;

        public MainWindow()
        {
            InitializeComponent();
            port = new System.IO.Ports.SerialPort(PortName, BaudRate)
            {
                /**
                 * this is just to be able to send the hex value of 0x0C because it will clear the screen
                 * Note : 0x0C will just clear the screen it will not reset the cursor of the screen 
                 *        so the correction method is still needed 
                 */
                Encoding = System.Text.Encoding.GetEncoding("Windows-1252")
            };

            try
            {
                // opening the port right after starting
                port.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
                Close();
            }
        }
        /**
         * handling the correction mode by filling up the screen with 000000000
         * and clearing the textbox
         * when don clrear the screen
         */

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (port.IsOpen)
            {
                if (Coreection.IsChecked == true)
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

        private void Total_TextChanged(object sender, TextChangedEventArgs e)
        {
            // if correcion is checked sending only " " once at a time to be able to see where is the cursor
            if (Coreection.IsChecked == true)
            {
                port.WriteLine(" ");
            }
            else if (port.IsOpen)
            {
                // sending the data here
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

        /**
         * making sur that the text onley containes numbers and "."  or "-"
         * other characters not accepted only if the correction is checked then accept only space
         */

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            closePort();
            Close();
        }

        private void closePort()
        {
            if (port.IsOpen)
            {
                // clearing the screen befor closing
                port.Write("\x0C");
                port.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closePort();
        }
    }
}
