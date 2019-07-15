using AppBongBan.Helpers;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Personal;
using AppChat.Views.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.PersonalViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailAccInfoPage : ContentPage
    {
        public DetailAccInfoPage()
        {
            InitializeComponent();
        }
        DetailAccInfoPageViewModel model;
        bool isFirst = true;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (model == null)
                model = BindingContext as DetailAccInfoPageViewModel;
            if (!isFirst)
                model.UpdateInfo();
            else
                isFirst = false;

        }
        /// <summary>
        /// chức năng chỉnh sửa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void Button_Clicked(object sender, EventArgs e)
        {
            if (model != null)
                if (model.IsChange)
                     Navigation.PushAsync(new MyProfilePage() { Title = "Chỉnh sửa thông tin" });
                else
                {
                    if (Helpers.Helper.Instance().IsFriend(model.NumberId).Equals("Bạn bè"))
                    {
                        AddFriendAction.NavigationChat(this, Helper.Instance().ConvertAccsToAcc(model.Acc));
                    }
                    else
                    {
                        AppBongBan.Helpers.NotifiDialog.Initiance().DialogAwaitFriend();
                    }
                }
        }
    }
}