
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.Images
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImgDetail : ContentPage
    {
        public ObservableCollection<View> myItemsSource
        {
            set; get;
        }
        public ImgDetail()
        {
            InitializeComponent();
            myItemsSource = new ObservableCollection<View>()
            {

            };

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
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
    }
}