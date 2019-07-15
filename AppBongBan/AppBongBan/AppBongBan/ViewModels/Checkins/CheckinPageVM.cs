using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
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

namespace AppBongBan.ViewModels.Checkins
{
    public class CheckinPageVM : ViewModelBase
    {
        private Club _myClub;
        private Accounts account;
        public INavigationService Navigation;
        public Club MyClub { get => _myClub; set { SetProperty(ref _myClub, value); } }
        public FlowObservableCollection<ImageNews> Images { get; set; }
        public CheckinInfo _checkinInfo { get; set; }
        public CheckinPageVM()
        {
            Images = new FlowObservableCollection<ImageNews>();
            account = Helper.Instance().MyAccount;
        }
        public override void Reset()
        {
            MyClub = new Club();
            Images = new FlowObservableCollection<ImageNews>();
            account = Helper.Instance().MyAccount;
            _checkinInfo = new CheckinInfo();
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
        /// <summary>
        /// gửi tin checkin lên server
        /// </summary>
        /// <param name="content"></param>
        public async void SendCheckin(string content)
        {
            _checkinInfo = new CheckinInfo();
            bool isPost = true;
            if (Images.Count > 0)
            {
                isPost = await postMuiltiImage();
            }
            QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_CHECK_IN_REQ);
            //kiểm tra account
            if (account != null)
            {
                msg.SetAt((byte)MsgCheckInReqArg.NumberID, new QHNumber(account.Number_Id));
                _checkinInfo.NumberID = account.Number_Id;
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorLogin();
                return;
            }
            //kiểm tra id câu lạc bộ
            if (MyClub != null)
            {
                msg.SetAt((byte)MsgCheckInReqArg.ClubID, new QHNumber(MyClub.ClubID));
                _checkinInfo.ClubID = MyClub.ClubID;
            }
            else
            {
                NotifiDialog.Initiance().DialogErrorNotClub();
                return;
            }
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
            //Chuyển content sang json
            if (content == null || content.Equals(""))
            {
                //NotifiDialog.Initiance().DialogNotContent();
                //return;
                content = "";
            }
            var json = new ContentFormat { Text = content, Images_Id = list };
            string output = JsonConvert.SerializeObject(json);
            Debug.WriteLine("mysContet: " + output);
            //Debug.WriteLine("msg: " +msg.JSONString());
            msg.SetAt((byte)MsgCheckInReqArg.Content, new QHString(output));
            _checkinInfo.Time = Helper.ConvertToUnixTime(DateTime.Now);
            msg.SetAt((byte)MsgCheckInReqArg.Time, new QHNumber(_checkinInfo.Time));
            Services.Service.Instiance().SendMessage(msg);
        }
        /// <summary>
        /// Post ảnh lên server hệ thống
        /// </summary>
        /// <returns></returns>
        private async Task<bool> postMuiltiImage()
        {
            if (Helper.Instance().CheckLogin())
            {
                var acc = Helper.Instance().MyAccount;
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
                                    //Debug.WriteLine("Hoàn thành: " + e);
                                });
                            };
                            var imageNews = await client.UploadMuitliImage(WebClientW.uploadServiceBaseAddress,
                                Images[index].UriTmp,
                                new UploadImage { token_id = 1, owner_id = acc.Number_Id });
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
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Nhận ack thêm checkin trong ứng dụng
        /// </summary>
        /// <param name="msg"></param>
        public void OnReciveMsg(QHMessage msg)
        {
            long Error = 0;
            long CheckInID = 0;
            if (msg.TryGetAt((byte)MsgCheckInAckArg.Error, ref Error))
            {
                if (Error == 0)
                {
                    if (msg.TryGetAt((byte)MsgCheckInAckArg.CheckInID, ref CheckInID))
                    {
                        _checkinInfo.CheckInID = CheckInID;
                        if (Helper.Instance().ListCheckInfo.ContainsKey(CheckInID))
                        {
                            Helper.Instance().ListCheckInfo.Add(CheckInID, _checkinInfo);

                        }
                        else
                        {
                            Helper.Instance().ListCheckInfo[CheckInID] = _checkinInfo;
                        }
                        if (Helper.Instance().ListClub.ContainsKey(_checkinInfo.ClubID))
                        {
                            Helper.Instance().ListClub[_checkinInfo.ClubID].CheckInCount += 1;
                        }
                    }
                }
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Navigation != null)
                    Navigation.GoBackAsync();
                NotifiDialog.Initiance().DialogCheckInSucc();
            });
        }
        /// <summary>
        /// Nhân in trong ứng dụng nếu có checkin
        /// </summary>
        /// <param name="msg"></param>
        public void OnReciveMsgInd(QHMessage msg)
        {
            _checkinInfo = new CheckinInfo();
            long NumberID = 0;
            long ClubID = 0;
            string Content = "";
            long Time = 0;
            long CheckInID = 0;
            if (msg.TryGetAt((byte)MsgCheckInIndArg.NumberID, ref NumberID))
            {
                _checkinInfo.NumberID = NumberID;
            }
            if (msg.TryGetAt((byte)MsgCheckInIndArg.ClubID, ref ClubID))
            {
                _checkinInfo.ClubID = ClubID;
            }
            if (msg.TryGetAt((byte)MsgCheckInIndArg.Content, ref Content))
            {
                _checkinInfo.Content = Content;
            }
            if (msg.TryGetAt((byte)MsgCheckInIndArg.Time, ref Time))
            {
                _checkinInfo.Time = Time;
            }
            if (msg.TryGetAt((byte)MsgCheckInIndArg.CheckInID, ref CheckInID))
            {
                _checkinInfo.CheckInID = CheckInID;
            }
            if (Helper.Instance().ListCheckInfo.ContainsKey(CheckInID))
            {
                Helper.Instance().ListCheckInfo[CheckInID] = _checkinInfo;
            }
            else
            {
                Helper.Instance().ListCheckInfo.Add(CheckInID, _checkinInfo);
            }
            if (Helper.Instance().ListClub.ContainsKey(_checkinInfo.ClubID))
            {
                Helper.Instance().ListClub[ClubID].CheckInCount += 1;
            }

        }
        public void DeleteImg(ImageNews img)
        {
            Images.Remove(img);
            Helper.prev.Remove(img);
            Helper.DeleteUri(new List<ImageNews> { img });
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
    }
}
