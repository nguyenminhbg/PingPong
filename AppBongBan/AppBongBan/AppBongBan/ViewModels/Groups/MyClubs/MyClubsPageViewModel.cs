
using System.Collections.Generic;
using System.Windows.Input;
using AppBongBan.Helpers;
using AppBongBan.Models.PingPongs;
using Plugin.ExternalMaps;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups.MyClubs
{
    public class MyClubsPageViewModel : BaseViewModel
    {
        private MyClubsVM _myClubs;
        public MyClubsVM MyClubs { get => _myClubs; set { SetProperty(ref _myClubs, value); } }
        public ICommand RefreshCmd { get; set; }
        public ICommand OpenTimeCmd { get; set; }
        public ICommand CloseTimeCmd { get; set; }
        public MyClubsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().myClubsVM.Reset();
            MyClubs = Services.Service.Instiance().myClubsVM;
            RefreshCmd = new DelegateCommand(RefreshExe);

        }
        private void RefreshExe()
        {
            if (MyClubs.IsLoading)
                return;
            GetList();
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            // GetList();
        }
        public void NaviDetailClub(Club club)
        {
            var param = new NavigationParameters();
            param.Add("Club", club.ClubID);
            Navigation.NavigateAsync("ClubPage", param);
        }
        /// <summary>
        /// Lấy danh sách Club theo numberId
        /// </summary>
        public void GetList()
        {
            if (Helper.Instance().MyAccount != null)
            {
                MyClubs.IsNull = false;
                MyClubs.IsLoading = true;
                Helper.Instance().CheckExistClubMyClubId(Helper.Instance().MyAccount.Number_Id);
                Device.StartTimer(System.TimeSpan.FromMilliseconds(500), () =>
                {
                    MyClubs.IsLoading = false;
                    return false;
                });
                // Lấy Club theo NumberId

            }
        }
        int countClub = 0;
        int clubTotal = 0;
        public void GetList(List<long> list)
        {
            clubTotal = 0;
            countClub = list.Count;
            if (list.Count == 0)
            {
                MyClubs.IsNull = true;
                return;
            }
            foreach (var item in list)
            {
                Helper.Instance().CheckExistClubAsync(item);
            }
        }
        public void GetClub(Club club)
        {
            clubTotal += 1;
            if (club != null)
                MyClubs.ListClubs.Add(club);
        }
        /// <summary>
        /// Chuyển sang site danh sách thành viên
        /// </summary>
        /// <param name="club"></param>
        public void NaviListUse(Club club)
        {
            var app = Application.Current as App;
            app.clubCurrent = club;
            if (Helper.Instance().MyAccount != null && Helper.Instance().MyAccount.Number_Id == club.AdminID)
            {
                Navigation.NavigateAsync("ListUserClubPage");
            }
            else
            {
                Navigation.NavigateAsync("ListMemberPage");
            }
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

    }
}
