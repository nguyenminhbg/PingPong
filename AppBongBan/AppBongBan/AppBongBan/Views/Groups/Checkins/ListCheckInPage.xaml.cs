using AppBongBan.Models;
using AppBongBan.ViewModels.Checkins;
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
    public partial class ListCheckInPage : ContentPage
    {
        public ListCheckInPage()
        {
            InitializeComponent();
        }
        ListCheckInPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = BindingContext as ListCheckInPageViewModel;
        }
        /// <summary>
        /// Thực hiện load more thêm dữ liệu trong checkin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var item = e.Item as CheckinInfo;
            if (model != null&&item!=null)
                model.ListCheckinVM.Loading(item);
        }
    }
}