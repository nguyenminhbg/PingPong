using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using AppBongBan.Models;
using AppBongBan.Services;
using FFImageLoading.Forms;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace AppBongBan.ViewModels.Groups.Images
{
    public class DetImage : BindableBase
    {
        private string _index;
        private string _img;
       
        public DetImage()
        {
            Index = "";
            Img = "";
        }
        public string Index { get => _index; set { SetProperty(ref _index, value); } }
        public string Img { get => _img; set { SetProperty(ref _img, value); } }
    }
    public class DetImagePageViewModel : BaseViewModel
    {
        private int _positionImg=1;
        public int positionImg
        {
            get { return _positionImg; }
            set
            {
                SetProperty(ref _positionImg, value);
            }
        }
        private ContentInfo _content;
        private string _title;
        private ObservableCollection<View> _myItemsSource;
        private ObservableCollection<DetImage> _itemSource;

        public ObservableCollection<DetImage> ItemSource { get => _itemSource; set { SetProperty(ref _itemSource, value); } }
        public DetImagePageViewModel(INavigationService navigationService) : base(navigationService)
        {

            if (Device.RuntimePlatform == Device.iOS)
            {
                ItemSource = new ObservableCollection<DetImage>();
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                MyItemsSource = new ObservableCollection<View>();
            }
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            if (parameters.ContainsKey("Content"))
            {
                var content = parameters["Content"] as ContentInfo;
                //   var uri = parameters["img"] as string;
                // if (content == null || string.IsNullOrEmpty(uri)) return;
                //  var ContentID = long.Parse(parameters["Content"].ToString());
                //  Helpers.Helper.Instance().obj = this;
                // Helpers.Helper.Instance().GetContentInfoFromId(ContentID);

                //foreach (var item in content.Detail.DetailContent.Images_Id)
                //{
                //    MyItemsSource.Add(new CachedImage()
                //    {
                //        Source = item,
                //         Aspect = Aspect.AspectFit,
                //    });
                //}
                //  positionImg = content.Detail.DetailContent.Images_Id.IndexOf(uri)+1;
                AddContentFromHelp(content);
            }
        }
        public void AddContentFromHelp(ContentInfo contetnInfo)
        {
            Content = contetnInfo;
            if (Device.RuntimePlatform == Device.iOS)
            {
                for (int index = 0; index < Content.Detail.DetailContent.Images_Id.Count; index++)
                {
                    ItemSource.Add(new DetImage()
                    {
                        Index = (index + 1) + "/" + Content.Detail.DetailContent.Images_Id.Count + " - " +
                        Content.Detail.DateImageCreated,
                        Img = Content.Detail.DetailContent.Images_Id[index]
                    });

                }

            }
            else
            {
                foreach (var item in Content.Detail.DetailContent.Images_Id)
                {
                    MyItemsSource.Add(new CachedImage()
                    {
                        Source = item,
                        Aspect = Aspect.AspectFit,
                    });
                }
            }
        }
        public ContentInfo Content { get => _content; set { SetProperty(ref _content, value); } }
        public ObservableCollection<View> MyItemsSource { get => _myItemsSource; set { SetProperty(ref _myItemsSource, value); } }
        public string Title { get => _title; set { SetProperty(ref _title, value); } }

        /// <summary>
        /// thực hiện hiển thị title theo từng ảnh
        /// </summary>
        public void SelectedExe(int index)
        {
            if (Content != null)
            {
                Title = index + "/" + Content.Detail.DetailContent.Images_Id.Count + " - " +
                    Content.Detail.DateImageCreated;
            }
        }
        public async void BackContent()
        {
            await Navigation.GoBackAsync();
        }


    }
}
