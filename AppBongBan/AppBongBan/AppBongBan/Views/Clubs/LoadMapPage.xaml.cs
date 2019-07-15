using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models.Db;
using AppBongBan.ViewModels.Clubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Clubs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadMapPage : ContentPage
    {
        LoadMapPageViewModel model;
        public LoadMapPage()
        {
            InitializeComponent();
            map.UiSettings.MapToolbarEnabled = true;
            map.PinDragStart += (_, e) => Position.Text = $"Bắt Đầu kéo thả - {PrintPin(e.Pin)}";
            map.PinDragging += (_, e) => Position.Text = $"Đang kéo vị trí - {PrintPin(e.Pin)}";
            map.PinDragEnd += (_, e) =>
            {
                Position.Text = $"Tọa độ - {PrintPin(e.Pin)}";
                if (model != null)
                    model.Position = e.Pin.Position;
            };
        }
        public bool isFirst = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as LoadMapPageViewModel;
            }
            if (!isFirst)
            {
                Device.StartTimer(TimeSpan.FromMilliseconds(1500), () =>
              {
                  if (model != null)
                  {
                      SearchMap(model.Addr, model.Name);
                  }
                  return false;
              });
                isFirst = true;
            }
            
        }

        private string PrintPin(Pin pin)
        {
            return $"{pin.Label}({pin.Position.Latitude.ToString("0.000")},{pin.Position.Longitude.ToString("0.000")})";
        }
        /// <summary>
        /// Search dữ liệu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchMap(string addr, string name)
        {
            try
            {
                var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
                var positions = await geocoder.GetPositionsForAddressAsync(addr);
                if (positions != null && positions.Count() > 0)
                {
                    if (map.Pins.Count > 0)
                        map.Pins.Clear();
                    var pos = positions.First();
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(1000)));
                    Pin pinLocation = new Pin()
                    {
                        Type = PinType.Place,
                        Label = name,
                        Address = addr,
                        Position = pos
                    };
                    Position.Text = $"Tọa độ - {PrintPin(pinLocation)}";
                    if (model != null)
                        model.Position = pos;
                    pinLocation.IsDraggable = true;
                    map.Pins.Add(pinLocation);
                    map.SelectedPin = pinLocation;
                }
                else
                {
                    await this.DisplayAlert("Not found", "Geocoder returns no results", "Close");
                }
            }catch(Exception e)
            {
                var answer = await DisplayAlert("Error", "Ấn \"Yes\" để reload lại map hoặc \"No\" để quay trở lại", "Yes", "No");
                if (answer)
                {
                    SearchMap(model.Addr, model.Name);
                }
                else
                {
                    UserDialogs.Instance.Toast("Không có dữ liệu tọa độ của club");
                    if (model != null)
                    {
                        model.BackPage();
                    }
                }
               
            }
        }
        private void AcceptPositon(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.BackPage();
            }

        }
    }
}