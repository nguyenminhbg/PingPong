using System;
using System.Collections.Generic;
using System.Text;
using AppBongBan.Dependency;
using AppBongBan.Models;
using AppBongBan.ViewModels.Actions;
using DLToolkit.Forms.Controls;
using PingPong;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Challenge
{
    public class ListChallengeVM : ViewModelBase
    {
        public FlowObservableCollection<ChallengeInfo> ListChallAcc { get; set; }
        public FlowObservableCollection<ChallengeInfo> ListChallClub { get; set; }
        public ListChallengeVM()
        {
            ListChallAcc = new FlowObservableCollection<ChallengeInfo>();
            ListChallClub = new FlowObservableCollection<ChallengeInfo>();

            foreach (var item in ChallengeAction.ListAccRecive)
            {
                ListChallAcc.Add(item.Value);
            }
            foreach (var item in ChallengeAction.ListClubRecive)
            {
                ListChallClub.Add(item.Value);
            }
        }
        public override void Reset()
        {
            ListChallAcc = new FlowObservableCollection<ChallengeInfo>();
            ListChallClub = new FlowObservableCollection<ChallengeInfo>();
        }
        /// <summary>
        /// Nhận bản tin trả lời thách đấu từ server trả về cho người nhận được
        /// </summary>
        /// <param name="msg"></param>
        public async void OnReciveCancelChallengeInd(QHMessage msg)
        {
            long SenderID = 0;   // QHNumber
            long TargetType = 0; // QHNumber{ 0 : Player, 1:Club }
            long TargetID = 0;   // QHNumber ID of TargetType
            string Content = "";    // QHString (Nội dung text hoặc json của Challenges)
            long StartTime = 0;  // QHNumber Thời gian thách đấu bắt đầu
            long EndTime = 0;    // QHNumber Thời gian thách đấu kết thúc
            long ChallengeID = 0;// QHNumber ID của lời mời thách đấu
            long Accept = 0;
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.SenderID, ref SenderID)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.TargetType, ref TargetType)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.TargetID, ref TargetID)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.Content, ref Content)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.StartTime, ref StartTime)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.EndTime, ref EndTime)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.ChallengeID, ref ChallengeID)) { }
            if (msg.TryGetAt((byte)MsgInviteChallengeCnfIndArg.Accept, ref Accept)) { }
            if (Accept == 0)
            {
                if (TargetType == 0)
                {
                    if (ChallengeAction.ListAccRecive.ContainsKey(TargetID))
                    {
                        ListChallAcc.Remove(ChallengeAction.ListAccRecive[TargetID]);
                        ChallengeAction.ListAccRecive.Remove(TargetID);
                        ChallengeAction.SeqIDsAccTarget.Remove(TargetID);
                        if (Helpers.Helper.Instance().ListAcclocal.ContainsKey(TargetID))
                            Helpers.Helper.Instance().ListAcclocal[TargetID].Challenge = "pingpong.png";
                    }
                }
                else
                {
                    if (ChallengeAction.ListClubRecive.ContainsKey(TargetID))
                    {
                        ListChallAcc.Remove(ChallengeAction.ListClubRecive[TargetID]);
                        ChallengeAction.ListClubRecive.Remove(TargetID);
                        ChallengeAction.SeqIDsClubTarget.Remove(TargetID);
                        if (Helpers.Helper.Instance().ListClub.ContainsKey(TargetID))
                            Helpers.Helper.Instance().ListClub[TargetID].Challenge = "pingpong.png";
                    }
                }
            }
            else
            {

                if (TargetType == 0)
                {

                    if (ChallengeAction.ListAccRecive.ContainsKey(TargetID))
                    {
                        ChallengeAction.ListAccRecive.Remove(TargetID);
                        ChallengeAction.SeqIDsAccTarget.Remove(TargetID);
                        if (Helpers.Helper.Instance().ListAcclocal.ContainsKey(TargetID))
                            Helpers.Helper.Instance().ListAcclocal[TargetID].Challenge = "pingpong.png";
                    }
                    // Yêu cầu Server trả về fullName để hiển thị thông báo
                    var accTarget = await Helpers.Helper.Instance().CheckExistAccount(TargetID);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", accTarget.fullname + " đã chấp nhận mời thách đấu của bạn", (int)Helpers.Notifi.Challenge, 0);
                    });

                }
                else
                {
                    if (ChallengeAction.ListClubRecive.ContainsKey(TargetID))
                    {
                        ChallengeAction.ListClubRecive.Remove(TargetID);
                        ChallengeAction.SeqIDsClubTarget.Remove(TargetID);
                        if (Helpers.Helper.Instance().ListClub.ContainsKey(TargetID))
                            Helpers.Helper.Instance().ListClub[TargetID].Challenge = "pingpong.png";
                    }
                    var club = await Helpers.Helper.Instance().CheckExistClub(TargetID);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<ILocalNotificationService>().LocalNotification("AppBongBan", "Câu lạc bộ " + club.ClubName + " đã chấp nhận mời thách đấu của bạn", (int)Helpers.Notifi.Challenge, 0);
                    });

                }
            }
        }
    }
}
