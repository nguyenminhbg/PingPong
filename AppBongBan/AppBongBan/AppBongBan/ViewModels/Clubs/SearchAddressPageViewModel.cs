using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AppBongBan.Models.Db;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Clubs
{
    public class SearchAddressPageViewModel : BaseViewModel
    {
        private string _textHolder;

        public SearchAddressPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        public string TextHolder
        {
            get => _textHolder; set
            {
                SetProperty(ref _textHolder, value);
            }
        }
        public int InCase { get; set; }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Search"))
            {
                var search = (int)parameters["Search"];
                if (search == 1)
                {
                    InCase = 1;
                    TextHolder = "Điền xã phường";
                }
                else if (search == 2)
                {
                    InCase = 2;
                    TextHolder = "Điền Quận/Huyện";
                }
                else
                {
                    InCase = 3;
                    TextHolder = "Điền Tỉnh";
                }
            }


        }
        public async void SelectExe(AddressDistrict addressDistrict)
        {
            NavigationParameters param = new NavigationParameters();
            param.Add("addressDistrict", addressDistrict);
            await Navigation.GoBackAsync(param);
        }
        public async void SelectExe(AddressProvince addressProvince)
        {
            NavigationParameters param = new NavigationParameters();
            param.Add("addressProvince", addressProvince);
            await Navigation.GoBackAsync(param);
        }
        public async void SelectExe(AddressCommuneWard addressCommuneWard)
        {
            NavigationParameters param = new NavigationParameters();
            param.Add("addressCommuneWard", addressCommuneWard);
            await Navigation.GoBackAsync(param);
        }
    }
}
