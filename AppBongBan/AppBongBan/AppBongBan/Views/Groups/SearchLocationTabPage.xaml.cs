using Naxam.Controls.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchLocationTabPage : TabbedPage
    {
        public SearchLocationTabPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           if(Device.RuntimePlatform == Device.iOS)
            {
                club.Icon = "club";
                person.Icon = "Personal";
            }
        }
    }
}