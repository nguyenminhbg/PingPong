using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using PingPong;
using Prism.Navigation;
using qhmono;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Actions
{
    public class ChallengeAction
    {
        /// <summary>
        /// tham số danh sách các thách đấu mà người dùng đã gửi đi
        /// </summary>
        public static Dictionary<long, ChallengeInfo> SeqIDs = new Dictionary<long, ChallengeInfo>();
        /// <summary>
        /// Danh sách các thách đấu mà người dùng là admin hoặc được thách đấu (người dùng là admin club)
        /// </summary>
        public static Dictionary<long, ChallengeInfo> ListSendAcc = new Dictionary<long, ChallengeInfo>();
        public static Dictionary<long, ChallengeInfo> ListSendClub = new Dictionary<long, ChallengeInfo>();
        /// <summary>
        /// danh sách các mà account đã gửi thách đấu đến id club và id account
        /// </summary>
        public static Dictionary<long, ChallengeInfo> ListAccRecive = new Dictionary<long, ChallengeInfo>();
        public static Dictionary<long, ChallengeInfo> ListClubRecive = new Dictionary<long, ChallengeInfo>();
        /// <summary>
        /// danh sách các id acc người dùng gửi đi 
        /// </summary>
        public static Dictionary<long, long> SeqIDsAccTarget = new Dictionary<long, long>();
        /// <summary>
        /// danh sách các id club mà người dùng gửi đi
        /// </summary>
        public static Dictionary<long, long> SeqIDsClubTarget = new Dictionary<long, long>();


        /// <summary>
        /// gửi bản tin thách đấu cá nhân lên server
        /// </summary>
        /// <param name="SenderID"></param>
        /// <param name="TargetID"></param>
        /// <param name="content"></param>
        /// <param name="startTimer"></param>
        /// <param name="EndTime"></param>
        public static void ChallengePersionReq(long SenderID, long TargetID,string content, long startTimer, long EndTime)
        {
            if (!SeqIDsAccTarget.ContainsKey(TargetID))
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_INVITE_CHALLENGE_REQ);
                var Challenge = new ChallengeInfo();
                Challenge.SenderID = SenderID;
                Challenge.TargetType = 0;
                Challenge.TargetID = TargetID;
                Challenge.Content = content;
                Challenge.StartTime = startTimer;
                Challenge.EndTime = EndTime;
                Challenge.SeqID = SeqIDs.Count + 1;
                msg.SetAt((byte)MsgInviteChallengeReqArg.SenderID, new QHNumber(SenderID));
                msg.SetAt((byte)MsgInviteChallengeReqArg.TargetType, new QHNumber(0));
                msg.SetAt((byte)MsgInviteChallengeReqArg.TargetID, new QHNumber(TargetID));
                msg.SetAt((byte)MsgInviteChallengeReqArg.Content, new QHString(content));
                msg.SetAt((byte)MsgInviteChallengeReqArg.StartTime, new QHNumber(startTimer));
                msg.SetAt((byte)MsgInviteChallengeReqArg.EndTime, new QHNumber(EndTime));
                msg.SetAt((byte)MsgInviteChallengeReqArg.SeqID, new QHNumber(SeqIDs.Count + 1));
                if (!SeqIDs.ContainsKey(SeqIDs.Count + 1))
                {
                    SeqIDs.Add(SeqIDs.Count + 1, Challenge);
                }
                Services.Service.Instiance().SendMessage(msg);
            }
            else
            {
                UserDialogs.Instance.Toast("Đang đợi duyệt từ đối thủ hoặc đợi 15 phút để tiếp tục gửi thách đấu");
            }
        }
        /// <summary>
        /// Gửi bản tin thách đấu lên club
        /// </summary>
        /// <param name="SenderID"></param>
        /// <param name="TargetID"></param>
        /// <param name="content"></param>
        /// <param name="startTimer"></param>
        /// <param name="EndTime"></param>
        public static void ChallengeClub(long SenderID, long TargetClubID, string content, long startTimer, long EndTime)
        {
            if (!SeqIDsClubTarget.ContainsKey(TargetClubID))
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_INVITE_CHALLENGE_REQ);
                var Challenge = new ChallengeInfo();
                Challenge.SenderID = SenderID;
                Challenge.TargetType = 1;
                Challenge.TargetID = TargetClubID;
                Challenge.Content = content;
                Challenge.StartTime = startTimer;
                Challenge.EndTime = EndTime;
                Challenge.SeqID = SeqIDs.Count + 1;
                msg.SetAt((byte)MsgInviteChallengeReqArg.SenderID, new QHNumber(SenderID));
                msg.SetAt((byte)MsgInviteChallengeReqArg.TargetType, new QHNumber(1));
                msg.SetAt((byte)MsgInviteChallengeReqArg.TargetID, new QHNumber(TargetClubID));
                msg.SetAt((byte)MsgInviteChallengeReqArg.Content, new QHString(content));
                msg.SetAt((byte)MsgInviteChallengeReqArg.StartTime, new QHNumber(startTimer));
                msg.SetAt((byte)MsgInviteChallengeReqArg.EndTime, new QHNumber(EndTime));
                msg.SetAt((byte)MsgInviteChallengeReqArg.SeqID, new QHNumber(SeqIDs.Count + 1));
                if (!SeqIDs.ContainsKey(SeqIDs.Count + 1))
                {
                    SeqIDs.Add(SeqIDs.Count + 1, Challenge);
                }
                Services.Service.Instiance().SendMessage(msg);
            }
            else
            {
                UserDialogs.Instance.Toast("Đang đợi duyệt từ Admin hoặc đợi 15 phút để tiếp tục gửi thách đấu");
            }
        }
        /// <summary>
        /// gửi bản tin trả lời thách đấu từ admin hoặc cá nhân
        /// </summary>
        /// <param name="NumberID"></param>
        /// <param name="ChallengeID"></param>
        public static void AcceptOrCancelChall(long ChallengeID, long Accept)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_INVITE_CHALLENGE_CNF);
            msg.SetAt((byte)MsgInviteChallengeCnfArg.ChallengeID, new QHNumber(ChallengeID));
            msg.SetAt((byte)MsgInviteChallengeCnfArg.Accept, new QHNumber(Accept));
            Services.Service.Instiance().SendMessage(msg);
        }
        /// <summary>
        /// gửi bản tin xóa lời mời thách đấu với ngườu khác
        /// </summary>
        /// <param name="NumberID">Number của người chủ lời mời</param>
        /// <param name="ChallengeID">ID của lời mời thách đấu</param>
        public static void CancelChallenge(long NumberID, long ChallengeID)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CANCEL_CHALLENGE_REQ);
            msg.SetAt((byte)MsgCancelChallengeReqArg.NumberID, new QHNumber(NumberID));
            msg.SetAt((byte)MsgCancelChallengeReqArg.ChallengeID, new QHNumber(ChallengeID));
            Services.Service.Instiance().SendMessage(msg);
        }

        /// <summary>
        /// Indicator cho các cá  nhân và club
        /// </summary>
        /// <param name="msg"></param>
        public void OnReciveChallengeInd(QHMessage msg)
        {

        }
        public static void OnReciveAckChall(QHMessage msg)
        {

            long Error = 0;       // QHNumber
            long ChallengeID = 0; // QHNumber(ID của lời mời)
            long SeqID = 0;       // QHNumber(ID tự sinh do client gửi lên trong MsgInviteChallengeReqArg)
            long RemainTime = 0; // QHNumber (Thời gian đếm ngược tính bằng giây khi có thể gửi lời mời thách đấu tiếp theo)
            if (msg.TryGetAt((byte)MsgInviteChallengeAckArg.Error, ref Error))
            {

            }
            if (msg.TryGetAt((byte)MsgInviteChallengeAckArg.ChallengeID, ref ChallengeID))
            {

            }
            if (msg.TryGetAt((byte)MsgInviteChallengeAckArg.SeqID, ref SeqID))
            {

            }
            if (msg.TryGetAt((byte)MsgInviteChallengeAckArg.RemainTime, ref RemainTime))
            {

            }
            var ChallengeError = (InviteChallengeError)Error;
            switch (ChallengeError)
            {
                case InviteChallengeError.SUCCESS:
                    if (SeqIDs.ContainsKey(SeqID))
                    {
                        var Chall = SeqIDs[SeqID];
                        //thực hiện thách đấu với cá nhân
                        if (Chall.TargetType == 0)
                        {
                            if (Helpers.Helper.Instance().ListAcclocal.ContainsKey(Chall.TargetID))
                            {
                                Helpers.Helper.Instance().ListAcclocal[Chall.TargetID].Challenge = "pingpong_invi.png";
                                NotifiDialog.Initiance().DialogChallengePer(Helpers.Helper.Instance().ListAcclocal[Chall.TargetID].fullname);
                                if (!SeqIDsAccTarget.ContainsKey(Chall.TargetID))
                                {
                                    SeqIDsAccTarget.Add(Chall.TargetID, SeqID);
                                    ListAccRecive.Add(Chall.TargetID, Chall);
                                }
                            }

                        }
                        //thực hiện thách đấu với club
                        else
                        {
                            if (Helpers.Helper.Instance().ListClub.ContainsKey(Chall.TargetID))
                            {
                                Helpers.Helper.Instance().ListClub[Chall.TargetID].Challenge = "pingpong_invi.png";
                                NotifiDialog.Initiance().DialogChallengeClub(Helpers.Helper.Instance().ListClub[Chall.TargetID].ClubName);
                                if (!SeqIDsClubTarget.ContainsKey(Chall.TargetID))
                                {
                                    SeqIDsClubTarget.Add(Chall.TargetID, SeqID);
                                    ListClubRecive.Add(Chall.TargetID,Chall);
                                }
                            }
                        }
                        AppChat.Helpers.Helper.Instiance().UpdateProfileFinish?.Invoke();
                    }
                    break;
                case InviteChallengeError.ERR_TOO_FREQUENTLY:
                    break;
            }
        }
        /// <summary>
        /// lấy danh sách các thách đấu của người dùng
        /// </summary>
        /// <param name="NumberID"></param>
        public static void SetGetChallenge(long NumberID, long ClubID)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_LIST_CHALLENGES_REQ);
            msg.SetAt((byte)MsgListChallengeReqArg.NumberID, new QHNumber(NumberID));
            if (ClubID > 0)
                msg.SetAt((byte)MsgListChallengeReqArg.ClubID, new QHNumber(ClubID));
            Services.Service.Instiance().SendMessage(msg);
        }

    }
}
