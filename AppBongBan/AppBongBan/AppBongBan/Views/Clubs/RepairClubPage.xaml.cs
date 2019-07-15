using AppBongBan.ViewModels;
using AppBongBan.ViewModels.Clubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Clubs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RepairClubPage : ContentPage
    {
        public RepairClubPage()
        {
            InitializeComponent();
        }
        RepairClubPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as RepairClubPageViewModel;
            } 
        }

        private void ClubView_TapAdd(object sender, EventArgs e)
        {

        }

        private void UpdateClub(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.AddClubViewModel.SendUpdate();
            }
        }
    }
}