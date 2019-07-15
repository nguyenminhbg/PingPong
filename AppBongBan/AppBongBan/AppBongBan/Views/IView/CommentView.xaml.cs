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
    public partial class CommentView : ContentView
    {
        public event EventHandler TabLike;
        public event EventHandler TabComment;
        public event EventHandler TabShowImage;
        public CommentView()
        {
            InitializeComponent();
            //thêm sự kiện cho chức năng like
            var gestureRecogn = new TapGestureRecognizer();
            gestureRecogn.Tapped += (sender, e) => TabLike?.DynamicInvoke(sender, e);
            like.GestureRecognizers.Add(gestureRecogn);
            likeComment.GestureRecognizers.Add(gestureRecogn);
            //thêm sự kiện cho chức năng comment
            gestureRecogn = new TapGestureRecognizer();
            gestureRecogn.Tapped += (sender, e) => TabComment?.DynamicInvoke(sender, e);
            answer.GestureRecognizers.Add(gestureRecogn);
            answerComment.GestureRecognizers.Add(gestureRecogn);
            //thêm sự kiện xem chi tiết ảnh
            gestureRecogn = new TapGestureRecognizer();
            gestureRecogn.Tapped += (sender, e) => TabShowImage?.DynamicInvoke(sender, e);
            ImageCmd.GestureRecognizers.Add(gestureRecogn);
            ImageReCmd.GestureRecognizers.Add(gestureRecogn);

        }
    }
}