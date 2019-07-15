using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Groups.ManageMember
{
    public class ListUserReqPageViewModel : BaseViewModel
    {
        private ListUserReqVM _listUserReq;
        public ListUserReqVM ListUserReq { get => _listUserReq; set { SetProperty(ref _listUserReq, value); } }
        public ListUserReqPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            ListUserReq = Services.Service.Instiance().listUserReqModel;
            ListUserReq.Reset();
        }
    }
}
