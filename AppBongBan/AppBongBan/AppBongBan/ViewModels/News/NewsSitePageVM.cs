
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.News;
using AppBongBan.Models.PingPongs;
using AppBongBan.Views.News;
using DLToolkit.Forms.Controls;
using PingPong;
using Plugin.Connectivity;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.News
{
    public class NewsSitePageVM : ViewModelBase
    {
        private FlowObservableCollection<ContentInfo> _listContent;
        private bool _isNewsNull;
        private bool _isFreshing;
        private HomePageInfo _homePageInfo;
        public HomePageInfo homePageInfo
        {
            get { return _homePageInfo; }
            set { SetProperty(ref _homePageInfo, value); }
        }
        public bool IsNewsNull { get => _isNewsNull; set { SetProperty(ref _isNewsNull, value); } }
        public bool IsFreshing { get => _isFreshing; set { SetProperty(ref _isFreshing, value); } }
        public FlowObservableCollection<ContentInfo> ListNews
        {
            get => _listContent; set { SetProperty(ref _listContent, value); }
        }
        public List<ContentInfo> ListNewsTemp = new List<ContentInfo>();
        public bool moreLoad = false;
        public NewsSitePageVM()
        {
            IsNewsNull = false;
            IsFreshing = false;
            ListNews = new FlowObservableCollection<ContentInfo>();
            // Đăng ký sự kiện khi người dùng Post bài
        }
        public override void Reset()
        {
            base.Reset();
            IsNewsNull = false;
            IsLoading = false;
            IsFreshing = false;
            ListNews = new FlowObservableCollection<ContentInfo>();
        }
        public void GetContentInfo()
        {
            foreach (var item in ListNewsTemp)
            {
                try
                {
                    if (ListNews.IndexOf(item) < 0)
                        ListNews.Add(item);
                }
                catch (Exception) { }
            }
        }
        /// <summary>
        /// Phương thức lấy bản tin HomePageId
        /// </summary>
        public void MsgHomePageReqSend()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                IsFreshing = false;
                if (Helper.Instance().GetContactGroupChat)
                {
                    AppChat.Services.Service.Instiance().chatHomePageViewModel.SendContactGroupContactPendSync();
                    Helper.Instance().GetContactGroupChat = false;
                }
            }
            else
            {
                if (homePageInfo != null)
                    HomePageGetContentReq(-1, -10);
                else
                {
                    QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_HOMEPAGE_INFO_REQ);
                    msg.SetAt((byte)MsgHomePageInfoReq.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
                    msg.SetAt((byte)MsgHomePageInfoReq.LastContentID, new QHNumber(0));
                    Services.Service.Instiance().SendMessage(msg);
                }
            }
        }

        public void HomePageGetContentReq(long FromContentID, long Offset)
        {
            Task.Run(() =>
            {
                IsNewsNull = false;
                // Gửi yêu cầu đăng bài
                if (Helper.Instance().MyAccount == null || homePageInfo == null) return;
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_HOMEPAGE_LIST_CONTENT_REQ);
                msg.SetAt((byte)MsgHomePageListContentsReq.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
                msg.SetAt((byte)MsgHomePageListContentsReq.FromContentID, new QHNumber(FromContentID));
                msg.SetAt((byte)MsgHomePageListContentsReq.Offset, new QHNumber(Offset));
                msg.SetAt((byte)MsgHomePageListContentsReq.HomePageID, new QHNumber(homePageInfo.homePageId));
                Services.Service.Instiance().SendMessage(msg);
                Debug.WriteLine("MSG_HOMEPAGE_LIST_CONTENT_REQ: " + msg.JSONString());
            });
        }
        /// <summary>
        /// Bản tin xử lý khi nhận được bản tin MsgHomePageInfoAck
        /// </summary>
        /// <param name="msg"></param>
        public void OnRecHomePageInfo(QHMessage msg)
        {
            Debug.WriteLine("MsgHomePageInfoAck: " + msg.JSONString());
            long errorCode = 0;
            long homePageId = 0;
            long ownerId = 0;
            string coverUrl = "";
            long createDate = 0;
            QHVector unreadContents = null;
            if (msg.TryGetAt((byte)MsgHomePageInfoAck.Error, ref errorCode)) { }
            if (errorCode == 0)
            {
                msg.TryGetAt((byte)MsgHomePageInfoAck.HomePageID, ref homePageId);
                msg.TryGetAt((byte)MsgHomePageInfoAck.OwnerID, ref ownerId);
                msg.TryGetAt((byte)MsgHomePageInfoAck.Cover, ref coverUrl);
                msg.TryGetAt((byte)MsgHomePageInfoAck.CreateDate, ref createDate);
                msg.TryGetAt((byte)MsgHomePageInfoAck.UnreadContents, ref unreadContents);
                homePageInfo = new HomePageInfo()
                {
                    homePageId = homePageId,
                    ownerId = ownerId,
                    coverUrl = coverUrl,
                    createDate = createDate,
                    unreadContents = unreadContents
                };
                // Gửi bản tin lấy thông tin HomePage
                HomePageGetContentReq(-1, -10);
            }
            else
            {
                if (Helper.Instance().GetContactGroupChat)
                {
                    AppChat.Services.Service.Instiance().chatHomePageViewModel.SendContactGroupContactPendSync();
                    Helper.Instance().GetContactGroupChat = false;
                }
            }
        }
        /// <summary>
        /// Phương thức xử lý Lấy danh sách Content trong hompage
        /// </summary>
        /// <param name="msg"></param>
        public void OnRecMsgHomePageGetContentsAck(QHMessage msg)
        {
            long errorCode = 0;
            msg.TryGetAt((byte)MsgHomePageListContentsAck.Error, ref errorCode);
            if (errorCode == 0)
                GetHomePageListContent(msg);
            if (errorCode == -1)
            {
                if (Helper.Instance().GetContactGroupChat)
                {
                    AppChat.Services.Service.Instiance().chatHomePageViewModel.SendContactGroupContactPendSync();
                    Helper.Instance().GetContactGroupChat = false;
                }
            }
            if (errorCode == -2)
            {
                if (Helper.Instance().GetContactGroupChat)
                {
                    AppChat.Services.Service.Instiance().chatHomePageViewModel.SendContactGroupContactPendSync();
                    Helper.Instance().GetContactGroupChat = false;
                }
            }
        }

        public async void OnReciveListContent(QHMessage msg)
        {
            Debug.WriteLine("trả về: " + msg.JSONString());
            var list = await GetListContent(msg);
            if (list != null && list.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ListNews.Clear();
                    foreach (var item in list)
                    {
                        ListNews.Add(item);
                    }
                });

            }
            Device.BeginInvokeOnMainThread(() =>
            {
                if (ListNews.Count == 0)
                    IsNewsNull = true;
            });
        }

        // Danh sách các bài viết từ Server trả về
        private Task<List<ContentInfo>> GetListContent(QHMessage msg)
        {
            Debug.WriteLine("GetListContentId {0}", Thread.CurrentThread.ManagedThreadId);
            long Error = 0;    // QHNumber
            QHTable Contents = new QHTable(); /* QHTable { 
	        ContentID(0) : QHNumber
	        OwnerID(1)   : QHNumber người tạo nội dung, muốn lấy thông tin về user này thì gửi PingPongMsg.MSG_PROFILE_INFO_REQ và ChatMessage.MSG_PROFILE_INFO_REQ
	        ClubId(2)    : QHNumber
	    }*/
            var list = new List<ContentInfo>();
            if (msg.TryGetAt((byte)MsgListUserPostContentsAckArg.Error, ref Error)) { }
            if (msg.TryGetAt((byte)MsgListUserPostContentsAckArg.Contents, ref Contents)) { }
            if (Error == 0)
            {
                for (int i = 0; i < Contents.GetRowCount(); i++)
                {
                    long ContentID = 0;
                    long OwnerID = 0;
                    long ClubID = 0;
                    if (Contents.TryGetAt(i, 0, ref ContentID)) { }
                    if (Contents.TryGetAt(i, 1, ref OwnerID)) { }
                    if (Contents.TryGetAt(i, 2, ref ClubID)) { }
                    Helper.Instance().ListContentClub[ContentID] = ClubID;
                    // Kiểm tra xem ContentID có chứa trong cache không và lấy ra content theo contentID
                    if (!Helper.Instance().ListContent.TryGetValue(ContentID, out ContentInfo contentInfo))
                    {
                        contentInfo = new ContentInfo();
                        contentInfo.Detail.Id = ContentID;
                        lock (Helper.Instance().ListContent)
                        {
                            Helper.Instance().ListContent.Add(ContentID, contentInfo);
                        }
                    }
                    // Check sự tồn tại của content
                    Helper.Instance().CheckExistContentAsync(ContentID);
                    var Content = Helper.Instance().ListContent[ContentID];
                    Debug.WriteLine("CurrentId {0}", Thread.CurrentThread.ManagedThreadId);

                    if (Content != null)
                    {
                        if (ClubID > 0)
                        {
                            if (!Helper.Instance().ListClub.TryGetValue(ClubID, out Club clubContent))
                            {
                                clubContent = new Club() { ClubID = ClubID };
                                Helper.Instance().ListClub.Add(ClubID, clubContent);
                            }
                            // Kiểm tra sự tồn tại của Club
                            Helper.Instance().CheckExistClubAsync(ClubID);
                            Content.clubContent = Helper.Instance().ListClub[ClubID];
                        }
                    }
                    list.Add(Content);
                }
            }
            return Task.FromResult(list);
        }

        /// <summary>
        /// Lấy danh sách bài đăng về trang chủ cá nhân
        /// </summary>
        /// <param name="msg"></param>

        public void GetHomePageListContent(QHMessage msg)
        {
            Task.Run(() =>
            {

                QHTable Contents = new QHTable();
                msg.TryGetAt((byte)MsgHomePageListContentsAck.Contents, ref Contents);
                var countRow = Contents.GetRowCount();
                if (countRow < 10) moreLoad = true;
                for (int i = 0; i < Contents.GetRowCount(); ++i)
                {
                    ContentInfo contentInfo = new ContentInfo();
                    long ContentID = 0;
                    long OwnerID = 0;
                    long ClubID = 0;
                    string Content = "";
                    long CreateDate = 0;
                    long HomePageId = 0;
                    QHVector Likes = null;
                    QHVector Comments = null;
                    string Title = "";

                    if (Contents.TryGetAt(i, 0, ref ContentID))
                    {
                        contentInfo.Detail.Id = ContentID;
                    }
                    if (Contents.TryGetAt(i, 1, ref OwnerID))
                    {
                        contentInfo.Detail.Owner = OwnerID;
                        //  kiểm tra sự tồn tại của Account trong cached
                        if (!Helper.Instance().ListAccounts.ContainsKey(OwnerID.ToString()))
                        {
                            Accounts acc = new Accounts() { Number_Id = OwnerID };
                            Helper.Instance().ListAccounts.Add(OwnerID.ToString(), acc);
                        }
                        contentInfo.Accounts = Helper.Instance().ListAccounts[OwnerID.ToString()];
                    }
                    if (Contents.TryGetAt(i, 8, ref ClubID))
                    {
                        if (ClubID > 0) contentInfo.IsNews = true;
                        else contentInfo.IsNews = false;
                        contentInfo.ClubID = ClubID;
                        // Kiểm tra sự tồn tại của Club trong cached theo Id mà Server trả về
                        if (!Helper.Instance().ListClub.TryGetValue(ClubID, out Club club))
                        {
                            club = new Club() { ClubID = ClubID };
                            // Xem lại luồng phần này vì ListClub bị execption
                            Helper.Instance().ListClub.Add(ClubID, club);
                        }
                        contentInfo.clubContent = Helper.Instance().ListClub[ClubID];
                    }
                    if (Contents.TryGetAt(i, 7, ref HomePageId))
                    {
                        var homeId = HomePageId;
                    }
                    if (Contents.TryGetAt(i, 2, ref Content))
                    {
                        contentInfo.Detail.Content = Content;
                    }
                    if (Contents.TryGetAt(i, 3, ref CreateDate))
                    {
                        contentInfo.Detail.Created = CreateDate;
                    }
                    if (Contents.TryGetAt(i, 4, ref Likes))
                    {
                        if (Likes.Length > 0)
                        {
                            var idAccount = Helper.Instance().MyAccount.Number_Id;
                            for (int j = 0; j < Likes.Length; ++j)
                            {
                                // thêm id của người đang xem content vào danh sách
                                long id = 0;
                                if (Likes.TryGetAt(j, ref id))
                                {
                                    if (!contentInfo.LikeContent.LikeContent.ContainsKey("" + id))
                                    {
                                        contentInfo.LikeContent.LikeContent.Add("" + id, new LikeContent { Content_Id = ContentID, Owner = id });
                                    }
                                }
                            }
                            contentInfo.LikeContent.Owner = idAccount;
                            contentInfo.LikeContent.Reset();
                        }
                    }
                    if (Contents.TryGetAt(i, 5, ref Comments))
                    {
                        contentInfo.CommentCount = Comments.Length;
                        for (int j = 0; j < Comments.Length; ++j)
                        {
                            long Id = 0;
                            if (Comments.TryGetAt(j, ref Id))
                                // List danh sách commentId
                                contentInfo.CommentIDs.Add(Id);
                        }
                    }
                    if (Contents.TryGetAt(i, 6, ref Title))
                    {
                        contentInfo.Detail.Title = Title;
                    }
                    if (Contents.TryGetAt(i, 7, ref HomePageId))
                    {

                    }
                    // Kiểm tra và lấy kết quả Content
                    if (!Helper.Instance().ListContent.TryGetValue(ContentID, out ContentInfo contentInfoOut))
                    {
                        contentInfoOut = contentInfo;
                        contentInfo.Detail.Id = ContentID;
                        try
                        {
                            lock (Helper.Instance().ListContent)
                            {
                                // Lưu ý chỗ này cần lock đối tượng ListContent để tránh exeption
                                Helper.Instance().ListContent.Add(ContentID, contentInfoOut);
                            }
                        }
                        catch { }
                    }

                    contentInfoOut.Detail.Content = Content;
                    contentInfoOut.Detail.Title = Title;
                    contentInfoOut.Detail.Created = CreateDate;
                    contentInfoOut.CommentCount = contentInfo.CommentCount;
                    contentInfoOut.CommentIDs = contentInfo.CommentIDs;
                    if (!Helper.Instance().checkOpenNewsSite)
                    {
                        try
                        {
                            if (ListNewsTemp.IndexOf(contentInfoOut) < 0)
                                ListNewsTemp.Add(contentInfoOut);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        try
                        {
                            if (ListNews.IndexOf(contentInfoOut) < 0)
                                ListNews.Add(contentInfoOut);
                        }
                        catch (Exception) { }
                    }
                    // Check Account và lấy ra Account để hiển thị lên giao diện
                    if (string.IsNullOrEmpty(Helper.Instance().ListAccounts[OwnerID.ToString()].Avatar_Uri) || string.IsNullOrEmpty(Helper.Instance().ListAccounts[OwnerID.ToString()].fullname))
                    {
                        Helper.Instance().CheckExistAccountAsync(contentInfo.Accounts.Number_Id);
                    }
                    // Kiểm tra sự tồn tại của club
                    if (ClubID > 0)
                    {
                        if (string.IsNullOrEmpty(Helper.Instance().ListClub[ClubID].ClubName))
                            Helper.Instance().CheckExistClubAsync(ClubID);
                    }
                }
                if (Helper.Instance().GetContactGroupChat)
                {
                    // Gửi yêu cầu lấy bản tin ContactPending, groupchat, contact
                    AppChat.Services.Service.Instiance().chatHomePageViewModel.SendContactGroupContactPendSync();
                    Helper.Instance().GetContactGroupChat = false;
                    Device.StartTimer(TimeSpan.FromMilliseconds(3000), () =>
                    {
                        if (ListNews.Count >= 50 || moreLoad == true)
                            return false;
                        else
                        {
                            if (!Helper.Instance().checkOpenNewsSite)
                            {
                                try
                                {
                                    if (ListNewsTemp.Count > 0)
                                        HomePageGetContentReq(ListNewsTemp[ListNewsTemp.Count - 1].Detail.Id, -10);
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {
                                    if (ListNews.Count > 0)
                                        HomePageGetContentReq(ListNews[ListNews.Count - 1].Detail.Id, -10);
                                }
                                catch { }
                            }
                            return true;
                        }
                    });
                }
            });
        }

        public void MsgAddContentIndicator(QHMessage msg)
        {
            long errorCode = 0;
            long contentId = 0;
            long homePageId = 0;
            long senderId = 0;
            string content = "";
            long createDate = 0;
            msg.TryGetAt((byte)MsgHomePageAddContentInd.Error, ref errorCode);
            if (errorCode != 0) return;
            msg.TryGetAt((byte)MsgHomePageAddContentInd.ContentID, ref contentId);
            msg.TryGetAt((byte)MsgHomePageAddContentInd.HomePageID, ref homePageId);
            msg.TryGetAt((byte)MsgHomePageAddContentInd.SenderID, ref senderId);
            msg.TryGetAt((byte)MsgHomePageAddContentInd.Content, ref content);
            msg.TryGetAt((byte)MsgHomePageAddContentInd.CreateDate, ref createDate);
            ContentInfo contentInfo = new ContentInfo()
            {
                Detail = new Contents() { Id = contentId, Content = content, Created = createDate, Owner = senderId }
            };
            if (!Helper.Instance().ListAccounts.ContainsKey(senderId.ToString()))
            {
                Accounts acc = new Accounts() { Number_Id = senderId };
                Helper.Instance().ListAccounts.Add(senderId.ToString(), acc);
            }
            contentInfo.Accounts = Helper.Instance().ListAccounts[senderId.ToString()];
            if (!Helper.Instance().ListContent.ContainsKey(contentId))
            {
                lock (Helper.Instance().ListContent)
                {
                    Helper.Instance().ListContent.Add(contentId, contentInfo);
                }
            }

            if (string.IsNullOrEmpty(Helper.Instance().ListAccounts[senderId.ToString()].fullname) || string.IsNullOrEmpty(Helper.Instance().ListAccounts[senderId.ToString()].Avatar_Uri))
            {
                Helper.Instance().CheckExistAccountAsync(senderId);
            }
            // thieu trường hợp đăng vào club
            Device.BeginInvokeOnMainThread(() =>
            {
                ListNews.Insert(0, Helper.Instance().ListContent[contentId]);
            });

        }
        public List<ContentInfo> listTemp = new List<ContentInfo>();
    }
}
