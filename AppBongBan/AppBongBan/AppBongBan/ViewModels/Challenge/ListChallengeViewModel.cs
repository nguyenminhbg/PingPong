using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace AppBongBan.ViewModels.Challenge
{
    public class ListChallengeViewModel : BaseViewModel
    {
        private ListChallengeVM _listChallengeVM;

        public ListChallengeVM listChallengeVM { get => _listChallengeVM; set { SetProperty(ref _listChallengeVM, value); } }
        public ListChallengeViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
    }
}
