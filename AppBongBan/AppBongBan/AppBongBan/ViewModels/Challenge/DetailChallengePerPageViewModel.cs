using System;
using System.Collections.Generic;
using System.Text;
using AppBongBan.Models;
using AppBongBan.ViewModels.Actions;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Challenge
{
    public class DetailChallengePerPageViewModel : BaseViewModel
    {
        private ChallengeInfo _chall;

        public ChallengeInfo Chall { get => _chall; set { SetProperty(ref _chall, value); } }
        public bool IsPerson { get; set; }
        public DetailChallengePerPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            IsPerson = true;
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("SendID"))
            {
                var SendID = long.Parse(parameters["SendID"].ToString());
                Chall = ChallengeAction.ListSendAcc[SendID];
                IsPerson = true;
            }
            else if (parameters.ContainsKey("SendClubID"))
            {
                var SendClubID = long.Parse(parameters["SendClubID"].ToString());
                Chall = ChallengeAction.ListSendClub[SendClubID];
                IsPerson = false;
            }
            else if (parameters.ContainsKey("TargetID"))
            {
                var TargetID = long.Parse(parameters["TargetID"].ToString());
                Chall = ChallengeAction.ListAccRecive[TargetID];
            }

        }
        /// <summary>
        /// chấp nhận thách đấu từ cá nhân khác
        /// </summary>
        public void OnBackAccept()
        {
            if (Chall != null && Chall.ChallengeID > 0)
            {
                var param = new NavigationParameters();
                param.Add("SendID", Chall.SenderID);
                Navigation.GoBackAsync(param);
            }
        }
        public void OnBackDelete()
        {
            if (Chall != null && Chall.ChallengeID > 0)
            {
                var param = new NavigationParameters();
                param.Add("SendID", Chall.SenderID);
                Navigation.GoBackAsync(param);
            }
        }
        public void OnBackDeleteClub()
        {
            if (Chall != null && Chall.ChallengeID > 0)
            {
                var param = new NavigationParameters();
                param.Add("SendClubID", Chall.SenderID);
                Navigation.GoBackAsync(param);
            }
        }

        public void OnBackAcceptClub()
        {
            if (Chall != null && Chall.ChallengeID > 0)
            {
                var param = new NavigationParameters();
                param.Add("SendClubID", Chall.SenderID);
                Navigation.GoBackAsync(param);
            }
        }
    }
}
