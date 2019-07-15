using AppBongBan.Helpers;
using AppBongBan.Models.Notify;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBongBan.ViewModels.Notify
{
   public class NotificationPageViewModel: ViewModelBase
    {
        private NotifyModel _notify;
        public NotifyModel notify
        {
            get { return _notify; }
            set
            {
                SetProperty(ref _notify, value);
            }
        }
        public NotificationPageViewModel()
        {
            notify = new NotifyModel();
            notify.addFriendReq = Helper.Instance().CountNotifi.ToString();
        }

    }
}
