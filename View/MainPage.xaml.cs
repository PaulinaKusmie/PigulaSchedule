using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PigulaSchedule.ViewModels;


namespace PigulaSchedule.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
          
            InitializeComponent();
            BindingContext = vm;
        }

    }
}