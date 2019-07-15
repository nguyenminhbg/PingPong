using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Notify;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Notify
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListChallengePage : ContentPage
    {
        public ListChallengePage()
        {
            InitializeComponent();
            ChangeTab(true);
        }
        ListChallengePageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = BindingContext as ListChallengePageViewModel;
        }
        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
        }
        /// <summary>
        /// ấn chấp nhận thách đấu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var parent = btn.BindingContext as ChallengeInfo;
            if (parent != null)
            {
                ChallengeAction.AcceptOrCancelChall(parent.ChallengeID, 1);
                parent.IsVisibleAccept = false;
                parent.IsvisibleDelete = false;
                model.Notifi.ListChallengesPer.Remove(parent);
                ChallengeAction.ListSendAcc.Remove(parent.SenderID);
                model.Notifi.count--;
                model.Notifi.notify = model.Notifi.count.ToString();
                NotifiDialog.Initiance().DialogChallengeClub("");
                //btn.IsVisible = false;
            }

        }
        /// <summary>
        /// ấn hủy thách đấu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var parent = btn.BindingContext as ChallengeInfo;
            if (parent != null)
            {
                ChallengeAction.AcceptOrCancelChall(parent.ChallengeID, 0);
                model.Notifi.ListChallengesPer.Remove(parent);
                ChallengeAction.ListSendAcc.Remove(parent.SenderID);
                model.Notifi.count--;
                model.Notifi.notify = model.Notifi.count.ToString();
                parent.IsVisibleAccept = false;
                parent.IsvisibleDelete = false;
                NotifiDialog.Initiance().DialogChallengeDelete();
            }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as ChallengeInfo;
            if (item != null)
            {
                model.NaviDetailChanllenge(item);
            }

        }
        /// <summary>
        /// Tap cá nhân
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ChangeTab(true);
        }
        /// <summary>
        /// Tap club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            ChangeTab(false);
        }
        private void ChangeTab(bool isPerson)
        {
            //thay đổi trạng thái của person
            if (isPerson)
            {
                AccountLayout.IsVisible = true;
                ClubLayout.IsVisible = false;
                TabPerson.BackgroundColor = Color.FromHex("#f15a25");
                TabClub.BackgroundColor = Color.FromHex("#787468");
            }
            else
            {
                AccountLayout.IsVisible = false;
                ClubLayout.IsVisible = true;
                TabClub.BackgroundColor = Color.FromHex("#f15a25");
                TabPerson.BackgroundColor = Color.FromHex("#787468");
                
            }
        }
        /// <summary>
        /// Chấp nhận thách đấu Club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAccept(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var parent = btn.BindingContext as ChallengeInfo;
            if (parent != null)
            {
                ChallengeAction.AcceptOrCancelChall(parent.ChallengeID, 1);
                model.Notifi.ListChallengesClub.Remove(parent);
                model.Notifi.count--;
                model.Notifi.notify = model.Notifi.count.ToString();
                ChallengeAction.ListSendClub.Remove(parent.SenderID);
                UserDialogs.Instance.Toast("Đã chấp nhận thách đấu thành công");
                //btn.IsVisible = false;
            }
        }
        /// <summary>
        /// Hủy thách đấu club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var parent = btn.BindingContext as ChallengeInfo;
            if (parent != null)
            {
                ChallengeAction.AcceptOrCancelChall(parent.ChallengeID, 0);
                model.Notifi.ListChallengesClub.Remove(parent);
                model.Notifi.count --;
                model.Notifi.notify = model.Notifi.count.ToString();
                ChallengeAction.ListSendClub.Remove(parent.SenderID);
                NotifiDialog.Initiance().DialogChallengeDelete();
            }
        }

        private void ListView_ItemTapped_1(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as ChallengeInfo;
            if (item != null)
            {
                model.NaviDetailChallengeClub(item);
            }
            
        }
    }
}