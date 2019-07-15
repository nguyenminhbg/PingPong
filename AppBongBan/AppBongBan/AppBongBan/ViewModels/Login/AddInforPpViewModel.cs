using AppBongBan.Models.Db;
using PingPong;
using Prism.Navigation;
using qhmono;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBongBan.ViewModels.Login
{
    public class AddInforPpViewModel:BaseViewModel
    {
        public AddInforPpViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        public void SendAddInfoPingPong(InfPpAccount Pp)
        {
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_PROFILE_INFO_REQ);
            //if (Pp != null)
            //{
            //    if(Pp)
            //}
           
        }
    }
}
