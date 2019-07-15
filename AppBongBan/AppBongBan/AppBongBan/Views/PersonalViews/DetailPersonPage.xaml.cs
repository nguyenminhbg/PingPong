using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Personal;
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
    public partial class DetailPersonPage : ContentPage
    {
        public DetailPersonPage()
        {
            InitializeComponent();
        }
        DetailPersonPageViewModel model;
        protected override void OnAppearing()
        {
            AppChat.Helpers.Helper.Instiance().TurnOffGesture();
            base.OnAppearing();
            if (model == null) model = BindingContext as DetailPersonPageViewModel;
            MessagingCenter.Subscribe<App, Accounts>((App)Application.Current, "NotifiContact", (sender, arg) =>
            {
                if (model != null)
                {
                    if (model.Acc.Number_Id == arg.Number_Id)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            btnStatue.Text = "Đợi chấp nhận";
                            string avartaStatus = Helper.Instance().IsFriendImg(model.Acc.Number_Id);
                            if (!Helper.Instance().addFriendStatus.ContainsKey(model.Acc.Number_Id))
                            {
                                IconAddFriend iconAddFriend = new IconAddFriend() { id = model.Acc.Number_Id, statusIcon = avartaStatus };
                                Helper.Instance().addFriendStatus.Add(model.Acc.Number_Id, iconAddFriend);
                            }
                            else
                            {
                                Helper.Instance().addFriendStatus[model.Acc.Number_Id].statusIcon = avartaStatus;
                            }
                            UserDialogs.Instance.Toast("Gửi kết bạn thành công, vui lòng chờ phản hồi");
                            //  NotifiDialog.Initiance().DialogAwaitFriend();

                        });
                    }
                }
            });
            MessagingCenter.Subscribe<App, long>((App)Application.Current, "NotifiAccept", (send, arg) =>
            {
                if (model != null)
                {
                    if (model.Acc.Number_Id == arg)
                    {
                        //string avartaStatus = Helper.Instance().IsFriendImg(model.Acc.Number_Id);
                        //if (!Helper.Instance().addFriendStatus.ContainsKey(model.Acc.Number_Id))
                        //    Helper.Instance().addFriendStatus.Add(model.Acc.Number_Id, avartaStatus);
                        //else Helper.Instance().addFriendStatus[model.Acc.Number_Id] = avartaStatus;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            btnChat.Text = "Tin nhắn";
                            btnStatue.Text = "Bạn bè";
                            NotifiDialog.Initiance().DialogtrustFriend();
                        });
                    }
                }
            });
            MessagingCenter.Subscribe<App, long>((App)Application.Current, "NotifiDelete", (send, arg) =>
            {
                if (model != null)
                {
                    if (model.Acc.Number_Id == arg)
                    {
                        //string avartaStatus = Helper.Instance().IsFriendImg(model.Acc.Number_Id);
                        //if (!Helper.Instance().addFriendStatus.ContainsKey(model.Acc.Number_Id))
                        //    Helper.Instance().addFriendStatus.Add(model.Acc.Number_Id, avartaStatus);
                        //else Helper.Instance().addFriendStatus[model.Acc.Number_Id] = avartaStatus;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            btnChat.Text = "Tin nhắn";
                            btnStatue.Text = "Gửi Kết bạn";
                            NotifiDialog.Initiance().DialogDeleteFriend();
                        });

                    }
                }
            });
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<App, long>((App)Application.Current, "NotifiContact");
            MessagingCenter.Unsubscribe<App, long>((App)Application.Current, "NotifiAccept");
            MessagingCenter.Unsubscribe<App, long>((App)Application.Current, "NotifiDelete");
        }
        /// <summary>
        /// chức năng xem thêm thông tin thi đấu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            NotifiDialog.Initiance().DialogDevelop();
        }
        /// <summary>
        /// Kết Bạn 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (Helpers.Helper.Instance().IsFriend(model.NumberId).Equals("Bạn bè"))
            {
                NotifiDialog.Initiance().DialogtrustFriend();
            }
            else if (Helpers.Helper.Instance().IsFriend(model.NumberId).Equals("Đợi chấp nhận"))
            {
                NotifiDialog.Initiance().DialogAwaitAcceptFriend();
            }
            else if (Helpers.Helper.Instance().IsFriend(model.NumberId).Equals("Chấp nhận"))
            {
                if (model != null)
                {
                    AppChat.Services.Service.Instiance().requestAddFriendViewModel.AcceptAction(Helper.Instance().ConvertAccsToAcc(model.Acc));
                }

            }
            else
            {
                Helper.Instance().listAccFriendDetail.Add(model.Acc);
                AddFriendAction.AddFriend((uint)Helpers.Helper.Instance().MyAccount.Number_Id, (uint)model.NumberId);
            }
        }
        /// <summary>
        /// chức năng chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.Text.Equals("Hủy"))
            {
                AppChat.Services.Service.Instiance().requestAddFriendViewModel.CancelAction(Helper.Instance().ConvertAccsToAcc(model.Acc));
                return;
            }
            if (Helpers.Helper.Instance().IsFriend(model.NumberId).Equals("Bạn bè"))
            {
                AddFriendAction.NavigationChat(this, Helper.Instance().ConvertAccsToAcc(model.Acc));
            }
            if (Helper.Instance().IsFriend(model.NumberId).Equals("Đợi chấp nhận"))
            {
                UserDialogs.Instance.Toast("Gửi yêu cầu thành công, đang chờ phản hồi");
            }
            else
            {
                AppBongBan.Helpers.NotifiDialog.Initiance().DialogAwaitFriend();
            }
        }
    }
}