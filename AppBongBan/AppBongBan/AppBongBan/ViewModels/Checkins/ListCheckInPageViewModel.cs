using System;
using System.Collections.Generic;
using System.Text;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Views.Groups.Checkins;
using PingPong;
using Prism.Navigation;
using qhmono;

namespace AppBongBan.ViewModels.Checkins
{
    public class ListCheckInPageViewModel : BaseViewModel
    {
        public ListCheckPageVM ListCheckinVM { get; set; }
        public ListCheckInPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().ListCheckinVM.Reset();
            ListCheckinVM = Services.Service.Instiance().ListCheckinVM;
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                long ClubID = long.Parse(parameters["Club"].ToString());
                ListCheckinVM.MyClub = Helper.Instance().ListClub[ClubID];
                if (ClubID > 0)
                {
                    ListCheckinVM.SendGetCheckins(ClubID, -1);
                }
            }
        }
       
    }
}
