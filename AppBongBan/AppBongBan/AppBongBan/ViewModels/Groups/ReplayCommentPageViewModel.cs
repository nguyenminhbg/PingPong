using AppBongBan.Helpers;
using AppBongBan.Models.Db.Content;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups
{
    public class ReplayCommentPageViewModel : BaseViewModel
    {
        private ReplayCommentVM _replyComment;
        public ReplayCommentVM ReplyComment { get => _replyComment; set { SetProperty(ref _replyComment, value); } }
        private bool _isLoading;
        public bool IsLoading { get => _isLoading; set { SetProperty(ref _isLoading, value); } }
        public ReplayCommentPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().ReplyModel.Reset();
            ReplyComment = Services.Service.Instiance().ReplyModel;
            IsLoading = true;
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Comment"))
            {
                var commentID = long.Parse(parameters["Comment"].ToString());
                Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
                {
                    IsLoading = false;
                    Helper.IdNews = commentID;
                    ReplyComment.SendGetComment(commentID);
                    return false;
                });
            }
        }
        public async void NavigationDetailImage(ReplyComments reply)
        {
            if (Navigation != null)
            {
                var param = new NavigationParameters();
                param.Add("RelyCmd", reply.Id);
                await Navigation.NavigateAsync("DetailImageCmdPage", param);
            }
        }
    }
}
