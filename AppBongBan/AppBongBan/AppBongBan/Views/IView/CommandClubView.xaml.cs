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
	public partial class CommandClubView : ContentView
	{
        public event EventHandler TapAddImage;
        public event EventHandler TapCheckin;
        public event EventHandler TapAddNews;
        public event EventHandler TapShare;
		public CommandClubView ()
		{
			InitializeComponent ();
            //chức năng add ảnh
            var gestureRecognizerShare = new TapGestureRecognizer();
            gestureRecognizerShare.Tapped += (sender, e) => TapAddImage?.DynamicInvoke(sender, e);
            ViewAddImage.GestureRecognizers.Add(gestureRecognizerShare);
            //chức năng checkin
            gestureRecognizerShare = new TapGestureRecognizer();
            gestureRecognizerShare.Tapped += (sender, e) => TapCheckin?.DynamicInvoke(sender, e);
            ViewCheckin.GestureRecognizers.Add(gestureRecognizerShare);
            //chức năng addnews
            gestureRecognizerShare = new TapGestureRecognizer();
            gestureRecognizerShare.Tapped += (sender, e) => TapAddNews?.DynamicInvoke(sender, e);
            ViewAddNews.GestureRecognizers.Add(gestureRecognizerShare);
            //chức năng share
            gestureRecognizerShare = new TapGestureRecognizer();
            gestureRecognizerShare.Tapped += (sender, e) => TapShare?.DynamicInvoke(sender, e);
            ViewShare.GestureRecognizers.Add(gestureRecognizerShare);

        }
	}
}