using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using Newtonsoft.Json;
using PingPong;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups.Images
{
    public class UpLoadImgPageViewModel : BaseViewModel
    {
        private UpLoadImageVM _uploadVM;

        public Club NewClub;
        public UpLoadImageVM UploadVM { get => _uploadVM; set { SetProperty(ref _uploadVM, value); } }
        public UpLoadImgPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Services.Service.Instiance().UpLoadVM.Reset();
            UploadVM = Services.Service.Instiance().UpLoadVM;
            NewClub = new Club();
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                var ClubID = long.Parse(parameters["Club"].ToString());
                if(!Helper.Instance().ListClub.TryGetValue(ClubID, out Club club))
                {
                    club = new Club() { ClubID = ClubID };
                    Helper.Instance().ListClub.Add(ClubID, club);
                }
                NewClub = Helper.Instance().ListClub[ClubID];
                Helper.Instance().CheckExistClubAsync(ClubID);
            }
        }
        public async void ChooseImgDevice()
        {
            await DependencyService.Get<IMediaService>().OpenGallery(true);
        }
        public async void TakePhoto()
        {
            List<ImageNews> list = await Helper.TakePhoto();
            if (list == null) return;
            if (list.Count == 1)
            {
                MessagingCenter.Send<App, List<ImageNews>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", list);
            }
        }
        public void SendImg()
        {
            if (NewClub.ClubID > 0)
            {
                UploadVM.SendAddContent(NewClub, Navigation);
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
            }

        }
    }
}
