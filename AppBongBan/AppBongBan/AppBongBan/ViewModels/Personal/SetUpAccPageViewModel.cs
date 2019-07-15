using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppBongBan.Helpers;
using AppBongBan.Models.Db.Content;
using Prism.Commands;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Personal
{
    public class SetUpAccPageViewModel : BaseViewModel
    {
        private string _selectedItem;
        private ICommand NaviCmd;
        public string Title { get => _title; set { SetProperty(ref _title, value); } }
        private Accounts _account;
        public Accounts accounts
        {
            get => _account;
            set
            {
                SetProperty(ref _account, value);
            }
        }
        public ObservableCollection<string> Actions { get; set; }
        public string SelectedItem
        {
            get => _selectedItem; set
            {
                _selectedItem = value;
                if (_selectedItem == null)
                    return;
                NaviCmd.Execute(_selectedItem);
                _selectedItem = null;

            }
        }
        public SetUpAccPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Actions = new ObservableCollection<string>();
            NaviCmd = new DelegateCommand<string>(NaviInfoAcc);
        }
        private int NumberId;
        private string _title;

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Account"))
            {
                NumberId = int.Parse(parameters["Account"].ToString());
                if (NumberId == 0)
                {
                    var Acc = Helper.Instance().MyAccount;
                    if (!Helper.Instance().ListAccounts.ContainsKey(Acc.Number_Id.ToString()))
                    {
                        Helper.Instance().ListAccounts.Add(Acc.Number_Id.ToString(), Acc);
                    }
                    accounts = Acc;
                    if (AppChat.Helpers.Helper.Instiance().accountCached.ContainsKey(Acc.Number_Id))
                        Helper.Instance().ListAccounts[Acc.Number_Id.ToString()].fullname = AppChat.Helpers.Helper.Instiance().accountCached[Acc.Number_Id].FullName;
                    Actions.Add("Thông tin cá nhân");
                    Actions.Add("Cập nhật thông tin thi đấu");
                    Actions.Add("Cập nhật hình đại diện");
                    Actions.Add("Cập nhật hình nền");
                    Actions.Add("Cài đặt chung");
                }
                else if (NumberId > 0)
                {
                    if (!Helper.Instance().ListAccounts.ContainsKey(NumberId.ToString()))
                    {
                        var account = new Accounts() { Number_Id = NumberId };
                        Helper.Instance().ListAccounts.Add(NumberId.ToString(), account);
                    }
                    if (string.IsNullOrEmpty(Helper.Instance().ListAccounts[NumberId.ToString()].fullname))
                    {
                        Helper.Instance().CheckExistAccountAsync(NumberId);
                    }
                    accounts = Helper.Instance().ListAccounts[NumberId.ToString()];
                    Actions.Add("Thông tin cá nhân");

                }
            }
        }
        private void NaviInfoAcc(string title)
        {
            var index = Actions.IndexOf(title);
            switch (index)
            {
                case 0:
                    var param = new NavigationParameters();
                    param.Add("Account", NumberId);
                    Navigation.NavigateAsync("DetailAccInfoPage", param);
                    break;
                default:
                    NotifiDialog.Initiance().DialogDevelop();
                    break;
            }
        }
    }
}
