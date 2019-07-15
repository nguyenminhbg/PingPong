using Acr.UserDialogs;
using AppBongBan.Helpers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AppBongBan.Services;
using AppBongBan.ViewModels;

namespace AppBongBan.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlashPage : ContentPage
    {
        public FlashPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        FlashPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = this.BindingContext as FlashPageViewModel;
            //if (Device.OS == TargetPlatform.iOS)
          //  UserDialogs.Instance.ShowLoading();
            if (Helper.Instance().CheckLogin())
            {
                // Gán nameDataBase để khởi tạo slqLite và AccountChat
                AppChat.Helpers.Helper.Instiance().nameDataBase = Helper.Instance().MyAccount.Number_Id.ToString();
                AppChat.Helpers.Helper.Instiance().myAccount = Helper.Instance().AccountChat;
                var list = AppChat.Helpers.Helper.Instiance().database.GetListAcc(Helper.Instance().MyAccount.Number_Id).Result;
                foreach (var item in list)
                {
                    Helper.Instance().MyAccount.Last_Time_Sync_Contact = item.last_time_sync_contact;
                }
                if (AppChat.Helpers.Helper.Instiance().accountCached.ContainsKey(Helper.Instance().AccountChat.NumberId))
                    AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().AccountChat.NumberId] = Helper.Instance().AccountChat;
                else AppChat.Helpers.Helper.Instiance().accountCached.Add(Helper.Instance().AccountChat.NumberId, Helper.Instance().AccountChat);
                // Gửi bản tin lấy dữ liệu cho trang chủ
                Helper.Instance().GetContactGroupChat = true;
                Service.Instiance().NewsSiteVM.MsgHomePageReqSend();
            }
            Device.StartTimer(TimeSpan.FromMilliseconds(2000), () =>
            {
                if (model != null)
                    model.Navigate();
              //  Navigation.PushAsync(new HomePage());
                return false;
            });
        }
    }
}