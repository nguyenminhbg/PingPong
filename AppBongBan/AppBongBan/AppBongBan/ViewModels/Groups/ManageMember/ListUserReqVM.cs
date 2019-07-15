using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using PingPong;
using qhmono;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups.ManageMember
{
    public class ListUserReqVM : ViewModelBase
    {
        public Club MyClub { get; set; }
        public int myAccept;
        /// <summary>
        /// Reset lại dữ liệu trong list ứng dụng
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (ListAccAwait != null && ListAccAwait.Count > 0)
            {
                ListAccAwait.Clear();
            }
            ListRequest = new ObservableCollection<AccJoinClub>();
        }
        public AccJoinClub myAccJoinClub;
        public ObservableCollection<AccJoinClub> ListRequest { get; set; }
        public List<AccJoinClub> ListAccAwait = new List<AccJoinClub>();
        public ListUserReqVM()
        {
            ListRequest = new ObservableCollection<AccJoinClub>();
        }
        public void CheckReQuest(List<AccJoinClub> list)
        {
            if (MyClub != null)
            {
                foreach (var item in list)
                {
                    if (item.ClubID == MyClub.ClubID)
                    {
                        bool req = false;
                        for (int j = 0; j < ListRequest.Count; j++)
                        {
                            if (item.Equals(ListRequest[j]))
                            {
                                req = true;
                                break;
                            }
                        }
                        if (!req)
                        {
                            ListRequest.Add(item);
                        }
                    }

                }
            }

        }
        public void SendAccept(AccJoinClub accJoinClub, int accept)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_JOIN_ACCEPT_REQ);
            if (accJoinClub != null)
            {
                //id của người được chấp nhận
                if (accJoinClub.IdNumberID > 0)
                {
                    msg.SetAt((byte)MsgClubJoinAcceptReqArg.NumberID, new QHNumber(accJoinClub.IdNumberID));
                }
                else
                {
                    return;
                }
                //tên của người được chấp nhận
                if (!accJoinClub.FullName.Equals(""))
                {
                    msg.SetAt((byte)MsgClubJoinAcceptReqArg.FullName, new QHString(accJoinClub.FullName));
                }
                else
                {
                    return;
                }
                //avatar của người được chấp nhận
                if (!accJoinClub.Avatar.Equals(""))
                {
                    msg.SetAt((byte)MsgClubJoinAcceptReqArg.Avatar, new QHString(accJoinClub.Avatar));
                }
                else
                {
                    return;
                }
                //id của club chấp nhận 
                if (accJoinClub.ClubID > 0)
                {
                    msg.SetAt((byte)MsgClubJoinAcceptReqArg.ClubID, new QHNumber(accJoinClub.ClubID));
                }
                else
                {
                    return;
                }
                //cover của club chấp nhận yêu cầu
                if (!accJoinClub.ClubCover.Equals(""))
                {
                    msg.SetAt((byte)MsgClubJoinAcceptReqArg.ClubCover, new QHString(accJoinClub.ClubCover));
                }
                else
                {
                    return;
                }
                msg.SetAt((byte)MsgClubJoinAcceptReqArg.AcceptTime, new QHString(DateTime.Now.ToLongDateString()));
                msg.SetAt((byte)MsgClubJoinAcceptReqArg.Accept, new QHNumber(accept));
                if (!Services.Service.Instiance().SendMessage(msg))
                {
                    NotifiDialog.Initiance().DialogErrorInternter();
                }
                else
                {
                    myAccJoinClub = accJoinClub;
                    myAccept = accept;
                }
            }
        }
        /// <summary>
        /// Lọc ack cho từ ứng dụng
        /// </summary>
        /// <param name="msg"></param>
        public void AcceptAck(QHMessage msg)
        {
            App app = Application.Current as App;
            if (myAccept == 1)
            {
                if (myAccJoinClub != null)
                {
                    Services.Service.Instiance().ClubModel.MyClub.Numbers.Add(myAccJoinClub.IdNumberID);
                    Services.Service.Instiance().ClubModel.MyClub.Requests.Remove(myAccJoinClub.IdNumberID);
                    Services.Service.Instiance().ClubModel.MyClub.ReloadReative();
                    app.ListAccInClub.Remove(myAccJoinClub);
                    ListRequest.Remove(myAccJoinClub);
                    myAccJoinClub = null;
                    NotifiDialog.Initiance().DialogAccept();
                }
            }
            else
            {
                if (myAccJoinClub != null)
                {
                    Services.Service.Instiance().ClubModel.MyClub.Requests.Remove(myAccJoinClub.IdNumberID);
                    //Services.Service.Instiance().ClubModel.MyClub.ReloadReative();
                    app.ListAccInClub.Remove(myAccJoinClub);
                    ListRequest.Remove(myAccJoinClub);
                    myAccJoinClub = null;
                }

            }
        }
        /// <summary>
        /// Bản tin Indicator khi người dùng nhận được
        /// </summary>
        /// <param name="msg"></param>
        public void ClubJoinAck(QHMessage msg)
        {
            int sampleId = 0;
            long Accept = -1;
            long NumberID = 0;
            long ClubId = 0;
            if (msg.TryGetAt((byte)MsgClubJoinAcceptIndArg.Accept, ref Accept))
            {
                if (msg.TryGetAt((byte)MsgClubJoinAcceptIndArg.ClubID, ref ClubId)) { }
                if (Accept == 1)
                {
                    if (msg.TryGetAt((byte)MsgClubJoinAcceptIndArg.NumberID, ref NumberID))
                    {
                        Services.Service.Instiance().ClubModel.MyClub.Numbers.Add(NumberID);
                        Services.Service.Instiance().ClubModel.MyClub.Requests.Remove(NumberID);
                        Services.Service.Instiance().ClubModel.MyClub.ReloadReative();
                    }
                    string Title = "AppBongBan";
                    string nameclub = "";
                    string notifi = "";
                    if (msg.TryGetAt((byte)MsgClubJoinAcceptIndArg.ClubName, ref nameclub))
                    {
                        notifi = nameclub;
                    }
                    if (msg.TryGetAt((byte)MsgClubJoinAcceptIndArg.FullName, ref nameclub))
                    {
                        notifi += " đã chấp nhận tham gia của " + nameclub;
                    }
                    // Thay đổi Icon trong ListClub
                    foreach(var club in Helper.Instance().ListClub)
                    {
                        if(club.Value.ClubID== ClubId)
                        {
                            club.Value.Relation = 0;
                        }
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<ILocalNotificationService>().LocalNotification(Title, notifi, sampleId, 0);
                    });

                }
                else if (Accept == 0)
                {
                    if (msg.TryGetAt((byte)MsgClubJoinAcceptIndArg.NumberID, ref NumberID))
                    {
                        Services.Service.Instiance().ClubModel.MyClub.Requests.Remove(NumberID);
                        Services.Service.Instiance().ClubModel.MyClub.ReloadReative();
                    }
                    // Thay đổi Icon trong ListClub
                    foreach (var club in Helper.Instance().ListClub)
                    {
                        if (club.Value.ClubID == ClubId)
                        {
                            club.Value.Relation =-1;
                        }
                    }
                }
            }
        }
        public async void GetAccountAccept()
        {
            if (MyClub != null)
            {
                if (MyClub.Requests != null && MyClub.Requests.Count > 0)
                {
                    var app = Application.Current as App;
                    foreach (var item in MyClub.Requests)
                    {
                        var acc = await Helper.Instance().CheckExistAccount(item);
                        if (acc.Number_Id > 0)
                        {
                            var accJoinClub = new AccJoinClub();
                            accJoinClub.Avatar = acc.Avatar_Uri;
                            accJoinClub.FullName = acc.fullname;
                            accJoinClub.IdNumberID = acc.Number_Id;
                            accJoinClub.Level = "Hạng C";
                            accJoinClub.Facebat = "Mặt A - Hãng A";
                            accJoinClub.Blade = "Cốt A - Hãng A";
                            accJoinClub.Challenge = "pingpong.png";
                            accJoinClub.ClubID = MyClub.ClubID;
                            accJoinClub.ClubCover = MyClub.ClubAvatarUri;
                            ListRequest.Add(accJoinClub);
                        }
                        else
                        {
                            var accJoinClub = new AccJoinClub();
                            accJoinClub.IdNumberID = item;
                            accJoinClub.Level = "Hạng C";
                            accJoinClub.Facebat = "Mặt A - Hãng A";
                            accJoinClub.Blade = "Cốt A - Hãng A";
                            accJoinClub.Challenge = "pingpong.png";
                            accJoinClub.ClubID = MyClub.ClubID;
                            accJoinClub.ClubCover = MyClub.ClubAvatarUri;
                            ListAccAwait.Add(accJoinClub);
                        }
                    }
                }
            }
        }
        //public async void GetAccountAck(QHMessage msg)
        //{
        //    long numberID = 0;
        //    string fullName = "";
        //    string avatar_uri = "";
        //    var account = new Accounts();
        //    if (msg.TryGetAt((byte)Chat.MsgProfileInfoAckArg.NumberID, ref numberID))
        //    {
        //        if (numberID > 0)
        //        {
        //            var list = new List<AccJoinClub>();
        //            foreach (var item in ListAccAwait)
        //            {
        //                if (numberID == item.IdNumberID)
        //                {
        //                    if (msg.TryGetAt((byte)Chat.MsgProfileInfoAckArg.FullName, ref fullName))
        //                    {
        //                        item.FullName = fullName;
        //                    }
        //                    if(msg.TryGetAt((byte)Chat.MsgProfileInfoAckArg.AvatarURI, ref avatar_uri))
        //                    {
        //                        Accounts acc;
        //                        if (avatar_uri.Equals(""))
        //                        {
        //                            acc = new Accounts { Number_Id = numberID, fullname = fullName, Avatar_Uri = "account.png" };
        //                        }
        //                        else
        //                        {
        //                            acc = new Accounts { Number_Id = numberID, fullname = fullName, Avatar_Uri = avatar_uri };
        //                        }
        //                        bool isInsert = await Helper.Instance().InsertAccount(acc);
        //                        if (!avatar_uri.Equals(""))
        //                        {
        //                            item.Avatar = avatar_uri;
        //                        }
        //                    }
        //                    list.Add(item);
        //                }
        //            }
        //            if (list.Count > 0)
        //            {
        //                foreach(var item in list)
        //                {
        //                    ListRequest.Add(item);
        //                    ListAccAwait.Remove(item);
        //                }
        //            }

        //        }

        //        //if (msg.TryGetAt((byte)Chat.MsgProfileInfoAckArg.AvatarURI, ref Avatar_Uri))
        //        //{
        //        //    Accounts acc;
        //        //    if (Avatar_Uri.Equals(""))
        //        //    {
        //        //        acc = new Accounts { Number_Id = Number_Id, fullname = fullname, Avatar_Uri = "account.png" };
        //        //    }
        //        //    else
        //        //    {
        //        //        acc = new Accounts { Number_Id = Number_Id, fullname = fullname, Avatar_Uri = Avatar_Uri };
        //        //    }
        //        //    bool isInsert = await Helper.Instance().InsertAccount(acc);
        //        //}
        //    }
        //}

    }

}
