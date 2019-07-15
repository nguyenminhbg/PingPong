using AppBongBan.ViewModels.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoreClubPage : ContentPage
    {
        public MoreClubPage()
        {
            InitializeComponent();
            //BindingContext = new MoreClubPageViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var model = BindingContext as MoreClubPageViewModel;
            if (model != null)
            {

            }
        }
        private void CommandClubView_TapAddNews(object sender, EventArgs e)
        {
            var view = sender as View;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });

            Services.Service.Instiance().ClubModel.NaviAddNews();
        }

        private void CommandClubView_TapAddImage(object sender, EventArgs e)
        {
            var view = sender as StackLayout;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            Services.Service.Instiance().ClubModel.NaviUpLoadImage();
        }
    }
}