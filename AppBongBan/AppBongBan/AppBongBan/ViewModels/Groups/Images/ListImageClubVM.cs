using DLToolkit.Forms.Controls;
using PingPong;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups.Images
{
    public class ListImageClubVM : ViewModelBase
    {
        private bool _loading;
        private string _title;

        public bool Loading { get => _loading; set { SetProperty(ref _loading, value); } }
        private FlowObservableCollection<AppBongBan.Models.Db.Content.Images> listImage;
        public FlowObservableCollection<AppBongBan.Models.Db.Content.Images> ListImage
        {
            get { return listImage; }
            set { SetProperty(ref listImage, value); }
        }
        public string Title { get => _title; set { SetProperty(ref _title, value); } }
        public ListImageClubVM()
        {
            Loading = false;
            ListImage = new FlowObservableCollection<Models.Db.Content.Images>();
        }
        public override void Reset()
        {
            base.Reset();
            Loading = false;
            ListImage = new FlowObservableCollection<Models.Db.Content.Images>();
        }
        /// <summary>
        /// Trả về 1 danh sách ảnh
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<Models.Db.Content.Images> GetImage(QHMessage msg)
        {
            //Debug.WriteLine("msg " + msg.JSONString());
            var list = new List<Models.Db.Content.Images>();
            QHTable Images = new QHTable();
            if (msg.TryGetAt((byte)MsgGetClubImagesAckArg.Images, ref Images))
            {
                for (int i = 0; i < Images.GetRowCount(); i++)
                {
                    var myImage = new AppBongBan.Models.Db.Content.Images();
                    long _imageId = 0;
                    long _clubId = 0;
                    long _contentId = 0;
                    if (Images.TryGetAt(i, 2, ref _imageId))
                    {
                        myImage.Id = _imageId;
                    }
                    if (Images.TryGetAt(i, 0, ref _clubId))
                    {
                        myImage.ClubID = _clubId;
                    }
                    if (Images.TryGetAt(i, 1, ref _contentId))
                    {
                        myImage.Content_Id = _contentId;
                    }
                    list.Add(myImage);

                }
            }
            return list;
        }
        public void OnReciveMsg(QHMessage msg)
        {
            var list = GetImage(msg);
            if (list.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (ListImage.Count > 0)
                        ListImage.Clear();
                    ListImage.AddRange(list);
                });
            }
            Device.BeginInvokeOnMainThread(() => {
                Title = ListImage.Count.ToString();
            });
            Loading = false;
        }
    }
}
