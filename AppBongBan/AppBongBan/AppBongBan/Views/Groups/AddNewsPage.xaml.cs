
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.ViewModels.Groups;
using FFImageLoading.Forms;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewsPage : ContentPage
    {
        public AddNewsPage()
        {
            InitializeComponent();
            enTitle.Completed += (s, e) =>
            {
                enContent.Focus();
            };
        }
        AddNewsPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Helper.Instance().CurrentPage = this;
            Helpers.Helper.CaseSelectImag = Helpers.SelectImage.SelectedItem;
            SelectImageCmd.Instance().ChooseImage = ChooseImage.AddNews;
            Services.Service.Instiance().StPostContent = Services.StatuePostContent.AddContent;
            if (model == null)
            {
                model = BindingContext as AddNewsPageViewModel;
            }
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            await image.ScaleTo(1.2, 150, Easing.CubicOut);
            await image.ScaleTo(1, 150, Easing.CubicIn);
            if (model != null)
                model.AddNews.AddImageExecute();
        }

        private void Post_Clicked(object sender, EventArgs e)
        {
            if (model != null)
            {
                if (enTitle != null && enContent != null)
                    model.AddNews.PostContent(enTitle.Text, enContent.Text);
            }
        }

        private void DeleteImg(object sender, EventArgs e)
        {
            var img = sender as CachedImage;
            var parent = (ImageNews)img?.BindingContext;
            if (model != null)
            {
                model.AddNews.DeleteImg(parent);
            }
        }
    }
}