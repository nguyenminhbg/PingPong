using AppBongBan.ViewModels.Groups.Images;
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
	public partial class DetailImageCmdPage : ContentPage
	{
		public DetailImageCmdPage ()
		{
			InitializeComponent ();
		}
        DetailImageCmdPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = BindingContext as DetailImageCmdPageViewModel;
        }
        private void TapDetailContent(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.Back();
            }
        }
    }
}