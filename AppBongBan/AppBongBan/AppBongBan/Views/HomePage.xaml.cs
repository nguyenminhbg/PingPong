using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Views.Login;
using AppBongBan.Views.News;
using AppBongBan.Views.PersonalViews;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace AppBongBan.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : MasterDetailPage
    {
        public static Action avartaChange;
        List<MenuPage> listMenu;
        public HomePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            listMenu = new List<MenuPage> {
                new MenuPage{Title="Tin tức", Icon="news.png",TargetType = typeof(NewsSiteHomePage) },
              //  new MenuPage{Title="Trang chủ cá nhân", Icon="home_menu.png",TargetType = typeof(PersonalPage) },
                //new MenuPage{Title="Đăng nhập", Icon="login.png", TargetType = typeof(LoginPage)  },
                new MenuPage{Title="Liên hệ", Icon="phone.png", TargetType = typeof(Page1) },
                new MenuPage{Title="Thông tin sản phẩm", Icon="information.png", TargetType = typeof(Page2) }
            };
            menu.ItemsSource = listMenu;
            if (Helper.Instance().MyAccount != null)
                Detail = new NavigationPage(new PersonalPage());
            else Detail = new NavigationPage(new LoginPage());
            NameUser.SetBinding(Label.TextProperty, "fullname");
            NameUser.BindingContext = Helper.Instance().MyAccount;
            Avatar.SetBinding(CachedImage.SourceProperty, "Avatar_Uri");
            Avatar.BindingContext = Helper.Instance().MyAccount;
            if (Helper.Instance().MyAccount == null || string.IsNullOrEmpty(Helper.Instance().MyAccount.Avatar_Uri))
                avartaStack.IsVisible = false;
            else avartaStack.IsVisible = true;
            avartaChange = ChangeAvarta;
            //if (Helper.Instance().CheckLogin())
            //{
            //    NameUser.SetBinding(Label.TextProperty, "fullname");
            //    NameUser.BindingContext = Helper.Instance().MyAccount;
            //    Avatar.SetBinding(CachedImage.SourceProperty, "Avatar_Uri");
            //    Avatar.BindingContext = Helper.Instance().MyAccount;
            //    // Gán nameDataBase để khởi tạo slqLite và AccountChat
            //    AppChat.Helpers.Helper.Instiance().nameDataBase = Helper.Instance().MyAccount.Number_Id.ToString();
            //    AppChat.Helpers.Helper.Instiance().myAccount = Helper.Instance().AccountChat;
            //    var list = AppChat.Helpers.Helper.Instiance().database.GetListAcc(Helper.Instance().MyAccount.Number_Id).Result;
            //    foreach (var item in list)
            //    {
            //        Helper.Instance().MyAccount.Last_Time_Sync_Contact = item.last_time_sync_contact;
            //    }
            //    if (AppChat.Helpers.Helper.Instiance().accountCached.ContainsKey(Helper.Instance().AccountChat.NumberId))
            //        AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().AccountChat.NumberId] = Helper.Instance().AccountChat;
            //    else AppChat.Helpers.Helper.Instiance().accountCached.Add(Helper.Instance().AccountChat.NumberId, Helper.Instance().AccountChat);
            //    // App đến Homepage
            //    Detail = new NavigationPage(new PersonalPage());
            //}
            //else
            //{
            //    Detail = new NavigationPage(new LoginPage());
            //}
            // Khởi tạo _mPosition
            Task.Run(() => Helper.Instance().MyPosition());
        }
        public void ChangeAvarta()
        {
            if (Helper.Instance().MyAccount == null || string.IsNullOrEmpty(Helper.Instance().MyAccount.Avatar_Uri))
                avartaStack.IsVisible = false;
            else avartaStack.IsVisible = true;
        }
        protected override void OnAppearing()
        {
            UserDialogs.Instance.HideLoading();
            if (Xamarin.Forms.Application.Current.MainPage is MasterDetailPage masterDetailPage)
            {
                masterDetailPage.IsGestureEnabled = true;
            }
            else if (Xamarin.Forms.Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
            {
                nestedMasterDetail.IsGestureEnabled = true;
            }
            base.OnAppearing();
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "Login1", (sender) =>
            {
                NameUser.SetBinding(Label.TextProperty, "fullname");
                NameUser.BindingContext = Helper.Instance().MyAccount;
                Avatar.SetBinding(CachedImage.SourceProperty, "Avatar_Uri");
                Avatar.BindingContext = Helper.Instance().MyAccount;
                AppChat.Helpers.Helper.Instiance().nameDataBase = Helper.Instance().MyAccount.Number_Id.ToString();
                AppChat.Helpers.Helper.Instiance().myAccount = Helper.Instance().AccountChat;
                AppChat.Helpers.Helper.Instiance().myAccount.last_time_sync_contact = (int)Helper.Instance().MyAccount.Last_Time_Sync_Contact;
                if (AppChat.Helpers.Helper.Instiance().accountCached.ContainsKey(Helper.Instance().AccountChat.NumberId))
                    AppChat.Helpers.Helper.Instiance().accountCached[Helper.Instance().AccountChat.NumberId] = Helper.Instance().AccountChat;
                else AppChat.Helpers.Helper.Instiance().accountCached.Add(Helper.Instance().AccountChat.NumberId, Helper.Instance().AccountChat);
                Detail = new NavigationPage(new PersonalPage());
            });
            MessagingCenter.Subscribe<App>((App)Application.Current, "Logout", (sender) =>
            {
                Detail = new NavigationPage(new LoginPage());
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        private  void menu_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var list = sender as ListView;
            MenuPage menuPage = (MenuPage)list.SelectedItem;
            Page detail = Detail.Navigation.NavigationStack.Last();
            Type page = menuPage.TargetType;
            list.SelectedItem = null;
            if (menuPage.Title.Equals("Trang chủ cá nhân"))
            {
                //if (Helper.Instance().CheckLogin())
                //{
                //    Page displayPage = (Page)Activator.CreateInstance(page);
                //    Detail.Navigation.InsertPageBefore(displayPage, detail);
                //     Detail.Navigation.PopAsync();
                //}
                //else
                //{
                //    Detail = new NavigationPage(new LoginPage());
                //}
            }

            if (menuPage.Title.Equals("Tin tức"))
            {
                Navigation.PushAsync(new CatagoryTabbedPage());
            }
            if (menuPage.Title.Equals("Liên hệ"))
            {
                Navigation.PushModalAsync(new Page1());
            }
            else if (menuPage.Title.Equals("Thông tin sản phẩm"))
            {
                Navigation.PushAsync(new Page2());
                //if (detail.Title != null && !detail.Title.Equals(menuPage.Title))
                //{
                //    Page displayPage = (Page)Activator.CreateInstance(page);
                //    if (displayPage != null)
                //    {
                //        Detail.Navigation.InsertPageBefore(displayPage, detail);
                //         Detail.Navigation.PopAsync();
                //    }
                //}
            }
            IsPresented = false;
        }

    }
}