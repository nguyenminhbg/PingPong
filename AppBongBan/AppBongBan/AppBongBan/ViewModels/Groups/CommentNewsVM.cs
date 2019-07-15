using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.Likes;
using AppBongBan.Models.PingPongs;
using AppBongBan.Services;
using AppBongBan.ViewModels.Actions;
using DLToolkit.Forms.Controls;
using Newtonsoft.Json;
using PingPong;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class CommentNewsVM : ViewModelBase
    {
        static object syncObj = new object();
        static object syncObjLike = new object();
        public string strCmd;
        public bool IsTyping { get => _isTyping; set { SetProperty(ref _isTyping, value); } }
        public TypeInfo typeInfo { get; set; }
        //thưc hiện chuyển club sang cho trang chi tiết comment
        private ContentInfo _newsComment;
        public ContentInfo NewsContent
        {
            get => _newsComment; set
            {
                SetProperty(ref _newsComment, value);
            }
        }
        private string myComment = "";
        private bool _isImage;
        private ImageNews _imageView;
        private bool _isTyping;
        public FlowObservableCollection<CommentInfor> ListComments { get; set; }
        public CommentNewsVM()
        {
            ListComments = new FlowObservableCollection<CommentInfor>();
            IsLoading = false;
            IsTyping = false;
        }
        public override void Reset()
        {
            base.Reset();
            NewsContent = null;
            ImageView = new ImageNews();
            ListComments = new FlowObservableCollection<CommentInfor>();
            isImage = false;
            IsLoading = false;
            IsTyping = false;
        }

        /// <summary>
        /// Gửi yêu cầu lấy danh sách các comment trong content
        /// </summary>
        public async void SendGetContent(long ContentID)
        {
            IsLoading = true;
            // Lấy Content trong Cached
            if (!Helper.Instance().ListContent.ContainsKey(ContentID)) return;
            NewsContent = Helper.Instance().ListContent[ContentID];
            if (NewsContent != null)
            {
                var commentTempt = new List<CommentInfor>();
                for (int i = 0; i < NewsContent.CommentIDs.Count; i++)
                {
                    var comment = await Helper.Instance().CheckExistComment(NewsContent.CommentIDs[i]);
                    if (comment != null)
                        commentTempt.Add(comment);
                }
                ListComments.AddRange(commentTempt);
                IsLoading = false;
            }
        }

        public async void SendComment(string comment)
        {
            SendTyping();
            if ((comment != null && comment.Length > 0) || (ImageView.UriTmp != null && !ImageView.UriTmp.Equals("")))
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_COMMENT_ADD_REQ);
                //thêm id của người đăng
                if (Helpers.Helper.Instance().MyAccount != null)
                {
                    var id = Helpers.Helper.Instance().MyAccount.Number_Id;
                    msg.SetAt((byte)MsgCommentAddReqArg.NumberID, new QHNumber(id));
                }
                else
                {
                    NotifiDialog.Initiance().DialogErrorNumber();
                    return;
                }
                //thêm id của content 
                if (NewsContent != null && NewsContent.Detail.Id > 0)
                {
                    msg.SetAt((byte)MsgCommentAddReqArg.ContentID, new QHNumber(NewsContent.Detail.Id));
                }
                else
                {
                    NotifiDialog.Initiance().DialogNotContent();
                    return;
                }
                bool isPost = false;
                if (ImageView != null && ImageView.UriTmp != null && !ImageView.UriTmp.Equals(""))
                {
                    isPost = await PostImage();
                }
                string contentComment = "";
                if (isPost)
                {
                    var cmd = new CmtRcmtFormat();
                    cmd.Text = comment;
                    cmd.Image_Id = ImageView.UriImage;
                    cmd.IsImage = true;
                    QHVector vector = new QHVector();
                    vector.SetAt(0, new QHNumber(ImageView.Image_Id));
                    msg.SetAt((byte)MsgCommentAddReqArg.ImagesID, vector);
                    contentComment = JsonConvert.SerializeObject(cmd);
                }
                else
                {
                    var cmd = new CmtRcmtFormat();
                    cmd.Text = comment;
                    contentComment = JsonConvert.SerializeObject(cmd);
                }
                myComment = contentComment;
                msg.SetAt((byte)MsgCommentAddReqArg.Comment, new QHString(contentComment));

                //gửi bản tin lên server
                Services.Service.Instiance().SendMessage(msg);
            }
            else
            {
                NotifiDialog.Initiance().DialogAddComment();
            }
        }
        /// <summary>
        /// Nhận ack comment từ server trả về
        /// </summary>
        /// <param name="msg"></param>
        public void OnMsgAddCommentAck(QHMessage msg)
        {
            var error = (CommentError)(msg.GetAt((byte)MsgCommentAddAckArg.Error) as QHNumber).value;
            switch (error)
            {
                case CommentError.SUCCESS:
                    //Thêm đối tượng account cho comment
                    long Id = 0;
                    if (msg.TryGetAt((byte)MsgCommentAddAckArg.CommentID, ref Id))
                    {
                        var cmd = new CommentInfor
                        {
                            Response = true,
                            Account = Helper.Instance().MyAccount,
                            Comment = new Models.Db.Content.Comments
                            {
                                Content_Id = NewsContent.Detail.Id,
                                Content = myComment,
                                Id = Id,
                                Created = Helper.ConvertToUnixTime(DateTime.UtcNow)
                            }
                        };
                        ListComments.Add(cmd);
                        if (!Helper.Instance().ListComment.ContainsKey(Id))
                        {
                            Helper.Instance().ListComment.Add(Id, cmd);
                        }
                        NewsContent.CommentIDs.Add(Id);
                        NewsContent.Reset();
                    }
                    break;
                case CommentError.ERR_PERMISSION_DENIED:
                    break;
                case CommentError.ERR_NOT_EXIST_CONTENTT:
                    break;
            }

        }
        /// <summary>
        /// xử lý khi nhận add CommentInd
        /// </summary>
        /// <param name="msg"></param>
        public async void OnMsgAddCommentInd(QHMessage msg)
        {
            try
            {
                long ContentID = 0;  // QHNumber
                long Owner = 0;      // QHNumber người tạo comment này
                long CommentID = 0;  // QHNumber
                string Content = "";    // QHString nội dung của Comment
                long Created = 0;    // QHNumber thời điểm tạo nội dung
                QHVector ImagesID = new QHVector();
                //QHVector Likes = new QHVector();      // QHVector<QHNumber> danh sách number_id đã like
                //QHVector Replies = new QHVector(); ;    // QHVector<QHNumber> danh sách các reply trên comment này
                if (msg.TryGetAt((byte)MsgCommentAddIndArg.ContentID, ref ContentID)) { }
                // if (msg.TryGetAt((byte)MsgGetCommentAckArg.Owner, ref Owner)) { }
                if (msg.TryGetAt((byte)MsgCommentAddIndArg.CommentID, ref CommentID)) { }
                if (msg.TryGetAt((byte)MsgCommentAddIndArg.Comment, ref Content)) { }
                if (msg.TryGetAt((byte)MsgCommentAddIndArg.Created, ref Created)) { }
                if (msg.TryGetAt((byte)MsgCommentAddIndArg.NumberID, ref Owner)) { }
                if(msg.TryGetAt((byte) MsgCommentAddIndArg.ImagesID, ref ImagesID)) { }

                CommentInfor comment = new CommentInfor();
                comment.Comment.Content_Id = ContentID;
                comment.Comment.Owner = Owner;
                comment.Comment.Id = CommentID;
                comment.Comment.Created = Created;
                if (!Helper.Instance().ListAccounts.ContainsKey(comment.Comment.Owner.ToString()))
                {
                    var Accounts = new Accounts() { Number_Id = comment.Comment.Owner };
                    Helper.Instance().ListAccounts.Add(Accounts.Number_Id.ToString(), Accounts);
                }
                comment.Account = Helper.Instance().ListAccounts[comment.Comment.Owner.ToString()];
                if(string.IsNullOrEmpty( comment.Account.fullname)|| string.IsNullOrEmpty(comment.Account.Avatar_Uri))
                {
                    Helper.Instance().CheckExistAccountAsync(comment.Account.Number_Id);
                }
                comment.Comment.Content = Content;
                comment.Reset();
                if (Helper.Instance().ListComment.ContainsKey(CommentID))
                {
                    Helper.Instance().ListComment[CommentID].Replies = comment.Replies;
                    Helper.Instance().ListComment[CommentID].likes = comment.likes;
                    Helper.Instance().ListComment[CommentID].Comment.Created = Created;
                    Helper.Instance().ListComment[CommentID].Comment.Content = comment.Comment.Content;
                    Helper.Instance().ListComment[CommentID].Reset();
                }
                else
                {
                    Helper.Instance().ListComment.Add(CommentID, comment);
                }
                Helper.Instance().ListComment[CommentID].TimeSyns = Helper.ConvertToUnixTime(DateTime.Now);
                // Lấy nội dung content trong contentCached ra để Update lại
                foreach(var item in Helper.Instance().ListContent)
                {
                    if(item.Value.Detail.Id== ContentID)
                    {
                        item.Value.CommentCount += 1;
                        // thêm vào danh sách conmmentId
                        item.Value.CommentIDs.Add(CommentID);
                        item.Value.Reset();
                    }
                }
                ListComments.Add(comment);
                    DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", comment.Account.fullname + " đã thêm một bình luận", (int)Notifi.AddComment, comment.Comment.Content_Id);
            }
            catch (Exception ex)
            {
                var currentPage = AppChat.Helpers.Helper.Instiance().GetCurrentPage();
                await currentPage.DisplayAlert("Lỗi CommentInd", ex.Message, "Ok");
            }
        }

        public void LikeComment(CommentInfor comment, bool isLike)
        {
            LikeAction.SendLike(isLike, ref comment);
        }
        /// <summary>
        /// đối tượng chứa đường dẫn ảnh trong ứng dụng
        /// </summary>
        public ImageNews ImageView { get => _imageView; set { SetProperty(ref _imageView, value); } }
        public bool isImage { get => _isImage; set { SetProperty(ref _isImage, value); } }
        /////////////////////////////////////////////////
        public void ShowImgExe(List<ImageNews> images)
        {
            if (images.Count > 0)
            {
                isImage = true;
                if (!Helper.prev.Equals(images))
                {
                    ImageView.UriTmp = images[0].UriTmp;

                    Helper.DeleteUri(Helper.prev);
                    Helper.prev.Clear();
                    Helper.prev = images;
                }
            }
        }
        private async Task<bool> PostImage()
        {
            try
            {
                if (ImageView != null && !ImageView.UriTmp.Equals(""))
                {
                    var client = new WebClientW();
                    client.UCEvent = (o) =>
                    {

                    };
                    client.UPCEvent = (o, e) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ImageView.Progess = e * 0.1;
                            //Debug.WriteLine("Hoàn thành: " + e);
                        });
                    };
                    var id = Helper.Instance().MyAccount.Number_Id;
                    var imageNews = await client.UploadMuitliImage(WebClientW.uploadServiceBaseAddress,
                        ImageView.UriTmp,
                        new UploadImage { token_id = 1, owner_id = id });
                    ImageView.Image_Id = imageNews.Image_Id;
                    ImageView.UriImage = imageNews.UriImage;
                    if (imageNews != null)
                        return true;
                }
            }
            catch (Exception e)
            {

            }
            return false;
        }
        public void OnReciveTyping(QHMessage msg)
        {
            long NumberID = 0;
            long ClubID = 0;
            long ContentID = 0;
            long ReplyID = 0;
            typeInfo = new TypeInfo();
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.NumberID, ref NumberID)) { typeInfo.NumberID = NumberID; }
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.ClubID, ref ClubID)) { typeInfo.ClubID = ClubID; }
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.ContentID, ref ContentID)) { typeInfo.ContentID = ContentID; }
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.ReplyID, ref ReplyID)) { typeInfo.ReplyID = ReplyID; }
            //Id của content 
            if (NewsContent != null)
                if (ContentID > 0 && ContentID == NewsContent.Detail.Id)
                {
                    if (!IsTyping)
                    {
                        IsTyping = true;
                    }

                }
        }
        public void SendTyping()
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_USER_TYPING_IND);
            msg.SetAt((byte)MsgUserTypingIndArg.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
            msg.SetAt((byte)MsgUserTypingIndArg.ContentID, new QHNumber(NewsContent.Detail.Id));
            Service.Instiance().SendMessage(msg);
        }
    }

}
