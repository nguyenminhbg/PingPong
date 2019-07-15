using AppBongBan.ViewModels.News;
using Sport.News;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.News
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemNewsPage : ContentPage
	{
		public ItemNewsPage (EntityID entityId)
		{
			InitializeComponent ();
            this.BindingContext = new ItemNewsPageViewModel(entityId);
		}
        private  void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            var item = e.SelectedItem as Sport.News.News;
            Helpers.Helper.Instance().news = item;
             Navigation.PushAsync(new DetailNewsPage());
            ((ListView)sender).SelectedItem = null;
        }
    }
}