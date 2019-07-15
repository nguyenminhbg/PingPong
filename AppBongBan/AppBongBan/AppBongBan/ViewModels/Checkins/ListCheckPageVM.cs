using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using DLToolkit.Forms.Controls;
using PingPong;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Checkins
{
    public class ListCheckPageVM : ViewModelBase
    {
        private Club _myClub;

        public FlowObservableCollection<CheckinInfo> ListCheckIn { get; set; }
        public Club MyClub { get => _myClub; set { SetProperty(ref _myClub, value); } }
        public ListCheckPageVM()
        {
            ListCheckIn = new FlowObservableCollection<CheckinInfo>();
            MyClub = new Club();
        }
        public override void Reset()
        {
            base.Reset();
            MyClub = new Club();
            ListCheckIn = new FlowObservableCollection<CheckinInfo>();
        }
        /// <summary>
        /// lấy danh sách check in trong club trong ứng dụng
        /// </summary>
        /// <param name="msg"></param>
        private List<CheckinInfo> onGetListCheckins(QHMessage msg)
        {
            Debug.WriteLine("msg: " + msg.JSONString());
            var list = new List<CheckinInfo>();
            //long Error = 0;
            QHTable UserCheckInList = new QHTable();
            long CheckInID = 0;
            long ClubID = 0;
            long NumberID = 0;
            long CheckInTime = 0;
            string Content = "";
            if (msg.TryGetAt((byte)MsgGetClubCheckInListAckArg.UserCheckInList, ref UserCheckInList))
            {
                for (int i = 0; i < UserCheckInList.GetRowCount(); i++)
                {
                    var checkin = new CheckinInfo();
                    if (UserCheckInList.TryGetAt(i, 0, ref CheckInID))
                    {
                        checkin.CheckInID = CheckInID;
                    }
                    if (UserCheckInList.TryGetAt(i, 1, ref ClubID))
                    {
                        checkin.ClubID = ClubID;
                    }
                    if (UserCheckInList.TryGetAt(i, 2, ref NumberID))
                    {
                        checkin.NumberID = NumberID;
                    }
                    if (UserCheckInList.TryGetAt(i, 3, ref CheckInTime))
                    {
                        checkin.Time = CheckInTime;
                    }
                    if (UserCheckInList.TryGetAt(i, 4, ref Content))
                    {
                        checkin.Content = Content;
                    }
                    if (Helpers.Helper.Instance().ListCheckInfo.ContainsKey(CheckInID))
                    {
                        Helpers.Helper.Instance().ListCheckInfo[CheckInID] = checkin;
                    }
                    else
                    {
                        Helpers.Helper.Instance().ListCheckInfo.Add(CheckInID, checkin);
                    }
                    list.Add(checkin);
                }
            }
            return list;
        }
        public void OnReciveMsg(QHMessage msg)
        {
            var list = onGetListCheckins(msg);
            if (list != null)
            {
                if (list.Count > 0)
                {
                    IsLoading = true;
                    if (IsMore)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ListCheckIn.AddRange(list);
                        });
                        IsMore = false;
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ListCheckIn.Clear();
                            ListCheckIn.AddRange(list);
                        });

                    }
                }
                else
                {
                    IsLoading = false;
                }
            }
            if (IsMore)
            {
                IsMore = false;
            }
        }
        public void Loading(CheckinInfo checkin)
        {
            if (checkin != null)
            {
                if (ListCheckIn.Count == 0 || ListCheckIn.Count % 10 != 0)
                {
                    IsLoading = false;
                    return;
                }       
                if (checkin.Equals(ListCheckIn[ListCheckIn.Count - 1]) && !IsMore && IsLoading)
                {
                    Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                    {
                        IsMore = true;
                        Debug.WriteLine("id cuối cùng club trong danh sách: " + checkin.CheckInID);
                        More(checkin);
                        return false;
                    });

                }
            }
            else
                return;
        }
        public void SendGetCheckins(long ClubID, long StartID)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_GET_CLUB_CHECK_IN_LIST_REQ);
            var NumberID = Helpers.Helper.Instance().MyAccount.Number_Id;
            if (NumberID > 0)
            {
                msg.SetAt((byte)MsgGetClubCheckInListReqArg.NumberID, new QHNumber(NumberID));
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNumber();
                return;
            }
            msg.SetAt((byte)MsgGetClubCheckInListReqArg.ClubID, new QHNumber(ClubID));
            msg.SetAt((byte)MsgGetClubCheckInListReqArg.FromCheckInID, new QHNumber(StartID));
            msg.SetAt((byte)MsgGetClubCheckInListReqArg.Limit, new QHNumber(-10));
            Services.Service.Instiance().SendMessage(msg);
        }
        private void More(CheckinInfo checkin)
        {
            SendGetCheckins(checkin.ClubID, checkin.CheckInID);
        }

    }

}
