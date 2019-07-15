using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.IView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemPerView : ContentView
	{
        public event EventHandler TapFriend;
        public event EventHandler TapMap;
        public event EventHandler TapClub;
        public event EventHandler TapChall;
		public ItemPerView ()
		{
			InitializeComponent ();
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapFriend?.DynamicInvoke(sender, e);
            ActionFriend.GestureRecognizers.Add(gestureRecognizer);

            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapMap?.DynamicInvoke(sender, e);
            Map.GestureRecognizers.Add(gestureRecognizer);

            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapClub?.DynamicInvoke(sender, e);
            ActionClub.GestureRecognizers.Add(gestureRecognizer);

            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapChall?.DynamicInvoke(sender, e);
            ActionChallenge.GestureRecognizers.Add(gestureRecognizer);

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}