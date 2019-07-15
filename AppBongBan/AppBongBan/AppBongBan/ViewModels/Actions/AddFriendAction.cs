using AppChat.Models.Login;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Actions
{
    public class AddFriendAction
    {
        public static void AddFriend(uint myId, uint targetId)
        {
            AppChat.Services.Service.Instiance().contactsPageViewModel.ContactReq(myId, targetId);
        }

        public static  void NavigationChat(Page page, Account TargetAcc)
        {
            if (AppChat.Helpers.Helper.Instiance().contacts.ContainsKey("ContactPage"))
            {
                AppChat.Helpers.Helper.Instiance().contacts["ContactPage"]= TargetAcc;
            }
           else AppChat.Helpers.Helper.Instiance().contacts.Add("ContactPage", TargetAcc);
             page.Navigation.PushAsync(new AppChat.Views.Chat.ListviewChatText());
        }
    }
}
