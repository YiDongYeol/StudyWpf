using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfBikeShop
{
    /// <summary>
    /// Bindings.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Bindings : Page
    {
        public Bindings()
        {
            InitializeComponent();

            Car c = new Car
            {
                Speed = 100,
                color = Colors.Crimson,
                Driver = new Human
                {
                    FirstName = "Nick",
                    HasDrivingLicense = true
                }
            };

            this.DataContext = c;

            var carlist = new List<Car>();
            for (int i = 0; i < 10; i++)
            {
                carlist.Add(new Car
                {
                    Speed = i * 10,
                    color = Colors.Purple
                });
            }

            lbxCars.DataContext = carlist;
        }
    }
}
