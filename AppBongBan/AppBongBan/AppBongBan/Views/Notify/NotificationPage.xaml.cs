

using AppBongBan.Custom.Badge;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.ViewModels.Notify;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Notify
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationPage : TabbedPage
    {
		public NotificationPage ()
		{
			InitializeComponent ();
            this.BindingContext =new NotificationPageViewModel();
		}
        NotificationPageViewModel model;
        protected override void OnAppearing()
        {
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            if(Device.RuntimePlatform == Device.iOS)
            {
                fiend.Icon = "ReqAdd";
                challegen.Icon = "pingpong";
                community.Icon = "club";
            }
            if (model == null)
                model = this.BindingContext as NotificationPageViewModel;
        }
        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            Title = CurrentPage.Title;
            if (Title== "Bạn bè")
            {
                AppChat.Helpers.Helper.Instiance().CountRequestAddFrd = 0;
            }
            if(Title == "Thách đấu")
            {
                Helper.Instance().CountChallengeNotifi = 0;
            }
        }
    }
}