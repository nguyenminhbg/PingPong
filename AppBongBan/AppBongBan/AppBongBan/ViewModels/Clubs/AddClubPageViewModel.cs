using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db;
using AppBongBan.Models.PingPongs;
using AppBongBan.Views.Clubs;
using PingPong;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace AppBongBan.ViewModels.Clubs
{
    public class AddClubPageViewModel : BaseViewModel
    {
        public AddClubViewModel AddClubViewModel { get; set; }
        public AddClubPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            AddClubViewModel = Services.Service.Instiance().AddClubModel;
            AddClubViewModel.navigationService = navigationService;
            AddClubViewModel.Reset();
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("addressCommuneWard"))
            {
                var communeWard = parameters["addressCommuneWard"] as AddressCommuneWard;
                AddClubViewModel.ChangeCommnuneWard(communeWard);
            }
            else if (parameters.ContainsKey("addressDistrict"))
            {
                var district = parameters["addressDistrict"] as AddressDistrict;
                AddClubViewModel.ChangeDistrict(district);
            }
            else if (parameters.ContainsKey("addressProvince"))
            {
                var province = parameters["addressProvince"] as AddressProvince;
                AddClubViewModel.ChangeProvince(province);
            }
            else if (parameters.ContainsKey("Position"))
            {
                AddClubViewModel.MyPostion = (Position)parameters["Position"];
            }
        }
    }
}
