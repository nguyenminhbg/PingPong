using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.Services;
using AppBongBan.ViewModels.Actions;
using DLToolkit.Forms.Controls;
using PingPong;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;
using static AppBongBan.Services.Service;

namespace AppBongBan.ViewModels.Group
{
    public class ClubPageVM : ViewModelBase
    {
        public INavigationService navigationService { get; set; }
        /// <summary>
        /// câu lạc bộ hiển thị trễ
        /// </summary>
        private Club _myClub;
        public Club MyClub { get => _myClub; set { SetProperty(ref _myClub, value); } }
        /// <summary>
        /// danh sách content để hiển thị  cho club
        /// </summary>
        public FlowObservableCollection<ContentInfo> ListContents { get; set; }
        public ClubPageVM()
        {
            ListContents = new FlowObservableCollection<ContentInfo>();
            MyClub = new Club();
        }
        public void ClubJoinReq(Club club)
        {
            MyClub = club;
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_JOIN_REQ);
            if (MyClub.TextRelation.Equals("Tham gia"))
            {
                if (Helper.Instance().CheckLogin())
                {
                    msg.SetAt((byte)MsgClubJoinReqArg.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
                }
                else
                {
                    NotifiDialog.Initiance().DialogErrorLogin();
                    return;
                }
                if (MyClub.ClubID > 0)
                {
                    msg.SetAt((byte)MsgClubJoinReqArg.ClubID, new QHNumber(MyClub.ClubID));
                }
                else
                {
                    NotifiDialog.Initiance().DialogErrorNotClub();
                    return;
                }
                Debug.WriteLine("Tham gia:" + msg.JSONString());
                if (!Service.Instiance().SendMessage(msg))
                {
                    NotifiDialog.Initiance().DialogErrorInternter();
                }

            }
            else if (MyClub.TextRelation.Equals("Đợi duyệt"))
            {
                NotifiDialog.Initiance().NotifiErrorAccept();
            }
            else
            {
                NotifiDialog.Initiance().NotifiErrorMember();
            }

        }
        /// <summary>
        /// chuyển sang trang chỉnh sửa trang club
        /// </summary>
        public void NextRepairClub()
        {
            var id = Helper.Instance().MyAccount.Number_Id;
            if (id == MyClub.AdminID)
            {
                if (navigationService != null)
                {
                    var param = new NavigationParameters();
                    param.Add("Club", MyClub);
                    navigationService.NavigateAsync("RepairClubPage", param);
                }
            }
            else
            {
                NotifiDialog.Initiance().DialogActionAdmin();
            }
        }
        /// <summary>
        /// sang trang comment
        /// </summary>
        /// <param name="news"></param>
        public void NaviComment(ContentInfo news)
        {
            IsPopup = true;
            if (navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Content", news.Detail.Id);
                navigationService.NavigateAsync("CommentNewsPage", param);
            }

        }
        /// <summary>
        /// Sang trang thêm comment
        /// </summary>
        /// <param name="page"></param>
        public async void NaviAddNews()
        {
            if (MyClub != null && navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Club", MyClub);
                await navigationService.NavigateAsync("AddNewsPage", param);
            }
        }
        /// <summary>
        /// sang trang xem thêm thông tin club
        /// </summary>
        /// <param name="page"></param>
        public void NaviMorepage()
        {
            if (MyClub != null && navigationService != null)
            {
                var param = new NavigationParameters();
                param.Add("Club", MyClub);
                navigationService.NavigateAsync("MoreClubPage", param);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }
        public void NaviListMember()
        {
            if (navigationService != null)
            {
                var app = Application.Current as App;
                app.clubCurrent = MyClub;

                if (Helper.Instance().MyAccount != null && Helper.Instance().MyAccount.Number_Id == MyClub.AdminID)
                {
                    navigationService.NavigateAsync("ListUserClubPage");
                }
                else
                {
                    navigationService.NavigateAsync("ListMemberPage");
                }
            }
        }
        public void BackListAddNews(ContentInfo content)
        {
            if (MyClub != null)
            {
                MyClub.ContentIDs.Insert(0, content.Detail.Id);
                MyClub.ContentCount = MyClub.ContentCount + 1;
                if (content.Detail.ImagesID != null)
                    MyClub.ImageCount += content.Detail.ImagesID.Count;
                if (!Helper.Instance().ListContent.ContainsKey(content.Detail.Id))
                {
                    lock (Helper.Instance().ListContent)
                    {
                        Helper.Instance().ListContent.Add(content.Detail.Id, content);
                    }
                }

                content.Detail.AdminID = MyClub.AdminID;
                content.Detail.Numbers = MyClub.Numbers;
                ListContents.Insert(0, content);
                // Báo lại cho trang chủ biết để Insert lại trang chủ
                Helper.Instance().PostNewsNotify?.Invoke(content);
            }
        }
        public void SendGetDetailClub(Club club, int indexLast, int limited)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_HOMEPAGE_REQ);
            msg.SetAt((byte)MsgClubHomePageReqArg.ClubID, new QHNumber(club.ClubID));
            if (Helper.Instance().CheckLogin())
            {
                msg.SetAt((byte)MsgClubHomePageReqArg.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorLogin();
                return;
            }
            msg.SetAt((byte)MsgClubHomePageReqArg.LastContent, new QHNumber(indexLast));
            msg.SetAt((byte)MsgClubHomePageReqArg.LimitContent, new QHNumber(limited));
            MyClub.clubPosition = club.clubPosition;
            Instiance().SendMessage(msg);
            Debug.WriteLine("Đã gửi tin chi tiết lên server " + msg.JSONString());
        }
        /// <summary>
        /// Phương thức xử lý chi tiết bản tin club
        /// </summary>
        /// <param name="msg"></param>
        public void GetDetailClub(QHMessage msg)
        {
            try
            {
                if (!IsMore)
                {
                    QHTable ClubInfo = new QHTable();
                    if (msg.TryGetAt((byte)MsgClubHomePageAckArg.ClubInfo, ref ClubInfo))
                    {
                        QHVector Members = new QHVector();
                        QHVector Requests = new QHVector();
                        long CheckInCount = 0;
                        long ImageCount = 0;
                        long ContentCount = 0;
                        if (ClubInfo.TryGetAt(0, 5, ref Members))
                        {
                            List<long> members = new List<long>();
                            for (int i = 0; i < Members.Length; i++)
                            {
                                long tmp = 0;
                                if (Members.TryGetAt(i, ref tmp))
                                {
                                    members.Add(tmp);
                                    Debug.WriteLine("Member: " + tmp);
                                }
                            }
                            Debug.WriteLine("Số lượng thành viên: " + members.Count + " id cá nhân " + Helper.Instance().MyAccount.Number_Id);
                            MyClub.Numbers = members;
                        }
                        if (ClubInfo.TryGetAt(0, 6, ref Requests))
                        {
                            List<long> request = new List<long>();

                            for (int i = 0; i < Requests.Length; i++)
                            {
                                long tmp = 0;
                                if (Requests.TryGetAt(i, ref tmp))
                                {
                                    request.Add(tmp);
                                }
                            }
                            Debug.WriteLine("Số lượng request: " + request.Count);
                            MyClub.Requests = request;
                        }
                        else
                        {
                            MyClub.Requests = new List<long>();
                        }
                        if (ClubInfo.TryGetAt(0, 7, ref CheckInCount))
                        {
                            MyClub.CheckInCount = CheckInCount;
                        }
                        if (ClubInfo.TryGetAt(0, 8, ref ImageCount))
                        {
                            MyClub.ImageCount = ImageCount;
                        }
                        if (ClubInfo.TryGetAt(0, 17, ref ContentCount))
                        {
                            MyClub.ContentCount = ContentCount;
                        }
                    }

                }
                QHTable ContentInfo = new QHTable();
                MyClub.ContentIDs.Clear();
                var list = new List<ContentInfo>();
                if (msg.TryGetAt((byte)MsgClubHomePageAckArg.ContentInfo, ref ContentInfo))
                {
                    if (ContentInfo.GetRowCount() != 0)
                    {
                        for (int i = 0; i < ContentInfo.GetRowCount(); i++)
                        {
                            var tmpContent = new ContentInfo();
                            long OwnerID = 0;
                            string Avatar = "";
                            string FullName = "";
                            long ContentID = 0;
                            string Title = "";
                            long Created = 0;
                            string Content = "";
                            long ContentType = 0;
                            QHVector likes = new QHVector();
                            QHVector Comments = new QHVector();
                            if (ContentInfo.TryGetAt(i, 0, ref OwnerID))
                            {
                                tmpContent.Detail.Owner = OwnerID;
                            }
                            if (ContentInfo.TryGetAt(i, 1, ref Avatar))
                            {
                                if (!Avatar.Equals(""))
                                {
                                    tmpContent.Accounts.Avatar_Uri = Avatar;
                                }
                            }
                            if (ContentInfo.TryGetAt(i, 2, ref FullName))
                            {
                                tmpContent.Accounts.fullname = FullName;
                            }
                            if (ContentInfo.TryGetAt(i, 3, ref ContentID))
                            {
                                tmpContent.Detail.Id = ContentID;
                            }
                            if (ContentInfo.TryGetAt(i, 4, ref Title))
                            {
                                tmpContent.Detail.Title = Title;
                            }
                            if (ContentInfo.TryGetAt(i, 5, ref Created))
                            {
                                tmpContent.Detail.Created = Created;
                            }
                            if (ContentInfo.TryGetAt(i, 6, ref Content))
                            {
                                tmpContent.Detail.Content = Content;
                            }
                            if (ContentInfo.TryGetAt(i, 7, ref ContentType))
                            {
                                tmpContent.Detail.Ctype = ContentType;
                            }
                            if (ContentInfo.TryGetAt(i, 8, ref likes))
                            {
                                var idAccount = Helper.Instance().MyAccount.Number_Id;
                                for (int j = 0; j < likes.Length; j++)
                                {
                                    //thêm id của người đang xem content vào danh sách
                                    long id = 0;
                                    if (likes.TryGetAt(j, ref id))
                                    {
                                        tmpContent.LikeContent.LikeContent.Add("" + id, new LikeContent { Content_Id = ContentID, Owner = id });
                                    }
                                }
                                tmpContent.LikeContent.Owner = idAccount;
                                tmpContent.LikeContent.Reset();
                            }
                            if (ContentInfo.TryGetAt(i, 9, ref Comments))
                            {
                                tmpContent.CommentCount = Comments.Length;
                                for (int j = 0; j < Comments.Length; j++)
                                {
                                    long Id = 0;
                                    if (Comments.TryGetAt(i, ref Id))
                                        tmpContent.CommentIDs.Add(Id);
                                }
                            }
                            tmpContent.Detail.AdminID = MyClub.AdminID;
                            tmpContent.Detail.Numbers = MyClub.Numbers;
                            if (Helper.Instance().ListContent.ContainsKey(ContentID))
                                Helper.Instance().ListContent[ContentID] = tmpContent;
                            else
                            {
                                lock (Helper.Instance().ListContent)
                                {
                                    Helper.Instance().ListContent.Add(ContentID, tmpContent);
                                }
                            }

                            MyClub.ContentIDs.Add(ContentID);
                            if (ListContents.IndexOf(Helper.Instance().ListContent[ContentID]) < 0)
                                ListContents.Add(Helper.Instance().ListContent[ContentID]);
                        }
                        Helper.Instance().ListClub[MyClub.ClubID] = MyClub;
                    }
                    else
                    {
                        if (isLoading)
                            isLoading = false;
                    }
                    IsMore = false;
                }
            }
            catch (Exception ex)
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    var currentPage = AppChat.Helpers.Helper.Instiance().GetCurrentPage();
                    currentPage.DisplayAlert("Lỗi Load Content sau đó back", ex.Message, "Ok");
                });
            }
        }
        /// <summary>
        /// Xử lý sự kiện nhận bản tin gửi yêu cầu tham gia thành công
        /// </summary>
        /// <param name="msg"></param>
        public void OnMsgClubJoinAck(QHMessage msg)
        {
            NotifiDialog.Initiance().DialogJoinClub();
            MyClub.Relation = 1;
            MyClub.TextRelation = "Đợi duyệt";
            MyClub.IconRelation = "group_add_invi.png";
        }
        public override void Reset()
        {
            base.Reset();
            MyClub = new Club();
            ListContents = new FlowObservableCollection<ContentInfo>();
            Service.Instiance().ReplyModel.Reset();
            Service.Instiance().CommentModel.Reset();
            navigationService = null;
            IsMore = false;
            isLoading = true;
        }
        /// <summary>
        /// Tham gia vào club ind
        /// </summary>
        /// <param name="msg"></param>
        public void OnMsgClubJoinInd(QHMessage msg)
        {
            var app = Application.Current as App;
            var AccInClub = new AccJoinClub();
            string title = "AppBongBan";
            string notifi = "";
            long NumberId = 0;
            string FullName = "";
            string Avatar = "";
            long ClubId = 0;
            string ClubName = "";
            string ClubCover = "";
            string RequestTime = "";
            if (msg.TryGetAt((byte)MsgClubJoinIndArg.NumberID, ref NumberId))
            {
                AccInClub.IdNumberID = NumberId;
            }
            if (msg.TryGetAt((byte)MsgClubJoinIndArg.FullName, ref FullName))
            {
                notifi = FullName;
                AccInClub.FullName = FullName;
            }
            if (msg.TryGetAt((byte)MsgClubJoinIndArg.Avatar, ref Avatar))
            {
                if (!Avatar.Equals(""))
                {
                    AccInClub.Avatar = Avatar;
                }
                else
                {
                    AccInClub.Avatar = "account.png";
                }
            }
            if (msg.TryGetAt((byte)MsgClubJoinIndArg.ClubID, ref ClubId))
            {
                AccInClub.ClubID = ClubId;
                if (MyClub.ClubID == ClubId)
                {
                    MyClub.Requests.Add(NumberId);
                }
            }
            if (msg.TryGetAt((byte)MsgClubJoinIndArg.ClubName, ref ClubName))
            {
                AccInClub.ClubName = ClubName;
                notifi += " yêu cầu tham gia club " + ClubName;
            }
            if (msg.TryGetAt((byte)MsgClubJoinIndArg.ClubCover, ref ClubCover))
            {
                if (string.IsNullOrEmpty(ClubCover))
                    ClubCover = "CoverImage.jpg";
                AccInClub.ClubCover = ClubCover;
            }
            if (msg.TryGetAt((byte)MsgClubJoinIndArg.RequestTime, ref RequestTime))
            {
                AccInClub.RequestTime = RequestTime;
            }
            // Thêm vào giao diện
            Service.Instiance().listUserReqModel.ListRequest.Add(AccInClub);
            //app.ListAccInClub.Add(AccInClub);
            Device.BeginInvokeOnMainThread(() =>
            {
                DependencyService.Get<ILocalNotificationService>().LocalNotification(title, notifi, (int)Notifi.JointClub, 0);
            });
        }
        /// <summary>
        /// thêm content từ trên server trả về
        /// </summary>
        /// <param name="msg"></param>
        public void OnMsgContentIn(QHMessage msg)
        {
            try
            {
                long NumberID = 0; // QHNumber (ID của người tạo nội dung)
                string Title = "";    // QHString (Title của bài viết)
                string Content = "";  // QHString (JSON{"text":"Gặp nhau lúc 8h","images_id":[uri,uri,uri,uri]}) với images_id là mảng id của các ảnh bài viết muốn POST lên, do đó cài phương thức, query cho phép cấp trước các images_id
                long ContentID = 0;
                long ClubID = 0;   // QHNumber which club_id belong to
                long Created = 0;  // QHNumber
                QHVector ImagesID = new QHVector(); // QHVector<QHNumber> Danh sách id ảnh đã gửi lên
                if (msg.TryGetAt((byte)MsgContentAddIndArg.NumberID, ref NumberID)) { }
                if (msg.TryGetAt((byte)MsgContentAddIndArg.Title, ref Title)) { }
                if (msg.TryGetAt((byte)MsgContentAddIndArg.Content, ref Content)) { }
                if (msg.TryGetAt((byte)MsgContentAddIndArg.ContentID, ref ContentID)) { }
                if (msg.TryGetAt((byte)MsgContentAddIndArg.ClubID, ref ClubID)) { }
                if (msg.TryGetAt((byte)MsgContentAddIndArg.Created, ref Created)) { }
                if (msg.TryGetAt((byte)MsgContentAddIndArg.ImagesID, ref ImagesID)) { }
                var content = new ContentInfo();
                content.Detail.Title = Title;
                content.Detail.Content = Content;
                content.Detail.Created = Created;
                content.Detail.Id = ContentID;
                if (ImagesID.Length > 0)
                {
                    for (int i = 0; i < ImagesID.Length; i++)
                    {
                        long idImg = 0;
                        if (ImagesID.TryGetAt(i, ref idImg))
                            content.Detail.ImagesID.Add(idImg);
                    }
                }
                content.Detail.Owner = NumberID;
                if (!Helper.Instance().ListAccounts.ContainsKey(NumberID.ToString()))
                {
                    var account = new Accounts() { Number_Id = NumberID };
                    Helper.Instance().ListAccounts.Add(NumberID.ToString(), account);
                }
                content.Accounts = Helper.Instance().ListAccounts[NumberID.ToString()];
                if (string.IsNullOrEmpty(content.Accounts.fullname) || string.IsNullOrEmpty(content.Accounts.fullname))
                {
                    Helper.Instance().CheckExistAccountAsync(NumberID);
                }
                if (!Helper.Instance().ListContent.ContainsKey(ContentID))
                {
                    lock (Helper.Instance().ListContent)
                    {
                        Helper.Instance().ListContent.Add(ContentID, content);
                    }
                }
                if (MyClub.ClubID == ClubID)
                {
                    content.Detail.AdminID = MyClub.AdminID;
                    content.Detail.Numbers = MyClub.Numbers;
                    ListContents.Insert(0, content);
                    MyClub.ContentCount += 1;
                    if (content.Detail.ImagesID != null)
                        MyClub.ImageCount += content.Detail.ImagesID.Count;
                    MyClub.ContentIDs.Insert(0, ContentID);
                }
                if (NumberID > 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", content.Accounts.fullname + " đã đăng bài viết vào câu lạc bộ " + MyClub.ClubName, (int)Notifi.AddContent, 0);
                    });
                }
            }
            catch (Exception ex)
            {
                var currentPage = AppChat.Helpers.Helper.Instiance().GetCurrentPage();
                Device.BeginInvokeOnMainThread(() =>
                {
                    currentPage.DisplayAlert("Lỗi MsgContentAddIndArg", ex.Message, "Ok");
                });
            }
        }

        public void SendLike(bool isLike, ContentInfo content)
        {
            LikeAction.SendLike(isLike, ref content);
        }
        public void TapCall()
        {
            if (!MyClub.ClubPhoneNumber.Equals(""))
            {
                var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
                if (PhoneCallTask.CanMakePhoneCall)
                    PhoneCallTask.MakePhoneCall(MyClub.ClubPhoneNumber);
            }
        }
        public async void TapShowMap()
        {
            if (MyClub.clubPosition.Latitude > 0 || MyClub.clubPosition.Longitude > 0)
            {
                var success = await CrossExternalMaps.Current.NavigateTo(MyClub.TextShow, MyClub.clubPosition.Latitude, MyClub.clubPosition.Longitude);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorPosition();
            }
        }
        private bool isLoading;
        public void Loading(ContentInfo content)
        {
            if (MyClub != null)
            {
                if (MyClub.ContentIDs.Count == 0)
                {
                    return;
                }
                if (content.Detail.Id.Equals(MyClub.ContentIDs[MyClub.ContentIDs.Count - 1]) && !IsMore && isLoading)
                {
                    Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                    {
                        IsMore = true;
                        Debug.WriteLine("id cuối cùng content trong danh sách: " + content.Detail.Id);
                        More(content);
                        return false;
                    });
                }
            }

        }
        private void More(ContentInfo content)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_HOMEPAGE_REQ);
            msg.SetAt((byte)MsgClubHomePageReqArg.ClubID, new QHNumber(MyClub.ClubID));
            if (Helper.Instance().CheckLogin())
            {
                msg.SetAt((byte)MsgClubHomePageReqArg.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorLogin();
                return;
            }
            msg.SetAt((byte)MsgClubHomePageReqArg.LastContent, new QHNumber(content.Detail.Id));
            msg.SetAt((byte)MsgClubHomePageReqArg.LimitContent, new QHNumber(-10));
            Debug.WriteLine("msg load more " + msg.JSONString());
            if (!Services.Service.Instiance().SendMessage(msg))
            {
                NotifiDialog.Initiance().DialogErrorInternter();
            }
        }
        public async void NaviUpLoadImage()
        {
            if (MyClub.ClubID > 0)
            {
                var param = new NavigationParameters();
                param.Add("Club", MyClub.ClubID);
                await navigationService.NavigateAsync("UpLoadImgPage", param);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }
        /// <summary>
        /// Thêm checkin cho club
        /// </summary>
        public async void NaviCheckIn()
        {
            if (MyClub.ClubID > 0)
            {
                var param = new NavigationParameters();
                param.Add("Club", MyClub.ClubID);
                await navigationService.NavigateAsync("CheckInPage", param);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }
        public async void NaviCheckIns()
        {
            if (MyClub.ClubID > 0)
            {
                var param = new NavigationParameters();
                param.Add("Club", MyClub.ClubID);
                await navigationService.NavigateAsync("ListCheckInPage", param);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }

    }
}
