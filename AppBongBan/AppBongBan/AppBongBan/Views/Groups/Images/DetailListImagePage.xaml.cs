
using AppBongBan.Controls;
using AppBongBan.ViewModels.Groups.Images;

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.Images
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailListImagePage : ContentPage
	{
		public DetailListImagePage ()
		{
			InitializeComponent ();
		}
        DetailListImagePageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = BindingContext as DetailListImagePageViewModel;
        }
        private void carousel_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            if (model != null)
                model.SelectedExe(e.NewValue + 1);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (model != null)
                model.NaviDetailContent();
        }
    }
}