using AppBongBan.Custom.Badge;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Personal;
using AppBongBan.Views.Notify;
using AppChat.Views.Chat;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.PersonalViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalPage : BottomTabbedPage
    {
        public PersonalPage()
        {
            InitializeComponent();
            AppChat.Helpers.Helper.Instiance().NotifiAction = UpdateNotify;
            AppChat.Helpers.Helper.Instiance().navigateProfilePingPong = NavigationProfile;
            AppChat.Helpers.Helper.Instiance().NotifyAddContactAck = NotifiContact;
            AppChat.Helpers.Helper.Instiance().DCountUnRead += UpdateUnReadCount;
        }
        public void UpdateUnReadCount(long unReadCount)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var page = this as BottomTabbedPage;
                if (Device.RuntimePlatform == Device.Android)
                {
                    if (page?.Tabs.Count > 0)
                    {
                        page.Tabs[1].BadgeCaption = (int)unReadCount;
                    }
                }
                else
                {
                    if (unReadCount <= 0)
                        TabBadge.SetBadgeText(conversation, "");
                    else
                    {
                        if (unReadCount > 5)
                            TabBadge.SetBadgeText(conversation, "5+" + "");
                        else TabBadge.SetBadgeText(conversation, unReadCount + "");
                    }

                }
            });
        }
        public bool isFirst;
        public bool isUpadate;
        PersonalPageViewModel model;
        protected override void OnAppearing()
        {
            if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
            {
                masterDetailPage.IsGestureEnabled = true;
            }
            else if (Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
            {
                nestedMasterDetail.IsGestureEnabled = true;
            }
            base.OnAppearing();
            UpdateUnReadCount(AppChat.Helpers.Helper.Instiance().UnReadMsg);
            // Update lại số lượng tin nhắn chưa đọc
            AppChat.Helpers.Helper.Instiance().NotifyAcceptOrCancelAddFrd += ResponseAccept;
            isUpadate = true;
            if (model == null)
                model = BindingContext as PersonalPageViewModel;
            model.personalPage = this;

            if (!isFirst)
            {
                AppChat.Services.Service.Instiance().inPage = 1;
                Helper.Instance().objMyClub = this;
                Helper.Instance().GetMyClub(Helper.Instance().MyAccount.Number_Id);
                // Gửi yêu cầu lấy số lượng thách đấu
                ChallengeAction.SetGetChallenge(Helper.Instance().MyAccount.Number_Id, 0);
                // Đăng ký delegate để lấy danh sách ClubId
                //  Helper.Instance().GetClubIdAck += GetClubId;
                //  Helper.Instance().GetClubAck += GetClub;
                // Helper.Instance().CheckExistClubMyClubId(Helper.Instance().MyAccount.Number_Id);
                isFirst = true;
            }
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                UpdateNotify();
                return false;
            });
        }

        /// <summary>
        /// Update lại số lượng thông báo ở chuông
        /// </summary>
        /// <param name="page"></param>
        public void UpdateNotify()
        {
            try
            {
                Helper.Instance().CountNotifi = Helper.Instance().CountChallengeNotifi + AppChat.Helpers.Helper.Instiance().CountRequestAddFrd;
                var showNotify = Helper.Instance().CountNotifi.ToString();
                if (Helper.Instance().CountNotifi > 5)
                    showNotify = "5+";
                DependencyService.Get<IToolbarItemBadgeService>().SetBadge(this, Notifi, showNotify, Color.Red, Color.White);
            }
            catch (Exception) { }
        }
        int countClub = 0;
        /// <summary>
        /// Lấy danh sách ClubId
        /// </summary>
        /// <param name="list"></param>
        public void GetClubId(List<long> list)
        {
            countClub = list.Count;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        //  var club = await Helper.Instance().CheckExistClub(item);
                        Helper.Instance().CheckExistClubAsync(item);
                    }
                }
            }
            isFirst = true;
            // Hủy delegate
            Helper.Instance().GetClubIdAck -= GetClubId;
        }
        int clubTotal = 0;
        public void GetClub(Club club)
        {
            clubTotal += 1;
            if (clubTotal == countClub)
            {
                // Hủy delegate
                Helper.Instance().GetClubAck -= GetClub;
            }
            if (Helper.Instance().MyAccount != null)
                if (club.AdminID == Helper.Instance().MyAccount.Number_Id)
                    ChallengeAction.SetGetChallenge(Helper.Instance().MyAccount.Number_Id, club.ClubID);
        }
        public void AddMyClub(List<long> list)
        {
            if (list != null)
            {
                //if (list.Count > 0)
                //{
                //    var lsClub = new List<Club>();
                //    foreach (var item in list)
                //    {
                //        var club = await Helper.Instance().CheckExistClub(item);
                //        if (club != null)
                //            lsClub.Add(club);
                //    }
                //    foreach (var item in lsClub)
                //    {
                //        if (item.AdminID == Helper.Instance().MyAccount.Number_Id)
                //        {
                //            ChallengeAction.SetGetChallenge(Helper.Instance().MyAccount.Number_Id, item.ClubID);
                //        }
                //    }
                //}
                //else
                //{
                //    ChallengeAction.SetGetChallenge(Helper.Instance().MyAccount.Number_Id, 0);
                //}

            }
        }


        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var exitApp = await DisplayAlert("Thông báo", "Bạn có thực sự muốn thoát khỏi SportTV", "Yes", "No");
                if (exitApp)
                {
                    DependencyService.Get<ICloseApp>().CloseApp();
                }
            });
            return true;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            AppChat.Helpers.Helper.Instiance().NotifyAcceptOrCancelAddFrd -= ResponseAccept;
            isUpadate = false;
            if (model != null)
                model.personalPage = null;
        }
        public void ResponseAccept(int status, long NumberID)
        {
            if (status == 1)
            {
                MessagingCenter.Send<App, long>((App)Application.Current, "NotifiAccept", NumberID);
            }
            else
            {
                MessagingCenter.Send<App, long>((App)Application.Current, "NotifiDelete", NumberID);
            }
        }
        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            Title = CurrentPage?.Title;
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Title == "Trò chuyện")
                {
                    //   AppChat.Services.Service.Instiance().conversationPageViewModel.GetMessage();
                    // Sắp xếp lại danh sách nhóm chat
                    AppChat.Helpers.Helper.Instiance().SortGroupChat?.Invoke();
                }
                if (Title == "Trang chủ")
                {
                    //   ((NewsSitePage)CurrentPage).Refresh();
                }
            });

            UpdateNotify();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SearchPage());
        }

        private void Notifi_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NotificationPage());
        }

        public void NavigationProfile()
        {
            if (AppChat.Helpers.Helper.Instiance().profileAccount != null)
            {
                model.NaviAccPage(AppChat.Helpers.Helper.Instiance().profileAccount.NumberId);
            }

        }
        /// <summary>
        /// Xem lai
        /// </summary>
        public void NotifiContact()
        {
            if (Helper.Instance().listAccFriendDetail.Count > 0)
            {
                AppChat.Helpers.Helper.
                    Instiance().
                    UpdateListAddContactAck(Helper.Instance().ConvertAccsToAcc(Helper.Instance().listAccFriendDetail[0]));
                // Helper.Instance().listAccFriendDetail[0].TextStatusFriend = "Đợi chấp nhận";
                MessagingCenter.Send<App, Accounts>((App)Application.Current, "NotifiContact", Helper.Instance().listAccFriendDetail[0]);
                Helper.Instance().listAccFriendDetail.RemoveAt(0);
            }
            else
            {
                if (Helper.Instance().listAddFriend.Count > 0)
                {
                    var numberId = Helper.Instance().listAddFriend[0].Number_Id;
                    Helper.Instance().addFriendStatus[numberId].statusIcon = "person_add_invi.png";
                    AppChat.Models.Login.Account acc = new AppChat.Models.Login.Account() { NumberId = (uint)numberId };
                    AppChat.Helpers.Helper.Instiance().UpdateListAddContactAck(acc);
                    Helper.Instance().listAddFriend.Clear();
                }
            }
        }
    }
}