using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.PersonalViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetUpAccPage : ContentPage
    {
        public SetUpAccPage()
        {
            InitializeComponent();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null) lv.SelectedItem = null;
        }

    }
}