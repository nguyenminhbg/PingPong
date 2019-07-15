
using AppBongBan.ViewModels;
using Sport.News;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.Views.News
{
    class CategoryTab : ContentPage
    {
        private readonly Sport.News.EntityID _category;

        public bool IsLoad { get; set; }

        public int CurrentPage { get; set; }

        public int LimitRecord { get { return 20; } }

        private List<Sport.News.News> _lstNews = new List<Sport.News.News>();

        public CategoryTab(Sport.News.EntityID category)
        {
            _category = category;
            Title = _category.name;

            CreateUI();
            InitData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //InitData();
        }


        private void CreateUI()
        {
            var listView = new ListView
            {
                ItemsSource = this._lstNews,
                ItemTemplate = new DataTemplate(typeof(NewCell)),
                HasUnevenRows = true,
                SelectionMode = ListViewSelectionMode.Single,
                IsPullToRefreshEnabled = true
            };

            listView.ItemSelected += OnItemClick;
            listView.ItemAppearing += OnItemAppearing;
            listView.RefreshCommand = new Command(() => {
                RefreshData();
                listView.IsRefreshing = false;
            });

            this.Content = listView;
        }

        public void InitData()
        {
            if(!this.IsLoad)
            {
                Debug.WriteLine(this._category.name);

                GetData();

                this.IsLoad = true;
            }
            
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if(e != null)
            {
                var currentIdx = this._lstNews.IndexOf(e.Item as Sport.News.News);

                //Debug.WriteLine("OnItemAppearing : " + currentIdx);

                if(currentIdx >= this.GetEndRecord() && currentIdx >= (this._lstNews.Count -1))
                {
                    //Debug.WriteLine("OnItemAppearing Bottom");
                    this.CurrentPage = this.CurrentPage + 1;

                    GetData();
                }
            }
        }

        private int GetEndRecord()
        {
            var currentPage = this.CurrentPage == 0 ? 1 : this.CurrentPage;
            return currentPage * this.LimitRecord - 3;
        }

        private void OnItemClick(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            //DisplayAlert("Selected", e.SelectedItem.ToString() + " was selected.", "OK");
            var item = e.SelectedItem as Sport.News.News;
             Navigation.PushAsync(new CategoryItemDetail(item));
            ((ListView)sender).SelectedItem = null;
        }

        private void RefreshData()
        {
            Debug.WriteLine("RefreshData: " + this._category.name);

            this.CurrentPage = 1;

            this._lstNews.Clear();

             GetData();
        }

        private void GetData()
        {
            // Debug.WriteLine("CurrentPage : " + (this.CurrentPage + 1));

            Task.Run(async () => {

                var ListCategoriesNewsResponse = await PingPongNews.GetCategoryNews((this.CurrentPage + 1), this.LimitRecord, _category.slug);

                Device.BeginInvokeOnMainThread(() => {

                    if (ListCategoriesNewsResponse.success)
                    {
                        var listNews = ListCategoriesNewsResponse.data.news;

                        if (listNews.Count > 0)
                        {
                            this._lstNews.AddRange(listNews);

                            // Debug.WriteLine("listNews : " + listNews.Count);
                        }

                        var listView = this.Content as ListView;
                        listView.ItemsSource = null;
                        listView.ItemsSource = this._lstNews;

                        //Debug.WriteLine(this._category.name + " : " + _lstNews.Count);
                    }
                });

            });
            
        }
        
    }

    public class NewCell : ViewCell
    {

        public NewCell()
        {
          
            OnInit();
            
        }

        private void OnInit()
        {
            StackLayout horizontalLayout = new StackLayout();
            horizontalLayout.Padding = new Thickness(5, 5, 5, 5);
            horizontalLayout.Orientation = StackOrientation.Horizontal;

            var image = new Image();
            image.SetBinding(Image.SourceProperty, "image");
            image.VerticalOptions = LayoutOptions.Center;
            horizontalLayout.Children.Add(image);

            StackLayout verticalLayout = new StackLayout();
            verticalLayout.Orientation = StackOrientation.Vertical;

            Label lblTitle = new Label();
            //lblTitle.Style = Device.Styles.TitleStyle;
            lblTitle.VerticalOptions = LayoutOptions.StartAndExpand;
            lblTitle.SetBinding(Label.TextProperty, "title");
            verticalLayout.Children.Add(lblTitle);

            StackLayout horizontalDate = new StackLayout();
            horizontalDate.Orientation = StackOrientation.Horizontal;
            horizontalDate.VerticalOptions = LayoutOptions.CenterAndExpand;

            Label lblPublishDate = new Label();
            //lblPublishDate.Style = Device.Styles.CaptionStyle;
            lblPublishDate.SetBinding(Label.TextProperty, "updated_at", BindingMode.Default, null, "{0:hh:mm dd/MM/yyyy}");

            Label lblUser = new Label();
            //lblUser.Style = Device.Styles.CaptionStyle;
            lblUser.HorizontalOptions = LayoutOptions.CenterAndExpand;
            lblUser.SetBinding(Label.TextProperty, "user.username");

            horizontalDate.Children.Add(lblPublishDate);
            horizontalDate.Children.Add(lblUser);

            verticalLayout.Children.Add(horizontalDate);

            horizontalLayout.Children.Add(verticalLayout);

            View = horizontalLayout;

        }
    }
}
