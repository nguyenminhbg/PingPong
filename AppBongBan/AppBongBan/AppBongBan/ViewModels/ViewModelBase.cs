using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware
    {
        public Page page;
        private bool _isMore;
        private bool _isLoading;

        public bool IsFrist { get; set; }
        public bool IsPopup { get; set; }
        public bool IsMore { get => _isMore; set { SetProperty(ref _isMore, value); } }
        public bool IsLoading { get => _isLoading; set { SetProperty(ref _isLoading, value); } }
        public ViewModelBase()
        {
            IsFrist = false;
            IsPopup = false;
        }
        public virtual void Reset()
        {

        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
         
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
         
        }
    }
}
