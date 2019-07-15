using System;
using System.Collections.Generic;
using System.Text;
using AppBongBan.Models;
using AppBongBan.ViewModels.Actions;
using PingPong;
using Prism.Navigation;
using qhmono;

namespace AppBongBan.ViewModels.Notify
{
    public class ListChallengePageViewModel : BaseViewModel
    {
        private ListChallengePageVM _notifi;

        public ListChallengePageVM Notifi { get => _notifi; set { SetProperty(ref _notifi, value); } }
        public ListChallengePageViewModel(INavigationService navigationService) : base(navigationService)
        {
         //   Services.Service.Instiance().NotifiChall.Reset();
            Notifi = Services.Service.Instiance().NotifiChall;
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("SendID"))
            {
                var SendID = long.Parse(parameters["SendID"].ToString());
                if (ChallengeAction.ListSendAcc.ContainsKey(SendID))
                {
                    var Chall = ChallengeAction.ListSendAcc[SendID];
                    ChallengeAction.ListSendAcc.Remove(SendID);
                    Notifi.ListChallengesPer.Remove(Chall);
                }
               
            }
            else if (parameters.ContainsKey("SendClubID"))
            {
                var SendClubID = long.Parse(parameters["SendClubID"].ToString());
                if (ChallengeAction.ListSendClub.ContainsKey(SendClubID))
                {
                    var Chall = ChallengeAction.ListSendClub[SendClubID];
                    ChallengeAction.ListSendClub.Remove(SendClubID);
                    Notifi.ListChallengesClub.Remove(Chall);
                }
            }
        }
        public void NaviDetailChanllenge(ChallengeInfo chall)
        {
            var param = new NavigationParameters();
            param.Add("SendID", chall.SenderID);
            Navigation.NavigateAsync("DetailChallengePerPage", param);
        }
        public void NaviDetailChallengeClub(ChallengeInfo chall)
        {
            var param = new NavigationParameters();
            param.Add("SendClubID", chall.SenderID);
            Navigation.NavigateAsync("DetailChallengePerPage", param);
        }

    }
}
