using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db;
using AppBongBan.Models.PingPongs;
using AppBongBan.Views.Clubs;
using PingPong;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
namespace AppBongBan.ViewModels.Clubs
{
    public class AddClubViewModel : ViewModelBase
    {
        private string _selectDate;
        private string _openTime;
        private string _closeTime;
        private string _avatarClub;
        private Club _newClub;
        private string _communWar;
        private string _district;
        private string _province;
        private string _lbNotifiAddress;
        private string _lbNotifi;
        private double _progress;
        public INavigationService navigationService;
        public Club NewClub
        {
            get => _newClub;
            set
            {
                SetProperty(ref _newClub, value);
            }
        }
        public Position MyPostion;
        public string AvatarClub
        {
            get => _avatarClub; set
            {
                SetProperty(ref _avatarClub, value);
            }
        }
        public string SelectDate
        {
            get => _selectDate; set
            {
                SetProperty(ref _selectDate, value);
            }
        }
        public string SelectOpenTime
        {
            get => _openTime; set
            {
                SetProperty(ref _openTime, value);
            }
        }
        public string SelectCloseTime
        {
            get => _closeTime; set
            {
                SetProperty(ref _closeTime, value);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        public string CommunWar { get => _communWar; set { SetProperty(ref _communWar, value); } }
        public string District { get => _district; set { SetProperty(ref _district, value); } }
        public string Province { get => _province; set { SetProperty(ref _province, value); } }
        public double Progress { get => _progress; set { SetProperty(ref _progress, value); } }
        public bool isSend = false;
        //biến thông báo kết thúc chọn ảnh làm ảnh đại diện
        public bool IsFinishAddCover;
        //biến thông báo đã đăng kí nhận message chọn ảnh
        public bool IsRegisMessage;
        //text thông báo có lỗi trong quá trình nhập dữ liêu địa chỉ
        public string lbNotifiAddress { get => _lbNotifiAddress; set { SetProperty(ref _lbNotifiAddress, value); } }
        //text thông báo có lỗi trong quá trình nhập tên câu lạc bộ
        public string lbNotifi { get => _lbNotifi; set { SetProperty(ref _lbNotifi, value); } }
        /// <summary>
        /// khởi tạo lớp viewmodel
        /// </summary>
        /// <param name="navigationService"></param>
        public AddClubViewModel()
        {
            CalendarCmd = new DelegateCommand(CalendarExe);
            OpenTimeCmd = new DelegateCommand(OpenTimeExe);
            CloseTimeCmd = new DelegateCommand(CloseTimeExe);
            AddClubCmd = new DelegateCommand(AddClubExe);
            SelectedCommunWarCmd = new DelegateCommand(SearchCommunExe);
            SelectedDistrict = new DelegateCommand(SearchDistrictExe);
            SelectedCity = new DelegateCommand(SearchCityExe);
            NewClub = new Club();
            CommunWar = "Điền phường/xã";
            District = "Điền quận/huyện";
            Province = "Điền tỉnh/thành phố";
            lbNotifi = "";
            lbNotifiAddress = "";
            SelectCloseTime = "";
            SelectOpenTime = "";
            SelectDate = "";
            AvatarClub = "avatarClubImg.png";
            IsFinishAddCover = false;
            IsRegisMessage = false;
        }
        public ICommand CalendarCmd { get; set; }
        public ICommand OpenTimeCmd { get; set; }
        public ICommand CloseTimeCmd { get; set; }
        public ICommand AddClubCmd { get; set; }
        public ICommand SelectedCommunWarCmd { get; set; }
        public ICommand SelectedDistrict { get; set; }
        public ICommand SelectedCity { get; set; }
        /// <summary>
        /// thực hiện chọn ảnh làm ảnh đại diện
        /// </summary>
        public async void SelectImageExe()
        {
            var action = await Helper.Instance().CurrentPage.DisplayActionSheet("Chọn làm ảnh đại diện", "Cancel", null, "Chụp ảnh", "Thư viện ảnh");
            if (action != null)
            {
                var list = new List<string>();
                if (!Helper.uriCover.Equals(""))
                {
                    list.Add(Helper.uriCover);
                    if (!Helper.uriCacheCover.Equals(""))
                    {
                        list.Add(Helper.uriCacheCover);
                    }
                    DependencyService.Get<IMediaService>().ClearFiles(list);
                }
                if (action.Contains("Chụp ảnh"))
                {
                    IsPopup = true;
                    _takePhoto();
                    return;
                }
                if (action.Contains("Thư viện ảnh"))
                {
                    IsPopup = true;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        await CrossMedia.Current.Initialize();
                        if (!CrossMedia.Current.IsPickPhotoSupported)
                        {
                            Debug.WriteLine("No Pick Photo Supported");
                            return;
                        }
                        var file = await CrossMedia.Current.PickPhotoAsync();

                        if (file == null)
                            return;
                        DependencyService.Get<IMediaService>().SavePhoto(file.Path);
                    }
                    else
                        await DependencyService.Get<IMediaService>().OpenGallery(false);
                }
            }
        }
        /// <sumphmary>
        /// Phương thức chụp ảnh
        /// </summary>
        private async void _takePhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Helper.Instance().CurrentPage.DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "TmpPictures",
                SaveToAlbum = true,
                CompressionQuality = 75,
                CustomPhotoSize = 60,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                DefaultCamera = CameraDevice.Front
            });

            if (file == null)
                return;
            IsPopup = true;
            DependencyService.Get<IMediaService>().SavePhoto(file.Path);
        }
        /// <summary>
        /// xử lý khi ack từ server nhận về từ server
        /// </summary>
        /// <param name="msg"></param>
        public void UpdateAck(QHMessage msg)
        {
            var errorCode = (msg.GetAt((byte)MsgClubInfoUpdateAckArg.ErrorCode) as QHNumber).value;
            switch (errorCode)
            {
                case 0:
                    UserDialogs.Instance.Toast("Cập nhật dữ liệu thành công");
                    //DependencyService.Get<IMediaService>().ClearFiles(new List<string> { NewClub.ClubAvatarUri });
                    IsFinishAddCover = true;
                    //Helper.uriCover = "";
                    SendUpdate();
                    break;
            }
        }
        /// <summary>
        /// thực hiện xử lý tạo club
        /// </summary>
        /// <param name="msg"></param>
        public async void AddClubAck(QHMessage msg)
        {
            ClubAddError ERRORCODE = (ClubAddError)(msg.GetAt((byte)MsgClubAddAckArg.ErrorCode) as QHNumber).value;
            switch (ERRORCODE)
            {
                case ClubAddError.SUCCESS:
                    long clubId = 0;
                    //lấy id club
                    if (msg.TryGetAt((byte)MsgClubAddAckArg.ClubID, ref clubId))
                    {
                        NewClub.ClubID = clubId;
                        if (!NewClub.ClubAvatarUri.Equals("CoverImage.jpg"))
                        {
                            try
                            {
                                var client = new WebClientW();
                                //client.UCEvent = (o) =>
                                //{
                                //    WebClientW c = o as WebClientW;
                                //    Debug.WriteLine("Hoàn thành: " );
                                //    NewClub.ClubAvatarUri = client.uri;
                                //    _updateAvartaInfor(NewClub);
                                //};
                                client.UPCEvent = (o, e) =>
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        Progress = e * 0.1;
                                    });
                                };
                                string uri = await client.UploadImage(WebClientW.uploadServiceBaseAddress,
                                    NewClub.ClubAvatarUri,
                                    new UploadImage { token_id = 1, owner_id = NewClub.AdminID });
                                _updateAvartaInfor(uri);

                                isSend = false;
                            }
                            catch (Exception e)
                            {

                            }
                            return;
                        }
                        // Update vao Dic
                        if (Helper.Instance().MyIDClubs.ContainsKey(Helper.Instance().MyAccount.Number_Id))
                        {
                            var listMyClub = Helper.Instance().MyIDClubs[Helper.Instance().MyAccount.Number_Id];
                            listMyClub.Add(NewClub.ClubID);
                            Helper.Instance().MyIDClubs[Helper.Instance().MyAccount.Number_Id] = listMyClub;
                        }
                        UserDialogs.Instance.Toast("Tạo club thành công");
                        SendUpdate();

                    }
                    break;
                case ClubAddError.ERR_MISSING_CLUB_NAME:
                    UserDialogs.Instance.Toast("Lỗi tên Club");
                    break;
                case ClubAddError.ERR_MISSING_PHONE_NUMBER:
                    UserDialogs.Instance.Toast("Lỗi số điện thoại");
                    break;
                case ClubAddError.ERR_NOT_EXIST_ADMIN:
                    UserDialogs.Instance.Toast("Lỗi không tồn tại admin");
                    break;
            }
        }
        private void _updateAvartaInfor(string uri)
        {
            var msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_INFO_UPDATE_REQ);
            if (!NewClub.ClubID.Equals(""))
            {
                msg.SetAt((byte)MsgClubInfoUpdateReqArg.ClubID, new QHNumber(NewClub.ClubID));
            }
            else
            {
                UserDialogs.Instance.Toast("Không có id club");
                return;
            }
            if (!NewClub.AdminID.Equals(""))
            {
                msg.SetAt((byte)MsgClubInfoUpdateReqArg.NumberID, new QHNumber(NewClub.AdminID));
            }
            else
            {
                UserDialogs.Instance.Toast("Không có number id");
                return;
            }
            if (uri.Contains("http"))
            {
                msg.SetAt((byte)MsgClubInfoUpdateReqArg.ClubCoverUri, new QHString(uri));
            }
            else
            {
                UserDialogs.Instance.Toast("Không có avatar");
                SendUpdate();
                return;
            }
            if (!Services.Service.Instiance().SendMessage(msg))
            {

            }

        }
        /// <summary>
        /// thay đổi ảnh nền của club
        /// </summary>
        /// <param name="images"></param>
        public void ChangeImageAvatar(List<ImageNews> images)
        {
            Helper.uriCover = images[0].UriTmp;
            AvatarClub = images[0].UriTmp;
            NewClub.ClubAvatarUri = images[0].UriTmp;
        }
        public void CropAvatar(List<ImageNews> images)
        {
            if (images.Count > 0 && !images[0].UriTmp.Equals(Helper.uriCacheCover))
            {
                IsPopup = true;
                Helper.uriCacheCover = images[0].UriTmp;
                DependencyService.Get<IMediaService>().SavePhoto(images[0].UriTmp);
            }
        }
        private void AddClubExe()
        {
            if (!isSend)
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_ADD_REQ);
                if (lbNotifi.Equals(""))
                {
                    if (!CheckIndex(NewClub.ClubName, "Điền tên club", msg, (byte)MsgClubAddReqArg.ClubName, true))
                    {
                        return;
                    }
                }
                else
                {
                    UserDialogs.Instance.Toast("Kiểm tra lại tên Club");
                    return;
                }
                if (lbNotifiAddress.Equals(""))
                {
                    if (!CheckIndex(NewClub.StreetAddress, "Thêm địa chỉ câu lạc bộ", msg, (byte)MsgClubAddReqArg.Street, true))
                    {
                        return;
                    }
                }
                else
                {
                    UserDialogs.Instance.Toast("Kiểm tra lại địa chỉ câu lạc bộ");
                    return;
                }
                if (!CheckIndex(NewClub.ClubPhoneNumber, "Điền số điện thoại", msg, (byte)MsgClubAddReqArg.ClubPhoneNumber, true))
                {
                    return;
                }
                if (!CheckIndex(NewClub.OpenTime + "", "Thêm thời gian mở", msg, (byte)MsgClubAddReqArg.OpenTime, false))
                {
                    return;
                }
                if (!CheckIndex(NewClub.CloseTime + "", "Thêm thời gian đóng", msg, (byte)MsgClubAddReqArg.CloseTime, false))
                {
                    return;
                }
                if (NewClub.CloseTime <= NewClub.OpenTime)
                {
                    SelectOpenTime = string.Empty;
                    SelectCloseTime = string.Empty;
                    UserDialogs.Instance.Toast("Thời gian đóng phải lớn hơn thời gian mở");
                    return;
                }
                if (NewClub.FoundDate == 0)
                    NewClub.FoundDate = Helper.ConvertToUnixTime(DateTime.Now);
                if (!CheckIndex(NewClub.FoundDate + "", "Thêm ngày thành lập", msg, (byte)MsgClubAddReqArg.FoundDate, false))
                {
                    return;
                }


                if (!CheckIndex(NewClub.CommuneWardAddress + "", "Thêm xã/phường", msg, (byte)MsgClubAddReqArg.Commune, false))
                {
                    return;
                }
                if (!CheckIndex(NewClub.DistrictAddress + "", "Thêm quận/huyện", msg, (byte)MsgClubAddReqArg.District, false))
                {
                    return;
                }
                if (!CheckIndex(NewClub.ProvinceCityAddress + "", "Thêm Tỉnh/Thành phố", msg, (byte)MsgClubAddReqArg.ProvinceCity, false))
                {
                    return;
                }
                if (NewClub.Description == null) NewClub.Description = "";
                msg.SetAt((byte)MsgClubAddReqArg.Description, NewClub.Description);
                if (Helper.Instance().MyAccount != null)
                {
                    msg.SetAt((byte)MsgClubAddReqArg.AdminID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
                    NewClub.AdminID = Helper.Instance().MyAccount.Number_Id;
                }
                else
                {
                    UserDialogs.Instance.Toast("Chưa đăng nhập vào hệ thống");
                    return;
                }
                if (Services.Service.Instiance().SendMessage(msg))
                {
                    Debug.WriteLine("json: " + msg.JSONString());
                    isSend = true;
                }
            }
        }
        /// <summary>
        /// Check thứ tự thêm vào 
        /// </summary>
        /// <param name="textCompare"></param>
        /// <param name="toast"></param>
        /// <param name="msg"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        private bool CheckIndex(string textCompare, string toast, QHMessage msg, byte index, bool type)
        {
            if (!textCompare.Equals(""))
            {
                if (type)
                {
                    msg.SetAt(index, new QHString(Encoding.UTF8.GetBytes(textCompare)));
                }
                else
                {
                    msg.SetAt(index, new QHNumber(long.Parse(textCompare)));
                }
                return true;
            }
            else
            {
                UserDialogs.Instance.Toast(toast);
                return false;
            }
        }
        /// <summary>
        /// phương thức chọn giờ đóng cửa
        /// </summary>
        private void CloseTimeExe()
        {
            CrossXDateTimeNumberPicker.Current.ShowTimePicker("Choose a time", DateTime.Now, seletedTime =>
            {
                SelectCloseTime = $"{seletedTime.Hour}:{seletedTime.Minute}";
                NewClub.CloseTime = seletedTime.Hour * 3600 + seletedTime.Minute * 60;
            });
        }
        /// <summary>
        /// phương thức chọn giờ mở cửa
        /// </summary>
        private void OpenTimeExe()
        {
            CrossXDateTimeNumberPicker.Current.ShowTimePicker("Choose a time", DateTime.Now, seletedTime =>
            {
                SelectOpenTime = $"{seletedTime.Hour}:{seletedTime.Minute}";
                NewClub.OpenTime = seletedTime.Hour * 3600 + seletedTime.Minute * 60;
            });
        }
        /// <summary>
        /// phương thức chọn ngày thành lập
        /// </summary>
        private void CalendarExe()
        {
            var now = DateTime.Now;
            var minDate = DateTime.Now.AddYears(-3);
            var maxDate = DateTime.Now.AddYears(5);

            CrossXDateTimeNumberPicker.Current.ShowDatePicker(now, minDate, maxDate, "Choose a date", selectedDate =>
            {
                SelectDate = $"{selectedDate:dd/MM/yyyy}";
                NewClub.FoundDate = Helper.ConvertToUnixTime(selectedDate);

            });
        }
        private async void SearchCommunExe()
        {
            IsPopup = true;
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Search", 1);
                await navigationService.NavigateAsync("SearchAddressPage", param);
            }

        }
        private async void SearchDistrictExe()
        {
            IsPopup = true;
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Search", 2);
                await navigationService.NavigateAsync("SearchAddressPage", param);
            }
        }
        private async void SearchCityExe()
        {
            IsPopup = true;
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Search", 2);
                await navigationService.NavigateAsync("SearchAddressPage", param);
            }

        }

        /// <summary>
        /// Thay đổi đia chỉ trên giao diện
        /// </summary>
        /// <param name="addressDictrict"></param>
        public async void ChangeDistrict(AddressDistrict addressDictrict)
        {
            NewClub.DistrictAddress = addressDictrict.Id;
            NewClub.ProvinceCityAddress = addressDictrict.Id_city;
            District = addressDictrict.District;
            Province = addressDictrict.ProvinceName;
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("addressDistrict", addressDictrict);
                param.Add("ClubName", NewClub.ClubName);
                param.Add("Street", NewClub.StreetAddress);
                await navigationService.NavigateAsync("LoadMapPage", param);
            }
        }
        public async void ChangeProvince(AddressProvince addressProvince)
        {
            NewClub.ProvinceCityAddress = addressProvince.Id;
            Province = addressProvince.ProvinceName;
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("addressProvince", addressProvince);
                param.Add("ClubName", NewClub.ClubName);
                param.Add("Street", NewClub.StreetAddress);
                await navigationService.NavigateAsync("LoadMapPage", param);
            }
        }
        public async void ChangeCommnuneWard(AddressCommuneWard addressCommuneWard)
        {
            NewClub.CommuneWardAddress = addressCommuneWard.Id_communeWards;
            NewClub.DistrictAddress = addressCommuneWard.Id;
            NewClub.ProvinceCityAddress = addressCommuneWard.Id_city;
            District = addressCommuneWard.District;
            Province = addressCommuneWard.ProvinceName;
            CommunWar = addressCommuneWard.CommnuneWard;
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("addressCommuneWard", addressCommuneWard);
                param.Add("ClubName", NewClub.ClubName);
                param.Add("Street", NewClub.StreetAddress);
                await navigationService.NavigateAsync("LoadMapPage", param);
            }
        }
        public override void Reset()
        {
            NewClub = new Club();
            CommunWar = "Điền phường/xã";
            District = "Điền quận/huyện";
            Province = "Điền tỉnh/thành phố";
            lbNotifi = "";
            lbNotifiAddress = "";
            SelectCloseTime = "";
            SelectOpenTime = "";
            SelectDate = "";
            AvatarClub = "avatarClubImg.png";
            IsFinishAddCover = false;
            IsRegisMessage = false;
            Progress = 0;
            MyPostion = new Position();
            isSend = false;

        }
        /// <summary>
        /// Cập nhật tọa độ của club
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
                if (MyPostion.Longitude > 0 || MyPostion.Latitude > 0)
                {
                    msg.SetAt((byte)MsgLocationUpdateReqArg.Latitude, new QHNumber((long)(MyPostion.Latitude * Math.Pow(10, 6))));

                    msg.SetAt((byte)MsgLocationUpdateReqArg.Longitude, new QHNumber((long)(MyPostion.Longitude * Math.Pow(10, 6))));

                    Services.Service.Instiance().Position = Services.StatueUpdatePosition.AddClub;
                    if (Services.Service.Instiance().SendMessage(msg))
                    {

                    }
                }
                else
                {
                    NotifiDialog.Initiance().DialogUpdateError();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (navigationService != null)
                        {
                            await Task.Delay(100);
                            await navigationService.GoBackAsync();
                        }
                    });
                }

            }
        }
        public void OnMsgLocationUpdateAck(QHMessage msg)
        {
            var error = (LocationUpdateError)(msg.GetAt((byte)MsgLocationUpdateAckArg.Error) as QHNumber).value;
            switch (error)
            {
                case LocationUpdateError.SUCCESS:
                    NotifiDialog.Initiance().DialogUpdateSuccess();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (navigationService != null)
                        {
                            await Task.Delay(100);
                            await navigationService.GoBackAsync();
                        }
                    });
                    break;
                case LocationUpdateError.ERR_PERMISSION_DENIED:
                    NotifiDialog.Initiance().DialogUpdateError();
                    break;
            }
        }
    }
}
