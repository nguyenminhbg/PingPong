using AppBongBan.Models;
using AppBongBan.ViewModels.Groups.ManageMember;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Groups.ManageMember
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListUserReqClubPage : ContentPage
    {
        public class AccPingPong
        {
            public string Avatar { get; set; }
            public string FullName { get; set; }
            public string Level { get; set; }
            public string AccepLevel { get; set; }
            public string Facebat { get; set; }
            public string Blade { get; set; }
            public string Challenge { get; set; }
            public string AddFriend { get; set; }
        }
        ListUserReqPageViewModel model;
        public ListUserReqClubPage()
        {
            InitializeComponent();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
            {
                model = BindingContext as ListUserReqPageViewModel;
            }

            if (model != null)
            {
                var app = Application.Current as App;
                if (app.clubCurrent != null)
                {
                    model.ListUserReq.MyClub = app.clubCurrent;
                    app.clubCurrent = null;
                    model.ListUserReq.GetAccountAccept();
                }
                //model.ListUserReq.CheckReQuest(app.ListAccInClub);
                
            }
        }
        /// <summary>
        /// thực hiện chấp nhận thêm thành viên
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemAccView_TapAccept(object sender, EventArgs e)
        {
            var btn = sender as View;
            var accJoinClub = (AccJoinClub)btn?.BindingContext;
            if (model != null)
            {
                model.ListUserReq.SendAccept(accJoinClub, 1);
            }
        }
        /// <summary>
        /// thực hiện xóa chấp nhận nhóm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemAccView_TapDelete(object sender, EventArgs e)
        {
            var btn = sender as View;
            var accJoinClub = (AccJoinClub)btn?.BindingContext;
            if (model != null)
            {
                model.ListUserReq.SendAccept(accJoinClub, 0);
            }
        }
    }
}