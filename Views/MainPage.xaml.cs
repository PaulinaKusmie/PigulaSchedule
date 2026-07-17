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


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel vm)
                await vm.InitializeAsync();
        }

    }
}