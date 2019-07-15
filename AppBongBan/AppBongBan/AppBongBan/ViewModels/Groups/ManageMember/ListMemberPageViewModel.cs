using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Groups.ManageMember
{
    public class ListMemberPageViewModel : BaseViewModel
    {
        private ListMemberVM _listMember;
        public ListMemberVM ListMember { get => _listMember; set { SetProperty(ref _listMember, value); } }
        public ListMemberPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            ListMember = Services.Service.Instiance().MemberPage;
            ListMember.Reset();
        }
        public async void NavigAccount(Accounts acc)
        {
            var param = new NavigationParameters();
            long numberId = (-1);
            if (acc.Number_Id == Helper.Instance().MyAccount.Number_Id)
            {
                numberId = 0;
            }
            else
            {
                numberId = acc.Number_Id;
            }
            param.Add("Account", numberId);
            await Navigation.NavigateAsync("DetailPersonPage", param);
        }
        public async void NavigChall(Accountlocal acc)
        {
            if (acc.Number_Id != Helper.Instance().MyAccount.Number_Id)
            {
                if (acc.Challenge.Equals("pingpong.png"))
                {
                    var param = new NavigationParameters();
                    param.Add("Account", acc.Number_Id);
                    await Navigation.NavigateAsync("ChallengePage", param);
                }
                else
                {
                    UserDialogs.Instance.Toast("Đợi duyệt thách đấu");
                }  
            }
            else
            {
                UserDialogs.Instance.Toast("Bạn không thể thách đấu bản thân bạn");
            }
        }
    }
}
