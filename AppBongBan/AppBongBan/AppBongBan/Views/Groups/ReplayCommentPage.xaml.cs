using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Services;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Groups;
using FFImageLoading.Forms;
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
    public partial class ReplayCommentPage : ContentPage
    {
        public ReplayCommentPage()
        {
            InitializeComponent();

        }
        ReplayCommentPageViewModel model;
        /// <summary>
        /// thêm đối tượng comment cho reply comment
        /// </summary>
        protected override void OnAppearing()
        {
            myEditor.WidthRequest = myEditor.Width;
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as ReplayCommentPageViewModel;
            }
            Service.Instiance().stTyping = StatueTyping.ReplyComment;
            Helpers.Helper.CaseSelectImag = Helpers.SelectImage.SelectedItem;
            SelectImageCmd.Instance().ChooseImage = ChooseImage.AddReplyCmd;

        }
        private void lvComment_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null) lv.SelectedItem = null;
            myEditor.Unfocus();
        }
        private void CommentView_TabComment(object sender, EventArgs e)
        {
            myEditor.Focus();
        }
        /// <summary>
        /// thực hiện like reply của comment trong content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentView_TabLike(object sender, EventArgs e)
        {
            var label = sender as Label;
            var parent = (ReplyComments)label.BindingContext;
            if (model != null)
            {
                if (parent.LikeColor.Equals("#5D6A76"))
                {
                    model.ReplyComment.SendLikeReply(parent, true);
                }
                else
                {
                    model.ReplyComment.SendLikeReply(parent, false);
                }
            }
        }

        private void TapSend(object sender, EventArgs e)
        {

            //if (myEditor.Text != null)
            //{
            if (model != null)
            {
                model.ReplyComment.SendReplyComment(myEditor.Text);
                //myEditor.Text = "";
                //Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                //{

                //    var lastMessage = this.lv.ItemsSource.Cast<object>().LastOrDefault();
                //    if (null != lastMessage)
                //    {
                //        this.lv.ScrollTo(lastMessage, ScrollToPosition.End, true);
                //    }
                //    return false;
                //});
                myEditor.Text = "";
                model.ReplyComment.isImage = false;
                model.ReplyComment.ImageView = new ImageNews();
            }
            //}

        }
        /// <summary>
        /// Trở về trang trước
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TapBack(object sender, EventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                await img.ScaleTo(1.2, 100, Easing.CubicOut);
                await img.ScaleTo(1, 100, Easing.CubicIn);

            }

        }

        private void ReplyCommentExe(object sender, EventArgs e)
        {
            myEditor.Focus();
        }

        private async void AddImage(object sender, EventArgs e)
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
            var parent = (ReplyComments)comment.BindingContext;
            if (model != null)
            {
                model.NavigationDetailImage(parent);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.ReplyComment.isImage = false;
                model.ReplyComment.ImageView = new ImageNews();
            }
        }
        /// <summary>
        /// Like trong chi tiết comment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var view = sender as StackLayout;
            var parent = ((ReplayCommentPageViewModel)view.BindingContext).ReplyComment.Comment;
            if (model != null)
            {
                if (parent.LikeColor.Equals("#5D6A76"))
                {
                    LikeAction.SendLike(true, ref parent);
                }
                else
                {
                    LikeAction.SendLike(false, ref parent);
                }
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