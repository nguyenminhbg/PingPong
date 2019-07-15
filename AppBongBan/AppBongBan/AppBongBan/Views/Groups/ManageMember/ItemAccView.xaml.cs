using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.ManageMember
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemAccView : ContentView
	{
        public event EventHandler TapAccept;
        public event EventHandler TapDelete;
		public ItemAccView ()
		{
			InitializeComponent ();
            //button chấp nhận thành viên
            var gestureRecogn = new TapGestureRecognizer();
            gestureRecogn.Tapped += (sender, e) => TapAccept?.DynamicInvoke(sender, e);
            btnAccept.GestureRecognizers.Add(gestureRecogn);
            //button xóa thêm thành viên
            gestureRecogn = new TapGestureRecognizer();
            gestureRecogn.Tapped += (sender, e) => TapDelete?.DynamicInvoke(sender, e);
            btnDelete.GestureRecognizers.Add(gestureRecogn);
        }
	}
}