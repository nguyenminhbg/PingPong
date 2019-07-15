using AppBongBan.ViewModels.News;
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
	public partial class DetailNewsPage : ContentPage
	{
		public DetailNewsPage ()
		{
			InitializeComponent ();
            this.BindingContext = new DetailNewsPageViewModel();
		}
	}
}