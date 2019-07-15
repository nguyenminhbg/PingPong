using AppBongBan.Helpers;
using AppBongBan.Models.Db;
using AppBongBan.Models.PingPongs;
using PingPong;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AppBongBan.ViewModels.Clubs
{
    public class RepairClubVM : ViewModelBase
    {
        private Club _newClub;
        private string _avatarClub;
        private string _communWar;
        private string _province;
        private string _district;
        private string _selectCloseTime;
        private string _selectOpenTime;
        private string _selectDate;
        public INavigationService navigationService { get; set; }
        public Club NewClub
        {
            get => _newClub; set
            {
                SetProperty(ref _newClub, value);
                AvatarClub = _newClub.ClubAvatarUri;
                CommunWar = _newClub.Commun;
                District = _newClub.District;
                Province = _newClub.Province;
                SelectCloseTime = Helper.ConvertToHourTime(_newClub.CloseTime);
                SelectOpenTime = Helper.ConvertToHourTime(_newClub.OpenTime);
                SelectDate = _newClub.FoundDateTime;
            }
        }
        public string CommunWar { get => _communWar; set { SetProperty(ref _communWar, value); } }
        public string Province { get => _province; set { SetProperty(ref _province, value); } }
        public string District { get => _district; set { SetProperty(ref _district, value); } }
        public string SelectCloseTime { get => _selectCloseTime; set { SetProperty(ref _selectCloseTime, value); } }
        public string SelectOpenTime { get => _selectOpenTime; set { SetProperty(ref _selectOpenTime, value); } }
        public string AvatarClub { get => _avatarClub; set { SetProperty(ref _avatarClub, value); } }
        public string SelectDate { get => _selectDate; set { SetProperty(ref _selectDate, value); } }
        public ICommand SelectedCommunWarCmd { get; set; }
        public RepairClubVM()
        {
            SelectedCommunWarCmd = new DelegateCommand(NavLoadMap);
        }
        private async void NavLoadMap()
        {
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("addressCommuneWard", new AddressCommuneWard
                {
                    CommnuneWard = CommunWar,
                    District = District,
                    ProvinceName = Province
                });
                param.Add("ClubName", NewClub.ClubName);
                param.Add("Street", NewClub.StreetAddress);
                await navigationService.NavigateAsync("LoadMapPage", param);
            }
        }
        /// <summary>
        /// Update tạo độ club
        /// </summary>
        public void SendUpdate()
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_LOCATION_UPDATE_REQ);
            var id = Helper.Instance().MyAccount.Number_Id;
            if (id > 0)
            {
                msg.SetAt((byte)MsgLocationUpdateReqArg.NumberID, new QHNumber(id));
            }
            else
            {
                return;
            }
            msg.SetAt((byte)MsgLocationUpdateReqArg.Kind, new QHNumber(1));
            if (NewClub != null && NewClub.ClubID > 0)
            {
                msg.SetAt((byte)MsgLocationUpdateReqArg.ClubID, new QHNumber(NewClub.ClubID));
                if (NewClub.clubPosition.Longitude > 0 || NewClub.clubPosition.Latitude > 0)
                {
                    msg.SetAt((byte)MsgLocationUpdateReqArg.Latitude, new QHNumber((long)(NewClub.clubPosition.Latitude * Math.Pow(10, 6))));
                    Debug.WriteLine("tọa độ" + (long)(NewClub.clubPosition.Latitude * Math.Pow(10, 6)));

                    msg.SetAt((byte)MsgLocationUpdateReqArg.Longitude, new QHNumber((long)(NewClub.clubPosition.Longitude * Math.Pow(10, 6))));
                    Debug.WriteLine("tọa độ" + (long)(NewClub.clubPosition.Longitude * Math.Pow(10, 6)));
                }
                Services.Service.Instiance().Position = Services.StatueUpdatePosition.UpdateClub;
                Services.Service.Instiance().SendMessage(msg);
            }
        }

        public void OnMsgLocationUpdateAck(QHMessage msg)
        {
            var error = (LocationUpdateError)(msg.GetAt((byte)MsgLocationUpdateAckArg.Error) as QHNumber).value;
            switch (error)
            {
                case LocationUpdateError.SUCCESS:
                    NotifiDialog.Initiance().DialogUpdateSuccess();
                    break;
                case LocationUpdateError.ERR_PERMISSION_DENIED:
                    NotifiDialog.Initiance().DialogUpdateError();
                    break;
            }
        }
    }
}
