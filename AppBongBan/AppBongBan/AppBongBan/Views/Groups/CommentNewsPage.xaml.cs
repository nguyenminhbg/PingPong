using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Services;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Groups;
using FFImageLoading.Forms;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommentNewsPage : ContentPage
    {
        public CommentNewsPage()
        {
            InitializeComponent();
        }
        CommentNewsViewModel model;
        protected override void OnAppearing()
        {
            myEditor.WidthRequest = myEditor.Width;
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
          //  scrollComment.ScrollToAsync(bottomView, ScrollToPosition.Center, true);
            if (model == null)
            {
                model = BindingContext as CommentNewsViewModel;
            }
            Helper.CaseSelectImag = Helpers.SelectImage.SelectedItem;
            Service.Instiance().stTyping = StatueTyping.Comment;
            SelectImageCmd.Instance().ChooseImage = ChooseImage.AddComment;
            if (Helper.Instance().CommentNofity.ContainsKey("Content"))
            {
                var ContentID = Helper.Instance().CommentNofity["Content"];
                model.IsLoading = false;
                Helper.IdNews = ContentID;
                model.CommentNews.SendGetContent(ContentID);
                Helper.Instance().CommentNofity.Clear();
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
          //  model.CommentNews.NewsContent = new ContentInfo();
        }
        private void CommentView_TabLike(object sender, EventArgs e)
        {
            var like = sender as Label;
            var parent = (CommentInfor)like?.BindingContext;
            //await like.ScaleTo(1.2, 100, Easing.CubicOut);
            //await like.ScaleTo(1, 100, Easing.CubicIn);
            if (model != null)
            {
                if (parent.LikeColor.Equals("#5D6A76"))
                {
                    model.CommentNews.LikeComment(parent, true);
                }
                else
                {
                    model.CommentNews.LikeComment(parent, false);
                }
            }
        }
        private void CommentView_TabComment(object sender, EventArgs e)
        {
            var comment = sender as Label;
            var parent = (CommentInfor)comment.BindingContext;
            if (model != null)
            {
                model.NavigationReplyComment(parent);
            }
        }
        private void lvComment_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null) lv.SelectedItem = null;
            myEditor.Unfocus();
        }

        private  void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //var send = sender as Image;
            //if(send != null)
            //{
            //    await send.ScaleTo(1.1, 100, Easing.CubicOut);
            //    await send.ScaleTo(1, 100, Easing.CubicIn);
            //}
            //if (myEditor.Text != null)
            //{
            if (model != null)
            {
                model.CommentNews.SendComment(myEditor.Text);
                //Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                //{
                //    var lastMessage = this.lv.ItemsSource.Cast<object>().LastOrDefault();
                //    if (lastMessage !=null)
                //    {
                //        this.lv.ScrollTo(lastMessage, ScrollToPosition.End, true);
                //    }
                //    return false;
                //});
                myEditor.Text = "";
                model.CommentNews.isImage = false;
                model.CommentNews.ImageView = new ImageNews();
            }
        }

        private async void TapImage(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Chọn ảnh Comment", "Cancel", null, "Chụp ảnh", "Thư viện ảnh");
            if (action != null)
            {

                if (action.Contains("Chụp ảnh"))
                {
                    List<ImageNews> list = await Helper.TakePhoto();
                    if (list == null) return;
                    if (list.Count == 1)
                    {
                        MessagingCenter.Send<App, List<ImageNews>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", list);
                    }
                    return;
                }
                if (action.Contains("Thư viện ảnh"))
                {
                    await DependencyService.Get<IMediaService>().OpenGallery(false);
                }
            }
        }

        private void CommentView_TabShowImage(object sender, EventArgs e)
        {
            var comment = sender as CachedImage;
            var parent = (CommentInfor)comment.BindingContext;
            //await comment.ScaleTo(1.2, 100, Easing.CubicOut);
            //await comment.ScaleTo(1, 100, Easing.CubicIn);
            if (model != null)
            {
                model.NavigationDetailImage(parent);
            }
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            model.CommentNews.isImage = false;
            model.CommentNews.ImageView = new ImageNews();
        }
        /// <summary>
        /// Like Content trong chi tiết content
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
                LikeAction.SendLike(true,ref parent);
            }
            else
            {
                LikeAction.SendLike(false,ref parent);
            }
        }
        /// <summary>
        /// Tap Xem danh sách các ảnh trong content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemNewsView_TapImage(object sender, EventArgs e)
        {
            var view = sender as FlexLayout;
            var parent = (ContentInfo)view?.BindingContext;
            if (parent != null)
            {
                if (model != null)
                {

                    model.NaviImage(parent);
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
                    model.NaviImage(parent);
                }
                return;
            }
        }

        private void ItemNewsView_TabComment(object sender, EventArgs e)
        {
            myEditor.Focus();
        }

        private void LbSend_Clicked(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.CommentNews.SendComment(myEditor.Text);
                //Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                //{
                //    var lastMessage = this.lv.ItemsSource.Cast<object>().LastOrDefault();
                //    if (lastMessage !=null)
                //    {
                //        this.lv.ScrollTo(lastMessage, ScrollToPosition.End, true);
                //    }
                //    return false;
                //});
                myEditor.Text = "";
                model.CommentNews.isImage = false;
                model.CommentNews.ImageView = new ImageNews();
            }
        }

        private void MyEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (myEditor.Text.Length > 100)
            {
                myEditor.IsExpandable = false;
                myEditor.AutoSize = EditorAutoSizeOption.Disabled;
            }
            else
            {
                myEditor.IsExpandable = true;
                myEditor.AutoSize = EditorAutoSizeOption.TextChanges;
            }
        }
    }
}