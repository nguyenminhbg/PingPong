using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using qhmono;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AppBongBan.ViewModels.Groups.ManageMember
{
    public class ListMemberVM : ViewModelBase
    {
        public ObservableCollection<Accountlocal> ListAccount { get; set; }
        public List<Accountlocal> ListAccAwait { get; set; }
        public Club MyClub { get; set; }
        public override void Reset()
        {
            ListAccount = new ObservableCollection<Accountlocal>();
            ListAccAwait = new List<Accountlocal>();
            MyClub = null;
        }
        //public Clubs 
        public ListMemberVM()
        {
            ListAccount = new ObservableCollection<Accountlocal>();
            ListAccAwait = new List<Accountlocal>();
        }
        public async void SendGetAccount()
        {
            if (MyClub != null)
            {
                if (MyClub.Numbers != null && MyClub.Numbers.Count > 0)
                {
                    foreach (var item in MyClub.Numbers)
                    {
                        var acc = await Helper.Instance().CheckExistAccount(item);
                        if (acc.Number_Id > 0)
                        {
                            var accJoinClub = new Accountlocal();
                            accJoinClub.Avatar_Uri = acc.Avatar_Uri;
                            accJoinClub.fullname = acc.fullname;
                            accJoinClub.Number_Id = acc.Number_Id;
                            accJoinClub.Level = "Hạng C";
                            accJoinClub.AccepLevel = "Đã Duyệt";
                            accJoinClub.Facebat = "Mặt A - Hãng A";
                            accJoinClub.Blade = "Cốt A - Hãng A";
                            if (!ChallengeAction.ListAccRecive.ContainsKey(acc.Number_Id))
                            {
                                accJoinClub.Challenge = "pingpong.png";
                            }
                            else
                            {
                                accJoinClub.Challenge = "pingpong_invi.png";
                            }
                            accJoinClub.AddFriend = Helper.Instance().IsFriendImg(acc.Number_Id);
                            ListAccount.Add(accJoinClub);
                            if (!Helper.Instance().ListAcclocal.ContainsKey(acc.Number_Id))
                            {
                                Helper.Instance().ListAcclocal.Add(acc.Number_Id, accJoinClub);
                            }
                        }
                    }
                }
            }
        }
    }
}
