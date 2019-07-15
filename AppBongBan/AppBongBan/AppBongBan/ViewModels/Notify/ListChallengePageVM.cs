using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using DLToolkit.Forms.Controls;
using PingPong;
using qhmono;
using System.Diagnostics;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Notify
{
    public class ListChallengePageVM : ViewModelBase
    {
        private string _notify;
        public string notify
        {
            get { return _notify; }
            set
            {
                SetProperty(ref _notify, value);
                if((ListChallengesPer.Count + ListChallengesClub.Count) > 5)
                {
                    _notify = "+5";
                }
                if ((ListChallengesPer.Count + ListChallengesClub.Count) == 0) _notify = null;
            }
        }
        public FlowObservableCollection<ChallengeInfo> ListChallengesPer { get; set; }
        public FlowObservableCollection<ChallengeInfo> ListChallengesClub { get; set; }
        private bool challenge=true;
        public bool Challenge
        {
            get { return challenge; }
            set
            {
                SetProperty(ref challenge, value);
            }
        }
        public ListChallengePageVM()
        {
            ListChallengesPer = new FlowObservableCollection<ChallengeInfo>();
            ListChallengesClub = new FlowObservableCollection<ChallengeInfo>();
        }
        public override void Reset()
        {
            base.Reset();
            ListChallengesPer = new FlowObservableCollection<ChallengeInfo>();
            ListChallengesClub = new FlowObservableCollection<ChallengeInfo>();
            count = 0;
        }
       public int count = 0;
        /// <summary>
        /// nhận indicator thách đấu từ server trả về
        /// </summary>
        /// <param name="msg"></param>
        public async void OnReciveChallengeInd(QHMessage msg)
        {
            long SenderID = 0;   // QHNumber
            long TargetType = 0; // QHNumber{ 0 : Player, 1:Club }
            long TargetID = 0;   // QHNumber ID of TargetType
            string Content = "";    // QHString (Nội dung text hoặc json của Challenges)
            long StartTime = 0;  // QHNumber Thời gian thách đấu bắt đầu
            long EndTime = 0;  // QHNumber Thời gian thách đấu kết thúc
            long ChallengeID = 0;// QHNumber ID của lời mời thách đấu
            if (msg.TryGetAt((byte)MsgInviteChallengeIndArg.SenderID, ref SenderID)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeIndArg.TargetType, ref TargetType)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeIndArg.TargetID, ref TargetID)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeIndArg.Content, ref Content)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeIndArg.StartTime, ref StartTime)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeIndArg.EndTime, ref EndTime)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeIndArg.ChallengeID, ref ChallengeID)) { }
            var Chall = new ChallengeInfo();
            Chall.SenderID = SenderID;
            Chall.TargetType = TargetType;
            Chall.TargetID = TargetID;
            Chall.Content = Content;
            Chall.StartTime = StartTime;
            Chall.EndTime = EndTime;
            Chall.ChallengeID = ChallengeID;
            Accountlocal AccLocal = new Accountlocal();
            if (Helper.Instance().ListAcclocal.ContainsKey(SenderID))
            {
                AccLocal = Helpers.Helper.Instance().ListAcclocal[SenderID];
            }
            else
            {
                var accounOwner = await Helper.Instance().CheckExistAccount(SenderID);
                if (accounOwner != null)
                {
                    var acccount = new Accountlocal();
                    acccount.Challenge = "pingpong.png";
                    acccount.Blade = "Cốt A - Hãng A";
                    acccount.Facebat = "Mặt A - Hãng A";
                    acccount.Level = "Hạng A";
                    acccount.AccepLevel = "Đã Duyệt";
                    acccount.AddFriend = Helper.Instance().IsFriendImg(accounOwner.Number_Id);
                    acccount.TextStatusFriend = Helper.Instance().IsFriend(accounOwner.Number_Id);
                    acccount.TextAcceptFriend = Helper.Instance().TextAcceptFriend;
                    acccount.Number_Id = accounOwner.Number_Id;
                    acccount.fullname = accounOwner.fullname;
                    acccount.Avatar_Uri = accounOwner.Avatar_Uri;
                    Helper.Instance().ListAcclocal.Add(SenderID, acccount);
                    AccLocal = acccount;
                }

            }
            // Thách đấu cá nhân
            if (TargetType == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", AccLocal.fullname + " đã gửi lời mời thách đấu đến bạn", (int)Helpers.Notifi.Challenge, 0);
                });
                Chall.Acc = AccLocal;
                if (!ChallengeAction.ListSendAcc.ContainsKey(SenderID))
                {
                    ChallengeAction.ListSendAcc.Add(SenderID, Chall);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    ListChallengesPer.Add(Chall);
                    count += ListChallengesPer.Count;
                    notify = count.ToString();
                  //  Helper.Instance().challengeNotify?.Invoke();
                });
            }
            // Thách đấu Club
            else
            {
                var club = await Helper.Instance().CheckExistClub(TargetID);
                Chall.Clubs = club;
                Chall.Acc = AccLocal;
                Device.BeginInvokeOnMainThread(() =>
                {
                    DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", AccLocal.fullname + " đã gửi lời mời thách đấu đến " + club.ClubName, (int)Helpers.Notifi.Challenge, 0);
                });
                if (!ChallengeAction.ListSendClub.ContainsKey(SenderID))
                {
                    ChallengeAction.ListSendClub.Add(SenderID, Chall);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    ListChallengesClub.Add(Chall);
                    count += ListChallengesClub.Count;
                    notify = count.ToString();
                });
            }

            // Thông báo để thêm vào chuông cho người dùng biết
            Helper.Instance().CountChallengeNotifi++;
            //Helper.Instance().challengeNotify?.Invoke();
            AppChat.Helpers.Helper.Instiance().NotifiAction?.Invoke();
        }
        
        /// <summary>
        /// Nhận danh sách các thách đấu từ server trả về liên quan đến người dùng và quản lý Club
        /// </summary>
        /// <param name="msg"></param>
        public async void OnReciveListChallenge(QHMessage msg)
        {
            long Error = 0;                          // QHNumber
            QHTable ClubChallenges = new QHTable();  // QHTable{ChallengeID(0):QHNumber, SenderID(1):QHNumber, TargetID(2):QHNumber, Content(3):QHString, CreateTime(4):QHNumber,StartTime(5):QHNumber,EndTime(6):QHNumber}
            QHTable PlayerChallenges = new QHTable(); // QHTable{ChallengeID(0):QHNumber, SenderID(1):QHNumber, TargetID(2):QHNumber, Content(3):QHString, CreateTime(4):QHNumber,StartTime(5):QHNumber,EndTime(6):QHNumber}
            if (msg.TryGetAt((byte)MsgListChallengeAckArg.Error, ref Error)) { }
            if (msg.TryGetAt((byte)MsgListChallengeAckArg.ClubChallenges, ref ClubChallenges)) { }
            if (msg.TryGetAt((byte)MsgListChallengeAckArg.PlayerChallenges, ref PlayerChallenges)) { }
            var acc = Helper.Instance().MyAccount;
            //Debug.WriteLine("row: "+ ClubChallenges.GetRowCount());
            //Debug.WriteLine("row: " + PlayerChallenges.GetRowCount());
            if (Error == 0)
            {
                var rowCount = ClubChallenges.GetRowCount();
                var columCount = ClubChallenges.GetColumnCount();
                //danh sách các thách đấu mà admin nhận được hoặc từ các 
                for (int i = 0; i < rowCount; i++)
                {
                   long ChallengeID = 0;
                    long SenderID = 0;
                    long TargetID = 0;
                    string Content = "";
                    long CreateTime = 0;
                    long StartTime = 0;
                    long EndTime = 0;

                    if (ClubChallenges.TryGetAt(i, 0, ref ChallengeID)) { }
                    if (ClubChallenges.TryGetAt(i, 1, ref SenderID)) { }
                    if (ClubChallenges.TryGetAt(i, 2, ref TargetID)) { }
                    if (ClubChallenges.TryGetAt(i, 3, ref Content)) { }
                    if (ClubChallenges.TryGetAt(i, 4, ref CreateTime)) { }
                    if (ClubChallenges.TryGetAt(i, 5, ref StartTime)) { }
                    if (ClubChallenges.TryGetAt(i, 6, ref EndTime)) { }

                    var Chall = new ChallengeInfo();
                    Chall.ChallengeID = ChallengeID;
                    Chall.SenderID = SenderID;
                    Chall.TargetID = TargetID;
                    Chall.Content = Content;
                    Chall.CreateTime = CreateTime;
                    Chall.StartTime = StartTime;
                    Chall.EndTime = EndTime;
                    if(!Helper.Instance().ListClub.TryGetValue(TargetID,out Club clubContent))
                    {
                        clubContent = new Club() { ClubID = TargetID };
                        Helper.Instance().ListClub.Add(TargetID, clubContent);
                    }

                    var club = Helper.Instance().ListClub[TargetID];
                    //  Lấy thông tin club
                    Helper.Instance().CheckExistClubAsync(TargetID);
                    Chall.Clubs = club;
                    if (acc.Number_Id == SenderID)
                    {
                        if (!ChallengeAction.ListClubRecive.ContainsKey(TargetID))
                        {
                            ChallengeAction.ListClubRecive.Add(TargetID, Chall);
                        }
                    }
                    else
                    {
                        if (!ChallengeAction.ListSendClub.ContainsKey(SenderID))
                        {
                            ChallengeAction.ListSendClub.Add(SenderID, Chall);
                            if (Helpers.Helper.Instance().ListAcclocal.ContainsKey(SenderID))
                            {
                                var AccLocal = Helpers.Helper.Instance().ListAcclocal[SenderID];
                                Chall.Acc = AccLocal;
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    ListChallengesClub.Add(Chall);
                                    // Cập nhật lại số lượng thông báo cho chuông
                                    Helper.Instance().CountChallengeNotifi++;
                                    count += ListChallengesClub.Count;
                                    notify = count.ToString();
                                    // Thông báo để thêm vào chuông cho người dùng biết
                                    // Helper.Instance().challengeNotify?.Invoke();
                                    AppChat.Helpers.Helper.Instiance().NotifiAction?.Invoke();
                                });
                            }
                            else
                            {
                                var accounOwner = await Helpers.Helper.Instance().CheckExistAccount(SenderID);
                                if (accounOwner != null)
                                {
                                    var acccount = new Accountlocal();
                                    acccount.Challenge = "pingpong.png";
                                    acccount.Blade = "Cốt A - Hãng A";
                                    acccount.Facebat = "Mặt A - Hãng A";
                                    acccount.Level = "Hạng A";
                                    acccount.AccepLevel = "Đã Duyệt";
                                    acccount.AddFriend = Helper.Instance().IsFriendImg(accounOwner.Number_Id);
                                    acccount.TextStatusFriend = Helper.Instance().IsFriend(accounOwner.Number_Id);
                                    acccount.TextAcceptFriend = Helper.Instance().TextAcceptFriend;
                                    acccount.Number_Id = accounOwner.Number_Id;
                                    acccount.fullname = accounOwner.fullname;
                                    acccount.Avatar_Uri = accounOwner.Avatar_Uri;
                                    Chall.Acc = acccount;
                                    Helpers.Helper.Instance().ListAcclocal.Add(acccount.Number_Id, acccount);
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        ListChallengesClub.Add(Chall);
                                        // Cập nhật lại số lượng thông báo cho chuông
                                        Helper.Instance().CountChallengeNotifi++;
                                        count += ListChallengesClub.Count;
                                        notify = count.ToString();
                                        // Thông báo để thêm vào chuông cho người dùng biết
                                        // Helper.Instance().challengeNotify?.Invoke();
                                        AppChat.Helpers.Helper.Instiance().NotifiAction?.Invoke();
                                    });
                                }
                            }
                        }

                    }
                }
                //danh sách các thách đấu mà người dùng gửi lên hoặc người dùng nhận được
                for (int i = 0; i < PlayerChallenges.GetRowCount(); i++)
                {
                    //Debug.WriteLine("Colum: " + PlayerChallenges.GetColumnCount());
                    long ChallengeID = 0;
                    long SenderID = 0;
                    long TargetID = 0;
                    string Content = "";
                    long CreateTime = 0;
                    long StartTime = 0;
                    long EndTime = 0;

                    if (PlayerChallenges.TryGetAt(i, 0, ref ChallengeID)) { }
                    if (PlayerChallenges.TryGetAt(i, 1, ref SenderID)) { }
                    if (PlayerChallenges.TryGetAt(i, 2, ref TargetID)) { }
                    if (PlayerChallenges.TryGetAt(i, 3, ref Content)) { }
                    if (PlayerChallenges.TryGetAt(i, 4, ref CreateTime)) { }
                    if (PlayerChallenges.TryGetAt(i, 5, ref StartTime)) { }
                    if (PlayerChallenges.TryGetAt(i, 6, ref EndTime)) { }

                    var Chall = new ChallengeInfo();
                    Chall.ChallengeID = ChallengeID;
                    Chall.SenderID = SenderID;
                    Chall.TargetID = TargetID;
                    Chall.Content = Content;
                    Chall.CreateTime = CreateTime;
                    Chall.StartTime = StartTime;
                    Chall.EndTime = EndTime;

                    //var club = await Helpers.Helper.Instance().CheckExistClub(TargetID);
                    //Chall.Clubs = club;
                    if (acc == null) return;
                    if (acc.Number_Id == SenderID)
                    {
                        Debug.WriteLine("Tra ve true");
                        Debug.WriteLine("Number_Id: " + acc.Number_Id);
                        Debug.WriteLine("SenderID: " + SenderID);
                        if (!ChallengeAction.ListAccRecive.ContainsKey(TargetID))
                        {
                            ChallengeAction.ListAccRecive.Add(TargetID, Chall);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Tra ve false");
                        if (!ChallengeAction.ListSendAcc.ContainsKey(SenderID))
                        {
                            ChallengeAction.ListSendAcc.Add(SenderID, Chall);
                            if (Helper.Instance().ListAcclocal.ContainsKey(SenderID))
                            {
                                var AccLocal = Helper.Instance().ListAcclocal[SenderID];
                                Chall.Acc = AccLocal;
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    ListChallengesPer.Add(Chall);
                                    // Cập nhật lại số lượng thông báo cho chuông
                                    Helper.Instance().CountChallengeNotifi++;
                                    count += ListChallengesPer.Count;
                                    notify = count.ToString();
                                    // Thông báo để thêm vào chuông cho người dùng biết
                                    // Helper.Instance().challengeNotify?.Invoke();
                                    AppChat.Helpers.Helper.Instiance().NotifiAction?.Invoke();
                                });
                            }
                            else
                            {
                                var accounOwner = await Helper.Instance().CheckExistAccount(SenderID);
                                if (accounOwner != null)
                                {
                                    var acccount = new Accountlocal();
                                    acccount.Challenge = "pingpong.png";
                                    acccount.Blade = "Cốt A - Hãng A";
                                    acccount.Facebat = "Mặt A - Hãng A";
                                    acccount.Level = "Hạng A";
                                    acccount.AccepLevel = "Đã Duyệt";
                                    acccount.AddFriend = Helper.Instance().IsFriendImg(accounOwner.Number_Id);
                                    acccount.TextStatusFriend = Helper.Instance().IsFriend(accounOwner.Number_Id);
                                    acccount.TextAcceptFriend = Helper.Instance().TextAcceptFriend;
                                    acccount.Number_Id = accounOwner.Number_Id;
                                    acccount.fullname = accounOwner.fullname;
                                    Debug.WriteLine("Name: " + accounOwner.fullname);
                                    Debug.WriteLine("Avatar: " + accounOwner.Avatar_Uri);
                                    acccount.Avatar_Uri = accounOwner.Avatar_Uri;
                                    Chall.Acc = acccount;

                                    Helper.Instance().ListAcclocal.Add(acccount.Number_Id, acccount);
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        ListChallengesPer.Add(Chall);
                                        count += ListChallengesPer.Count;
                                        notify = count.ToString();
                                        // Cập nhật lại số lượng thông báo cho chuông
                                        Helper.Instance().CountChallengeNotifi++;
                                        // Thông báo để thêm vào chuông cho người dùng biết
                                        //Helper.Instance().challengeNotify?.Invoke();
                                        AppChat.Helpers.Helper.Instiance().NotifiAction?.Invoke();
                                    });
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
