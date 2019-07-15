
using System;
using System.Collections.Generic;
using System.Windows.Input;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.News
{
    public class NewsSitePageViewModel : BaseViewModel
    {
        private NewsSitePageVM _newsSite;
        public NewsSitePageVM NewsSite
        {
            get => _newsSite;
            set
            {
                SetProperty(ref _newsSite, value);
            }
        }
        static INavigationService navi;
        public NewsSitePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            // Reset lại thư viện collection
            Services.Service.Instiance().NewsSiteVM.Reset();
            // Khởi tạo là vấy giá trị của NewsSite
            NewsSite = Services.Service.Instiance().NewsSiteVM;
            // Đăng ký sự kiện khi refresh lại trang
            RefreshCommand = new DelegateCommand(ReFresh);
            // Đăng ký delegate để khi người dùng đăng bài sẽ báo lại cho trang chủ biết
            Helper.Instance().PostNewsNotify = InsertContent;
            navi = Navigation;
        }
        /// <summary>
        /// Phương thức xử lý khi người dùng đăng tin trong club
        /// </summary>
        /// <param name="contentInfo"></param>
        public void InsertContent(ContentInfo contentInfo)
        {
            if (contentInfo.ClubID > 0) contentInfo.IsNews = true;
            else contentInfo.IsNews = false;
            // Kiểm tra sự tồn tại của Club trong khi đăng bài trong club
            if (!Helper.Instance().ListClub.TryGetValue(contentInfo.ClubID, out Club club))
            {
                club = new Club() { ClubID = contentInfo.ClubID };
                Helper.Instance().ListClub.Add(club.ClubID, club);
            }
            contentInfo.clubContent = Helper.Instance().ListClub[contentInfo.ClubID];
            if (string.IsNullOrEmpty(contentInfo.clubContent.ClubName))
                Helper.Instance().CheckExistClubAsync(contentInfo.clubContent.ClubID);
            // Thêm vào danh sách Listcontent Cached
            if(!Helper.Instance().ListContent.TryGetValue(contentInfo.Detail.Id, out ContentInfo content))
            {
                lock (Helper.Instance().ListContent)
                {
                    Helper.Instance().ListContent.Add(contentInfo.Detail.Id, content);
                }
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                NewsSite.ListNews.Insert(0, contentInfo);
            });
        }
        /// <summary>
        /// Lấy thêm bản tin Content
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limit"></param>
        public void GetMoreContent(long id, long limit)
        {
            NewsSite.HomePageGetContentReq(id, limit);
        }

        public void GetMore(int limit)
        {
            GetMoreContent(NewsSite.ListNews[NewsSite.ListNews.Count - 1].Detail.Id, limit);
        }

        public void ReFresh()
        {
            // Xóa dữ liệu content ở viewmodel page
            //  NewsSite.ListNews.Clear();
            NewsSite.IsFreshing = true;
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                NewsSite.IsFreshing = false;
                return false;
            });
            NewsSite.MsgHomePageReqSend();
        }
        public ICommand RefreshCommand { get; set; }
        public void NaviImage(ContentInfo content,string urlImg)
        {
            var param = new NavigationParameters();
            param.Add("Content", content);
         //   param.Add("img", urlImg);
            Navigation.NavigateAsync("DetImagePage", param);
        }
        // Hiển thị từ 4 ảnh trở lên
        public async void NaviListImage(ContentInfo content)
        {
            if (content.Detail.Id > 0)
            {
                //if (Helper.Instance().CachedClubFllowContent.ContainsKey(content.Detail.Id))
                //{
                //    var ClubID = Helper.Instance().CachedClubFllowContent[content.Detail.Id];
                var param = new NavigationParameters();
                // truyền contentInfo cho DetailListImagePage
                param.Add("imgs", content);
                await Navigation.NavigateAsync("ListImageClubPage", param);
                //   }
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }
        }
        public void NaviComment(ContentInfo news)
        {
            var param = new NavigationParameters();
            param.Add("Content", news.Detail.Id);
            Navigation.NavigateAsync("CommentNewsPage", param);
        }
        public void SendLike(bool isLike, ContentInfo content)
        {
            LikeAction.SendLike(isLike, ref content);
        }
        public static void ShowImgExe(List<ImageNews> images)
        {
            if (images.Count > 0)
            {
                var param = new NavigationParameters();
                param.Add("ImgLibrary", images);
                navi.NavigateAsync("PersonalPostPage", param);
            }
        }
    }
}
