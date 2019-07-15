
using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Services;
using PingPong;
using Plugin.ExternalMaps;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
   public class PersonalSearchPageViewModel: BaseViewModel
    {
        public ICommand AccRefreshCmd { get; set; }
        private PersonalSearchPageVM _persons;
        public PersonalSearchPageVM persons
        {
            get => _persons;
            set { SetProperty(ref _persons, value); }
        }

        public PersonalSearchPageViewModel(INavigationService navigationService ) :base(navigationService)
        {
            Service.Instiance().personalSearchPageVM.Reset();
            persons = Service.Instiance().personalSearchPageVM;
            persons.navigationService = Navigation;
            AccRefreshCmd = new DelegateCommand(RefreshAccExe);
        }
        public void RefreshAccExe()
        {
            LoadObject(0, persons.Radius);
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                Task.Run(() =>
                {
                    persons.IsFreshing = false;
                });
                return false;
            });
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
        /// <summary>
        /// thực hiện load danh sách club qua tọa độ
        /// </summary>
        /// <param name="Kind"></param>
        public async void LoadObject(long Kind, long Radius)
        {
            //Kiểm tra điều kiện xem đã bật vị trí hay chưa
            if (! await Helper.Instance().CheckGps())
            {
                persons.IsFreshing = false;
                return;
            }
               
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
                return;
            msg.SetAt((byte)MsgScanLocationReqArg.Radius, new QHNumber(Radius));
            Service.Instiance().SendMessage(msg);

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
    }
}
