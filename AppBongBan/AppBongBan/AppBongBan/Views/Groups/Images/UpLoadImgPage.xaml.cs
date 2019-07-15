using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.ViewModels.Groups.Images;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.Images
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpLoadImgPage : ContentPage
    {
        public UpLoadImgPage()
        {
            InitializeComponent();
        }
        UpLoadImgPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as UpLoadImgPageViewModel;
            }
            Helpers.Helper.CaseSelectImag = Helpers.SelectImage.SelectedItem;
            SelectImageCmd.Instance().ChooseImage = ChooseImage.UploadImage;
            Services.Service.Instiance().StPostContent = Services.StatuePostContent.UpLoadImage;
        }
        private void DeleteImage(object sender, EventArgs e)
        {
            var img = sender as CachedImage;
            var parent = (ImageNews)img?.BindingContext;
            if (model != null)
            {
                model.UploadVM.DeleteImg(parent);
            }
        }

        private void UploadImage(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.SendImg();
            }
        }

        private void TakePhoto(object sender, EventArgs e)
        {
            var stacklayout = sender as StackLayout;
            stacklayout.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stacklayout.BackgroundColor = Color.Default;
                return false;
            });
            if (model != null)
            {
                model.TakePhoto();
            }
        }

        private void FlowListView_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void ChooseImgDevice(object sender, EventArgs e)
        {
            var stacklayout = sender as StackLayout;
            stacklayout.BackgroundColor = Color.Gray;
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                stacklayout.BackgroundColor = Color.Default;
                return false;
            });
            if (model != null)
            {
                model.ChooseImgDevice();
            }
        }

        private void FlowImageShow(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.UploadVM.IsShow = true;
            }
        }
    }
}