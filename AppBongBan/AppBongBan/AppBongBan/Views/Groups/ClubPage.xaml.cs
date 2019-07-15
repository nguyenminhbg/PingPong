using AppBongBan.Models;
using AppBongBan.ViewModels.Group;
using AppBongBan.ViewModels.Groups;
using AppBongBan.Views.Groups.ManageMember;
using FFImageLoading.Forms;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClubPage : ContentPage
    {
        public ClubPage()
        {
            InitializeComponent();
        }
        ClubPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as ClubPageViewModel;
            }
            if (Helpers.Helper.prev.Count > 0)
            {
                Helpers.Helper.DeleteUri(Helpers.Helper.prev);
            }
        }

        private void LvNews_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null)
                lv.SelectedItem = null;
        }
        /// <summary>
        /// thực hiểm Kiểm tra để load thêm danh sách
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvNews_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (model != null)
            {
                model?.ClubVM.Loading(e.Item as ContentInfo);
            }
        }
        /// <summary>
        /// Thực hiện Thêm bài viết
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandClubView_TapAddNews(object sender, EventArgs e)
        {
            var view = sender as View;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            if (model != null)
            {
                model.ClubVM.NaviAddNews();
            }
        }
        /// <summary>
        /// Thực hiện like tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemNewsView_TabLike(object sender, EventArgs e)
        {
            var model = BindingContext as ClubPageViewModel;
            var stack = sender as StackLayout;
            var parent = (ContentInfo)stack?.BindingContext;
            Label like = stack.Children[1] as Label;
            CachedImage img = stack.Children[0] as CachedImage;
            if (parent.LikeContent.LikeColor.Equals("#5D6A76"))
            {
                if (model != null)
                {
                    model.ClubVM.SendLike(true, parent);
                }
            }
            else
            {
                if (model != null)
                {
                    model.ClubVM.SendLike(false, parent);
                }
            }


        }
        /// <summary>
        /// Thực hiện Xem chi tiết Comment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemNewsView_TabComment(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (ContentInfo)stack?.BindingContext;
            if (model != null)
                model.ClubVM.NaviComment(parent);
        }
        /// <summary>
        /// Sự kiện xem thêm thông tin club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapShowMoreInfo(object sender, EventArgs e)
        {
            model.ClubVM.NaviMorepage();
        }
        /// <summary>
        /// Thực hiện yêu cầu tham gia club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapJoin(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.ClubVM.ClubJoinReq(model.ClubVM.MyClub);
            }
            
        }
        /// <summary>
        /// Xem danh sách thành viên club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapListMembers(object sender, EventArgs e)
        {
            var view = sender as StackLayout;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            model.ClubVM.NaviListMember();
        }

        private void MoreTab(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.ClubVM.NextRepairClub();
            }
        }
        /// <summary>
        /// Tap gọi điện thoại cho club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapCall(object sender, EventArgs e)
        {
            var view = sender as Image;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            if (model != null)
            {
                model.ClubVM.TapCall();
            }
        }

        private void TapShowMap(object sender, EventArgs e)
        {
            var view = sender as Image;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            if (model != null)
            {
                model.ClubVM.TapShowMap();
            }
        }

        private void TapImage(object sender, EventArgs e)
        {
            var img = sender as CachedImage;
            var view = sender as FlexLayout;
            var parent = (ContentInfo)img?.BindingContext;
            if (parent != null)
            {
                if (model != null)
                {
                    // model.NaviImage(parent);
                    model.NaviListImage(parent);
                }
                return;
            }
            //nếu là hiển thị 4 ảnh trở lên thì cần chuyển sang giao diện mới
            var Gridview = sender as Grid;
            parent = (ContentInfo)Gridview?.BindingContext;
            if (parent != null)
            {
                if (model != null)
                {
                    model.NaviListImage(parent);
                }
                return;
            }


        }
        /// <summary>
        /// Xem danh sách ảnh để hiển thị
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var view = sender as StackLayout;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            if (model != null)
            {
                model.NaviListImage();
            }
        }
        /// <summary>
        /// sang trang thêm ảnh trong club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandClubView_TapAddImage(object sender, EventArgs e)
        {
            var view = sender as StackLayout;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            if (model != null)
            {
                model.ClubVM.NaviUpLoadImage();
            }

        }
        /// <summary>
        /// Thực hiện checknin trong club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandClubView_TapCheckin(object sender, EventArgs e)
        {
            var view = sender as StackLayout;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            if (model != null)
                model.ClubVM.NaviCheckIn();

        }
        /// <summary>
        /// Xem danh sách checkin trên club
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var view = sender as StackLayout;
            view.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                view.BackgroundColor = Color.White;
                return false;
            });
            if (model != null)
                model.ClubVM.NaviCheckIns();
        }
    }
}