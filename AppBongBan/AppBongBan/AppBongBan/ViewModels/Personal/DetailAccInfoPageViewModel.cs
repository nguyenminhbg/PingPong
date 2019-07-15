using AppBongBan.Helpers;
using AppBongBan.Models.Db.Content;
using Prism.Navigation;
using System.Threading.Tasks;

namespace AppBongBan.ViewModels.Personal
{
    public class DetailAccInfoPageViewModel : BaseViewModel
    {
        private Accounts _acc;
        private string _birthDay;
        private bool _isChange;
        private string _textChange;

        public bool IsChange { get => _isChange; set { SetProperty(ref _isChange, value); } }
        public string TextChange { get => _textChange; set { SetProperty(ref _textChange, value); } }
        public string BirthDay { get => _birthDay; set { SetProperty(ref _birthDay, value); } }
        public Accounts Acc { get => _acc; set { SetProperty(ref _acc, value); } }
        public DetailAccInfoPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        public long NumberId;
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Account"))
            {
                NumberId = int.Parse(parameters["Account"].ToString());
                if (NumberId == 0)
                {
                  
                    Acc = Helper.Instance().MyAccount;
                    if (AppChat.Helpers.Helper.Instiance().accountCached.ContainsKey(Acc.Number_Id))
                    {
                        Acc.fullname = AppChat.Helpers.Helper.Instiance().accountCached[Acc.Number_Id].FullName;
                        Acc.Avatar_Uri = AppChat.Helpers.Helper.Instiance().accountCached[Acc.Number_Id].AvartaURI;
                    }

                        BirthDay = Helper.ConvertToDateTime(Acc.Birthday);
                    IsChange = true;
                    TextChange = "Chỉnh sửa thông tin";
                }
                else if (NumberId > 0)
                {
                    if (!Helper.Instance().ListAccounts.ContainsKey(NumberId.ToString()))
                    {
                        Accounts acc = new Accounts() { Number_Id = NumberId };
                        Helper.Instance().ListAccounts.Add(NumberId.ToString(), acc);
                    }
                    Acc = Helper.Instance().ListAccounts[NumberId.ToString()];
                    if (string.IsNullOrEmpty(Acc.Birthday.ToString()))
                    {
                      Helper.Instance().CheckExistAccountAsync(Acc.Number_Id);
                    }
                    BirthDay = Helper.ConvertToDateTime(Acc.Birthday);
                    IsChange = false;
                    TextChange = "Nhắn tin";
                }
            }
        }

        public void UpdateInfo()
        {
            if (Helpers.Helper.Instance().CheckLogin())
            {
                Acc = Helpers.Helper.Instance().MyAccount;
                BirthDay = Helpers.Helper.ConvertToDateTime(Acc.Birthday);
            }
        }
    }
}
