using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using Prism.Navigation;
using qhmono;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class PersonalSearchPageVM : ViewModelBase
    {
        public static readonly object balanceLock = new object();
        private bool isFreshing;
        public bool IsFreshing
        {
            get => isFreshing;
            set { SetProperty(ref isFreshing,value); }
        }
        public INavigationService navigationService { get; set; }
        private bool _isAccNull;
        public bool IsAccNull { get => _isAccNull; set { SetProperty(ref _isAccNull, value); } }
        private List<SearchLocalObject> _personalList;
        public List<SearchLocalObject> personalList
        {
            get => _personalList;
            set
            {
                SetProperty(ref _personalList, value);
            }
        }
        private string _title;
        public string title
        {
            get => _title;
            set { SetProperty(ref _title, value); }
        }
        public long Radius = 50000;
        private ObservableCollection<Accountlocal> _listPerson;
        public ObservableCollection<Accountlocal> listPerson
        {
            get => _listPerson;
            set
            {
                SetProperty(ref _listPerson, value);
            }
        }
        public PersonalSearchPageVM()
        {

        }
        public override void Reset()
        {
            listPerson = new ObservableCollection<Accountlocal>() ;
            personalList = new List<SearchLocalObject>();
            Radius = 25000;
        }
        /// <summary>
        /// Phương thức lấy chi tiết Account
        /// </summary>
        /// <param name="obj"></param>
        public void SendGetDetailAcc(SearchLocalObject obj)
        {
            var acccount = new Accountlocal();
            if (obj != null)
            {
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
                    // Thiếu thông tin để hiển thị clubName
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
                    lock (balanceLock)
                    {
                        listPerson.Add(Helper.Instance().ListAcclocal[acccount.Number_Id]);
                    }

                    acccount.Distance = obj.Distance;
                    acccount.Latitude = obj.Latitude;
                    acccount.Longtitude = obj.longitude;
                }
                Helper.Instance().CheckExistClubMyClubId(accounOwner.Number_Id);
            }
        }
        public void GetAcc(QHTable Result)
        {
            long id = 0;
            long kind = 0;
            long log = 0;
            long la = 0;
            long distance = 0;
            personalList.Clear();
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
                    personalList.Add(SearchLocal);
            }
            if (personalList.Count > 0)
            {
                lock (balanceLock)
                {
                    if (listPerson != null && listPerson.Count > 0)
                        listPerson.Clear();
                }

                foreach (var item in personalList)
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
        public async void NextClubPage(Club club)
        {
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Club", club.ClubID);
                await navigationService.NavigateAsync("ClubPage", param);
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
            await navigationService.NavigateAsync("DetailPersonPage", param);
        }
    }
}
