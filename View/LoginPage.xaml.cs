using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PigulaSchedule.ViewModels;


namespace PigulaSchedule.View
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
          
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

    }
}