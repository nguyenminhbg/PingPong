using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using DLToolkit.Forms.Controls;
using Newtonsoft.Json;
using PingPong;
using Prism.Navigation;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups.Images
{
    public class UpLoadImageVM : ViewModelBase
    {
        private bool _isShow;
        public ContentInfo contentPost;
        public INavigationService navigationService;
        public UpLoadImageVM()
        {
            Images = new FlowObservableCollection<ImageNews>();
            IsShow = true;
            contentPost = new ContentInfo();
        }
        public override void Reset()
        {
            base.Reset();
            IsShow = true;
            Images = new FlowObservableCollection<ImageNews>();
            contentPost = new ContentInfo();
        }
        public bool IsShow { get => _isShow; set { SetProperty(ref _isShow, value); } }
        public FlowObservableCollection<ImageNews> Images { get; set; }
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
                    if (IsShow) IsShow = false;
                }
            }
        }
        public void DeleteImg(ImageNews img)
        {

            Images.Remove(img);
            Helper.prev.Remove(img);
            Helper.DeleteUri(new List<ImageNews> { img });
            if (Images.Count == 0)
            {
                IsShow = true;
            }
        }
        public async void SendAddContent(Club NewClub, INavigationService navigationService)
        {
            this.navigationService = navigationService;
            bool isPost = true;
            if (Images.Count > 0)
            {
                isPost = await postMuiltiImage();

                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CLUB_CONTENT_ADD_REQ);

                //kiểm tra account
                var account = Helper.Instance().MyAccount;
                if (account != null)
                {
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
                string title = "Đã đăng ảnh lên";
                msg.SetAt((byte)MsgContentAddReqArg.Title, new QHString(title));
                contentPost.Detail.Title = title;
                //thêm nội dung cho content
                var list = new List<string>();
                //Thêm ảnh vào trong content
                QHVector Image_Id = new QHVector();
                if (!isPost)
                    for (int i = 0; i < Images.Count; i++)
                    {
                        list.Add(Images[i].UriImage);
                        Image_Id.SetAt(i, new QHNumber(Images[i].Image_Id));
                    }

                var json = new ContentFormat { Text = "", Images_Id = list };
                string output = JsonConvert.SerializeObject(json);
                msg.SetAt((byte)MsgContentAddReqArg.Content, new QHString(output));
                if (Image_Id.Length > 0)
                {
                    msg.SetAt((byte)MsgContentAddReqArg.ImagesID, Image_Id);
                }
                account.Avatar_Uri = "account.png";
                contentPost.Accounts = account;
                contentPost.Detail.Content = output;
                contentPost.Detail.Numbers = NewClub.Numbers;
                contentPost.Detail.AdminID = NewClub.AdminID;
                NewClub.ImageCount += Image_Id.Length;
                Services.Service.Instiance().SendMessage(msg);
            }
            else
            {
                NotifiDialog.Initiance().DialogAddImg();
            }
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
                        var account = Helper.Instance().MyAccount;
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
                    }
                    if (msg.TryGetAt((byte)MsgContentAddAckArg.ContentID, ref id))
                    {
                        contentPost.Detail.Id = id;
                    }
                    if (msg.TryGetAt((byte)MsgContentAddAckArg.ClubID, ref clubId))
                        contentPost.ClubID = clubId;
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

    }
}
