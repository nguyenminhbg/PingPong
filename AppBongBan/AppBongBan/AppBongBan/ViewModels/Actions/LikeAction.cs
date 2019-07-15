using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Services;
using PingPong;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Actions
{
    public class LikeAction
    {
        /// <summary>
        /// Gửi Like và UnLike Content
        /// </summary>
        /// <param name="isLike"></param>
        /// <param name="content"></param>
        public static void SendLike(bool isLike, ref ContentInfo content)
        {
            // kiểm tra tồn tại của content và cập nhật lại like cho content
            if (!Helper.Instance().ListContent.ContainsKey(content.Detail.Id))
            {
                lock (Helper.Instance().ListContent)
                {
                    Helper.Instance().ListContent.Add(content.Detail.Id, content);
                }
            }
            else
            {
                Helper.Instance().ListContent[content.Detail.Id] = content;
            }
            //Like
            if (isLike)
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_LIKE_REQ);
                if (Helper.Instance().MyAccount != null)
                {
                    msg.SetAt((byte)MsgLikeReqArg.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
                }
                else
                {
                    return;
                }
                if (content != null && content.Detail.Id > 0)
                {
                    msg.SetAt((byte)MsgLikeReqArg.ContentID, new QHNumber(content.Detail.Id));
                }
                else
                {
                    return;
                }
                Service.Instiance().Like = StatueLike.Content;
                if (!Service.Instiance().SendMessage(msg))
                {
                }
            }
            //unlike
            else
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_UNLIKE_REQ);
                if (Helper.Instance().MyAccount != null)
                {
                    msg.SetAt((byte)MsgLikeReqArg.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
                }
                else
                {
                    return;
                }
                if (content != null && content.Detail.Id > 0)
                {
                    msg.SetAt((byte)MsgLikeReqArg.ContentID, new QHNumber(content.Detail.Id));
                }
                else
                {
                    return;
                }
                Service.Instiance().Like = StatueLike.Content;
                Service.Instiance().SendMessage(msg);
            }
        }
        /// <summary>
        /// Gửi Like và unlike Comment
        /// </summary>
        /// <param name="isLike"></param>
        /// <param name="comment"></param>
        public static void SendLike(bool isLike, ref CommentInfor comment)
        {
            if (comment != null)
            {
                if (Helper.Instance().ListComment.ContainsKey(comment.Comment.Id))
                {
                    Helper.Instance().ListComment[comment.Comment.Id] = comment;
                }
                else
                {
                    Helper.Instance().ListComment.Add(comment.Comment.Id, comment);
                }
                QHMessage msg;
                if (isLike)
                {
                    msg = new QHMessage((ushort)PingPongMsg.MSG_LIKE_REQ);
                }
                else
                {
                    msg = new QHMessage((ushort)PingPongMsg.MSG_UNLIKE_REQ);
                }
                var Id = Helper.Instance().MyAccount.Number_Id;
                //thêm id của người like vào trong club
                if (Id > 0)
                {
                    msg.SetAt((byte)MsgLikeReqArg.NumberID, new QHNumber(Id));
                }
                else
                {
                    return;
                }
                //thêm id của comment vào trong club
                if (comment.Comment.Id > 0)
                {
                    msg.SetAt((byte)MsgLikeReqArg.CommentID, new QHNumber(comment.Comment.Id));
                }
                else
                {
                    return;
                }
                Service.Instiance().Like = StatueLike.Comment;
                Service.Instiance().SendMessage(msg);
            }
        }
        /// <summary>
        /// Gửi like và unlike Reply
        /// </summary>
        /// <param name="isLike"></param>
        /// <param name="reply"></param>
        public static void SendLike(bool isLike, ref ReplyComments reply)
        {
            if (Helper.Instance().ListReplyComment.ContainsKey(reply.Id))
            {
                Helper.Instance().ListReplyComment[reply.Id] = reply;
            }
            else
            {
                Helper.Instance().ListReplyComment.Add(reply.Id, reply);
            }
            QHMessage msg;
            //like reply
            if (isLike)
            {
                msg = new QHMessage((ushort)PingPongMsg.MSG_LIKE_REQ);
            }
            //unlike reply
            else
            {
                msg = new QHMessage((ushort)PingPongMsg.MSG_UNLIKE_REQ);
            }
            if (reply != null)
            {
                var id = Helper.Instance().MyAccount.Number_Id;
                if (id > 0)
                {
                    msg.SetAt((byte)MsgLikeReqArg.NumberID, new QHNumber(id));
                }
                else
                {
                    return;
                }
                if (reply.Id > 0)
                {
                    msg.SetAt((byte)MsgLikeReqArg.ReplyID, new QHNumber(reply.Id));
                }
                else
                {
                    return;
                }
                Services.Service.Instiance().Like = Services.StatueLike.Reply;
                Services.Service.Instiance().SendMessage(msg);
            }
        }
        /// <summary>
        /// Like Content Ack
        /// </summary>
        /// <param name="msg"></param>
        public static void LikeContentAck(QHMessage msg)
        {
            var error = (LikeError)(msg.GetAt((byte)MsgLikeAckArg.Error) as QHNumber).value;
            switch (error)
            {
                case LikeError.SUCCESS:
                    //Debug.WriteLine("Thành công");
                    long ContentID = 0;
                    long CommentID = 0;
                    long ReplyID = 0;
                    if (msg.TryGetAt((byte)MsgLikeAckArg.ContentID, ref ContentID))
                    {
                        if (msg.TryGetAt((byte)MsgLikeAckArg.CommentID, ref CommentID))
                        {
                            if (msg.TryGetAt((byte)MsgLikeAckArg.ReplyID, ref ReplyID))
                            {

                            }
                        }
                    }
                    //Like Reply
                    if (ReplyID > 0)
                    {
                        if (Helper.Instance().ListReplyComment.ContainsKey(ReplyID))
                        {
                            var id = Helper.Instance().MyAccount.Number_Id;

                            if (id > 0)
                            {
                                if (Helper.Instance().ListReplyComment[ReplyID].Likes.IndexOf(id) < 0)
                                {
                                    Helper.Instance().ListReplyComment[ReplyID].Likes.Add(id);
                                    Helper.Instance().ListReplyComment[ReplyID].Owner = id;
                                    Helper.Instance().ListReplyComment[ReplyID].Reset();
                                }
                            }
                        }
                        return;
                    }
                    //Like Comment
                    if (CommentID > 0)
                    {
                        if (Helper.Instance().ListComment.ContainsKey(CommentID))
                        {
                            var id = Helper.Instance().MyAccount.Number_Id;
                            var item = Helper.Instance().ListComment[CommentID];
                            if (id > 0)
                            {
                                item.NumberId = id;
                                if (item.likes.IndexOf(id) < 0)
                                {
                                    item.likes.Add(id);
                                    item.Reset();
                                }
                            }
                        }
                        return;
                    }
                    //Like Content
                    if (ContentID > 0)
                    {
                        if (Helper.Instance().ListContent.ContainsKey(ContentID))
                        {
                            var item = Helper.Instance().ListContent[ContentID];
                            var like = (new LikeContent
                            {
                                Content_Id = ContentID,
                                Owner = Helper.Instance().MyAccount.Number_Id
                            });
                            item.LikeContent.Owner = Helper.Instance().MyAccount.Number_Id;
                            if (!item.LikeContent.LikeContent.ContainsKey("" + item.LikeContent.Owner))
                            {
                                item.LikeContent.LikeContent.Add("" + item.LikeContent.Owner, like);
                                item.LikeContent.Reset();
                            }

                        }
                        return;
                    }
                    break;
                case LikeError.ERR_PERMISSION_DENIED:
                    Debug.WriteLine("");
                    break;
                case LikeError.ERR_NOT_EXIST_REPLY:
                    break;
                case LikeError.ERR_NOT_EXIST_CONTENT:
                    break;
                case LikeError.ERR_NOT_EXIST_COMMENT:
                    break;


            }

        }
        /// <summary>
        /// Unlike Content Ack
        /// </summary>
        /// <param name="msg"></param>
        public static void UnlikeContentAck(QHMessage msg)
        {
            long error = -1;
            if (msg.TryGetAt((byte)MsgUnlikeAckArg.Error, ref error))
            {
                switch ((UnlikeError)error)
                {
                    case UnlikeError.SUCCESS:
                        long NumberID = 0;
                        long ContentID = 0;
                        long CommentID = 0;
                        long ReplyID = 0;
                        if (msg.TryGetAt((byte)MsgUnlikeAckArg.ContentID, ref ContentID))
                        {
                            if (msg.TryGetAt((byte)MsgUnlikeAckArg.CommentID, ref CommentID))
                            {
                                if (msg.TryGetAt((byte)MsgUnlikeAckArg.ReplyID, ref ReplyID))
                                {

                                }
                            }
                        }
                        //unlike Reply
                        if (ReplyID > 0)
                        {
                            if (Helper.Instance().ListReplyComment.ContainsKey(ReplyID))
                            {
                                var item = Helper.Instance().ListReplyComment[ReplyID];
                                if (msg.TryGetAt((byte)MsgUnlikeAckArg.NumberID, ref NumberID))
                                {
                                    if (item.Likes.IndexOf(NumberID) >= 0)
                                    {
                                        item.Likes.Remove(NumberID);
                                        item.Owner = NumberID;
                                        item.Reset();
                                    }

                                }
                            }
                            return;
                        }
                        //unlike Comment
                        if (CommentID > 0)
                        {
                            if (Helper.Instance().ListComment.ContainsKey(CommentID))
                            {
                                var item = Helper.Instance().ListComment[CommentID];
                                if (msg.TryGetAt((byte)MsgUnlikeAckArg.NumberID, ref NumberID))
                                {
                                    if (item.likes.IndexOf(NumberID) >= 0)
                                    {
                                        item.likes.Remove(NumberID);
                                        item.Reset();
                                    }
                                }
                            }
                            return;
                        }
                        //unlike Content
                        if (ContentID > 0)
                        {
                            if (Helper.Instance().ListContent.ContainsKey(ContentID))
                            {
                                var item = Helper.Instance().ListContent[ContentID];

                                if (msg.TryGetAt((byte)MsgUnlikeAckArg.NumberID, ref NumberID))
                                {
                                    //tìm kiếm và xóa danh sách item like trong danh sách like
                                    if (item.LikeContent.LikeContent.ContainsKey("" + NumberID))
                                    {
                                        item.LikeContent.LikeContent.Remove("" + NumberID);
                                        item.LikeContent.Reset();
                                    }
                                }
                            }
                            return;
                        }
                        break;
                    case UnlikeError.ERR_PERMISSION_DENIED:
                        break;
                    case UnlikeError.ERR_NOT_EXIST_REPLY:
                        break;
                    case UnlikeError.ERR_NOT_EXIST_CONTENT:
                        break;
                    case UnlikeError.ERR_NOT_EXIST_COMMENT:
                        break;
                }
            }
        }
        /// <summary>
        /// Like indicator
        /// </summary>
        /// <param name="msg"></param>
        public async static void LikeInd(QHMessage msg)
        {
            long NumberId = 0;
            string FullName = "";
            string avatar = "";
            long ContentID = 0;
            long CommentID = 0;
            long ReplyID = 0;
            long OwnerID = 0;
            if (msg.TryGetAt((byte)MsgLikeIndArg.FullName, ref FullName))
            { }
            if (msg.TryGetAt((byte)MsgLikeIndArg.OwnerID, ref OwnerID))
            {
            }
            if (msg.TryGetAt((byte)MsgLikeIndArg.NumberID, ref NumberId))
            {

            }
            if (msg.TryGetAt((byte)MsgLikeIndArg.ContentID, ref ContentID))
            {
                if (msg.TryGetAt((byte)MsgLikeIndArg.CommentID, ref CommentID))
                {
                    if (msg.TryGetAt((byte)MsgLikeIndArg.ReplyID, ref ReplyID))
                    {

                    }
                }

            }
            if (msg.TryGetAt((byte)MsgLikeIndArg.CommentID, ref CommentID)) { }
            if (msg.TryGetAt((byte)MsgLikeIndArg.ReplyID, ref ReplyID)) { }
            //LikeReply
            if (ReplyID > 0)
            {
                if (Helper.Instance().ListReplyComment.ContainsKey(ReplyID))
                {
                    var item = Helper.Instance().ListReplyComment[ReplyID];
                    item.Owner = Helper.Instance().MyAccount.Number_Id;
                    if (item.Likes.IndexOf(NumberId) < 0)
                    {
                        item.Likes.Add(NumberId);
                        item.Reset();
                    }
                    if (Helper.IdNews == CommentID && Helper.IndexPage == Helpers.IsDetailNews.Reply)
                    {

                    }
                    else
                    {
                        if (OwnerID == Helper.Instance().MyAccount.Number_Id)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích trả lời bình luận của bạn", (int)Helpers.Notifi.LikeReply, 0);
                            });
                        }
                        else
                        {
                            var acc = await Helper.Instance().CheckExistAccount(OwnerID);
                            if (acc.Number_Id > 0)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích trả lời bình luận của " + acc.fullname, (int)Helpers.Notifi.LikeReply, 0);
                                });

                            }
                        }
                    }
                }
                else
                {
                    if (OwnerID == Helper.Instance().MyAccount.Number_Id)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích trả lời bình luận của bạn", (int)Helpers.Notifi.LikeReply, 0);
                        });
                    }
                    else
                    {
                        var acc = await Helper.Instance().CheckExistAccount(OwnerID);
                        if (acc.Number_Id > 0)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích trả lời bình luận của " + acc.fullname, (int)Helpers.Notifi.LikeReply, 0);
                            });
                        }
                    }
                }

                return;
            }
            //LikeComment
            if (CommentID > 0)
            {
                if (Helper.Instance().ListComment.ContainsKey(CommentID))
                {
                    var item = Helper.Instance().ListComment[CommentID];
                    if (item.likes.IndexOf(NumberId) < 0)
                    {
                        item.likes.Add(NumberId);
                        item.Reset();
                    }
                    if (Helper.IdNews == ContentID && Helper.IndexPage == Helpers.IsDetailNews.Comment)
                    {

                    }
                    //nếú chưa có trong danh sách comment
                    else
                    {
                        if (OwnerID == Helper.Instance().MyAccount.Number_Id)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bình luận của bạn", (int)Helpers.Notifi.LikeComment, 0);
                            });
                        }
                        else
                        {
                            var accounOwner = await Helper.Instance().CheckExistAccount(OwnerID);
                            if (accounOwner != null && accounOwner.Number_Id > 0)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bài viết của" + accounOwner.fullname, (int)Helpers.Notifi.LikeComment, 0);
                                });

                            }
                        }
                    }
                }
                else
                {
                    if (OwnerID == Helper.Instance().MyAccount.Number_Id)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bình luận của bạn", (int)Helpers.Notifi.LikeComment, 0);
                        });
  
                    }
                    else
                    {
                        var accounOwner = await Helper.Instance().CheckExistAccount(OwnerID);
                        if (accounOwner != null && accounOwner.Number_Id > 0)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bài viết của" + accounOwner.fullname, (int)Helpers.Notifi.LikeComment, 0);
                            });
                        }
                    }
                }
                return;
            }
            //LikeContent
            if (ContentID > 0)
            {
                if (Helper.Instance().ListContent.ContainsKey(ContentID))
                {
                    var item = Helper.Instance().ListContent[ContentID];
                    var Like = new LikeContent { Content_Id = ContentID, Owner = NumberId };
                    if (!item.LikeContent.LikeContent.ContainsKey("" + NumberId))
                    {
                        item.LikeContent.LikeContent.Add("" + NumberId, Like);
                        item.LikeContent.Reset();
                    }

                    if (Helper.IdNews == ContentID && Helper.IndexPage == Helpers.IsDetailNews.Content)
                    {

                    }
                    //Nếu người dùng chưa vào giao diện chi tiết content
                    else
                    {
                        var id = Helper.Instance().MyAccount.Number_Id;
                        if (id == OwnerID)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bài viết của bạn", (int)Notifi.LikeContent, 0);
                            });
                        }
                           
                        else
                        {
                            var accounOwner = await Helper.Instance().CheckExistAccount(OwnerID);
                            if (accounOwner != null && accounOwner.Number_Id > 0)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bài viết của " + accounOwner.fullname, (int)Notifi.LikeContent, 0);
                                });
                            }
                        }
                    }
                }
                else
                {
                    var id = Helper.Instance().MyAccount.Number_Id;
                    if (id == OwnerID)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bài viết của bạn", (int)Notifi.LikeContent, 0);
                        });
                    }

                    else
                    {
                        var accounOwner = await Helper.Instance().CheckExistAccount(OwnerID);
                        if (accounOwner != null && accounOwner.Number_Id > 0)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", FullName + " đã thích bài viết của " + accounOwner.fullname, (int)Notifi.LikeContent, 0);
                            });
                        }
                    }
                }
                return;
            }
        }
        /// <summary>
        /// UnLike Indicator trong content, comment, reply
        /// </summary>
        /// <param name="msg"></param>
        public static void UnLikeInd(QHMessage msg)
        {
            long NumberID = 0;
            long ContentID = 0;
            long CommentID = 0;
            long ReplyID = 0;
            if (msg.TryGetAt((byte)MsgUnlikeAckArg.ContentID, ref ContentID))
            {
                if (msg.TryGetAt((byte)MsgUnlikeAckArg.CommentID, ref CommentID))
                {
                    if (msg.TryGetAt((byte)MsgUnlikeAckArg.ReplyID, ref ReplyID))
                    {

                    }
                }
            }
            //Reply
            if (ReplyID > 0)
            {
                if (Helper.Instance().ListReplyComment.ContainsKey(ReplyID))
                {
                    var item = Helper.Instance().ListReplyComment[ReplyID];
                    if (msg.TryGetAt((byte)MsgUnlikeAckArg.NumberID, ref NumberID))
                    {
                        if (item.Likes.IndexOf(NumberID) > 0)
                        {
                            item.Likes.Remove(NumberID);
                            item.Owner = NumberID;
                            item.Reset();
                        }

                    }
                }
                return;
            }
            //Comment
            if (CommentID > 0)
            {
                if (Helper.Instance().ListComment.ContainsKey(CommentID))
                {
                    var item = Helper.Instance().ListComment[CommentID];
                    if (msg.TryGetAt((byte)MsgUnlikeAckArg.NumberID, ref NumberID))
                    {
                        if (item.likes.IndexOf(NumberID) > 0)
                        {
                            item.likes.Remove(NumberID);
                            item.Reset();
                        }
                    }
                }
                return;
            }
            //Content
            if (ContentID > 0)
            {
                if (Helper.Instance().ListContent.ContainsKey(ContentID))
                {
                    var item = Helper.Instance().ListContent[ContentID];

                    if (msg.TryGetAt((byte)MsgUnlikeAckArg.NumberID, ref NumberID))
                    {
                        //tìm kiếm và xóa danh sách item like trong danh sách like
                        if (item.LikeContent.LikeContent.ContainsKey("" + NumberID))
                        {
                            item.LikeContent.LikeContent.Remove("" + NumberID);
                            item.LikeContent.Reset();
                        }
                    }
                }
                return;
            }
        }
    }
}
