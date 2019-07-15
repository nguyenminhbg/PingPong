using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Groups.MyClubs;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.MyClubs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyClubsPage : ContentPage
    {
        public MyClubsPage()
        {
            InitializeComponent();
            checkGetList = false;
        }
        MyClubsPageViewModel model;
        bool checkGetList = false;
        protected override void OnAppearing()
        {
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            if (model == null)
                model = BindingContext as MyClubsPageViewModel;
            // đăng ký sự kiện lấy tất cả clubID của trang
            Helpers.Helper.Instance().GetClubIdAck += GetClubId;
            // Đăng ký sự kiện lấy Club theo clubId
            Helpers.Helper.Instance().GetClubAck += GetClub;
            // Đăng ký delegate để khi có club trả về sẽ được thêm vào giao diện
            if (checkGetList == false)
            {
                model.GetList();
                checkGetList = true;
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
           // model.MyClubs?.ListClubs?.Clear();
            Helpers.Helper.Instance().GetClubIdAck -= GetClubId;
            Helpers.Helper.Instance().GetClubAck -= GetClub;
        }
        public void GetClubId(List<long> list)
        {
            model?.GetList(list);
        }
        public void GetClub(Club club)
        {
            model?.GetClub(club);
        }
        private void LvDp_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var club = e.Item as Club;
            if (model != null)
                model.NaviDetailClub(club);
        }

        private void LvDp_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null) lv.SelectedItem = null;
        }

        private void ItemClubView_TapMap(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            //stack.BackgroundColor = Color.Gray;
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    stack.BackgroundColor = Color.White;
            //    return false;
            //});
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.ShowMap(parent);
        }

        private void ItemClubView_TapJoin(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            //stack.BackgroundColor = Color.Gray;
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    stack.BackgroundColor = Color.White;
            //    return false;
            //});
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                Services.Service.Instiance().ClubModel.ClubJoinReq(parent);
            stack.BackgroundColor = Color.Transparent;
        }

        private void ItemClubView_TapListUsr(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            //stack.BackgroundColor = Color.Gray;
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    stack.BackgroundColor = Color.White;
            //    return false;
            //});
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.NaviListUse(parent);
        }

        private void ItemClubView_TapCheckin(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            //stack.BackgroundColor = Color.Gray;
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    stack.BackgroundColor = Color.White;
            //    return false;
            //});
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.NaviCheckin(parent);
        }
    }
}