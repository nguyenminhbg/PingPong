using Acr.UserDialogs;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.Models.News;
using DLToolkit.Forms.Controls;
using Newtonsoft.Json;
using PingPong;
using Prism.Navigation;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.News
{
    public class PersonalPostPageViewModel : ViewModelBase
    {
        private FlowObservableCollection<ImageNews> _images;
        public FlowObservableCollection<ImageNews> images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }
        private PersonalPost _personalPost;
        public PersonalPost personalPost
        {
            get { return _personalPost; }
            set { SetProperty(ref _personalPost, value); }
        }
        public Accounts account { get; set; }
        public PersonalPostPageViewModel()
        {
            images = new FlowObservableCollection<ImageNews>();
            if (Helper.Instance().CheckLogin())
            {
                personalPost = new PersonalPost()
                {
                    urlAvarta = Helper.Instance().MyAccount.Avatar_Uri,
                    fullName = Helper.Instance().AccountChat.FullName
                };
            }
        }
        public override void Reset()
        {
            images = new FlowObservableCollection<ImageNews>();
            if (Helper.Instance().CheckLogin())
            {
                personalPost = new PersonalPost()
                {
                    urlAvarta = Helper.Instance().MyAccount.Avatar_Uri,
                    fullName = Helper.Instance().AccountChat.FullName
                };
            }
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if(parameters.TryGetValue<List<ImageNews>>("ImgLibrary", out List<ImageNews> listImg))
            {
                listImg = parameters["ImgLibrary"] as List<ImageNews>;
                ShowImgExe(listImg);
            }
        }
        /// <summary>
        /// Lấy ảnh từ thư viện
        /// </summary>
        /// <param name="images"></param>
        public void ShowImgExe(List<ImageNews> images)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (images.Count > 0)
                {
                    if (!Helper.prev.Equals(images))
                    {
                        this.images.Clear();
                        this.images.AddRange(images);
                        Helper.DeleteUri(Helper.prev);
                        Helper.prev.Clear();
                        Helper.prev = images;
                    }
                }
            });
        }
        
        /// <summary>
        /// Bản tin đăng bài từ Server trả về
        /// </summary>
        /// <param name="msg"></param>
        public void UpdateContent(QHMessage msg)
        {
            long errorCode = 0;
            long contentId = 0;
            long homePageId = 0;
            long CreateDate = 0;
            msg.TryGetAt((byte)MsgHomePageAddContentAck.Error, ref errorCode);
            if (errorCode == 0)
            {
                if (msg.TryGetAt((byte)MsgHomePageAddContentAck.ContentID, ref contentId)) { }
                if (msg.TryGetAt((byte)MsgHomePageAddContentAck.HomePageID, ref homePageId)) { }
                var contentInfo = new ContentInfo();
                contentInfo.Accounts = Helper.Instance().MyAccount;
                if (msg.TryGetAt((byte)MsgHomePageAddContentAck.CreateDate, ref CreateDate))
                {
                    contentInfo.Detail.Created = CreateDate;
                }
                contentInfo.Detail.Id = contentId;
                contentInfo.Detail.Content = JsonConvert.SerializeObject(json);
                // Thêm vào cached content để lần sau lấy ra dùng
                if (!Helper.Instance().ListContent.ContainsKey(contentId))
                {
                    lock (Helper.Instance().ListContent)
                    {
                        Helper.Instance().ListContent.Add(contentId, contentInfo);
                    }
                }

                else Helper.Instance().ListContent[contentId] = contentInfo;
                Device.BeginInvokeOnMainThread(() =>
                {
                    Services.Service.Instiance().NewsSiteVM.ListNews.Insert(0, Helper.Instance().ListContent[contentId]);
                });
            }
            else
            {
                Debug.WriteLine("Error code: " + msg.JSONString());
            }
        }

        ContentFormat json;
        /// <summary>
        /// Gửi Bản tin đăng bài cá nhân lên Server
        /// </summary>
        public async void SendContentPost()
        {
            // Kiểm tra xem có homepageInfo chưa?
            if (Services.Service.Instiance().NewsSiteVM.homePageInfo == null)
            {
                UserDialogs.Instance.Toast("Không xác định được trang cá nhân");
                return;
            }
            var idhome = Services.Service.Instiance().NewsSiteVM.homePageInfo.homePageId;
            bool isPost = true;
            account = Helper.Instance().MyAccount;
            //thêm nội dung cho content
            var list = new List<string>();
            // Lấy ảnh của bài đăng
            if (images.Count > 0)
            {
                isPost = await postMuiltiImage();
            }
            QHVector Image_Id = new QHVector();
            var imagesID = new List<long>();
            if (!isPost)
                for (int i = 0; i < images.Count; i++)
                {
                    list.Add(images[i].UriImage);
                    Image_Id.SetAt(i, new QHNumber(images[i].Image_Id));
                    imagesID.Add(images[i].Image_Id);
                }
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_HOMEPAGE_ADD_CONTENT_REQ);
            json = new ContentFormat { Text = personalPost.content, Images_Id = list };
            string output = JsonConvert.SerializeObject(json);
            //kiểm tra account
            if (Helper.Instance().CheckLogin())
            {
                account = Helper.Instance().MyAccount;
                msg.SetAt((byte)MsgHomePageAddContentReq.SenderID, new QHNumber(account.Number_Id));
                msg.SetAt((byte)MsgHomePageAddContentReq.HomePageID, new QHNumber(Services.Service.Instiance().NewsSiteVM.homePageInfo.homePageId));
                msg.SetAt((byte)MsgHomePageAddContentReq.Content, new QHString(output));
                msg.SetAt((byte)MsgHomePageAddContentReq.ImagesID, Image_Id);
                Services.Service.Instiance().SendMessage(msg);
                Debug.WriteLine("MSG_HOMEPAGE_ADD_CONTENT_REQ: " + msg.JSONString());
            }
        }
        /// <summary>
        /// Gán url mới cho danh sách ảnh gửi lên Server
        /// </summary>
        /// <returns></returns>
        public async Task<bool> postMuiltiImage()
        {
            for (int i = 0; i < images.Count; ++i)
            {
                int index = i;
                if (!images[index].UriTmp.Equals("") || images[index].UriTmp != null)
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
                                images[index].Progess = e * 0.1;

                            });
                        };
                        var indeximg = images[index].UriTmp;
                        var uploadimg = new UploadImage { token_id = 1, owner_id = account.Number_Id };
                        var imageNews = await client.UploadMuitliImage(WebClientW.uploadServiceBaseAddress,
                            images[index].UriTmp,
                            new UploadImage { token_id = 1, owner_id = account.Number_Id });
                        images[index].UriImage = imageNews.UriImage;
                        images[index].Image_Id = imageNews.Image_Id;
                    }
                    catch (Exception ex)
                    {
                        var currentPage = AppChat.Helpers.Helper.Instiance().GetCurrentPage();
                        await currentPage.DisplayAlert("Lỗi up ảnh", ex.Message, "Ok");
                    }
                }
            }
            bool loop = true;
            while (loop)
            {
                loop = false;
                await Task.Delay(100);
                for (int i = 0; i < images.Count; i++)
                {
                    if (!images[i].UriImage.Equals("") || images[i].UriImage != null)
                    {
                        if (!images[i].UriImage.Contains("http"))
                        {
                            loop = true;
                        }
                    }
                }
            }
            return loop;

        }
    }
}
