using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using Prism.Navigation;
using Xamarin.Forms;

namespace AppBongBan.ViewModels
{
    public class FlashPageViewModel : BaseViewModel
    {
        public FlashPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            
        }
        public void Navigate()
        {
            Navigation.NavigateAsync("Home");
        }
    }
}
