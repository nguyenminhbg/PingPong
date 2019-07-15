using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Groups;
using AppBongBan.Views.IView;
using AppChat.Dependency;
using DLToolkit.Forms.Controls;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchLocationPage : ContentPage
    {
        public FlowObservableCollection<AutoComplete> ListAutoComplete = new FlowObservableCollection<AutoComplete>();
        AutoComplete itemseleted = null;
        SearchLocationPageViewModel model;
        public SearchLocationPage()
        {
            InitializeComponent();
            lvAutoComplete.ItemsSource = ListAutoComplete;
            lvAutoComplete.ItemTapped += LvAutoComplete_ItemTapped;
            MySearch.IsEnabled = false;
            checkFirst = false;
        }
        /// <summary>
        /// Chọn item trong danh sách autoComplete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvAutoComplete_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null)
            {
                itemseleted = lv.SelectedItem as AutoComplete;
                lv.SelectedItem = null;
            }
            if (itemseleted != null)
            {
                MySearch.Text = itemseleted.TextShow;
                model.SearchLocation.SearchAreaExe(itemseleted);
                lvAutoComplete.IsVisible = false;
                lvAutoComplete.BackgroundColor = Color.Transparent;
                itemseleted = null;
            }
            MySearch.Unfocus();
        }
        bool checkFirst = false;
        protected override void OnAppearing()
        {
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            doubclick = false;
            MessagingCenter.Subscribe<App, long>((App)Application.Current, "NotifiAccept", (send, arg) =>
            {
                // Thay đổi giao diện khi có thay đổi
                Helper.Instance().addFriendStatus[arg].statusIcon= "chat.png";
                NotifiDialog.Initiance().DialogtrustFriend();
            });
            MessagingCenter.Subscribe<App, long>((App)Application.Current, "NotifiDelete", (send, arg) =>
            {
                // Thay đổi giao diện khi có thay đổi
                Helper.Instance().addFriendStatus[arg].statusIcon = "person_add.png";
                NotifiDialog.Initiance().DialogDeleteFriend();
            });
            if (model == null)
                model = BindingContext as SearchLocationPageViewModel;
            if (model != null)
            {
                if (lvAutoComplete.IsVisible)
                {
                    lvAutoComplete.IsVisible = false;
                    lvAutoComplete.BackgroundColor = Color.Transparent;
                }
                if (!checkFirst)
                {
                    ChangeTab(true);
                    checkFirst = true;
                }

            }
            // đăng ký sự kiện lấy danh sách các clubId
            Helper.Instance().GetClubIdAck += GetListClubId;
            // Đăng ký sự kiện lấy club
            Helper.Instance().GetClubAck += GetClub;
        }
        public void GetListClubId(List<long> list)
        {
            model.SearchLocation.GetListClubId(list);
        }
        public void GetClub(Club club)
        {
            model.SearchLocation.GetClub(club);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<App, long>((App)Application.Current, "NotifiAccept");
            MessagingCenter.Unsubscribe<App, long>((App)Application.Current, "NotifiDelete");
            Helper.Instance().GetClubIdAck -= GetListClubId;
            Helper.Instance().GetClubAck -= GetClub;
        }
        /// <summary>
        /// Chọn tìm kiếm trong bảng search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_SearchButtonPressed(object sender, EventArgs e)
        {
            if (ListAutoComplete.Count != 0)
            {
                if (lvAutoComplete.IsVisible)
                {
                    lvAutoComplete.IsVisible = false;
                    lvAutoComplete.BackgroundColor = Color.Transparent;
                }
                itemseleted = ListAutoComplete[0];
                MySearch.Text = itemseleted.TextShow;
                model.SearchLocation.SearchAreaExe(itemseleted);
                lvAutoComplete.IsVisible = false;
                lvAutoComplete.BackgroundColor = Color.Transparent;
                itemseleted = null;
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorArea();
            }

        }
        /// <summary>
        /// search từng 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = sender as SearchBar;
            //nếu người dùng tìm kiếm theo tên
            //if (lvAutoComplete.IsVisible)
            //{
            //    lvAutoComplete.IsVisible = false;
            //    lvAutoComplete.BackgroundColor = Color.Transparent;
            //}

            //if (search != null && !Search.Text.Equals(""))
            //{
            //    model.SearchClub(search.Text, 0);
            //}
            //Nếu người dùng tìm kiếm theo địa điểm 

            if (search != null && search.Text != null && !search.Text.Equals(""))
            {
                if (!lvAutoComplete.IsVisible)
                {
                    lvAutoComplete.IsVisible = true;
                    lvAutoComplete.BackgroundColor = Color.White;
                }

                Task.Run(async () =>
                {
                    var list = await Helpers.Helper.Database.SearchArea(search.Text);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (list.Count > 0)
                        {
                            ListAutoComplete.Clear();
                            ListAutoComplete.AddRange(list);
                        }
                    });
                });
            }
            else
            {
                if (lvAutoComplete.IsVisible)
                {
                    lvAutoComplete.IsVisible = false;
                    lvAutoComplete.BackgroundColor = Color.Transparent;
                }
                ListAutoComplete.Clear();
            }
        }
        /// <summary>
        /// Chọn định dạng item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as CustomPicker;
            //nếu người dùng chọn item trong picker là Cá nhân
            if (picker.SelectedIndex == 0)
            {
                TabPerson.IsVisible = true;
                TabClub.IsVisible = true;
                TabClubArea.IsVisible = false;
                MySearch.Placeholder = "Đang tìm kiếm quanh 50km";
                MySearch.PlaceholderColor = Color.White;
                MySearch.Text = "";
                //if (MySearch.IsEnabled)
                //    MySearch.IsEnabled = false;
                if (AreatLayout.IsVisible)
                    AreatLayout.IsVisible = false;
                if (!AccountLayout.IsVisible)
                    AccountLayout.IsVisible = true;
                if (!ClubLayout.IsVisible)
                    ClubLayout.IsVisible = true;
                ChangeTab(true);

            }
            //nếu người dùng chọn item trong picker là club
            else
            {
                TabPerson.IsVisible = false;
                TabClub.IsVisible = false;

                MySearch.Placeholder = "Điền Xã/Phường, Quận/Huyện, Tỉnh/Thành Phố";
                MySearch.PlaceholderColor = Color.Gray;
                if (!MySearch.IsEnabled)
                    MySearch.IsEnabled = true;
                if (AccountLayout.IsVisible)
                    AccountLayout.IsVisible = false;
                if (ClubLayout.IsVisible)
                    ClubLayout.IsVisible = false;
                if (!AreatLayout.IsVisible)
                    AreatLayout.IsVisible = true;
                TabClubArea.IsVisible = true;
                TabClubArea.BackgroundColor = Color.FromHex("#f15a25");
                if (model != null)
                    model.SearchLocation.ListClubArea.Clear();
            }
        }

        private void View_TapMap1(object sender, EventArgs e)
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
                model.ShowMap(parent);
        }

        /// <summary>
        /// sự kiện tap thêm bạn bè
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_TapFriend(object sender, EventArgs e)
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
        }

        private void View_TapListUsr(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            stack.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stack.BackgroundColor = Color.White;
                return false;
            });
            var parent = (Club)stack?.BindingContext;
            if (model != null)
            {
                model.SearchLocation.NaviListUse(parent);
            }

        }

        private void View_TapJoin(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            //var parent = (Club)view.BindingContext;
            stack.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stack.BackgroundColor = Color.White;
                return false;
            });
            var parent = (Club)stack?.BindingContext;
            if (model != null)
            {
                Services.Service.Instiance().ClubModel.ClubJoinReq(parent);
            }
        }
        private void View_TapMap(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            //var parent = (Club)view.BindingContext;
            stack.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stack.BackgroundColor = Color.White;
                return false;
            });
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.ShowMap(parent);
        }
        bool doubclick = false;
        /// <summary>
        /// Chọn xem item club xem chi tiết
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvDp_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (doubclick)
            {
                var lv1 = sender as ListView;
                lv1.SelectedItem = null;
                return;
            }
            doubclick = true;
            var lv = sender as ListView;
            var itemClub = lv.SelectedItem as Club;
            if (model != null && itemClub != null)
            {
                model.SearchLocation.NextClubPage(itemClub);
                if (lv != null) lv.SelectedItem = null;
                return;
            }
            var itemMember = lv.SelectedItem as Accounts;
            if (model != null && itemMember != null)
            {
                model.NavigAccount(itemMember);
                if (lv != null) lv.SelectedItem = null;
            }
        }
        /// <summary>
        /// Thực hiện load thêm danh sách từ server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvDp_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (model != null)
            {
                model?.SearchLocation.Loading(e.Item as Club);
            }
        }
        private void TapClubAction(object sender, EventArgs e)
        {
            ChangeTab(false);
        }
        private void TapPersonAction(object sender, EventArgs e)
        {
            ChangeTab(true);
        }
        private async void ChangeTab(bool isPerson)
        {
            if (model != null && model.SearchLocation.ListClubArea != null && model.SearchLocation.listAcc != null)
            {
                model.SearchLocation.ListClubArea.Clear();
                model.SearchLocation.listAcc.Clear();
            }

            //thay đổi trạng thái của person
            if (isPerson)
            {
                AccountLayout.IsVisible = true;
                ClubLayout.IsVisible = false;
                TabPerson.BackgroundColor = Color.FromHex("#f15a25");
                TabClub.BackgroundColor = Color.FromHex("#787468");
                if (model != null)
                {
                    //tìm kiếm tọa độ theo người dùng
                    if (lvAutoComplete.IsVisible)
                    {
                        lvAutoComplete.IsVisible = false;
                        lvAutoComplete.BackgroundColor = Color.Transparent;
                    }
                    bool gpsCheck = await CheckGps();
                    if (!gpsCheck) return;
                    model.SearchLocation.LoadObject(0, model.SearchLocation.Radius);
                }
            }
            else
            {
                AccountLayout.IsVisible = false;
                ClubLayout.IsVisible = true;
                TabClub.BackgroundColor = Color.FromHex("#f15a25");
                TabPerson.BackgroundColor = Color.FromHex("#787468");
                if (model != null)
                {
                    //tìm kiếm tọa độ theo club
                    if (lvAutoComplete.IsVisible)
                    {
                        lvAutoComplete.IsVisible = false;
                        lvAutoComplete.BackgroundColor = Color.Transparent;
                    }
                    bool gpsCheck = await CheckGps();
                    if (!gpsCheck) return;
                    model.SearchLocation.LoadObject(1, model.SearchLocation.Radius);
                }
            }
        }
        public async Task<bool> CheckGps()
        {
            if (!DependencyService.Get<ILocationGps>().CheckGps())
            {
                var currentPage = AppChat.Helpers.Helper.Instiance().GetCurrentPage();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool btn = await currentPage.DisplayAlert("Thông báo GPS", "Bạn có muốn bật vị trí để sử dụng chức năng này","Open","Cancel");
                    if (btn)
                    {
                        DependencyService.Get<ILocationGps>().OpenGps();
                    }
                });
            }
            // DependencyService.Get<ILocationGps>().CheckGps() check vị trí của máy xem có được bật hay không, nếu không được bật thì bật vị trí, nếu được bật thì check của App
            if (DependencyService.Get<ILocationGps>().CheckGps())
            {
                // Nếu cho phép App cập nhật vị trí thì vào bật vị trí, nếu không thì trả về
                // Kiểm tra permission
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    if (results.Count > 0)
                        status = results[Permission.Location];
                    if (status != PermissionStatus.Granted)
                    {
                        return false;
                    }
                    else return true;
                }
                else return true;
            }
            else return false;
        }
        private void ItemClubView_TapCheckin(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            //var parent = (Club)view.BindingContext;
            stack.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stack.BackgroundColor = Color.White;
                return false;
            });
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.NaviCheckin(parent);
        }

        private void ItemPerView_TapClub(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;

            stack.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stack.BackgroundColor = Color.White;
                return false;
            });
            var parent = (Accountlocal)stack?.BindingContext;
            if (model != null && parent != null)
            {
                if (parent.ClubID > 0)
                    model.SearchLocation.NextClubPage(new Club() { ClubID = parent.ClubID, ClubName = parent.ClubName });
                else
                    UserDialogs.Instance.Toast("không có club");
            }
        }

        private void ItemPerView_TapChall(object sender, EventArgs e)
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
        private void ItemClubView_TapChall(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.NavigChall(parent);
        }
    }
}