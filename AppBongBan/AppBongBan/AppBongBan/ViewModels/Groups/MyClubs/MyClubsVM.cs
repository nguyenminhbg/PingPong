using AppBongBan.Models.PingPongs;
using DLToolkit.Forms.Controls;

namespace AppBongBan.ViewModels.Groups.MyClubs
{
    public class MyClubsVM : ViewModelBase
    {
        private bool _isNull;
       
        public FlowObservableCollection<Club> ListClubs { set; get; }
        public bool IsNull { get => _isNull; set { SetProperty(ref _isNull, value); } }
        //public bool IsLoading { get => _isLoading; set { SetProperty(ref _isLoading, value); } }
        public MyClubsVM()
        {
            ListClubs = new FlowObservableCollection<Club>();
            IsLoading = false;
        }
        public override void Reset()
        {
            ListClubs = new FlowObservableCollection<Club>();
            IsLoading = false;
        }
    }
}
