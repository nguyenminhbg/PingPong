using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models.Db;
using AppBongBan.ViewModels.Clubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Clubs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchAddressPage : ContentPage
    {
        public SearchAddressPage()
        {
            InitializeComponent();
            //BindingContext = new SearchAddressPageViewModel();

        }
        SearchAddressPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Search.Focus();
            if (model == null)
            {
                model = BindingContext as SearchAddressPageViewModel;
            }
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<AddressCommuneWard> list = null;
            if (!Search.Text.Equals(""))
            {
                if (Search.Text.Contains("'"))
                {
                    UserDialogs.Instance.Toast("Địa chỉ không được chứa ký tự \" ' \"");
                    return;
                }
                if (model.InCase == 1)
                {

                    await Task.Run(() =>
                    {
                        list = Helper.Database.SearchCharCommnuneWard(Search.Text).Result;
                    });
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        lv.ItemsSource = list;

                    });
                }
                else if (model.InCase == 2)
                {
                    lv.ItemsSource = await Helper.Database.SearchCharDistrict(Search.Text);
                }
                else if (model.InCase == 3)
                {
                    lv.ItemsSource = Helper.Database.SearchChar(Search.Text);
                }
            }
            else
            {
                lv.ItemsSource = null;
                list = null;
            }
        }

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {

        }

        private void lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var lv = sender as ListView;
            var item = lv.SelectedItem;
            if (lv != null) lv.SelectedItem = null;
            if (model.InCase == 1)
            {
                model.SelectExe(item as AddressCommuneWard);
                return;
            }
            if (model.InCase == 2)
            {
                model.SelectExe(item as AddressDistrict);
                return;
            }
            if (model.InCase == 3)
            {
                model.SelectExe(item as AddressProvince);
                return;
            }

        }
    }
}