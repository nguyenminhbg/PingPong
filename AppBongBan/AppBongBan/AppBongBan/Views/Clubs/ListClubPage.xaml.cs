using AppBongBan.Models;
using AppBongBan.Views.IView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Clubs
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListClubPage : ContentPage
	{
		public ListClubPage ()
		{
			InitializeComponent ();
            Init();
        }
        public void Init()
        {
            ListClub.ItemTemplate = new DataTemplate(() => {
                return new ViewCell
                {
                    View = new ClubItemView()
                };
            });
            ListClub.ItemsSource = new List<Group>
                {
                    new Group{Avatar="club.png", Title="Câu lạc bộ A",
                        AddressClub ="Số 12 ngõ 151/77 Nguyễn Đức Cảnh Phường Tương Mai " +
                        "\nQuận Hoàng Mai " +
                        "\nThành Phố Hà Nội",
                        imagePerson="account.png",
                        numbPe="12",
                        imageCheck="my_location.png",
                        numbCh="12",
                        imageLocate="location.png",
                        numbLoc="300m",
                        imageChall="pingpong_invisible.png",
                        imageFriend="group_add.png",

                    },
                    new Group{Avatar="club.png", Title="Câu lạc bộ B",
                        AddressClub ="Số 12 ngõ 151/77 Nguyễn Đức Cảnh Phường Tương Mai " +
                        "\nQuận Hoàng Mai " +
                        "\nThành Phố Hà Nội",
                        imagePerson="account.png",
                        numbPe="12",
                        imageCheck="my_location.png",
                        numbCh="12",
                        imageLocate="location.png",
                        numbLoc="500m",
                        imageChall="pingpong.png",
                        imageFriend="member_group.png",


                    },
                     new Group{Avatar="club.png", Title="Câu lạc bộ D",
                        AddressClub ="Số 12 ngõ 151/77 Nguyễn Đức Cảnh Phường Tương Mai " +
                        "\nQuận Hoàng Mai " +
                        "\nThành Phố Hà Nội",
                        imagePerson="account.png",
                        numbPe="12",
                        imageCheck="my_location.png",
                        numbCh="12",
                        imageLocate="location.png",
                        numbLoc="700m",
                        imageChall="pingpong.png",
                        imageFriend="group_add.png",


                    },
                      new Group{Avatar="club.png", Title="Câu lạc bộ V",
                        AddressClub ="Số 12 ngõ 151/77 Nguyễn Đức Cảnh Phường Tương Mai " +
                        "\nQuận Hoàng Mai " +
                        "\nThành Phố Hà Nội",
                        imagePerson="account.png",
                        numbPe="12",
                        imageCheck="my_location.png",
                        numbCh="12",
                        imageLocate="location.png",
                        numbLoc="800m",
                        imageChall="pingpong_invisible.png",
                        imageFriend="member_group.png",


                    },
                       new Group{Avatar="club.png", Title="Câu lạc bộ T",
                        AddressClub ="Số 12 ngõ 151/77 Nguyễn Đức Cảnh Phường Tương Mai " +
                        "\nQuận Hoàng Mai " +
                        "\nThành Phố Hà Nội",
                        imagePerson="account.png",
                        numbPe="12",
                        imageCheck="my_location.png",
                        numbCh="12",
                        imageLocate="location.png",
                        numbLoc="1000m",
                        imageChall="pingpong.png",
                        imageFriend="group_add.png",


                    }

                };
        }
        private void ListClub_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}