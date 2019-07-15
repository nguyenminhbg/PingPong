using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.PingPongs;
using AppBongBan.Services;
using DLToolkit.Forms.Controls;
using Newtonsoft.Json;
using PingPong;
using Prism.Commands;
using Prism.Navigation;
using qhmono;
using Xamarin.Forms;
using static AppBongBan.Services.Service;

namespace AppBongBan.ViewModels.Groups
{
    public class AddNewsVM : ViewModelBase
    {
        private Club _newClub;
        private FlowObservableCollection<ImageNews> _images;
        public Club NewClub
        {
            get => _newClub; set { SetProperty(ref _newClub, value); }
        }
        public Accounts account { get; set; }
        public FlowObservableCollection<ImageNews> Images
        {
            get => _images; set { SetProperty(ref _images, value); }
        }
        public INavigationService navigationService { get; set; }
        ContentInfo contentPost = new ContentInfo();
        public AddNewsVM()
        {
            Images = new FlowObservableCollection<ImageNews>();
        }
        public void OnMsgContentAddAck(QHMessage msg)
        {
            var error = (msg.GetAt((byte)MsgContentAddAckArg.Error) as QHNumber).value;
            switch (error)
            {
                case 0:
                    //Debug.WriteLine("trả về danh sách bản tin " + msg.JSONString());
                    long created = 0;
                    long id = 0;
                    long clubId = 0;
                    if (msg.TryGetAt((byte)MsgContentAddAckArg.Created, ref created))
                    {
                        contentPost.Detail.Created = created;
                        var timelong = AppChat.Helpers.Helper.Instiance().ConvertToUnixTime(DateTime.UtcNow.ToLocalTime());
                    }
                    if (msg.TryGetAt((byte)MsgContentAddAckArg.ContentID, ref id))
                    {
                        contentPost.Detail.Id = id;
                    }
                    if(msg.TryGetAt((byte)MsgContentAddAckArg.ClubID,ref clubId))
                    {
                        contentPost.ClubID = clubId;
                    }
                    Device.BeginInvokeOnMainThread(async () =>
                   {
                       if (navigationService != null)
                       {
                           var param = new NavigationParameters();
                           param.Add("Content", contentPost);
                           await navigationService.GoBackAsync(param);
                       }
                   });
                
                    break;
                case 1:
                    NotifiDialog.Initiance().DialogErrAddConent();
                    break;

            }
        }
        /// <summary>
        /// đăng bài đăng lên server
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public async void PostContent(string title, string content)
        {
            bool isPost = true;
            if (Images.Count > 0)
            {
                isPost = await postMuiltiImage();
            }
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_CONTENT_ADD_REQ);
            
            //kiểm tra account
            if (Helper.Instance().CheckLogin())
            {
                account = Helper.Instance().MyAccount;
                msg.SetAt((byte)MsgContentAddReqArg.NumberID, new QHNumber(account.Number_Id));
                contentPost.Detail.Owner = account.Number_Id;
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorLogin();
                return;
            }
            //kiểm tra id câu lạc bộ
            if (NewClub != null)
            {
                msg.SetAt((byte)MsgContentAddReqArg.ClubID, new QHNumber(NewClub.ClubID));
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
                return;
            }
            //kiểm tra title của bài đăng
            //if (title != null && !title.Equals(""))
            //{
            if (title == null) title = "";
                msg.SetAt((byte)MsgContentAddReqArg.Title, new QHString(title));
                contentPost.Detail.Title = title;
            //}
            //else
            //{
            //    NotifiDialog.Initiance().DialogErrorNotTitle();
            //    return;
            //}
            //thêm nội dung cho content
            var list = new List<string>();
            //Thêm ảnh vào trong content
            QHVector Image_Id = new QHVector();
            var imagesID = new List<long>();
            if (!isPost)
                for (int i = 0; i < Images.Count; i++)
                {
                    list.Add(Images[i].UriImage);
                    Image_Id.SetAt(i, new QHNumber(Images[i].Image_Id));
                    imagesID.Add(Images[i].Image_Id);
                }
            //Chuyển content sang json
            if (content == null || content.Equals(""))
            {
                NotifiDialog.Initiance().DialogNotContent();
                return;
            }
            var json = new ContentFormat { Text = content, Images_Id = list };
            string output = JsonConvert.SerializeObject(json);
            //Debug.WriteLine("mysContet: " + output);
            //Debug.WriteLine("msg: " +msg.JSONString());
            msg.SetAt((byte)MsgContentAddReqArg.Content, new QHString(output));
            if (Image_Id.Length > 0)
            {
                msg.SetAt((byte)MsgContentAddReqArg.ImagesID, Image_Id);
            }
            contentPost.Accounts = account;
            contentPost.Detail.Content = output;
            contentPost.Detail.ImagesID = imagesID;
            //contentPost.Detail.Numbers = NewClub.Numbers;
            //contentPost.Detail.AdminID = NewClub.AdminID;
            //NewClub.ImageCount += Image_Id.Length;

            //Debug.WriteLine("gửi " + msg.JSONString());
            Services.Service.Instiance().SendMessage(msg);
        }
        private async Task<bool> postMuiltiImage()
        {
            for (int i = 0; i < Images.Count; i++)
            {
                int index = i;
                if (!Images[index].UriTmp.Equals("") || Images[index].UriTmp != null)
                {
                    try
                    {
                        var client = new WebClientW();
                        client.UCEvent = (o) =>
                        {

                        };
                        client.UPCEvent = (o, e) =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Images[index].Progess = e * 0.1;
                                Debug.WriteLine("Hoàn thành: " + e);
                            });
                        };
                        var imageNews = await client.UploadMuitliImage(WebClientW.uploadServiceBaseAddress,
                            Images[index].UriTmp,
                            new UploadImage { token_id = 1, owner_id = account.Number_Id });
                        Images[index].UriImage = imageNews.UriImage;
                        Images[index].Image_Id = imageNews.Image_Id;
                        Debug.WriteLine("Id của ảnh: " + Images[index].Image_Id);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            bool loop = true;
            while (loop)
            {
                loop = false;
                await Task.Delay(100);
                for (int i = 0; i < Images.Count; i++)
                {
                    if (!Images[i].UriImage.Equals("") || Images[i].UriImage != null)
                    {
                        if (!Images[i].UriImage.Contains("http"))
                        {
                            loop = true;
                        }
                    }
                }
            }
            return loop;
        }
        public void ShowImgExe(List<ImageNews> images)
        {
            if (images.Count > 0)
            {
                if (!Helper.prev.Equals(images))
                {
                    Images.Clear();
                    Images.AddRange(images);
                    Helper.DeleteUri(Helper.prev);
                    Helper.prev.Clear();
                    Helper.prev = images;
                }
            }
        }
        public async void AddImageExecute()
        {
            var action = await Helper.Instance().CurrentPage.DisplayActionSheet("Chọn ảnh đăng", "Cancel", null, "Chụp ảnh", "Thư viện ảnh");
            if (action != null)
            {

                if (action.Contains("Chụp ảnh"))
                {
                    IsPopup = true;
                    List<ImageNews> list = await Helper.TakePhoto();
                    if (list == null) return;
                    if (list.Count == 1)
                    {
                        MessagingCenter.Send<App, List<ImageNews>>((App)Xamarin.Forms.Application.Current, "ImagesSelected", list);
                    }
                    return;
                }
                if (action.Contains("Thư viện ảnh"))
                {
                    IsPopup = true;
                    await DependencyService.Get<IMediaService>().OpenGallery(true);
                }
            }
        }
        public override void Reset()
        {
            base.Reset();
            contentPost = new ContentInfo();
            account = Helper.Instance().MyAccount;
            Images = new FlowObservableCollection<ImageNews>();
        }
        public void DeleteImg(ImageNews img)
        {
            Images.Remove(img);
            Helper.prev.Remove(img);
            Helper.DeleteUri(new List<ImageNews> { img });
        }
    }
}
