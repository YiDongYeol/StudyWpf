using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfBikeShop
{
    /// <summary>
    /// Menu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void btnProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new Uri("/ProductManagement.xaml", UriKind.RelativeOrAbsolute)
                );
        }

        private void btnLiveSupport_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new Uri("/TemplatePage.xaml", UriKind.RelativeOrAbsolute)
                );
        }

        private void btnEmailSupport_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(
                new Uri("/Contact.xaml", UriKind.RelativeOrAbsolute)
                );
        }
    }
}