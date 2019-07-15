
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.ViewModels.News;
using AppBongBan.Views.Groups.Images;
using FFImageLoading.Forms;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsSitePage : ContentPage
    {
        public NewsSitePage()
        {
            InitializeComponent();
            // Kiểm tra Login
            if (Helper.Instance().MyAccount != null)
            {
                Avatar.SetBinding(CachedImage.SourceProperty, "Avatar_Uri");
                Avatar.BindingContext = Helper.Instance().MyAccount;
            }
            firstSite = false;
        }

        NewsSitePageViewModel model;
        bool firstSite = false;
        protected override void OnAppearing()
        {
            // Animation quay
            Task.Run(async () =>
           {
               uint duration = 10 * 60 * 1000;
               await Task.WhenAll(
                   imgAnimation.RotateTo(307 * 360, duration),
                   imgAnimation.RotateXTo(251 * 360, duration),
                   imgAnimation.RotateYTo(199 * 360, duration)
                                );
               imgAnimation.Rotation = 0;
               imgAnimation.RotationX = 0;
               imgAnimation.RotationY = 0;
           });
            SelectImageCmd.Instance().ChooseImage = ChooseImage.NewsSite;
            Helper.CaseSelectImag = SelectImage.SelectedImagePersonal;
            base.OnAppearing();
            if (model == null)
                model = BindingContext as NewsSitePageViewModel;
            // Đăng ký delegate để nhận bạn tin khi có content về
            Helper.Instance().ContentAck += GetContent;
            // Neu chua co du lieu, thi lay du lan dau va la 10 ban moi nhat
            if (!firstSite)
            {
                firstSite = true;
                Helper.Instance().checkOpenNewsSite = true;
                // Helper.Instance().GetContactGroupChat = true;
                // Lấy danh sách bài đăng từ biến Tempt đã được lấy trong khi flash
                model.NewsSite.GetContentInfo();
                model.NewsSite.IsFreshing = true;
                Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
                {
                    model.NewsSite.IsFreshing = false;
                    return false;
                });
            }
            // Nếu Login rồi thì gửi yêu cầu lấy bản tin
            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {
                Task.Run(() =>
                {
                    model.NewsSite.IsFreshing = false;
                    if (model.NewsSite.ListNews.Count < 10)
                        model.ReFresh();
                });
                return false;
            });
        }
        public void GetContent(ContentInfo contentInfo)
        {
            if (model.NewsSite.ListNews.IndexOf(contentInfo) < 0)
            {
                model.NewsSite.ListNews.Add(contentInfo);
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Helper.Instance().ContentAck -= GetContent;
        }
        /// <summary>
        /// Phương thức xử lý khi người dùng tap vào ảnh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemNewsView_TapImage(object sender, EventArgs e)
        {
            var type = sender.GetType();
            if(type==typeof(Xamarin.Forms.Label))
            {

            }
            else
            {
                var img = sender as CachedImage;
                var uri = img?.Source.GetValue(UriImageSource.UriProperty).ToString();
                // Lấy ra thông tin ảnh từ giao diện
                var parent = (ContentInfo)img?.BindingContext;
                //  var contentId = parent.Detail.Id;
                if (parent !=null && !string.IsNullOrEmpty(uri))
                {
                    if (model != null)
                    {
                        //  model.NaviImage(parent, uri);
                        //  return;
                    }

                }
                //if (parent != null && img != null)
                //{
                //    if (model != null)
                //        model.NaviListImage(parent);
                //    return;
                //}
                ////nếu là hiển thị 4 ảnh trở lên thì cần chuyển sang giao diện mới
                //var Gridview = sender as Grid;
                //parent = (ContentInfo)Gridview?.BindingContext;
                //if (parent != null)
                //{
                //    if (model != null)
                //        model.NaviListImage(parent);
                //    return;
                //}
            }
          
        }

        private void ItemNewsView_TabComment(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (ContentInfo)stack?.BindingContext;
            if (model != null)
                model.NaviComment(parent);
        }

        private void ItemNewsView_TabLike(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            var parent = (ContentInfo)stack?.BindingContext;
            Label like = stack.Children[1] as Label;
            CachedImage img = stack.Children[0] as CachedImage;
            if (parent.LikeContent.LikeColor.Equals("#5D6A76"))
            {
                if (model != null)
                    model.SendLike(true, parent);
            }
            else
            {
                if (model != null)
                    model.SendLike(false, parent);
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null)
                lv.SelectedItem = null;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PersonalPostPage());
        }

        private void ListNews_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (model.NewsSite.ListNews.Count < 10) return;
            if ((e.Item as ContentInfo) == model.NewsSite.ListNews[model.NewsSite.ListNews.Count - 1])
            {
                model.GetMore(-10);
            }
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PersonalPostPage());
            // await DependencyService.Get<IMediaService>().OpenGallery(true);
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PersonalPostPage());
            //List<ImageNews> list = await Helper.TakePhoto();
            //if (list.Count == 1)
            //{
            //    MessagingCenter.Send<App, List<ImageNews>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", list);
            //}
        }
    }
}