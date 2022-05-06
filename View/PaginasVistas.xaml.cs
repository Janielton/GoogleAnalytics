using GoogleAnalytics.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoogleAnalytics.View
{
    /// <summary>
    /// Interação lógica para PaginasVistas.xam
    /// </summary>
    public partial class PaginasVistas : Page
    {
        bool dataI;
        public PaginasVistas()
        {
            InitializeComponent();
            DataContext = new VMAnalytics(1);
        }

        private void DataInicial_GotFocus(object sender, RoutedEventArgs e)
        {
            selectData.Visibility = Visibility.Visible;
            dataI = true;
        }

        private void DataFinal_GotFocus(object sender, RoutedEventArgs e)
        {
            selectData.Visibility = Visibility.Visible;
            dataI = false;
        }

        private void selectData_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            char[] charSeparators = new char[] { ' ' };
            string[] data = sender.ToString().Split(charSeparators);
            Console.WriteLine(data[0]);
            if (dataI)
            {
                editDataInicial.Text = data[0];
            }
            else
            {
                editDataFinal.Text = data[0];
            }
            selectData.Visibility = Visibility.Collapsed;
        }
    }
}
