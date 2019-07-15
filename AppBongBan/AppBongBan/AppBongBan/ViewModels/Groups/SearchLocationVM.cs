using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db;
using AppBongBan.Models.PingPongs;
using AppBongBan.Services;
using AppBongBan.ViewModels.Actions;
using DLToolkit.Forms.Controls;
using PingPong;
using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class SearchLocationVM : ViewModelBase
    {
        private readonly object objectLock = new object();
        public INavigationService navigationService { get; set; }
        private AutoComplete _selectItemComplete;
        public bool IsLoadAcc { get => _isLoadAcc; set { SetProperty(ref _isLoadAcc, value); } }
        public bool IsLoadClub { get => _isLoadClub; set { SetProperty(ref _isLoadClub, value); } }
        public bool IsLoadArea { get => _isLoadArea; set { SetProperty(ref _isLoadArea, value); } }
        public bool IsAreaNull { get => _isAreaNull; set { SetProperty(ref _isAreaNull, value); } }
        public bool IsAccNull { get => _isAccNull; set { SetProperty(ref _isAccNull, value); } }
        public bool IsClubNull { get => _isClubNull; set { SetProperty(ref _isClubNull, value); } }
        //thực hiện load danh sách mới
        private bool _isMore;
        public string TextLoadMap
        {
            get => _textTextLoadMap; set
            {
                SetProperty(ref _textTextLoadMap, value);
            }
        }
        public string ColorLoad
        {
            get => _colorLoad; set
            {
                SetProperty(ref _colorLoad, value);
            }
        }
        public new bool IsMore
        {
            get => _isMore; set
            {
                SetProperty(ref _isMore, value);
            }
        }
        /// <summary>
        /// Search club qua tên club
        /// </summary>
        /// <param name="stringSearch"></param>
        /// <param name="lastIndex"></param>
        public void SearchClub(string stringSearch, int lastIndex)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_SEARCH_REQ);
            msg.SetAt((byte)MsgClubSearchReqArg.SearchBy, new QHNumber(0));
            if (!stringSearch.Equals(""))
            {
                msg.SetAt((byte)MsgClubSearchReqArg.ClubName, new QHString(stringSearch));
            }
            else
            {
                return;
            }
            if (lastIndex >= 0)
            {
                msg.SetAt((byte)MsgClubSearchReqArg.FromClubID, new QHNumber(lastIndex));
            }
            else
            {
                return;
            }
            msg.SetAt((byte)MsgClubSearchReqArg.Limit, new QHNumber(10));
            if (!Services.Service.Instiance().SendMessage(msg))
            {
                NotifiDialog.Initiance().DialogErrorInternter();
            }
        }
        public FlowObservableCollection<Accountlocal> listAcc { get; set; }
        public FlowObservableCollection<Club> ListClubArea { get; set; }
        private List<Club> GetListClub(QHMessage msg)
        {
            QHTable listUser = new QHTable();
            if (msg.TryGetAt((byte)PingPong.MsgClubSearchAckArg.Clubs, ref listUser))
            {
                Debug.WriteLine("vào load dữ liệu" + msg.JSONString());
                var list = new List<Club>();
                if (listUser.GetRowCount() != 0)
                {
                    for (int i = 0; i < listUser.GetRowCount(); i++)
                    {
                        long _clubID = 0;
                        string _clubName = "";
                        string _clubAvatarUri = "";
                        string _streetAddress = "";
                        long _communeWardAddress = 0;
                        long _districtAddress = 0;
                        long _provinceCityAddress = 0;
                        long _checkinCount = 0;
                        long _long = 0;
                        long _lag = 0;
                        QHVector Members = null;

                        QHVector Request = new QHVector();

                        var club = new Club();

                        if (listUser.TryGetAt(i, 0, ref _clubID))
                        {
                            club.ClubID = _clubID;
                            Debug.WriteLine("id của club đã nhận: " + _clubID);
                        }

                        if (listUser.TryGetAt(i, 1, ref _clubName))
                        {
                            club.ClubName = _clubName;
                        }

                        if (listUser.TryGetAt(i, 2, ref _clubAvatarUri))
                        {
                            if (!_clubAvatarUri.Equals(""))
                            {
                                club.ClubAvatarUri = _clubAvatarUri;
                            }
                        }

                        if (listUser.TryGetAt(i, 5, ref _streetAddress))
                        {
                            club.StreetAddress = _streetAddress;
                        }

                        if (listUser.TryGetAt(i, 6, ref _communeWardAddress))
                        {
                            club.CommuneWardAddress = _communeWardAddress;
                        }

                        if (listUser.TryGetAt(i, 7, ref _districtAddress))
                        {
                            club.DistrictAddress = _districtAddress;
                        }

                        if (listUser.TryGetAt(i, 8, ref _provinceCityAddress))
                        {
                            club.ProvinceCityAddress = _provinceCityAddress;
                        }
                        bool x = listUser.TryGetAt(i, 9, ref Members);
                        if (x)
                        {
                            if (Members.Length > 0)
                            {
                                club.MemberCount = Members.Length;
                            }
                            List<long> members = new List<long>();
                            for (int j = 0; j < Members.Length; j++)
                            {
                                long tmp = 0;
                                if (Members.TryGetAt(j, ref tmp))
                                {
                                    members.Add(tmp);
                                }
                            }
                            club.Numbers = members;
                        }
                        if (listUser.TryGetAt(i, 11, ref _checkinCount))
                        {
                            club.CheckInCount = _checkinCount;
                        }
                        if (listUser.TryGetAt(i, 10, ref Request))
                        {
                            List<long> request = new List<long>();
                            for (int j = 0; j < Request.Length; j++)
                            {
                                long tmp = 0;
                                if (Request.TryGetAt(j, ref tmp))
                                {
                                    request.Add(tmp);
                                }
                            }
                            club.Requests = request;
                        }
                        else
                        {
                            club.Requests = new List<long>();
                        }
                        if (listUser.TryGetAt(i, 12, ref _long))
                        {

                        }
                        if (listUser.TryGetAt(i, 13, ref _lag))
                        {

                        }
                        club.clubPosition = new Xamarin.Forms.GoogleMaps.Position(
                            _lag * Math.Pow(10, -6),
                             _long * Math.Pow(10, -6));
                        club.CalDistance();
                        // Thêm vào Cached
                        if (Helper.Instance().ListClub.ContainsKey(club.ClubID))
                        {
                            Helper.Instance().ListClub[club.ClubID].address = club.address;
                            Helper.Instance().ListClub[club.ClubID].AdminID = club.AdminID;
                            Helper.Instance().ListClub[club.ClubID].CommuneWardAddress = club.CommuneWardAddress;
                            Helper.Instance().ListClub[club.ClubID].ContentCount = club.ContentCount;
                            Helper.Instance().ListClub[club.ClubID].ContentIDs = club.ContentIDs;
                            Helper.Instance().ListClub[club.ClubID].Description = club.Description;
                            Helper.Instance().ListClub[club.ClubID].Distance = club.Distance;
                            Helper.Instance().ListClub[club.ClubID].ClubName = club.ClubName;
                            Helper.Instance().ListClub[club.ClubID].ProvinceCityAddress = club.ProvinceCityAddress;
                            Helper.Instance().ListClub[club.ClubID].OpenTime = club.OpenTime;
                            Helper.Instance().ListClub[club.ClubID].Relation = club.Relation;
                            Helper.Instance().ListClub[club.ClubID].TextRelation = club.TextRelation;
                        }

                        else Helper.Instance().ListClub.Add(club.ClubID, club);
                        list.Add(Helper.Instance().ListClub[club.ClubID]);
                    }
                }
                return list;
            }
            else
            {
                return null;
            }

        }
        public FlowObservableCollection<Club> ListClub { get; set; }
        public List<SearchLocalObject> ListObjectAcc = new List<SearchLocalObject>();
        public List<SearchLocalObject> ListObjectClub = new List<SearchLocalObject>();

        public SearchLocationVM()
        {
            IsLoadAcc = false;
            IsLoadClub = false;
            IsLoadArea = false;
            IsAreaNull = false;
            IsAccNull = false;
            IsClubNull = false;
            ListClub = new FlowObservableCollection<Club>();
            //danh sách các account tìm kiếm mà người dùng muốn hiển thị
            listAcc = new FlowObservableCollection<Accountlocal>();
            ListClubArea = new FlowObservableCollection<Club>();
            SearchAreaCmd = new DelegateCommand<AutoComplete>(SearchAreaExe);
        }
        /// <summary>
        /// Thực hiện load danh sách club tìm kiếm từ server trả về
        /// </summary>
        /// <param name="msg"></param>
        public void SearchClubAck(QHMessage msg)
        {
            var list = GetListClub(msg);
            if (list != null)
            {
                if (list.Count > 0)
                {
                    isLoading = true;
                    if (Service.Instiance().clubSearchVM.IsMore)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            // ListClubArea.AddRange(list);
                            Service.Instiance().clubSearchVM.ListClub.AddRange(list);
                        });
                        Service.Instiance().clubSearchVM.IsMore = false;
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ListClubArea.Clear();
                            // ListClubArea.AddRange(list);
                            Service.Instiance().clubSearchVM.ListClub.AddRange(list);
                            Service.Instiance().clubSearchVM.ivisibleList = true;
                        });

                    }
                }
                else
                {
                    isLoading = false;
                }
            }
            if (Service.Instiance().clubSearchVM.IsMore)
            {
                Service.Instiance().clubSearchVM.IsMore = false;
            }
            if (IsLoadArea)
                IsLoadArea = false;
            Device.BeginInvokeOnMainThread(() =>
            {
                if (ListClubArea.Count == 0)
                    IsAreaNull = true;
            });

        }
        private bool isLoading = false;
        public async void NextClubPage(Club club)
        {
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Club", club.ClubID);
                await navigationService.NavigateAsync("ClubPage", param);
            }
        }
        private ICommand SearchAreaCmd { get; set; }
        public AutoComplete SelectItemComplete
        {
            get => _selectItemComplete; set
            {
                _selectItemComplete = value;
            }
        }
        /// <summary>
        /// tìm kiếm theo khu vực (theo tên khu vực)
        /// </summary>
        /// <param name="autoComplete"></param>
        public void SearchAreaExe(AutoComplete autoComplete)
        {
            IsAreaNull = false;
            if (IsLoadArea)
                return;
            IsLoadArea = true;
            ListClubArea.Clear();
            SelectItemComplete = autoComplete;
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_SEARCH_REQ);
            //thêm offset cho tìm kiếm
            msg.SetAt((byte)MsgClubSearchReqArg.SearchBy, new QHNumber(1));
            QHVector area = new QHVector();
            area.SetAt(0, new QHNumber(autoComplete.Id_City));
            area.SetAt(1, new QHNumber(autoComplete.Id_District));
            area.SetAt(2, new QHNumber(autoComplete.Id_CommuneWard));
            //thêm dữ liệu vào code area
            msg.SetAt((byte)MsgClubSearchReqArg.Areas, area);
            msg.SetAt((byte)MsgClubSearchReqArg.FromClubID, new QHNumber(0));
            msg.SetAt((byte)MsgClubSearchReqArg.Limit, new QHNumber(10));
            //Debug.WriteLine("string tìm kiếm: " + msg.JSONString());
            Service.Instiance().SendMessage(msg);
        }
        /// <summary>
        /// Thực hiện Load thêm item từ Server
        /// </summary>
        /// <param name="club"></param>
        public void Loading(Club club)
        {
            if (club != null)
            {
                if (ListClubArea.Count == 0 || ListClubArea.Count % 10 != 0)
                    return;
                if (club.Equals(ListClubArea[ListClubArea.Count - 1]) && !IsMore && isLoading)
                {
                    Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                    {
                        IsMore = true;
                        Debug.WriteLine("id cuối cùng club trong danh sách: " + club.ClubID);
                        More(club);
                        return false;
                    });
                }
            }
        }
        public void More(Club club)
        {
            if (SelectItemComplete != null)
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_SEARCH_REQ);
                //thêm offset cho tìm kiếm
                msg.SetAt((byte)MsgClubSearchReqArg.SearchBy, new QHNumber(1));
                QHVector area = new QHVector();
                area.SetAt(0, new QHNumber(SelectItemComplete.Id_City));
                area.SetAt(1, new QHNumber(SelectItemComplete.Id_District));
                area.SetAt(2, new QHNumber(SelectItemComplete.Id_CommuneWard));
                //thêm dữ liệu vào code area
                msg.SetAt((byte)MsgClubSearchReqArg.Areas, area);
                msg.SetAt((byte)MsgClubSearchReqArg.FromClubID, new QHNumber(club.ClubID));
                msg.SetAt((byte)MsgClubSearchReqArg.Limit, new QHNumber(10));

                if (CrossConnectivity.Current.IsConnected)
                {
                    Service.Instiance().SendMessage(msg);
                }
                else
                {
                    UserDialogs.Instance.Toast("Kiểm tra kết nối internet", TimeSpan.FromMilliseconds(1000));
                }
            }

        }
        /// <summary>
        /// thực hiện load danh sách club qua tọa độ
        /// </summary>
        /// <param name="Kind"></param>
        public async void LoadObject(long Kind, long Radius)
        {
            //tìm kiếm theo acc
            if (Kind == 0)
            {
                IsAccNull = false;
                IsLoadAcc = true;
            }
            else
            {
                IsClubNull = false;
                IsLoadClub = true;
            }
            //TextLoadMap = "Tìm kiếm quanh bán kính " + Radius + " m";
            ColorLoad = "Blue";
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
                IsLoadAcc = false;
                IsLoadClub = false;
                return;
            }
            msg.SetAt((byte)MsgScanLocationReqArg.Radius, new QHNumber(Radius));
            Service.Instiance().SendMessage(msg);
        }
        public bool isFirstClub = false;
        /// <summary>
        /// Get Club
        /// </summary>
        /// <param name="msg"></param>
        private void GetClub(QHTable Result)
        {
            long id = 0;
            long kind = 0;
            long log = 0;
            long la = 0;
            long distance = 0;
            ListObjectClub.Clear();
            for (int i = 0; i < Result.GetRowCount(); i++)
            {
                var SearchLocal = new SearchLocalObject();
                if (Result.TryGetAt(i, 0, ref id))
                {
                    SearchLocal.Id = id;
                }
                SearchLocal.kink = kind;
                if (Result.TryGetAt(i, 2, ref distance))
                {
                    SearchLocal.Distance = distance;
                }
                if (Result.TryGetAt(i, 3, ref log))
                {
                    SearchLocal.longitude = log;
                }
                if (Result.TryGetAt(i, 4, ref la))
                {
                    SearchLocal.Latitude = la;
                }
                ListObjectClub.Add(SearchLocal);
            }
            if (ListObjectClub.Count > 0)
            {
                if (ListClub != null && ListClub.Count > 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ListClub.Clear();
                    });

                }
                foreach (var item in ListObjectClub)
                {
                    SendGetDetailClub(item);
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsClubNull = true;

                });
            }

        }
        public void nullResult()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (IsLoadClub)
                    IsLoadClub = false;
                IsClubNull = true;
            });
        }
        /// <summary>
        /// Get Acc
        /// </summary>
        /// <param name="Result"></param>
        private void GetAcc(QHTable Result)
        {
            long id = 0;
            long kind = 0;
            long log = 0;
            long la = 0;
            long distance = 0;
            ListObjectAcc.Clear();
            for (int i = 0; i < Result.GetRowCount(); i++)
            {
                var SearchLocal = new SearchLocalObject();
                if (Result.TryGetAt(i, 0, ref id))
                {
                    SearchLocal.Id = id;
                }
                SearchLocal.kink = kind;
                if (Result.TryGetAt(i, 2, ref distance))
                {
                    SearchLocal.Distance = distance;
                }
                if (Result.TryGetAt(i, 3, ref log))
                {
                    SearchLocal.longitude = log;
                }
                if (Result.TryGetAt(i, 4, ref la))
                {
                    SearchLocal.Latitude = la;
                }
                if (id != Helper.Instance().MyAccount.Number_Id)
                    ListObjectAcc.Add(SearchLocal);
            }
            if (ListObjectAcc.Count > 0)
            {
                if (listAcc != null && listAcc.Count > 0)
                {
                    listAcc.Clear();
                }
                foreach (var item in ListObjectAcc)
                {
                    SendGetDetailAcc(item);
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsAccNull = true;
                });
            }
        }
        /// <summary>
        /// Kết quả tìm kiếm club từ server trả về
        /// </summary>
        /// <param name="msg"></param>
        public void OnMsgScanLocationAc(QHMessage msg)
        {
            var error = (ScanLocationError)(msg.GetAt((byte)MsgScanLocationAckArg.Error) as QHNumber).value;
            switch (error)
            {
                case ScanLocationError.SUCCESS:
                    QHTable Result = new QHTable();
                    if (msg.TryGetAt((byte)MsgScanLocationAckArg.Result, ref Result))
                    {
                        Debug.WriteLine("số lượng club tìm kiếm được: " + Result.GetRowCount());
                        long kind = 0;
                        if (Result.GetRowCount() <= 0)
                        {
                            nullResult();
                            return;
                        }
                        for (int i = 0; i < Result.GetRowCount(); i++)
                        {
                            if (Result.TryGetAt(i, 1, ref kind))
                            {

                            }
                            if (kind > 0)
                            {
                                if (IsLoadClub)
                                    IsLoadClub = false;
                                //  GetClub(Result);
                                Service.Instiance().clubSearchVM.GetClub(Result);
                            }
                            else
                            {
                                if (IsLoadAcc)
                                    IsLoadAcc = false;
                                  Service.Instiance().personalSearchPageVM.GetAcc(Result);
                               // GetAcc(Result);
                            }
                            return;
                        }
                    }
                    else
                    {
                        nullResult();
                    }
                    break;
            }
        }
        public long Radius = 50000;
        private string _textTextLoadMap;
        private string _colorLoad;
        private bool _isLoadAcc;
        private bool _isLoadClub;
        private bool _isLoadArea;
        private bool _isAreaNull;
        private bool _isAccNull;
        private bool _isClubNull;
        /// <summary>
        /// Thực hiện reset lại danh sách club và bán kính tìm kiếm
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Radius = 25000;
            IsLoadAcc = false;
            IsLoadClub = false;
            IsLoadArea = false;
            IsAreaNull = false;
            IsAccNull = false;
            IsClubNull = false;
            if (ListClub != null && ListClub.Count > 0)
                ListClub.Clear();
            if (listAcc != null && listAcc.Count > 0)
                listAcc.Clear();
            if (ListClubArea != null && ListClubArea.Count > 0)
                ListObjectClub.Clear();
        }
        public void SendGetDetailClub(SearchLocalObject obj)
        {
            if (!Helper.Instance().ListClub.TryGetValue(obj.Id, out Club clubContent))
            {
                clubContent = new Club() { ClubID = obj.Id };
                Helper.Instance().ListClub.Add(obj.Id, clubContent);
            }
            // Kiểm tra sự tồn tại của Club
            Helper.Instance().CheckExistClubAsync(obj.Id);
            if (Helper.Instance().ListClub[obj.Id] != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (ChallengeAction.ListClubRecive.ContainsKey(Helper.Instance().ListClub[obj.Id].ClubID))
                    {
                        Helper.Instance().ListClub[obj.Id].Challenge = "pingpong_invi.png";
                    }
                    else
                        Helper.Instance().ListClub[obj.Id].Challenge = "pingpong.png";

                    Helper.Instance().ListClub[obj.Id].clubPosition = new Xamarin.Forms.GoogleMaps.Position(obj.Latitude * Math.Pow(10, -6), obj.longitude * Math.Pow(10, -6));
                    Helper.Instance().ListClub[obj.Id].CalDistance();
                    ListClub.Add(Helper.Instance().ListClub[obj.Id]);
                });
            }

        }

        public void SendGetDetailAcc(SearchLocalObject obj)
        {
            var acccount = new Accountlocal();
            if (obj != null)
            {
                Debug.WriteLine("id search: " + obj.Id);
                if (!Helper.Instance().ListAccounts.ContainsKey(obj.Id.ToString()))
                {
                    var account = new Accountlocal() { Number_Id = obj.Id };
                    Helper.Instance().ListAccounts.Add(obj.Id.ToString(), account);
                }
                var accounOwner = Helper.Instance().ListAccounts[obj.Id.ToString()];
                if (string.IsNullOrEmpty(accounOwner.fullname) || string.IsNullOrEmpty(accounOwner.Avatar_Uri))
                {
                    Helper.Instance().CheckExistAccountAsync(obj.Id);
                }
                acccount.accounts = accounOwner;
                lock (objectLock)
                {
                    if (accounOwner.Number_Id > 0)
                    {
                        if (ChallengeAction.ListAccRecive.ContainsKey(accounOwner.Number_Id))
                        {
                            acccount.Challenge = "pingpong_invi.png";
                        }
                        else
                            acccount.Challenge = "pingpong.png";
                        acccount.Blade = "Cốt A - Hãng A";
                        acccount.Facebat = "Mặt A - Hãng A";
                        acccount.Level = "Hạng A";
                        acccount.AccepLevel = "Đã Duyệt";

                        if (!Helper.Instance().addFriendStatus.ContainsKey(accounOwner.Number_Id))
                        {
                            IconAddFriend iconAddFriend = new IconAddFriend() { id = accounOwner.Number_Id, statusIcon = Helper.Instance().IsFriendImg(accounOwner.Number_Id) };
                            Helper.Instance().addFriendStatus.Add(iconAddFriend.id, iconAddFriend);
                        }
                        else Helper.Instance().addFriendStatus[accounOwner.Number_Id].statusIcon = Helper.Instance().IsFriendImg(accounOwner.Number_Id);
                        IconAddFriend iconAdd = Helper.Instance().addFriendStatus[accounOwner.Number_Id];
                        acccount.iconAddFriend = iconAdd;
                        // acccount.AddFriend = iconAdd.statusIcon;
                        //  acccount.AddFriend = Helper.Instance().IsFriendImg(accounOwner.Number_Id);
                        acccount.TextStatusFriend = Helper.Instance().IsFriend(accounOwner.Number_Id);
                        acccount.TextAcceptFriend = Helper.Instance().TextAcceptFriend;
                        acccount.Number_Id = accounOwner.Number_Id;
                        acccount.fullname = acccount.accounts.fullname;
                        acccount.Avatar_Uri = acccount.accounts.Avatar_Uri;
                        listAcc.Add(acccount);
                        if (Helper.Instance().ListAcclocal.ContainsKey(acccount.Number_Id))
                        {
                            Helper.Instance().ListAcclocal[acccount.Number_Id].Blade = acccount.Blade;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].Facebat = acccount.Facebat;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].Level = acccount.Level;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].AccepLevel = acccount.AccepLevel;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].iconAddFriend.statusIcon = acccount.iconAddFriend.statusIcon;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].TextStatusFriend = acccount.TextStatusFriend;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].TextAcceptFriend = acccount.TextAcceptFriend;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].fullname = acccount.fullname;
                            Helper.Instance().ListAcclocal[acccount.Number_Id].Avatar_Uri = acccount.Avatar_Uri;
                        }
                        else
                        {
                            Helper.Instance().ListAcclocal.Add(acccount.Number_Id, acccount);
                        }
                        acccount.Distance = obj.Distance;
                        acccount.Latitude = obj.Latitude;
                        acccount.Longtitude = obj.longitude;
                    }
                }
                Helper.Instance().CheckExistClubMyClubId(accounOwner.Number_Id);
            }
        }
        public void GetListClubId(List<long> list)
        {
            foreach (var item in list)
            {
                Helper.Instance().CheckExistClubAsync(item);
            }
        }
        public void GetClub(Club club)
        {
            foreach (var accountLocal in Helper.Instance().ListAcclocal)
            {
                if (accountLocal.Value.ClubID == club.ClubID)
                    accountLocal.Value.ClubName = club.ClubName;
            }

        }
        public void NaviListUse(Club club)
        {
            var app = Application.Current as App;
            app.clubCurrent = club;
            if (Helper.Instance().MyAccount != null && Helper.Instance().MyAccount.Number_Id == club.AdminID)
            {
                navigationService.NavigateAsync("ListUserClubPage");
            }
            else
            {
                navigationService.NavigateAsync("ListMemberPage");
            }
        }
    }
}
public class SearchLocalObject
{
    private long _longitude;
    private long _latitude;

    public long Id { get; set; }
    public long kink { get; set; }
    public long Distance { get; set; }
    public long longitude
    {
        get => _longitude; set
        {
            _longitude = value;

        }
    }
    public long Latitude
    {
        get => _latitude; set
        {
            _latitude = value;

        }
    }


}