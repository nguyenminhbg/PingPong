using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.ViewModels.Checkins;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.Checkins
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckInPage : ContentPage
    {
        public CheckInPage()
        {
            InitializeComponent();
        }
        CheckinPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Helper.Instance().CurrentPage = this;
            Helpers.Helper.CaseSelectImag = Helpers.SelectImage.SelectedItem;
            SelectImageCmd.Instance().ChooseImage = ChooseImage.Checkin;
            Services.Service.Instiance().StPostContent = Services.StatuePostContent.AddContent;
            if (model == null)
            {
                model = BindingContext as CheckinPageViewModel;
            }
        }
        /// <summary>
        /// Thực hiện click đăng check in lên server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (model != null)
            {
                if (enContent != null)
                    model.CheckVM.SendCheckin(enContent.Text);
            }
        }

        private void enContent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DeleteImg(object sender, EventArgs e)
        {
            var img = sender as CachedImage;
            var parent = (ImageNews)img?.BindingContext;
            if (model != null)
            {
                model.CheckVM.DeleteImg(parent);
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            await image.ScaleTo(1.2, 150, Easing.CubicOut);
            await image.ScaleTo(1, 150, Easing.CubicIn);
            if (model != null)
                model.CheckVM.AddImageExecute();
        }
    }
}