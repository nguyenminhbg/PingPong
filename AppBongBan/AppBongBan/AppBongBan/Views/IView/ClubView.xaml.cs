using AppBongBan.Custom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppBongBan.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.IView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClubView : ContentView
    {
        //public event EventHandler TapCommunWar;
        //public event EventHandler TapDistrict;
        //public event EventHandler TapCity;
        public event EventHandler TapAdd;
        public bool clubstreet;
        public ClubView()
        {
            InitializeComponent();
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) => TapAdd?.DynamicInvoke(sender, e);
            ImgAdd.GestureRecognizers.Add(gestureRecognizer);
            avarta.GestureRecognizers.Add(gestureRecognizer);
            fillName.Completed += (s, e) =>
            {
                if(clubstreet)
                fillStreet.Focus();
            };
            fillPhoneNumber.Completed += (s, e) =>
            {
                scroll.ScrollToAsync(0, scroll.Height, true);
            };
            MyEditor.TextColor = Color.Gray;
            MyEditor.Text = "Điền mô tả chung cho club tối đa 1000 kí tự";
            MyEditor.Focused += MyEditor_Focused;
            MyEditor.Unfocused += MyEditor_Unfocused;
        }
       
        private void MyEditor_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(MyEditor.Text))
            {
                MyEditor.Text = "Điền mô tả chung cho club tối đa 1000 kí tự";
                MyEditor.TextColor = Color.Gray;
            }
        }

        private void MyEditor_Focused(object sender, FocusEventArgs e)
        {
            if (!MyEditor.Focus())
            {
                Debug.WriteLine("keyboard chưa hiển thị lên");
            }
            else
            {
                Debug.WriteLine("keyboard hiển thị lên");
            }
        }

        /// <summary>
        /// Kiểm tra số lượng kí tự
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fillName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as ExtendedEditorControl;
            if (entry.Text.Count() != 0)
            {
                if (entry.Text.Count() > 150)
                {
                    lbNotifi.Text = "Số lượng kí tự vượt quá cho phép ";
                }
                else
                {
                    lbNotifi.Text = "";
                }
                lbName.Text = entry.Text.Count() + "/150";
                if (Helper.Instance().CheckSpecChar(entry.Text))
                {
                    lbNotifi.Text += "\nTên không chứa các kí tự đặc biệt *,#,@,$,#,%";
                }
            }
            else
            {
                lbName.Text = "0/150";
                lbNotifi.Text = "";
            }
        }

        private void fillStreet_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as ExtendedEditorControl;
            if (entry.Text.Count() != 0)
            {

                if (entry.Text.Count() > 500)
                {
                    lbNotifiAddress.Text = "Số lượng kí tự vượt quá cho phép ";
                    return;
                }
                else
                {
                    lbNotifiAddress.Text = "";
                }
                lbAddress.Text = entry.Text.Count() + "/500";
                if (Helper.Instance().CheckSpecChar(entry.Text))
                {
                    lbNotifiAddress.Text += "\nĐịa chỉ không chứa các kí tự đặc biệt *,#,@,$,#,%";
                    return;
                }
                else
                {
                    lbNotifiAddress.Text += "";
                }

            }
            else
            {
                lbAddress.Text = "0/500";
                lbNotifiAddress.Text = "";
            }
        }
        public bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }
    }
}