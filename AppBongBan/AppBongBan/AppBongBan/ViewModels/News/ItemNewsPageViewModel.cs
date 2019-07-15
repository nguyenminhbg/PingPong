using Sport.News;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.News
{
    public class ItemNewsPageViewModel : ViewModelBase
    {
        private string _titleNews;
        public string titleNews
        {
            get { return _titleNews; }
            set { SetProperty(ref _titleNews, value); }
        }
        private Sport.News.EntityID _category;
        public int CurrentPage { get; set; }
        private List<Sport.News.News> _listNews;
        public int LimitRecord { get { return 20; } }
        public List<Sport.News.News> listNews
        {
            get { return _listNews; }
            set { SetProperty(ref _listNews, value); }
        }
        public ItemNewsPageViewModel(EntityID entityID)
        {
            _category = entityID;
            GetData();
        }
        public  void GetData()
        {
            Task.Run(async () =>
            {
                var ListCategoriesNewsResponse = await PingPongNews.GetCategoryNews((this.CurrentPage + 1), this.LimitRecord, _category.slug);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if(Device.RuntimePlatform == Device.iOS)
                    {
                        var title = _category.name.Substring(0,9);
                        titleNews = title;
                    }
                    else titleNews = _category.name;
                    listNews = new List<Sport.News.News>();
                    if (ListCategoriesNewsResponse != null && ListCategoriesNewsResponse.success)
                    {
                        var listNews = ListCategoriesNewsResponse.data.news;
                        if (listNews.Count > 0)
                        {
                            this.listNews.AddRange(listNews);
                        }
                    }
                });
            });
        }
    }
}
