using AppBongBan.Dependency;
using AppBongBan.Helpers;
using Prism.Mvvm;
using Prism.Navigation;
using qhmono;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels
{
    public class BaseViewModel : BindableBase, INavigationAware
    {
        protected INavigationService Navigation { get; }
        public bool IsFrist { get; set; }
        public bool IsPopup { get; set; }
        public bool IsDeleRecive { get; set; }
        public BaseViewModel(INavigationService navigationService)
        {
           Navigation = navigationService;
            IsFrist = false;
            IsPopup = false;
            IsDeleRecive = true;
            AppChat.Helpers.Helper.Instiance().UpdateProfileFinish = BackUpdateProfile;
        }
        public void BackUpdateProfile()
        {
            // Tạm thời dùng như  này sau phải sửa để tham chiều tới  Helper.Instance().ListAccounts[Helper.Instance().MyAccount.Number_Id.ToString()]
            Device.BeginInvokeOnMainThread(() =>
            {
                //  foreach(var content in Helper.Instance().)
                Helper.Instance().ListAccounts[Helper.Instance().MyAccount.Number_Id.ToString()].fullname = AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().MyAccount.Number_Id].FullName;
                Helper.Instance().ListAccounts[Helper.Instance().MyAccount.Number_Id.ToString()].Avatar_Uri = AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().MyAccount.Number_Id].AvartaURI;

                //foreach (var account in Helper.Instance().ListContent)
                //{
                //    if (account.Value.Accounts.Number_Id == AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().MyAccount.Number_Id].NumberId)
                //    {
                //        account.Value.Accounts.fullname = AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().MyAccount.Number_Id].FullName;
                //        account.Value.Accounts.Avatar_Uri = AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().MyAccount.Number_Id].AvartaURI;
                //    }
                //}
                Navigation.GoBackAsync();
            });
        }
        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }
    }
}
