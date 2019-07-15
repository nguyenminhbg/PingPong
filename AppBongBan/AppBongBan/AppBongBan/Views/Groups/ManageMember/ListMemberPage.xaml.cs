using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Groups.ManageMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.ManageMember
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListMemberPage : ContentPage
    {
        public bool isSendMember;
        public ListMemberPage()
        {
            InitializeComponent();
        }
        ListMemberPageViewModel model;
        public bool isFirst = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = BindingContext as ListMemberPageViewModel;
            if (model != null)
            {
                var app = Application.Current as App;
                if (app.clubCurrent != null)
                {
                    model.ListMember.MyClub = app.clubCurrent;
                    model.ListMember.ListAccount.Clear();
                    model.ListMember.SendGetAccount();
                }
            }

        }
        /// <summary>
        /// thực hiện xem chi tiết cá nhân
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListUser_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var lv = sender as ListView;
            var itemMember = lv.SelectedItem as Accounts;
            if (model != null && itemMember != null)
            {
                model.NavigAccount(itemMember);
                if (lv != null) lv.SelectedItem = null;
            }
        }
        /// <summary>
        /// Thực hiện chức năng kết bạn nếu chưa là bạn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            stack.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stack.BackgroundColor = Color.White;
                return false;
            });
            var parent = (Accountlocal)stack.BindingContext;
            if (model != null && parent != null)
            {
                if(parent.AddFriend.Equals("chat.png"))
                {
                    AddFriendAction.NavigationChat(this, Helper.Instance().ConvertAccsToAcc(parent));
                }
                else if (parent.AddFriend.Equals("person_add_invi.png"))
                {
                    NotifiDialog.Initiance().DialogAwaitAcceptFriend();
                }
                else if (parent.AddFriend.Equals("person_add.png"))
                {
                    AddFriendAction.AddFriend((uint)Helpers.Helper.Instance().MyAccount.Number_Id, (uint)parent.Number_Id);
                }
            }
        }
        /// <summary>
        /// Thực hiện chức năng thách đấu từ bạn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            stack.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stack.BackgroundColor = Color.White;
                return false;
            });
            var parent = (Accountlocal)stack?.BindingContext;
            if (model != null)
                model.NavigChall(parent);
        }
    }
}