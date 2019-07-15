using System;
using System.Windows.Input;
using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using Plugin.ExternalMaps;
using Prism.Commands;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Groups
{
    public class SearchLocationPageViewModel : BaseViewModel
    {
        private SearchLocationVM _searchLocation;
        /// <summary>
        /// AccRefresh danh sách account
        /// </summary>
        public ICommand AccRefreshCmd { get; set; }
        public ICommand ClubFreshCmd { get; set; }
        public ICommand AreaClubFreshCmd { get; set; }
        public SearchLocationVM SearchLocation { get => _searchLocation; set { SetProperty(ref _searchLocation, value); } }
        public SearchLocationPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().SearchLocationModel.Reset();
            SearchLocation = Services.Service.Instiance().SearchLocationModel;
            SearchLocation.navigationService = Navigation;
            AccRefreshCmd = new DelegateCommand(RefreshAccExe);
            ClubFreshCmd = new DelegateCommand(RefreshClubExe);
            AreaClubFreshCmd = new DelegateCommand(AreaClubFreshExe);
        }
        private void AreaClubFreshExe()
        {
            if (SearchLocation.SelectItemComplete != null)
                SearchLocation.SearchAreaExe(SearchLocation.SelectItemComplete);
        }
        private void RefreshClubExe()
        {
            SearchLocation.LoadObject(1, SearchLocation.Radius);
        }
        /// <summary>
        /// Refresh account
        /// </summary>
        private void RefreshAccExe()
        {
            SearchLocation.LoadObject(0, SearchLocation.Radius);
        }

        public async void ShowMap(Club club)
        {
            if (club.clubPosition.Latitude > 0 || club.clubPosition.Longitude > 0)
            {
                var success = await CrossExternalMaps.Current.NavigateTo(club.TextShow, club.clubPosition.Latitude, club.clubPosition.Longitude);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorPosition();
            }
        }

        public async void NaviCheckin(Club club)
        {
            if (club.ClubID > 0)
            {
                var param = new NavigationParameters();
                param.Add("Club", club.ClubID);
                await Navigation.NavigateAsync("ListCheckInPage", param);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }

        public async void ShowMap(Accountlocal acc)
        {
            if (acc.Latitude > 0 || acc.Longtitude > 0)
            {

                var success = await CrossExternalMaps.Current.NavigateTo(acc.fullname, acc.Latitude * Math.Pow(10, -6), acc.Longtitude * Math.Pow(10, -6));
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorPosition();
            }
        }

        public async void NavigAccount(Accounts acc)
        {
            var param = new NavigationParameters();
            long numberId = (-1);
            if (acc.Number_Id == Helper.Instance().MyAccount.Number_Id)
            {
                numberId = 0;
            }
            else
            {
                numberId = acc.Number_Id;
            }
            param.Add("Account", numberId);
            await Navigation.NavigateAsync("DetailPersonPage", param);
        }
        /// <summary>
        /// Thực hiện chuyển sang trang thách đấu cá nhân
        /// </summary>
        /// <param name="acc"></param>
        public async void NavigChall(Accountlocal acc)
        {
            if (acc.Challenge.Equals("pingpong.png"))
            {
                var param = new NavigationParameters();
                param.Add("Account", acc.Number_Id);
                await Navigation.NavigateAsync("ChallengePage", param);
            }
            else
            {
                UserDialogs.Instance.Toast("Đợt duyệt thách đấu");
            }
           
        }
        /// <summary>
        /// Chuyển sang trang thách đấu Club
        /// </summary>
        /// <param name="acc"></param>
        public async void NavigChall(Club club)
        {
            if (club.Challenge.Equals("pingpong.png"))
            {
                var param = new NavigationParameters();
                param.Add("Club", club.ClubID);
                await Navigation.NavigateAsync("ChallengeClubPage", param);
            }
            else
            {
                UserDialogs.Instance.Toast("Đợt duyệt thách đấu");
            }

        }

    }
}
