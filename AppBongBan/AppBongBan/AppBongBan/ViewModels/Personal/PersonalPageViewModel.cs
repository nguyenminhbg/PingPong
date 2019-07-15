using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models.Db.Content;
using Prism.Navigation;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Personal
{
    public class PersonalPageViewModel : BaseViewModel
    {
        public PersonalPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            AppChat.Helpers.Helper.Instiance().ContactSyncNotify = ContactSyncFinish;
          //  Helper.Instance().challengeNotify = ChallengeNotification;
        }
        public Page personalPage;
        public void ContactSyncFinish()
        {
            Helper.Instance().UpdateNotify(personalPage);
        }
        public void ChallengeNotification()
        {
         // Helper.Instance().UpdateNotify(personalPage);
        }
        public async void NaviAccPage(long NumberId)
        {
            var param = new NavigationParameters();
            param.Add("Account", NumberId);
            await Navigation.NavigateAsync("DetailPersonPage", param);
        }
    }
}
