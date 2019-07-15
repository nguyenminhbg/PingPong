using System;
using System.Collections.Generic;
using System.Text;
using AppBongBan.Helpers;
using AppBongBan.Models.Db;
using Prism.Navigation;
using Xamarin.Forms.GoogleMaps;

namespace AppBongBan.ViewModels.Clubs
{
    public class LoadMapPageViewModel : BaseViewModel
    {
        public string address { get => _address; set { SetProperty(ref _address, value); } }
        public string CommunWard { get => _communWard; set { SetProperty(ref _communWard, value); } }
        public string District { get => _district; set { SetProperty(ref _district, value); } }
        public string Province { get => _province; set { SetProperty(ref _province, value); } }
        public string Title { get => _title; set { SetProperty(ref _title, value); } }
        public string Name { get; set; }
        public string Addr { get => _addr; set { SetProperty(ref _addr, value); } }
        public Position Position { get => _positon; set { SetProperty(ref _positon, value); } }
        private Position _positon;
        private string _address;
        private string _communWard;
        private string _district;
        private string _province;
        private string _title;
        private string _addr;

        public LoadMapPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Position = new Position();
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("addressCommuneWard"))
            {
                var addr = (AddressCommuneWard)parameters["addressCommuneWard"];
                CommunWard = addr.CommnuneWard;
                District = addr.District;
                Province = addr.ProvinceName;
                Addr = addr.TextShow;
            }
            if (parameters.ContainsKey("addressDistrict"))
            {
                var addr = (AddressDistrict)parameters["addressDistrict"];
                District = addr.District;
                Province = addr.ProvinceName;
                Addr = addr.TextShow;
            }
            if (parameters.ContainsKey("addressProvince"))
            {
                var addr = (AddressDistrict)parameters["addressDistrict"];
                Province = addr.ProvinceName;
                Addr = addr.TextShow;
            }
            if (parameters.ContainsKey("ClubName"))
            {
                Title = parameters["ClubName"].ToString();
                Name = Title;
            }
            if (parameters.ContainsKey("Street"))
            {
                address = parameters["Street"].ToString() + ", " + Addr;
            }
        }

        public async void BackPage()
        {
            if (Position.Latitude > 0 || Position.Longitude > 0)
            {
                var param = new NavigationParameters();
                param.Add("Position", Position);
                await Navigation.GoBackAsync(param);
            }
            else
            {
                NotifiDialog.Initiance().DilogErrorPosisionClub();
            }
        }
    }
}
