using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using Plugin.DeviceInfo;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Login
{
    public class RegisterPageViewModel : ViewModelBase
    {
        public Page Page { get; set; }
        public RegisterPageViewModel()
        {

        }
        public void RegisterProcess(QHMessage msg)
        {
            Chat.RegisterErrorCode ErrorCode = (Chat.RegisterErrorCode)(msg.GetAt((byte)Chat.MsgRegisterAckArg.ErrorCode) as QHNumber).value;
            switch (ErrorCode)
            {
                case Chat.RegisterErrorCode.SUCCESS:
                    {
                        UserDialogs.Instance.Toast("Bạn đã đăng ký thành công tài khoản");
                        // Nếu đăng ký thành công thì gửi bản tin Login lên Server luôn
                        if (!PhoneNumber.Equals("") && !PassWord.Equals(""))
                        {
                            QHMessage msgLogin = new QHMessage((ushort)Chat.ChatMessage.MSG_LOGIN_REQ);
                            msgLogin.SetAt((byte)Chat.MsgLoginReqArg.PhoneNumber, new QHString(PhoneNumber));
                            msgLogin.SetAt((byte)Chat.MsgLoginReqArg.MD5Password, new QHString(MD5.MD5.GetMd5String(PassWord)));
                            msgLogin.SetAt((byte)Chat.MsgLoginReqArg.DeviceID, CrossDeviceInfo.Current.Id);
                            if (!Services.Service.Instiance().SendMessage(msgLogin))
                            {
                                NotifiDialog.Initiance().DialogErrorInternter();
                            }
                        }
                        else
                        {
                            NotifiDialog.Initiance().DialogErrorNumber();
                        }  
                        break;
                    }
                case Chat.RegisterErrorCode.ERR_PHONE_IN_USED:
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            NotifiDialog.Initiance().DialogExistPhoneNumber();
                        });
                        break;
                    }
            }
        }
        /// <summary>
        /// Thực hiện đăng kí tài khoản lên server
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <param name="PassWord"></param>
        /// <param name="FullName"></param>
        public void Register(string PhoneNumber, string FullName, string PassWord)
        {
            this.PhoneNumber = PhoneNumber;
            this.PassWord = PassWord;
            this.FullName = FullName;
            QHMessage msg = new QHMessage((ushort)Chat.ChatMessage.MSG_REGISTER_REQ);
            msg.SetAt((byte)Chat.MsgRegisterReqArg.PhoneNumber, new QHString(PhoneNumber));
            msg.SetAt((byte)Chat.MsgRegisterReqArg.MD5Password, new QHString(MD5.MD5.GetMd5String(PassWord)));
            msg.SetAt((byte)Chat.MsgRegisterReqArg.FullName, FullName);
            Services.Service.Instiance().SendMessage(msg);
        }
        public string PhoneNumber = "";
        public string PassWord = "";
        public string FullName = "";

        public void NextAreaCodePage()
        {
            //NavigationService.NavigateAsync("AreaCodePage");
        }
        public ICommand AreaCodeCommand { set; get; }
        public ICommand ContinueCommand { set; get; }
        private string _contryName;
        public string CountryName
        {
            get { return _contryName; }
            set { SetProperty(ref _contryName, value); }
        }
    }
}
