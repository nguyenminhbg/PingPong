using AppBongBan.Controls;
using AppBongBan.ViewModels.Groups.Images;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.Images
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetImagePage : ContentPage
    {
        public ObservableCollection<View> myItemsSource
        {
            set; get;
        }
        public DetImagePage()
        {
            InitializeComponent();
            myItemsSource = new ObservableCollection<View>()
            {

            };
            myItemsSource.Add(new Image()
            {
                Source = "https://i.pinimg.com/736x/56/72/4e/56724e8e91c5cab130c4eeb06f907ea2.jpg"
            });
            myItemsSource.Add(new Image()
            {
                Source = "https://cdn.en.ntvbd.com/site/photo-1520846503"
            });
            carousel.ItemsSource = myItemsSource;
        }
        DetImagePageViewModel model;
        protected override void OnAppearing()
        {
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as DetImagePageViewModel;
            }
          
        }
        private void TapDetailContent(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.BackContent();
            }
        }

        private void carousel_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            if (model != null)
            {
               // model.SelectedExe(e.NewValue + 1);
            }
        }
    }
}