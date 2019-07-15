using AppBongBan.Helpers;
using System.Windows.Input;
using AppBongBan.Models.Db.Content;
using Prism.Commands;
using Prism.Navigation;
namespace AppBongBan.ViewModels.Personal
{
    public class DetailPersonPageViewModel : BaseViewModel
    {
        private Accounts _acc;

        public ICommand MoreCmd { get; set; }
        public Accounts Acc { get => _acc; set { SetProperty(ref _acc, value); } }
        public bool IsAddFriend { get => _isAddFriend; set { SetProperty(ref _isAddFriend, value); } }
        public bool IsChat { get => _isChat; set { SetProperty(ref _isChat, value); } }
        public string TextStatusFriend { get => _textStatusFriend; set { SetProperty(ref _textStatusFriend, value); } }
        public string TextAcceptFriend { get => _textAcceptFriend; set { SetProperty(ref _textAcceptFriend, value); } }
        public string Avatar { get; set; }
        public DetailPersonPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            MoreCmd = new DelegateCommand(NavigMoreAcc);
        }
        private async void NavigMoreAcc()
        {
            var param = new NavigationParameters();
            param.Add("Account", NumberId);
            await Navigation.NavigateAsync("SetUpAccPage", param);
        }
        public int NumberId = -1;
        private bool _isAddFriend;
        private bool _isChat;
        private string _textStatusFriend;
        private string _textAcceptFriend;

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Account"))
            {
                //id của cá nhân muốn được xem chi tiết
                NumberId = int.Parse(parameters["Account"].ToString());
                if (NumberId == 0)
                {
                    if (AppChat.Helpers.Helper.Instiance().accountCached.ContainsKey(Helper.Instance().MyAccount.Number_Id))
                    {
                        Helper.Instance().MyAccount.fullname = AppChat.Helpers.Helper.Instiance().
                            accountCached[Helper.Instance().MyAccount.Number_Id].FullName;
                        Helper.Instance().MyAccount.Avatar_Uri = AppChat.Helpers.Helper.Instiance().
                           accountCached[Helper.Instance().MyAccount.Number_Id].AvartaURI;
                    }
                    Acc = Helper.Instance().MyAccount;
                    IsAddFriend = false;
                    IsChat = false;
                }
                else if (NumberId > 0)
                {
                    // Kiểm tra lại Account trên Server
                    if (!Helper.Instance().ListAccounts.ContainsKey(NumberId.ToString()))
                    {
                       var accou = new Accounts() { Number_Id = NumberId };
                        Helper.Instance().ListAccounts.Add(NumberId.ToString(), accou);
                    }
                    Acc = Helper.Instance().ListAccounts[NumberId.ToString()];
                    // Check lại thông tin Account từ Server
                    Helper.Instance().CheckExistAccountAsync(NumberId);
                    IsAddFriend = true;
                    IsChat = true;
                    TextStatusFriend = Helpers.Helper.Instance().IsFriend(NumberId);
                    TextAcceptFriend = Helpers.Helper.Instance().TextAcceptFriend;
                }
            }
        }

    }
}
