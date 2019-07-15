using System;
using System.Collections.Generic;
using System.Text;
using AppBongBan.Models.PingPongs;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Checkins
{
    public class CheckinPageViewModel : BaseViewModel
    {
        private CheckinPageVM _checkVM;
        public CheckinPageVM CheckVM { get => _checkVM; set { SetProperty(ref _checkVM, value); } }
        public CheckinPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().CheckinVM.Reset();
            CheckVM = Services.Service.Instiance().CheckinVM;
            CheckVM.Navigation = navigationService;
        }
        /// <summary>
        /// thực hiện lấy object club trong ứng dụng
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                long ClubId = long.Parse(parameters["Club"].ToString());
                if (ClubId > 0)
                {
                    if (Helpers.Helper.Instance().ListClub.TryGetValue(ClubId, out Club club))
                        CheckVM.MyClub = club;
                }
            }
        }
    }
}
