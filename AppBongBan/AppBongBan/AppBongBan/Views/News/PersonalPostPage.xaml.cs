using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.ViewModels.News;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalPostPage : ContentPage
    {
        PersonalPostPageViewModel model;
        public PersonalPostPage()
        {
            InitializeComponent();
            Services.Service.Instiance().PersonalPostViewModelInstance.Reset();
            this.BindingContext = Services.Service.Instiance().PersonalPostViewModelInstance;
        }
        protected override void OnAppearing()
        {
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            if (model == null)
                model = this.BindingContext as PersonalPostPageViewModel;
            model.page = this;
            SelectImageCmd.Instance().ChooseImage = ChooseImage.PersonalPost;
            Helper.CaseSelectImag = SelectImage.SelectedImagePersonal;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            List<ImageNews> list;
            list = await Helper.TakePhoto();
            if(list == null)
            {
                return;
             // await  DisplayAlert("","Bạn cần cấp quyền Camera để xử dụng ứng dụng này.", "Ok");
            }
            if (list.Count == 1)
            {
                MessagingCenter.Send<App, List<ImageNews>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", list);
            }
        }
        /// <summary>
        /// Lấy ảnh từ thư viện
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await DependencyService.Get<IMediaService>().OpenGallery(true);
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            var img = (CachedImage)sender;
            var parent = img.BindingContext as ImageNews;
            if (model.images.Contains(parent))
            {
                model.images.Remove(parent);
                Helper.prev.Remove(parent);
                Helper.DeleteUri(new List<ImageNews> { parent });
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            model.SendContentPost();
             Navigation.PopAsync();
        }
    }
}