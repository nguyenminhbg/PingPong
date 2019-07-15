using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.ViewModels.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            BindingContext = Services.Service.Instiance().LoginViewModel;
            InitializeComponent();
          //  NavigationPage.SetHasNavigationBar(this, false);
            Phone.Completed += (s, e) => { PassWord.Focus(); };
        }

       

        private void PassWord_Focused(object sender, FocusEventArgs e)
        {
            throw new NotImplementedException();
        }

        LoginPageViewModel model;
        protected override void OnAppearing()
        {
            UserDialogs.Instance.HideLoading();
         //   AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            if(model==null)
            model = this.BindingContext as LoginPageViewModel;
            model.Page = this;
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var exitApp = await DisplayAlert("Thông báo", "Bạn có thực sự muốn thoát khỏi SportTV", "Yes", "No");
                if (exitApp)
                {
                    DependencyService.Get<ICloseApp>().CloseApp();
                }
            });
            return true;
        }
        private void TapRegis(object sender, EventArgs e)
        {
            var view = sender as View;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.Default;
                return false;
            });
            model.navigation();
        }
        private void TapLogin(object sender, EventArgs e)
        {
            if (Phone.Text == null || Phone.Text.Equals(""))
            {
                NotifiDialog.Initiance().DialogErrorNumber();
                return;
            }
            if (PassWord.Text == null || PassWord.Text.Equals(""))
            {
                NotifiDialog.Initiance().DialogErrorPassWord();
                return;
            }
            if (model != null)
            {
                model.loginExe(Phone.Text, PassWord.Text);
            }
        }
    }
}