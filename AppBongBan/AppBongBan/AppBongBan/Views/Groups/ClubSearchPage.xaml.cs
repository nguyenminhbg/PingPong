using AppBongBan.Models.Db;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Groups.MyClubs;
using AppBongBan.Views.IView;
using DLToolkit.Forms.Controls;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClubSearchPage : ContentPage
    {
        public FlowObservableCollection<AutoComplete> ListAutoComplete = new FlowObservableCollection<AutoComplete>();
        public ClubSearchPage()
        {
            InitializeComponent();
            lvAutoComplete.ItemsSource = ListAutoComplete;
            firstApper = true;
        }
        ClubSearchPageViewModel model;
        bool firstApper = true;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            doubclick = false;
            if (model == null)
                model = this.BindingContext as ClubSearchPageViewModel;
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                Task.Run(() =>
                {
                    if (firstApper)
                    {
                        model.clubSearch.isFreshing = true;
                        model?.LoadObject(1, 25000);
                        Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
                        {
                            Task.Run(() =>
                            {
                                model.clubSearch.isFreshing = false;
                            });
                            return false;
                        });
                        firstApper = false;
                    }
                });
                return false;
            });
        }
        CustomPicker picker;
        /// <summary>
        /// Người dùng chọn option mong muốn tìm kiếm, khi khởi tạo thì chọn mặc định là tìm quanh tôi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            picker = sender as CustomPicker;
            if (picker.SelectedIndex == 0)
            {
                if (firstApper == false)
                {
                    model.clubSearch.isFreshing = false;
                    model.clubSearch.ListClub.Clear();
                    model?.LoadObject(1, 25000);
                    Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
                    {
                        Task.Run(() =>
                        {
                            model.clubSearch.isFreshing = false;
                        });
                        return false;
                    });
                }
                MySearch.Placeholder = "Đang tìm kiếm quanh 50km";
                MySearch.PlaceholderColor = Color.Gray;
                MySearch.Text = "";
                MySearch.IsEnabled = false;
                nearSearch.IsVisible = true;
                lvAutoComplete.IsVisible = false;
            }
            else
            {
                nearSearch.IsVisible = false;
                lvAutoComplete.IsVisible = true;
                MySearch.Placeholder = "Điền Xã/Phường, Quận/Huyện, Tỉnh/Thành Phố";
                MySearch.PlaceholderColor = Color.Gray;
                MySearch.IsEnabled = true;
            }
        }
        /// <summary>
        /// Hàm xử lý khi có thay đổi về Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = sender as SearchBar;
            if (search != null && search.Text != null && !search.Text.Equals(""))
            {
                lvAutoComplete.IsVisible = true;
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
                ListAutoComplete.Clear();
            }
        }
        AutoComplete itemseleted = null;
        /// <summary>
        /// Hàm xử lý khi được chọn Item tìm kiếm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvAutoComplete_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            model.clubSearch.ListClub.Clear();
            model.clubSearch.IsMore = false;
            var lv = sender as ListView;
            if (lv != null)
            {
                itemseleted = lv.SelectedItem as AutoComplete;
                lv.SelectedItem = null;
            }
            if (itemseleted != null)
            {
                model.clubSearch.isFreshing = true;
                MySearch.Text = itemseleted.TextShow;
                model.clubSearch.SearchAreaExe(itemseleted);
                Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
                {
                    model.clubSearch.isFreshing = false;
                    return false;
                });
                lvAutoComplete.IsVisible = false;
                nearSearch.IsVisible = true;
                lvAutoComplete.BackgroundColor = Color.Transparent;
                // itemseleted = null;
            }
            MySearch.Unfocus();
        }
        /// <summary>
        /// Xử lý bản tin người dùng Refresh lại trang
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NearSearch_Refreshing(object sender, EventArgs e)
        {
            model.clubSearch.isFreshing = true;
            if (picker.SelectedIndex == 0)
            {
                model?.LoadObject(1, 25000);
            }
            else
            {
                model.clubSearch.SearchAreaExe(itemseleted);
            }
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                Task.Run(() =>
                {
                    model.clubSearch.isFreshing = false;
                });
                return false;
            });
        }
        bool doubclick = false;
        /// <summary>
        /// Phương thức xử lý khi người dùng click vào item danh sách tìm kiếm club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NearSearch_ItemTapped(object sender, ItemTappedEventArgs e)
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
                model.clubSearch.NextClubPage(itemClub);
                if (lv != null) lv.SelectedItem = null;
                return;
            }
        }

        private void ItemClubView_TapChall(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.NavigChall(parent);
        }

        private void ItemClubView_TapCheckin(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.NaviCheckin(parent);
        }

        private void ItemClubView_TapMap(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.ShowMap(parent);
        }

        private void ItemClubView_TapJoin(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Club)stack?.BindingContext;
            if (model != null)
            {
                Services.Service.Instiance().ClubModel.ClubJoinReq(parent);
            }
        }

        private void ItemClubView_TapListUsr(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (Club)stack?.BindingContext;
            if (model != null)
                model.NaviCheckin(parent);
        }

        private void NearSearch_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (model != null)
            {
                model?.clubSearch.Loading(e.Item as Club);
            }
        }
    }
}