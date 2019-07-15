using Sport.News;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.News
{
    public class DetailNewsPageViewModel : ViewModelBase
    {
        public string DetailNews
        {
            get { return _detailNews; }
            set { SetProperty(ref _detailNews, value); }
        }
        private string _detailNews;
        private string _imageSource;
        public string imageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }
        private static Dictionary<string, NewsDetailResponse> _cache = new Dictionary<string, NewsDetailResponse>();
        public DetailNewsPageViewModel()
        {
            Task.Run(() =>
            {
                NewsDetailResponse newsDetailResponse = null;

                _cache.TryGetValue(Helpers.Helper.Instance().news.slug, out newsDetailResponse);

                if (newsDetailResponse == null)
                {
                    newsDetailResponse = PingPongNews.GetNewsDetail(Helpers.Helper.Instance().news.slug);

                    _cache.Add(Helpers.Helper.Instance().news.slug, newsDetailResponse);
                }
                var data = newsDetailResponse.data.content;
                imageSource = Helpers.Helper.Instance().news.image;
                string html =
                           "<html><head><style>img{max-width: 100%; width:auto; max-height: 250;}h1{color:blue;font-size: 100%}</style></head>" +
                           "<body>" +
                           "<img src=\"" + imageSource + "\">" +
                           data +
                           "<h1>Nguồn Sporttv.vn<h1>" +
                           "</body></html>";
                Device.BeginInvokeOnMainThread(() =>
                {
                    DetailNews = html;
                });
            });
        }

    }
}
