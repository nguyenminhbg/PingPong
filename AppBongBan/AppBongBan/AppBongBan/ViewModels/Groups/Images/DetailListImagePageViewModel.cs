

using System.Collections.ObjectModel;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using FFImageLoading.Forms;
using Prism.Navigation;
using Xamarin.Forms;
using DLToolkit.Forms.Controls;
using AppBongBan.Helpers;
namespace AppBongBan.ViewModels.Groups.Images
{
    public class DetailListImagePageViewModel : BaseViewModel
    {
        private int _position;
        //private string _title;
        public DetailListImagePageVM DetailVM { get => _detailVM; set { SetProperty(ref _detailVM, value); } }
        private ObservableCollection<DetImage> _itemSource;

        public ObservableCollection<DetImage> ItemSource { get => _itemSource; set { SetProperty(ref _itemSource, value); } }
        public DetailListImagePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            if(Device.RuntimePlatform == Device.Android)
            MyItemsSource = new ObservableCollection<View>();
            else if (Device.RuntimePlatform == Device.iOS)
            {
                ItemSource = new ObservableCollection<DetImage>();
            }
            Services.Service.Instiance().DetailImgVM.Reset();
            DetailVM = Services.Service.Instiance().DetailImgVM;
        }
        // string Title { get => _title; set { SetProperty(ref _title, value); } }
        public Club club { get; set; }
        public ObservableCollection<View> MyItemsSource { get; set; }
        public int Position { get => _position; set { SetProperty(ref _position, value); } }
        private int count = 0;
        private DetailListImagePageVM _detailVM;
        FlowObservableCollection<Models.Db.Content.Images> imgContents;
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                // Lấy ra clubID để lấy club
                var clubID = long.Parse(parameters["Club"].ToString());
                if(!Helper.Instance().ListClub.TryGetValue(clubID, out Club clubContent))
                {
                    clubContent = new Club() { ClubID = clubID };
                    Helper.Instance().ListClub.Add(clubID, clubContent);
                }
                club = Helper.Instance().ListClub[clubID];
                // Kiểm tra sự tồn tại của Club

                // Lấy ra Index của ảnh
                int indexclub = 0;
                if (parameters.ContainsKey("index"))
                {
                    indexclub = int.Parse(parameters["index"].ToString());
                }
                var listclub = Services.Service.Instiance().ListImageVM.ListImage;
                for (int i = 0; i < listclub.Count; i++)
                {
                    MyItemsSource.Add(new CachedImage()
                    {
                        Source = listclub[i].ImageUrl,
                        Aspect = Aspect.AspectFit,
                    });
                }
                count = listclub.Count;
                Position = indexclub;
                SelectedExe(indexclub + 1);
            }
            else
            {
                if (parameters.ContainsKey("contentImg"))
                {
                    var contentInfo = parameters["contentImg"] as ContentInfo;
                    imgContents = new FlowObservableCollection<Models.Db.Content.Images>();
                    // Lấy ra danh sách ảnh
                    foreach (var item in contentInfo.Detail.DetailContent.Images_Id)
                    {
                        imgContents.Add(new Models.Db.Content.Images() { ImageUrl = item, Content_Id = contentInfo.Detail.Id });
                    }

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        if (parameters.ContainsKey("index"))
                        {
                            var index = int.Parse(parameters["index"].ToString());
                            Position = index + 1;
                        }
                        for (int i = 0; i < imgContents.Count; i++)
                        {
                            MyItemsSource.Add(new CachedImage()
                            {
                                Source = imgContents[i].ImageUrl,
                                Aspect = Aspect.AspectFit,
                            });
                        }
                        count = imgContents.Count;
                        Services.Service.Instiance().ListImageVM.ListImage = imgContents;
                        SelectedExe(Position);
                    }
                   else if(Device.RuntimePlatform == Device.iOS)
                    {
                        for (int i = 0; i < imgContents.Count; i++)
                        {
                            ItemSource.Add(new DetImage()
                            {
                                Index = (i + 1) + "/" + contentInfo.Detail.DetailContent.Images_Id.Count + "-" + contentInfo.Detail.DateCreated,
                                Img = contentInfo.Detail.DetailContent.Images_Id[i]
                            });
                        }
                    }
                }
                
            }
        }
        public string checkRequest;
        public async void SelectedExe(int index)
        {
            var img = Services.Service.Instiance().ListImageVM.ListImage[(index-1)];
             Position = index - 1;
            DetailVM.Title = index + "/" + count;
            if(img?.Content_Id > 0)
            {
                var Content = await Helpers.Helper.Instance().CheckExistContent(img.Content_Id);
                if (Content != null)
                {
                    DetailVM.Title += " - " + Content.Detail.DateImageCreated;
                    DetailVM.LikeCount = Content.LikeContent.LikeCount;
                    DetailVM.CommentCount = Content.CommentCount;
                    DetailVM.IconLike = Content.LikeContent.IconLike;
                }
            }
        }
        public void AddContentFromHelpForSelect(ContentInfo content)
        {
            var Content = content;
            if (Content != null)
            {
                DetailVM.Title += " - " + Content.Detail.DateImageCreated;
                DetailVM.LikeCount = Content.LikeContent.LikeCount;
                DetailVM.CommentCount = Content.CommentCount;
                DetailVM.IconLike = Content.LikeContent.IconLike;
            }
        }

        public async void NaviDetailContent()
        {
            if (Services.Service.Instiance().ListImageVM.ListImage == null) return;
            var img = Services.Service.Instiance().ListImageVM.ListImage[(Position)];
            // Lấy DetailVM.Content
            if (img.Content_Id > 0)
            {
                DetailVM.Content = Helpers.Helper.Instance().ListContent[img.Content_Id];
              //  DetailVM.Content = await Helpers.Helper.Instance().CheckExistContent(img.Content_Id);
                if (DetailVM.Content.Detail.Id > 0)
                {
                    var param = new NavigationParameters();
                    param.Add("Content", DetailVM.Content.Detail.Id);
                    await Navigation.NavigateAsync("CommentNewsPage", param);
                }
            }
        }
    }
}
