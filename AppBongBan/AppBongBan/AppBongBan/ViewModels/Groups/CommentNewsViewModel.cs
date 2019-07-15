using AppBongBan.Models;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class CommentNewsViewModel : BaseViewModel
    {
        private CommentNewsVM _commentNews;
        private bool _isLoading;
        public bool IsLoading { get => _isLoading; set { SetProperty(ref _isLoading, value); } }
        public CommentNewsVM CommentNews { get => _commentNews; set { SetProperty(ref _commentNews, value); } }
        public CommentNewsViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().CommentModel.Reset();
            CommentNews = Services.Service.Instiance().CommentModel;
            IsLoading = true;
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Content"))  
            {
                var ContentID = long.Parse(parameters["Content"].ToString());
                IsLoading = false;
                Helpers.Helper.IdNews = ContentID;
                CommentNews.SendGetContent(ContentID);
            }
        }
        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
   
        }
        public async void NavigationDetailImage(CommentInfor comment)
        {
            if (Navigation != null)
            {
                var param = new NavigationParameters();
                param.Add("Comment", comment.Comment.Id);
                await Navigation.NavigateAsync("DetailImageCmdPage", param);
            }
        }
        public async void NavigationReplyComment(CommentInfor comment)
        {
            if (Navigation != null)
            {
                var param = new NavigationParameters();
                param.Add("Comment", comment.Comment.Id);
                await Navigation.NavigateAsync("ReplayCommentPage", param);
            }
        }
        public void NaviImage(ContentInfo content)
        {
            var param = new NavigationParameters();
            param.Add("Content", content.Detail.Id);
            Navigation.NavigateAsync("DetImagePage", param);
        }
    }
}
