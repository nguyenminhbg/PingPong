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
	public partial class ItemClubView : ContentView
	{
        public event EventHandler TapMap;
        public event EventHandler TapJoin;
        public event EventHandler TapListUsr;
        public event EventHandler TapCheckin;
        public event EventHandler TapChall;

        public ItemClubView ()
		{
			InitializeComponent ();
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapMap?.DynamicInvoke(sender, e);
            ShowMap.GestureRecognizers.Add(gestureRecognizer);

            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapJoin?.DynamicInvoke(sender, e);
            JoinClub.GestureRecognizers.Add(gestureRecognizer);

            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapListUsr?.DynamicInvoke(sender, e);
            ListUsr.GestureRecognizers.Add(gestureRecognizer);

            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapCheckin?.DynamicInvoke(sender, e);
            CheckIn.GestureRecognizers.Add(gestureRecognizer);

            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapChall?.DynamicInvoke(sender, e);
            Chall.GestureRecognizers.Add(gestureRecognizer);
        }
        
	}
}