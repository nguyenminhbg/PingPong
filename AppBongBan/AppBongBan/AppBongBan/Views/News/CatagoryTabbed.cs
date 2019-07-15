
using Sport.News;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.Views.News
{
    class CatagoryTabbed : TabbedPage
    {
        public CatagoryTabbed()
        {
            OnInit();
        }

        private void OnInit()
        {
            CreateTabCategories();

        }

        private void CreateTabCategories()
        {
            Task.Run( async () => {

                LoginResponse loginResponse = PingPongNews.Login();

                ListCategoriesResponse categoriesResponse = await PingPongNews.GetListCategories(1, 10);

                //var ListCategoriesResponse = await GetListCategories();

                Device.BeginInvokeOnMainThread(() => {

                    if (categoriesResponse.success)
                    {
                        var Categories = categoriesResponse.data.categories;

                        foreach (var category in Categories)
                        {
                            Children.Add(new CategoryTab(category));
                        }
                    }
                    else
                    {
                        Debug.WriteLine("List Categories can't get");
                    }

                });

            });

            
        }
        
    }
}
