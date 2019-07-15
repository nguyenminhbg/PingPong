using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Group;
using AppBongBan.Views.Groups.Images;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class ClubPageViewModel : BaseViewModel
    {
        private ClubPageVM _clubVM;
        public ClubPageVM ClubVM { get => _clubVM; set { SetProperty(ref _clubVM, value); } }
        public ClubPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().ClubModel.Reset();
            ClubVM = Services.Service.Instiance().ClubModel;
            ClubVM.navigationService = navigationService;
        }
        public void NaviImage(ContentInfo content)
        {
            var param = new NavigationParameters();
            // Truyền content sang DetImagePage
            // param.Add("Content", content.Detail.Id);
            param.Add("Content", content);
            Navigation.NavigateAsync("DetImagePage", param);
        }
        public async void NaviListImage()
        {
            if (ClubVM.MyClub.ClubID > 0)
            {
                var param = new NavigationParameters();
                param.Add("Club", ClubVM.MyClub.ClubID);
                await Navigation.NavigateAsync("ListImageClubPage", param);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }
        // Hiển thị từ 4 ảnh trở lên
        public async void NaviListImage(ContentInfo content)
        {
            if (content.Detail.Id > 0)
            {
                // Nếu chưa có content.Detail.Id thì phải gửi yêu cầu lấy về
                //if (Helper.Instance().ListContentClub.ContainsKey(content.Detail.Id))
                //{
                // var ClubID = Helper.Instance().ListContentClub[content.Detail.Id];
                var param = new NavigationParameters();
                param.Add("imgs", content);
                await Navigation.NavigateAsync("ListImageClubPage", param);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                var ClubID = long.Parse(parameters["Club"].ToString());
                if (!Helper.Instance().ListClub.TryGetValue(ClubID, out Club club))
                {
                    club = new Club() { ClubID = ClubID };
                    Helper.Instance().ListClub.Add(ClubID, club);
                }
                ClubVM.MyClub = Helper.Instance().ListClub[ClubID];
                // gửi bản tin lấy chi tiết clubPage của người dùng
                ClubVM.SendGetDetailClub(ClubVM.MyClub, 0, 10);
            }

            else if (parameters.ContainsKey("Content"))
            {
                var news = (ContentInfo)parameters["Content"];
                // Thêm vào trang chủ bài viết
                ClubVM.BackListAddNews(news);
            }
        }

    }
}
