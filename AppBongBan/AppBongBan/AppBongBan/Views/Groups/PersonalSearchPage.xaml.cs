using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Groups;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalSearchPage : ContentPage
    {
        public PersonalSearchPage()
        {
            InitializeComponent();
            theFirst = true;
        }
        public PersonalSearchPageViewModel model;
        bool theFirst = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            doubclick = false;
            if (model == null)
                model = BindingContext as PersonalSearchPageViewModel;
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                Task.Run(() =>
                {
                    if (theFirst)
                    {
                        model.persons.IsFreshing = true;
                        model?.LoadObject(0, model.persons.Radius);
                        Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
                        {
                            Task.Run(() =>
                            {
                                model.persons.IsFreshing = false;
                            });
                            return false;
                        });
                        theFirst = false;
                    }
                });
                return false;
            });
        }

        private void ItemPerView_TapChall(object sender, System.EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Accountlocal)stack?.BindingContext;
            model.NavigChall(parent);
        }

        private void ItemPerView_TapClub(object sender, System.EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Accountlocal)stack?.BindingContext;
            if (model != null && parent != null)
            {
                if (parent.ClubID > 0)
                    model.persons.NextClubPage(new Club() { ClubID = parent.ClubID, ClubName = parent.ClubName });
                else
                    UserDialogs.Instance.Toast("không có club");
            }
        }

        private void ItemPerView_TapFriend(object sender, System.EventArgs e)
        {

            var stack = sender as StackLayout;
            var parent = (Accountlocal)stack.BindingContext;
            if (model != null && parent != null)
            {
                if (Helper.Instance().addFriendStatus[parent.Number_Id].statusIcon.Equals("chat.png"))
                {
                    AddFriendAction.NavigationChat(this, Helper.Instance().ConvertAccsToAcc(parent));
                }
                else if (Helper.Instance().addFriendStatus[parent.Number_Id].statusIcon.Equals("person_add_invi.png"))
                {
                    NotifiDialog.Initiance().DialogAwaitAcceptFriend();
                }
                else if (Helper.Instance().addFriendStatus[parent.Number_Id].statusIcon.Equals("person_add.png"))
                {
                    Helper.Instance().listAddFriend.Add(parent);
                    AddFriendAction.AddFriend((uint)Helper.Instance().MyAccount.Number_Id, (uint)parent.Number_Id);
                    NotifiDialog.Initiance().DialogSendAddFriend();
                }
            }
            // parent = null;
        }

        private void ItemPerView_TapMap(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;

            var parent = (Accountlocal)stack?.BindingContext;
            if (model != null)
                model.ShowMap(parent);
        }
        bool doubclick = false;
        private void Itemsource_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (doubclick)
            {
                var lv1 = sender as ListView;
                lv1.SelectedItem = null;
                return;
            }
            doubclick = true;
            var lv = sender as ListView;
            var itemMember = lv.SelectedItem as Accounts;
            if (model != null && itemMember != null)
            {
                model.persons.NavigAccount(itemMember);
                if (lv != null) lv.SelectedItem = null;
            }
        }
    }
}