using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models.Db.Content;
using AppBongBan.Views;
using AppBongBan.Views.Login;
using Plugin.Connectivity;
using Plugin.DeviceInfo;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Login
{
    public class LoginPageViewModel : ViewModelBase
    {
        public Page Page { get; set; }
        public ICommand RegisterNaviCmd { get; set; }
        public ICommand LoginCommand { get; set; }
        public LoginPageViewModel()
        {
        }
        /// <summary>
        /// thực hiện login vào hệ thống
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <param name="PassWord"></param>
        public void loginExe(string PhoneNumber, string PassWord)
        {
            QHMessage msg = new QHMessage((ushort)Chat.ChatMessage.MSG_LOGIN_REQ);
            msg.SetAt((byte)Chat.MsgLoginReqArg.PhoneNumber, new QHString(PhoneNumber));
            msg.SetAt((byte)Chat.MsgLoginReqArg.MD5Password, new QHString(MD5.MD5.GetMd5String(PassWord)));
            msg.SetAt((byte)Chat.MsgLoginReqArg.DeviceID, CrossDeviceInfo.Current.Id);
            if (!Services.Service.Instiance().SendMessage(msg))
            {
                NotifiDialog.Initiance().DialogErrorInternter();
            }
        }
        public void navigation()
        {
            if (Page != null)
            {
                Page.Navigation.PushAsync(new RegisterPage());
            }
        }

        /// <summary>
        /// Xử lý bản tin login từ hệ thóng trả vế
        /// </summary>
        /// <param name="msg"></param>
        public void LoginAckProcess(QHMessage msg)
        {
            //try
            //{
            Chat.LoginErrorCode login_error = (Chat.LoginErrorCode)(msg.GetAt((byte)Chat.MsgLoginAckArg.ErrorCode) as QHNumber).value;
            switch (login_error)
            {
                case Chat.LoginErrorCode.SUCCESS:

                    if (!Helper.Instance().CheckLogin())
                    {
                        Helper.Instance().MyAccount = new Accounts();
                    }
                    long sessionId = 0;
                    string phone = "";
                    string fullname = "";
                    string email = "";
                    long numberId = 0;
                    string avarta = "";
                    string address = "";
                    long birthday = 0;
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.SessionID, ref sessionId))
                    {
                        App.SessionID = sessionId;
                    }
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.PhoneNumber, ref phone))
                    {
                        Helper.Instance().MyAccount.Phone = phone;
                    }
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.FullName, ref fullname))
                    {
                        Helper.Instance().MyAccount.fullname = fullname;
                    }
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.Email, ref email))
                    {
                        Helper.Instance().MyAccount.Email = email;
                    }
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.NumberID, ref numberId))
                    {
                        Helper.Instance().MyAccount.Number_Id = (uint)numberId;
                    }
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.AvatarURI, ref avarta))
                    {
                        Helper.Instance().MyAccount.Avatar_Uri = avarta;
                    }
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.Address, ref address))
                    {
                        Helper.Instance().MyAccount.Address = address;
                    }
                    if (msg.TryGetAt((byte)Chat.MsgLoginAckArg.BirthDay, ref birthday))
                    {
                        Helper.Instance().MyAccount.Birthday = birthday;
                    }
                    //Kiểm tra filename
                    Helper.Instance().ConvertPingPongToChat();
                    Helper.Instance().MyAccount.Last_Time_Sync_Contact = 0;
                    //  Services.Service.Instiance().Namefile = Helper.Instance().MyAccount.Number_Id.ToString();
                    AppChat.Helpers.Helper.Instiance().nameDataBase = Helper.Instance().AccountChat.NumberId.ToString();
                    //   AppChat.Helpers.Helper.Instiance().database.InsertOrReAccount(account);
                    AppChat.Helpers.Helper.Instiance().database.InsertOrReAccount(Helper.Instance().AccountChat);
                    // Thay layout ảnh đại diện
                    HomePage.avartaChange();
                    if (AppChat.Helpers.Helper.Instiance().database != null)
                    {
                        if (Helper.Instance().MyAccount.Avatar_Uri.Equals("") || Helper.Instance().MyAccount.Avatar_Uri == null)
                        {
                            Helper.Instance().MyAccount.Avatar_Uri = "account.png";
                        }
                        if (Page != null)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                Xamarin.Forms.DependencyService.Get<AppChat.Dependency.ILocalCache>().SetData(CrossDeviceInfo.Current.Id, Helper.Instance().AccountChat);
                                //   MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "Login");
                                Helper.Instance().GetContactGroupChat = true;
                                MessagingCenter.Send<App>((App)Xamarin.Forms.Application.Current, "Login1");
                                await Page.Navigation.PopToRootAsync();
                            });
                        }
                    }
                    Debug.WriteLine("LoginJson: " + msg.JSONString());
                    break;
                case Chat.LoginErrorCode.ERR_PHONE_NOT_EXIST:
                    {
                        // Login in fale thì xoa file
                        Xamarin.Forms.DependencyService.Get<ILocalCache>().RemoveData<Accounts>(CrossDeviceInfo.Current.Id);
                        if (Page != null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Page.DisplayAlert("Thông báo", "Số điện thoại không tồn tại", "Ok");
                            });
                        }
                        break;
                    }

                case Chat.LoginErrorCode.ERR_PASSWORD_MISMATCH:
                    {
                        Xamarin.Forms.DependencyService.Get<ILocalCache>().RemoveData<Accounts>(CrossDeviceInfo.Current.Id);
                        if (Page != null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Page.DisplayAlert("Thông báo", "Mật khẩu không đúng", "Ok");
                            });
                        }
                        break;
                    }
                case Chat.LoginErrorCode.ERR_USERNAME_NOT_EXIST:
                    {
                        Xamarin.Forms.DependencyService.Get<ILocalCache>().RemoveData<Accounts>(CrossDeviceInfo.Current.Id);
                        if (Page != null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Page.DisplayAlert("Thông báo", "Tài khoản đăng nhập không đúng", "Ok");
                            });
                        }
                        break;
                    }
            }
            //}
            //catch (Exception ex)
            //{
            //    if (Page != null)
            //    {
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            Page.DisplayAlert("Lỗi Login", ex.Message, "Ok");
            //        });
            //    }
            //}
        }
    }
}
