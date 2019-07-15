using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Services;
using PingPong;
using qhmono;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups.Images
{
    public class DetailListImagePageVM : ViewModelBase
    {
        //public Dictionary<string, ContentInfo> DicContent;
        public ContentInfo Content;
        public string IconLike { get => _iconLike; set { SetProperty(ref _iconLike, value); } }
        List<ContentInfo> listAwait = new List<ContentInfo>();
        private string _title;

        public string Title { get => _title; set { SetProperty(ref _title, value); } }
        private long _likeCount;
        private long _commentCount;
        private string _iconLike;

        public long LikeCount { get => _likeCount; set { SetProperty(ref _likeCount, value); } }
        public long CommentCount { get => _commentCount; set { SetProperty(ref _commentCount, value); } }
        public DetailListImagePageVM()
        {
            //DicContent = new Dictionary<string, ContentInfo>();
            Content = new ContentInfo();
        }
        public override void Reset()
        {
            base.Reset();
           //DicContent = new Dictionary<string, ContentInfo>();
            Content = new ContentInfo();
        }
    }
}
