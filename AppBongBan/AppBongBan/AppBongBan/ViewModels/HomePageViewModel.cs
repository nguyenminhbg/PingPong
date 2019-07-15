using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AppBongBan.Models;
using Prism.Commands;
using Prism.Navigation;

namespace AppBongBan.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private MenuPage _selectedItem;
        private ICommand _selectCmd;
        public MenuPage SelectedItem
        {
            get => _selectedItem; set
            {
                _selectedItem = value;
                if (_selectedItem == null)
                    return;
                _selectCmd.Execute(_selectedItem);
                _selectedItem = null;
            }
        }
        public HomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _selectCmd = new DelegateCommand<MenuPage>(_selectExe);
        }
        private void _selectExe(MenuPage item)
        {
            if(item.Title.Equals("Đăng nhập")){
                Navigation.NavigateAsync("LoginPage");
            }
        }
    }
}
