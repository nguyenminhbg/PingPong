using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models.Db;
using AppBongBan.Models.PingPongs;
using AppBongBan.Services;
using AppBongBan.ViewModels.Actions;
using DLToolkit.Forms.Controls;
using PingPong;
using Plugin.Connectivity;
using Prism.Navigation;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class ClubSearchVM : ViewModelBase
    {
        public INavigationService navigationService { get; set; }
        private bool _isFreshing;
        public bool isFreshing
        {
            get => _isFreshing;
            set
            {
                SetProperty(ref _isFreshing, value);
            }
        }
        private bool _suggesIsVisible;
        public bool suggesIsVisible
        {
            get => _suggesIsVisible;
            set { SetProperty(ref _suggesIsVisible, value); }
        }
        private bool _ivisibleList;
        public bool ivisibleList
        {
            get => _ivisibleList;
            set { SetProperty(ref _ivisibleList, value); }
        }
        private bool _isClubNull;
        public bool IsClubNull { get => _isClubNull; set { SetProperty(ref _isClubNull, value); } }
        public FlowObservableCollection<Club> ListClub { get; set; }
        public List<SearchLocalObject> ListObjectClub = new List<SearchLocalObject>();
        public AutoComplete SelectItemComplete
        {
            get => _selectItemComplete; set
            {
                _selectItemComplete = value;
            }
        }
        private AutoComplete _selectItemComplete;
        public ClubSearchVM()
        {
            ListClub = new FlowObservableCollection<Club>();
            IsClubNull = false;
            suggesIsVisible = false;
        }
        public override void Reset()
        {
            ListClub = new FlowObservableCollection<Club>();
            IsClubNull = false;
            suggesIsVisible = false;
            IsMore = false;
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
                           IsClubNull=true;
                            return;
                        }
                        for (int i = 0; i < Result.GetRowCount(); i++)
                        {
                            if (Result.TryGetAt(i, 1, ref kind))
                            {

                            }
                            if (kind > 0)
                            {
                                GetClub(Result);
                            }
                            else
                            {
                                Service.Instiance().personalSearchPageVM.GetAcc(Result);

                            }
                            return;
                        }
                    }
                    else
                    {
                        IsClubNull = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Xử lý dữ liệu club nhận được từ Server trả về
        /// </summary>
        /// <param name="Result"></param>
        public void GetClub(QHTable Result)
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
                suggesIsVisible = false;
                IsClubNull = false;
                if (ListClub != null && ListClub.Count > 0 && !IsMore)
                {
                    ListClub.Clear();
                }
                else { IsMore = false; }
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
                    suggesIsVisible = false;
                });
            }
        }
        /// <summary>
        /// Gửi yêu cầu lấy chi tiết club
        /// </summary>
        /// <param name="obj"></param>
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
                if (ChallengeAction.ListClubRecive.ContainsKey(Helper.Instance().ListClub[obj.Id].ClubID))
                {
                    Helper.Instance().ListClub[obj.Id].Challenge = "pingpong_invi.png";
                }
                else
                    Helper.Instance().ListClub[obj.Id].Challenge = "pingpong.png";

                Helper.Instance().ListClub[obj.Id].clubPosition = new Xamarin.Forms.GoogleMaps.Position(obj.Latitude * Math.Pow(10, -6), obj.longitude * Math.Pow(10, -6));
                Helper.Instance().ListClub[obj.Id].CalDistance();
                ListClub.Add(Helper.Instance().ListClub[obj.Id]);
            }
        }
        private bool isLoading = false;
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
                            ListClub.Clear();
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
            //if (IsLoadArea)
            //    IsLoadArea = false;
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    if (ListClubArea.Count == 0)
            //        IsAreaNull = true;
            //});

        }
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
        /// <summary>
        /// tìm kiếm theo khu vực (theo tên khu vực)
        /// </summary>
        /// <param name="autoComplete"></param>
        public void SearchAreaExe(AutoComplete autoComplete)
        {
            IsMore = false;
            SelectItemComplete = autoComplete;
            ListClub.Clear();
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

        public async void NextClubPage(Club club)
        {
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Club", club.ClubID);
                await navigationService.NavigateAsync("ClubPage", param);
            }
        }
        /// <summary>
        /// Thực hiện Load thêm item từ Server
        /// </summary>
        /// <param name="club"></param>
        public void Loading(Club club)
        {
            if (club != null)
            {
                if (ListClub.Count == 0 || ListClub.Count % 10 != 0)
                    return;
                if (club.Equals(ListClub[ListClub.Count - 1]) && !IsMore)
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
    }
}
