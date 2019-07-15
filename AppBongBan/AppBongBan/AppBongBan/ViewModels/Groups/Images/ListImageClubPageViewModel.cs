using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using DLToolkit.Forms.Controls;
using PingPong;
using Prism.Navigation;
using qhmono;

namespace AppBongBan.ViewModels.Groups.Images
{
    public class ListImageClubPageViewModel : BaseViewModel
    {

        private ListImageClubVM _listImageVM;

        public ListImageClubPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Loading = false;
            Services.Service.Instiance().ListImageVM.Reset();
            listImageVM = Services.Service.Instiance().ListImageVM;
        }
        public Club club { get; set; }

        public ListImageClubVM listImageVM { get => _listImageVM; set { SetProperty(ref _listImageVM, value); } }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                contentInfo = null;
                var clubID = long.Parse(parameters["Club"].ToString());
                if (!Helper.Instance().ListClub.TryGetValue(clubID, out Club clubContent))
                {
                    clubContent = new Club() { ClubID = clubID };
                    Helper.Instance().ListClub.Add(clubID, clubContent);
                }
                club = Helper.Instance().ListClub[clubID];
                // Kiểm tra sự tồn tại của Club
                Helper.Instance().CheckExistClubAsync(clubID);
                if (club != null)
                {
                    listImageVM.Loading = true;
                    // Lấy toàn bộ ảnh của Club về
                    SendGetImage(clubID);
                }
            }
            // Lấy từ trang chủ
            else
            {
                club = null;
                //  listImageVM.Loading = true;
                contentInfo = parameters["imgs"] as ContentInfo;
                FlowObservableCollection<Models.Db.Content.Images> imglist = new FlowObservableCollection<Models.Db.Content.Images>();
                foreach (var item in contentInfo?.Detail.DetailContent.Images_Id)
                {
                    imglist.Add(new Models.Db.Content.Images() { ImageUrl =item, Content_Id = contentInfo.Detail.Id});
                }
                // Danh sách các ảnh lấy ra
               listImageVM.ListImage = imglist;
            }
        }
        ContentInfo contentInfo;
        public void SendGetImage(long IdClub)
        {
            if (IdClub > 0)
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_GET_CLUB_IMAGES_REQ);
                msg.SetAt((byte)MsgGetClubImagesReqArg.ClubID, new QHNumber(IdClub));
                Services.Service.Instiance().SendMessage(msg);
            }
        }
        public async void DetailImage(AppBongBan.Models.Db.Content.Images img)
        {
            var param = new NavigationParameters();
             if (club !=null)
             param.Add("Club", club.ClubID);
            // Truyền Index để lấy vị trí
            int index = listImageVM.ListImage.IndexOf(img);
            param.Add("index", index);
            if(contentInfo != null)
            param.Add("contentImg", contentInfo);
            await Navigation.NavigateAsync("DetailListImagePage", param);
        }
    }

}
