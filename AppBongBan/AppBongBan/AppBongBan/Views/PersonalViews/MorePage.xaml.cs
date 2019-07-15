using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Personal;
using AppBongBan.Services;
using AppBongBan.Views.IView;
using AppChat.Views.Chat;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FFImageLoading.Forms;
using AppBongBan.Views.Clubs;

namespace AppBongBan.Views.PersonalViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MorePage : BaseContentPage
    {
        public MorePage()
        {
            InitializeComponent();
            firstOpenSite = false;
        }
        MorePageViewModel model;
        bool firstOpenSite = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            checkAddclub = false;
            if (model == null)
            {
                model = this.BindingContext as MorePageViewModel;
            }
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "Login", (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    AccountName.Text = Helper.Instance().MyAccount.fullname;
                    if (Helper.Instance().MyAccount.Avatar_Uri != "")
                    {
                        Avatar.Source = Helper.Instance().MyAccount.Avatar_Uri;
                    }
                });
            });
            if (!firstOpenSite)
            {
                if (Helper.Instance().MyAccount !=null)
                {
                    AccountName.Text = Helper.Instance().MyAccount.fullname;
                    if (string.IsNullOrEmpty(Helper.Instance().MyAccount.Avatar_Uri))
                        Avatar.Source = "account.png";
                    else
                        Avatar.Source = Helper.Instance().MyAccount.Avatar_Uri;
                    AccountName.Text = Helper.Instance().MyAccount.fullname;
                }
                imgEdit.Source = "edit_picture.png";
                imgSearch.Source = "Search_location.png";
                txtSearch.Text = "Tìm kiếm khu vực";
                imgClub.Source = "add_club.png";
                txtAddclub.Text = "Thêm câu lạc bộ";
                imgMyclub.Source = "club.png";
                txtMyClub.Text = "Câu lạc bộ";
                txtLoguot.Text = "Đăng xuất";
            }
          
            //xóa ảnh đã crop lên server
            if (!Helper.uriCover.Equals(""))
            {
                var list = new List<string>();
                list.Add(Helper.uriCover);
                if (!Helper.uriCacheCover.Equals(""))
                {
                    list.Add(Helper.uriCacheCover);
                    Helper.uriCacheCover = "";
                }
                DependencyService.Get<IMediaService>().ClearFiles(list);
            }

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }
        /// <summary>
        /// chức năng tìm kiếm khu vực
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //var stacklayout = sender as StackLayout;
            //stacklayout.BackgroundColor = Color.Gray;
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    stacklayout.BackgroundColor = Color.Default;
            //    return false;
            //});
            if (model != null)
            {
                model.NavSearchPageExecute();
                //  Navigation.PushAsync(new SearchLocationTabPage());
            }
        }
        bool checkAddclub = false;
        /// <summary>
        /// Chức năng add club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            if (checkAddclub == false)
            {
                //var stacklayout = sender as StackLayout;
                //stacklayout.BackgroundColor = Color.Gray;
                //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                //{
                //    stacklayout.BackgroundColor = Color.Default;
                //    return false;
                //});
                if (model != null)
                {
                    model.NavAddClubExecute();
                }
                checkAddclub = true;
            }
        }
        /// <summary>
        /// Xem danh sách các club có thành viên
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            //var stacklayout = sender as StackLayout;
            //stacklayout.BackgroundColor = Color.Gray;
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    stacklayout.BackgroundColor = Color.Default;
            //    return false;
            //});
            if (model != null)
                model.NaviMyClubs();
            //NotifiDialog.Initiance().DialogDevelop();
        }

        private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            var img = sender as CachedImage;
            img.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                img.BackgroundColor = Color.Default;
                return false;
            });
            if (model != null)
                model.NaviMyAccount();
        }
        /// <summary>
        /// Đăng xuất 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            //var stacklayout = sender as StackLayout;
            //stacklayout.BackgroundColor = Color.Gray;
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            //{
            //    stacklayout.BackgroundColor = Color.Default;
            //    return false;
            //});
            bool isLogout = await DisplayAlert("Thông báo", "Bạn muốn đăng xuất tài khoản hiện tại", "Ok", "Cancel");
            if (isLogout)
            {
                AppChat.Services.Service.Instiance().contactsPageViewModel.contactsTemp?.Clear();
                AppChat.Services.Service.Instiance().conversationPageViewModel.ListTemp?.Clear();
                Service.Instiance().NewsSiteVM.ListNewsTemp.Clear();
                Service.Instiance().NewsSiteVM.ListNews.Clear();
                Service.Instiance().NewsSiteVM.moreLoad = false;
                Service.Instiance().NewsSiteVM.homePageInfo = null;
                ContactsPage.firstOpenSite = false;
                ConversationPage.firstOpenSite = false;
                Helper.Instance().checkOpenNewsSite = false;
                AppChat.Services.Service.Instiance().myProfilePageViewModel.Logout();
                Helper.Instance().ListContent.Clear();
                Helper.Instance().ListClub.Clear();
                Helper.Instance().ListAcclocal.Clear();
                Helper.Instance().ListAccountsCached.Clear();
                DeleteAllAccounts();
                Helper.Instance().MyAccount.Avatar_Uri = "";
                Helper.Instance().MyAccount.fullname = "";
                Helper.Instance().MyAccount = null;
                Helper.Instance().AccountChat = null;
                Helper.Instance().CountChallengeNotifi = 0;
               AppChat.Helpers. Helper.Instiance().UnReadMsg = 0;
                AppChat.Helpers.Helper.Instiance().CountRequestAddFrd = 0;
                Helper.Instance().ListAccounts.Clear();
                ChallengeAction.SeqIDs.Clear();
                ChallengeAction.SeqIDsAccTarget.Clear();
                ChallengeAction.SeqIDsClubTarget.Clear();
                ChallengeAction.ListAccRecive.Clear();
                ChallengeAction.ListClubRecive.Clear();
                ChallengeAction.ListSendAcc.Clear();
                ChallengeAction.ListSendClub.Clear();
                Service.Instiance().NotifiChall.Reset();
                Helper.Instance().CountChallengeNotifi = 0;
                if (Service.Instiance().NotifiChall.ListChallengesPer != null)
                {
                    Service.Instiance().NotifiChall.ListChallengesPer.Clear();
                }
                if (Service.Instiance().NotifiChall.ListChallengesClub != null)
                {
                    Service.Instiance().NotifiChall.ListChallengesClub.Clear();
                }
                // Thay layout ảnh đại diện
                HomePage.avartaChange();
                // Kích hoạt delegate
                MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "Logout");
            }
        }
        // Xóa tất cả các bảng Account
        public void DeleteAllAccounts()
        {
            string deleteAll = string.Format("DELETE FROM Account");
            try
            {
                Helper.Database.Database.ExecuteAsync(deleteAll);
            }
            catch (Exception)
            {

            }
        }
    }
}