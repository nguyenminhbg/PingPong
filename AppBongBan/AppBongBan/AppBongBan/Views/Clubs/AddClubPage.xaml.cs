using AppBongBan.Helpers;
using AppBongBan.ViewModels.Clubs;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Clubs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddClubPage : ContentPage
    {
        public AddClubPage()
        {
            InitializeComponent();
        }
        AddClubPageViewModel model;
        protected override void OnAppearing()
        {
            view.clubstreet = true;
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as AddClubPageViewModel;
            }
            Helper.Instance().CurrentPage = this;
            //đăng kí nhận tin về chọn ảnh làm ảnh nền club
            Helper.CaseSelectImag = SelectImage.SelectedCover;
            if (model != null)
            {
                var app = Application.Current as App;
                if (app.ClubPosition.Latitude > 0 || app.ClubPosition.Longitude > 0)
                {
                    model.AddClubViewModel.MyPostion = app.ClubPosition;
                    app.ClubPosition = new Xamarin.Forms.GoogleMaps.Position();
                }
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            view.clubstreet =false;
        }
        private void ClubView_TapAdd(object sender, EventArgs e)
        {
            //var imag = sender as Image;
            //await imag.ScaleTo(1.3, 150, Easing.CubicOut);
            //await imag.ScaleTo(1, 150, Easing.CubicIn);
            var model = BindingContext as AddClubPageViewModel;
            model.AddClubViewModel.SelectImageExe();
        }
    }
}