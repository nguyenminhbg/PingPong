using AppBongBan.Models;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Challenge
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailChallengePerPage : ContentPage
    {
        public DetailChallengePerPage()
        {
            InitializeComponent();
        }
        DetailChallengePerPageViewModel model;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = BindingContext as DetailChallengePerPageViewModel;
        }
        private void Content_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        /// <summary>
        /// hủy thách đấu cá nhân
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var parent = btn.BindingContext as ChallengeInfo;
            if (model != null && parent != null)
            {
                ChallengeAction.AcceptOrCancelChall(parent.ChallengeID, 0);
                if (model.IsPerson)
                {
                    model.OnBackDelete();
                }
                else
                {
                    model.OnBackDeleteClub();
                }

            }
        }
        /// <summary>
        /// chấp nhận thách đấu cá nhân
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            var btn = sender as Button;
            //var parent = btn.BindingContext as ChallengeInfo;
            if (model != null)
            {
                ChallengeAction.AcceptOrCancelChall(model.Chall.ChallengeID, 1);
                if (model.IsPerson)
                {
                    model.OnBackAccept();
                }
                else
                {
                    model.OnBackAcceptClub();
                }
                
            }
        }
    }
}