using Acr.UserDialogs;
using AppBongBan.ViewModels.Challenge;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Challenge
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChallengePage : ContentPage
    {
        public ChallengePage()
        {
            InitializeComponent();
        }
        ChallengePageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null) model = BindingContext as ChallengePageViewModel;

        }
        /// <summary>
        /// thực hiện gửi bản tin thách đấu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (Content != null && Content.Text!=null&& !Content.Text.Equals(""))
            {
                if (!isOutLenght)
                {
                    if (model != null) model.SendChall(Content.Text);
                }
                else
                {
                    UserDialogs.Instance.Toast("Nội dung vượt quá 150 kí tự");
                }
            }
            else
            {
                UserDialogs.Instance.Toast("Hãy thêm nội dung thách đấu");
            }
        }
        bool isOutLenght = false;
        private void CustomEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Content != null && Content.Text.Length > 150)
            {
                isOutLenght = true;
                if (!lbNotifi.IsVisible) { lbNotifi.IsVisible = true; }
            }
            else
            {
                isOutLenght = false;
                if (lbNotifi.IsVisible) { lbNotifi.IsVisible = false; }
            }
        }
    }
}