using Sport.News;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CatagoryTabbedPage : TabbedPage
    {
        public CatagoryTabbedPage()
        {
            InitializeComponent();
            GetNewsData();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // await GetData();
        }
        public void GetNewsData()
        {
            Task.Run(async () =>
            {
                LoginResponse loginResponse = PingPongNews.Login();
                ListCategoriesResponse categoriesResponse = await PingPongNews.GetListCategories(1, 10);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (categoriesResponse != null && categoriesResponse.success)
                    {
                        var Categories = categoriesResponse.data.categories;
                        if (Categories.Count > 0)
                        {
                            foreach (var item in Categories)
                            {
                                this.Children.Add(new ItemNewsPage(item) { Title=item.name});
                            }
                        }
                    }
                });
            });
        }

        public async Task GetData()
        {
            Debug.WriteLine("ThreadId: " + Thread.CurrentThread.ManagedThreadId.ToString());
            LoginResponse loginResponse = PingPongNews.Login();
            ListCategoriesResponse categoriesResponse = await PingPongNews.GetListCategories(1, 10);
            Device.BeginInvokeOnMainThread(() =>
            {
                if (categoriesResponse != null && categoriesResponse.success)
                {
                    var Categories = categoriesResponse.data.categories;
                    if (Categories.Count > 0)
                    {
                        foreach (var item in Categories)
                        {
                            this.Children.Add(new ItemNewsPage(item));
                        }
                    }
                }
            });
        }
    }
}