using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Sport.News;
using Xamarin.Forms;


namespace AppBongBan.Views.News
{
    class CategoryItemDetail : ContentPage
    {
        private readonly Sport.News.News _news;
        private StackLayout contentLayout;

        private static Dictionary<string, NewsDetailResponse> _cache = new Dictionary<string, NewsDetailResponse>();

        public CategoryItemDetail(Sport.News.News news)
        {
            _news = news;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Debug.WriteLine(_news.title);

            Title = "Tin tức";

            this.OnInit();
        }

        private void OnInit()
        {
            CreateUI();
            GetNewsDetail();
        }
        HtmlWebViewSource htmlSource;
        WebView webview;
        private void CreateUI()
        {
            contentLayout = new StackLayout();
            webview = new WebView();
            htmlSource = new HtmlWebViewSource();
            contentLayout.Children.Add(webview);
            //contentLayout = new StackLayout();

            //var image = new Image();
            //image.Source = _news.image;
            //image.Aspect = Aspect.AspectFill;
            //contentLayout.Children.Add(image);

            //var lblTitle = new Label();
            //lblTitle.FontSize = 14;
            //lblTitle.TextColor = Color.Black;
            //lblTitle.Text = _news.title;
            //contentLayout.Children.Add(lblTitle);

            //StackLayout horizontalDate = new StackLayout();
            //horizontalDate.Orientation = StackOrientation.Horizontal;
            ////horizontalDate.VerticalOptions = LayoutOptions.CenterAndExpand;

            //var lblPublishDate = new Label();
            //lblPublishDate.Text = _news.publish_date.ToString("hh:mm dd/MM/yyyy");

            //var lblUser = new Label();
            ////lblUser.HorizontalOptions = LayoutOptions.CenterAndExpand;
            //lblUser.Text = _news.user.username;

            //horizontalDate.Children.Add(lblPublishDate);
            //horizontalDate.Children.Add(lblUser);

            //contentLayout.Children.Add(horizontalDate);


            //ScrollView scrollView = new ScrollView();
            //scrollView.Content = contentLayout;

            //this.Content = scrollView;
            //this.Padding = new Thickness(5, 5, 5, 5);
        }

        public string DetailNews
        {
            get { return _detailNews; }
            set
            {
                _detailNews = value;
                OnPropertyChanged("DetailNews");
            }
        }
        private string _detailNews;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        private void GetNewsDetail()
        {

            NewsDetailResponse newsDetailResponse = null;

            _cache.TryGetValue(_news.slug, out newsDetailResponse);

            if (newsDetailResponse == null)
            {
                newsDetailResponse = PingPongNews.GetNewsDetail(_news.slug);

                _cache.Add(_news.slug, newsDetailResponse);
            }


            if (newsDetailResponse.success)
            {
                var data = newsDetailResponse.data.content;
                //lblContent.Text = "<!DOCTYPE><html><body>" + data + "</body></html>";
                //lblContent.Text = "<html><body><h1>Xamarin.Forms</h1><p>Welcome to WebView.</p></body></html>";
                // var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                // htmlDoc.LoadHtml(data);
                //var header = htmlDoc.DocumentNode.Descendants("header")
                //               .Where(e => e.GetAttributeValue("class", "") == "article__title");
                //var contentNews = htmlDoc.DocumentNode.Descendants("section")
                //    .Where(e => e.GetAttributeValue("class", "") == "article__text type");
                //var htmlNodes = contentNews as HtmlNode[] ?? contentNews.ToArray();
                //var contentNode = htmlNodes.First().Descendants("ul")
                //    .Where(e => e.GetAttributeValue("class", "") == "related_post");
                //var enumerable = contentNode as HtmlNode[] ?? contentNode.ToArray();
                //if (enumerable.Count() != 0)
                //{
                //    htmlNodes.First().RemoveChild(enumerable.First());
                //}
                //   string content = header.First().InnerHtml;
                // string content = htmlNodes.First().InnerHtml;
                string html =
                    "<html><head><style>img{max-width: 100%; width:auto; max-height: 250;}h1{color:blue;font-size: 100%}</style></head><body>" +
                    data +
                    "<h1>Nguồn gametv.vn<h1></body></html>";

                //lblContent = new HtmlFormattedLabel()
                //{
                //    FontSize = 12,
                //    Text = data
                //};
                //contentLayout.Children.Add(lblContent);
                htmlSource.Html = html;
                webview.Source = htmlSource;
            }
            else
            {
                Debug.WriteLine("CategoryItemDetail: " + newsDetailResponse.message);
            }
        }


    }
}
