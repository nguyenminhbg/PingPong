using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBongBan.ViewModels.Groups
{
    public class MoreClubPageViewModel : BaseViewModel
    {
        private Club _myClub;

        public Club MyClub { get => _myClub; set { SetProperty(ref _myClub, value); } }
        public MoreClubPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                MyClub = (Club)parameters["Club"];
            }
            else if (parameters.ContainsKey("Content"))
            {
                //var news = (ContentInfo)parameters["Content"];
                //ClubVM.BackListAddNews(news);
                Navigation.GoBackAsync(parameters);
            }
        }
    }
}
