using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Clubs;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace AppBongBan.ViewModels
{
    public class RepairClubPageViewModel : BaseViewModel
    {
        private RepairClubVM _addClubViewModel;
        public RepairClubVM AddClubViewModel { get => _addClubViewModel; set { SetProperty(ref _addClubViewModel, value); } }
        public RepairClubPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            AddClubViewModel = Services.Service.Instiance().RepairClub;
            AddClubViewModel.navigationService = navigationService;
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                AddClubViewModel.NewClub = (Club)parameters["Club"];
            }
            if(parameters.ContainsKey("Position"))
            {
                AddClubViewModel.NewClub.clubPosition = (Position)parameters["Position"];
            }
        }
    }
}
