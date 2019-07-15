using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.Services;
using AppBongBan.ViewModels.Actions;
using DLToolkit.Forms.Controls;
using Newtonsoft.Json;
using PingPong;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class ReplayCommentVM : ViewModelBase
    {
        static object synObject = new object();
        private string myReplyComment = "";
        private CommentInfor _comment;
        public CommentInfor Comment { get => _comment; set { SetProperty(ref _comment, value); } }
        public bool IsTyping { get => _isTyping; set { SetProperty(ref _isTyping, value); } }
        public TypeInfo typeInfo { get; set; }
        public FlowObservableCollection<ReplyComments> ListReplyComment { get; set; }
        public ReplayCommentVM()
        {
            ListReplyComment = new FlowObservableCollection<ReplyComments>();
            IsLoading = false;
            IsTyping = false;
        }
        public override void Reset()
        {
            base.Reset();
            Comment = null;
            ImageView = new ImageNews();
            ListReplyComment = new FlowObservableCollection<ReplyComments>();
            isImage = false;
            IsLoading = false;
            IsTyping = false;
        }
        /// <summary>
        /// gửi bản tin lấy danh sách id của replycomment trong comment
        /// </summary>
        public async void SendGetComment(long CommentID)
        {

            IsLoading = true;
            Comment = await Helper.Instance().CheckExistComment(CommentID);
            if (Comment != null)
            {
                var relplyCommentTemp = new List<ReplyComments>();
                for (int i = 0; i < Comment.Replies.Count; i++)
                {
                    var reply = await Helper.Instance().ChechExistReplyComment(Comment.Replies[i]);
                    if (reply != null)
                        relplyCommentTemp.Add(reply);
                }
                ListReplyComment.AddRange(relplyCommentTemp);
                IsLoading = false;
            }
        }
        private void sendGetReplyComment(long idReply)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_GET_REPLY_REQ);
            var id = Helpers.Helper.Instance().MyAccount.Number_Id;
            if (id > 0)
            {
                msg.SetAt((byte)MsgGetReplyReqArg.NumberID, new QHNumber(id));
            }
            else { return; }
            if (idReply > 0)
            {
                msg.SetAt((byte)MsgGetReplyReqArg.ReplyID, new QHNumber(idReply));
            }
            else { return; }
            Debug.WriteLine("gửi reply: " + msg.JSONString());
            if (!Services.Service.Instiance().SendMessage(msg))
            {

            }
        }
        public async void SendReplyComment(string reply)
        {
            SendTyping();
            if (Comment != null || (reply != null && reply.Length > 0) || (ImageView.UriTmp != null && !ImageView.UriTmp.Equals("")))
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_REPLY_ADD_REQ);
                var id = Helper.Instance().MyAccount.Number_Id;
                if (id > 0)
                {
                    msg.SetAt((byte)MsgReplyAddRegArg.NumberID, new QHNumber(id));
                }
                else
                {
                    return;
                }
                msg.SetAt((byte)MsgReplyAddRegArg.ContentID, new QHNumber(Comment.Comment.Content_Id));
                msg.SetAt((byte)MsgReplyAddRegArg.CommentID, new QHNumber(Comment.Comment.Id));
                bool isPost = false;
                if (ImageView != null && ImageView.UriTmp != null && !ImageView.UriTmp.Equals(""))
                {
                    isPost = await PostImage();
                }
                string contentComment = "";
                if (isPost)
                {
                    var cmd = new CmtRcmtFormat();
                    cmd.Text = reply;
                    cmd.Image_Id = ImageView.UriImage;
                    cmd.IsImage = true;
                    QHVector vector = new QHVector();
                    vector.SetAt(0, new QHNumber(ImageView.Image_Id));
                    msg.SetAt((byte)MsgReplyAddRegArg.ImagesID, vector);
                    contentComment = JsonConvert.SerializeObject(cmd);
                }
                else
                {
                    var cmd = new CmtRcmtFormat();
                    cmd.Text = reply;
                    contentComment = JsonConvert.SerializeObject(cmd);
                }
                myReplyComment = contentComment;
                msg.SetAt((byte)MsgReplyAddRegArg.ReplyContent, new QHString(contentComment));
                Services.Service.Instiance().SendMessage(msg);
            }
            else
            {
                NotifiDialog.Initiance().DialogAddComment();
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
            catch (Exception)
            {
            }
            return false;
        }
        /// <summary>
        /// thực hiện nhaanj ack từ server trả về cho thêm reply comment
        /// </summary>
        /// <param name="msg"></param>
        public void OnMsgReplyAddAck(QHMessage msg)
        {
            var error = (ReplyAddError)(msg.GetAt((byte)MsgReplyAddAckArg.Error) as QHNumber).value;
            switch (error)
            {
                case ReplyAddError.SUCCESS:
                    long ReplyID = 0;
                    long CommentID = 0;
                    long ContentID = 0;
                    var reply = new ReplyComments();
                    if (msg.TryGetAt((byte)MsgReplyAddAckArg.ReplyID, ref ReplyID))
                    {
                        reply.Id = ReplyID;
                    }
                    if (msg.TryGetAt((byte)MsgReplyAddAckArg.ContentID, ref ContentID))
                    {
                        reply.ContentID = ContentID;
                    }
                    if (msg.TryGetAt((byte)MsgReplyAddAckArg.CommentID, ref CommentID))
                    {
                        reply.CommentID = CommentID;
                    }
                    reply.Created = Helper.ConvertToUnixTime(DateTime.UtcNow);
                    reply.Response = false;
                    reply.ReplyContent = myReplyComment;
                    reply.Owner = Helper.Instance().MyAccount.Number_Id;
                    reply.Account = Helper.Instance().MyAccount;
                    Comment.Replies.Add(ReplyID);
                    Comment.Reset();

                    if (!Helper.Instance().ListReplyComment.ContainsKey(ReplyID))
                    {
                        Helper.Instance().ListReplyComment.Add(ReplyID, reply);
                    }
                    else
                    {
                        Helper.Instance().ListReplyComment[ReplyID].Created = reply.Created;
                        Helper.Instance().ListReplyComment[ReplyID].ReplyContent = reply.ReplyContent;
                    }
                    ListReplyComment.Add(reply);
                    break;
                case ReplyAddError.ERR_NOT_EXIST_REPLY:
                    break;
                case ReplyAddError.ERR_NOT_EXIST_CONTENT:
                    break;
                case ReplyAddError.ERR_NOT_EXIST_COMMENT:
                    break;
            }
        }

        /// <summary>
        /// thực hiện nhận bản tin thêm reply cho comment
        /// </summary>
        /// <param name="msg"></param>
        public async void OnMsgReplyAddInd(QHMessage msg)
        {
            long NumberID = 0;      // QHNumber : ai sở hữu reply này
            long ReplyID = 0;        // QHNumber : id của reply là gì
            string ReplyContent = "";   // QHString : Nội dung của reply
            QHVector ImagesID = new QHVector();       // QHVector<QHNumber> danh sách ảnh có trong reply
            long CommentID = 0;     // QHNumber : ID của comment
            long ContentID = 0;      // QHNumber : ID của content chứa comment
            long CommentOwner = 0;   // QHNumber : Ai là người sở hữu comment này
            long Created = 0;        // QHNumber : Reply lúc nào

            if (msg.TryGetAt((byte)MsgReplyAddIndArg.NumberID, ref NumberID)) { }
            if (msg.TryGetAt((byte)MsgReplyAddIndArg.ReplyID, ref ReplyID)) { }
            if (msg.TryGetAt((byte)MsgReplyAddIndArg.ReplyContent, ref ReplyContent)) { }
            if (msg.TryGetAt((byte)MsgReplyAddIndArg.ImagesID, ref ImagesID)) { }
            if (msg.TryGetAt((byte)MsgReplyAddIndArg.CommentID, ref CommentID)) { }
            if (msg.TryGetAt((byte)MsgReplyAddIndArg.ContentID, ref ContentID)) { }
            if (msg.TryGetAt((byte)MsgReplyAddIndArg.CommentOwner, ref CommentOwner)) { }
            if (msg.TryGetAt((byte)MsgReplyAddIndArg.Created, ref Created)) { }

            var replyComments = new ReplyComments();
            replyComments.Id = ReplyID;
            replyComments.ReplyContent = ReplyContent;
            replyComments.Owner = NumberID;
            replyComments.Created = Created;
            replyComments.Response = false;
            if (ImagesID != null && ImagesID.Length > 0)
            {
                for (int i = 0; i < ImagesID.Length; i++)
                {
                    long Id = 0;
                    if (ImagesID.TryGetAt(i, ref Id))
                        replyComments.ImagesID.Add(Id);
                }
            }
            replyComments.CommentID = CommentID;
            replyComments.ContentID = ContentID;
            replyComments.Reset();
            if (!Helper.Instance().ListAccounts.ContainsKey(NumberID.ToString()))
            {
                var account = new Accounts() { Number_Id = NumberID };
                Helper.Instance().ListAccounts.Add(NumberID.ToString(), account);
            }
            replyComments.Account = Helper.Instance().ListAccounts[NumberID.ToString()];
            if (string.IsNullOrEmpty(replyComments.Account.fullname) || string.IsNullOrEmpty(replyComments.Account.fullname))
            {
                 Helper.Instance().CheckExistAccountAsync(NumberID);
            }
            var accComment = await Helper.Instance().CheckExistAccount(CommentOwner);

            if (!Helper.Instance().ListReplyComment.ContainsKey(ReplyID))
            {
                Helper.Instance().ListReplyComment.Add(ReplyID, replyComments);
            }
            else
            {
                Helper.Instance().ListReplyComment[ReplyID].Created = Created;
                Helper.Instance().ListReplyComment[ReplyID].Likes = replyComments.Likes;
                Helper.Instance().ListReplyComment[ReplyID].ImagesID = replyComments.ImagesID;
                Helper.Instance().ListReplyComment[ReplyID].ReplyContent = ReplyContent;
            }
            Helper.Instance().ListReplyComment[ReplyID].TimeSyns = Helper.ConvertToUnixTime(DateTime.Now);
            ListReplyComment.Add(replyComments);
            if (Comment != null && Comment.Comment.Id == CommentID)
            {
                Comment.Replies.Add(ReplyID);
                Comment.Reset();
            }
            else
            {
                DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", replyComments.Account.fullname + " đã trả lời bình luận của " + accComment.fullname, (int)Notifi.AddReplyComment, 0);
            }

        }
        public void SendLikeReply(ReplyComments reply, bool isLike)
        {
            LikeAction.SendLike(isLike, ref reply);
        }
        private ImageNews _imageView;
        private bool _isImage;
        private bool _isTyping;

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
        public void OnReciveTyping(QHMessage msg)
        {
            long NumberID = 0;
            long ClubID = 0;
            long ContentID = 0;
            long CommentID = 0;
            long ReplyID = 0;

            typeInfo = new TypeInfo();
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.NumberID, ref NumberID)) { typeInfo.NumberID = NumberID; }
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.ClubID, ref ClubID)) { typeInfo.ClubID = ClubID; }
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.ContentID, ref ContentID)) { typeInfo.ContentID = ContentID; }
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.ReplyID, ref ReplyID)) { typeInfo.ReplyID = ReplyID; }
            if (msg.TryGetAt((byte)MsgUserTypingIndArg.CommentID, ref CommentID)) { typeInfo.CommentID = CommentID; }
            //Id của content 
            if (Comment != null)
                if (CommentID > 0 && CommentID == Comment.Comment.Id)
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
            msg.SetAt((byte)MsgUserTypingIndArg.ContentID, new QHNumber(Comment.Comment.Content_Id));
            msg.SetAt((byte)MsgUserTypingIndArg.CommentID, new QHNumber(Comment.Comment.Id));
            Service.Instiance().SendMessage(msg);
        }
    }
}
