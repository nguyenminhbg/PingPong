
using Prism.Navigation;

namespace AppBongBan.ViewModels.Personal
{
    public class MorePageViewModel : BaseViewModel
    {
        public MorePageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        public void NavListClubPage()
        {
            Navigation.NavigateAsync("ListClubPage");
        }
        public  void NavAddClubExecute()
        {
            Navigation.NavigateAsync("AddClubPage");
        }
        public void NavSearchPageExecute()
        {
             Navigation.NavigateAsync("SearchLocationTabPage");
           // Navigation.NavigateAsync("SearchLocationPage");
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }
        public void NaviMyClubs()
        {
             Navigation.NavigateAsync("MyClubsPage");
        }
        public void NaviMyAccount()
        {
            var param = new NavigationParameters();
            param.Add("Account", 0);
            Navigation.NavigateAsync("DetailPersonPage", param);
        }
    }
}
