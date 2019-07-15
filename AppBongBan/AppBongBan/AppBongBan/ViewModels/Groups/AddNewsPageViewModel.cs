using System;
using System.Collections.Generic;
using System.Text;
using AppBongBan.Models.PingPongs;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Groups
{
    public class AddNewsPageViewModel : BaseViewModel
    {
        private AddNewsVM _addNews;

        public AddNewsVM AddNews { get => _addNews; set { SetProperty(ref _addNews, value); } }
        public AddNewsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().AddNewModel.Reset();
            AddNews = Services.Service.Instiance().AddNewModel;
            AddNews.navigationService = navigationService;
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                var club = (Club)parameters["Club"];
                AddNews.NewClub = club;
            }
        }
    }
}
