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
	public partial class ItemNewsView : ContentView
	{

        public event EventHandler TabLike;
        public event EventHandler TabComment;
        public event EventHandler TabShare;
        public event EventHandler TapImage;

		public ItemNewsView ()
		{
			InitializeComponent ();
            //thưc hiện tác vụ like
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TabLike?.DynamicInvoke(sender, e);
            like.GestureRecognizers.Add(gestureRecognizer);
            //thực hiện tác vụ comment
            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TabComment?.DynamicInvoke(sender, e);
            Comment.GestureRecognizers.Add(gestureRecognizer);
            //thực hiện tác vụ share
            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TabShare?.DynamicInvoke(sender, e);
            share.GestureRecognizers.Add(gestureRecognizer);
            //thực hiện tác vụ SelectImage
            gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapImage?.DynamicInvoke(sender, e);
           // LayoutFour.GestureRecognizers.Add(gestureRecognizer);
            fourOne.GestureRecognizers.Add(gestureRecognizer);
            fourTwo.GestureRecognizers.Add(gestureRecognizer);
            fourThree.GestureRecognizers.Add(gestureRecognizer);
            fourLbl.GestureRecognizers.Add(gestureRecognizer);
          //  LayoutThree.GestureRecognizers.Add(gestureRecognizer);
            threeimg1.GestureRecognizers.Add(gestureRecognizer);
            threeimg2.GestureRecognizers.Add(gestureRecognizer);
            threeimg3.GestureRecognizers.Add(gestureRecognizer);
           // LayoutTwo.GestureRecognizers.Add(gestureRecognizer);
            twoimg1.GestureRecognizers.Add(gestureRecognizer);
            twoimg2.GestureRecognizers.Add(gestureRecognizer);
            oneimg.GestureRecognizers.Add(gestureRecognizer);
            // LayoutOne.GestureRecognizers.Add(gestureRecognizer);
        }
    }
}