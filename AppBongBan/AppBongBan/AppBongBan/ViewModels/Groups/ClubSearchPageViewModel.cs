using System;
using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.Services;
using PingPong;
using Plugin.ExternalMaps;
using Prism.Navigation;
using qhmono;

namespace AppBongBan.ViewModels.Groups.MyClubs
{
    public class ClubSearchPageViewModel : BaseViewModel
    {
        private ClubSearchVM _clubSearch;
        public ClubSearchVM clubSearch
        {
            get => _clubSearch;
            set { SetProperty(ref _clubSearch, value); }
        }
        public ClubSearchPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Service.Instiance().clubSearchVM.Reset();
            clubSearch = Service.Instiance().clubSearchVM;
            clubSearch.navigationService = navigationService;
        }
        public async void LoadObject(long Kind, long Radius)
        {
            // Kiểm tra vị trí
            if (! await Helper.Instance().CheckGps())
            {
                clubSearch.isFreshing = false;
                return;
            }
            //tìm kiếm theo acc
            clubSearch.ListClub.Clear();
            //TextLoadMap = "Tìm kiếm quanh bán kính " + Radius + " m";
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_SCAN_LOCATION_REQ);
            if (Helper.Instance().MyAccount == null)
            {
                return;
            }
            var id = Helper.Instance().MyAccount.Number_Id;
            //id của người gửi yêu cầu scane
            if (id > 0)
            {
                msg.SetAt((byte)MsgScanLocationReqArg.NumberID, new QHNumber(id));
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNumber();
                return;
            }
            msg.SetAt((byte)MsgScanLocationReqArg.Kind, new QHNumber(Kind));
            var position = await Helper.Instance().MyPosition();
            if (position.Latitude > 0 || position.Longitude > 0)
            {
                msg.SetAt((byte)MsgScanLocationReqArg.Latitude, new QHNumber((long)(position.Latitude / Math.Pow(10, -6))));
                msg.SetAt((byte)MsgScanLocationReqArg.Longitude, new QHNumber((long)(position.Longitude / Math.Pow(10, -6))));
            }
            else
            {
                return;
            }
            msg.SetAt((byte)MsgScanLocationReqArg.Radius, new QHNumber(Radius));
            Service.Instiance().SendMessage(msg);
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
