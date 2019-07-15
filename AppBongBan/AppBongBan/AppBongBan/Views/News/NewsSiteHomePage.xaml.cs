using AppBongBan.Custom.Badge;
using AppBongBan.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsSiteHomePage : TabbedBadgeRenderer
    {
        public NewsSiteHomePage ()
        {
            InitializeComponent();
        }
        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            Title = CurrentPage.Title;
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var exitApp = await DisplayAlert("Thông báo", "Bạn có thực sự muốn thoát khỏi SportTV", "Yes", "No");
                if (exitApp)
                {
                    DependencyService.Get<ICloseApp>().CloseApp();
                }
            });
            return true;
        }
    }
}