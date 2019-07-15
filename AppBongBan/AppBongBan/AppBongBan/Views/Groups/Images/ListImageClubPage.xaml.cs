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
    public partial class ListImageClubPage : ContentPage
    {
        public ListImageClubPage()
        {
            InitializeComponent();
        }

        private void FlowListView_FlowItemTapped(object sender, ItemTappedEventArgs e)
        {
            var image = e.Item as AppBongBan.Models.Db.Content.Images;
            var model = BindingContext as ListImageClubPageViewModel;
            if (model != null)
                model.DetailImage(image);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
        }
    }
}