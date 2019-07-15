using Acr.UserDialogs;
using AppBongBan.ViewModels.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Challenge
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChallengeClubPage : ContentPage
	{
		public ChallengeClubPage ()
		{
			InitializeComponent ();
		}
        ChallengeClubPageViewModel model;
        bool isOutLenght = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            base.OnAppearing();
            if (model == null) model = BindingContext as ChallengeClubPageViewModel;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (Content != null && Content.Text != null && !Content.Text.Equals(""))
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

        private void Content_TextChanged(object sender, TextChangedEventArgs e)
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