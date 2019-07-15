using AppBongBan.ViewModels.Login;
using AppBongBan.Helpers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppBongBan.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
          //  ContryName.Text = "VN";
            EntryPhone.Keyboard = Keyboard.Telephone;
            //EntryPhone.BackgroundColor = Color.FromRgb(242, 242, 242);
            //EntryFullName.BackgroundColor = Color.FromRgb(242, 242, 242);
            //EntryPass.BackgroundColor = Color.FromRgb(242, 242, 242);
            //EntryRePass.BackgroundColor = Color.FromRgb(242, 242, 242);
            BindingContext = Services.Service.Instiance().RegisViewModel;
        }
        RegisterPageViewModel model;
        bool checkbool = true;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model = BindingContext as RegisterPageViewModel;
            checkbool = true;
           Check();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            checkbool = false;
        }
        public void Check()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {
                if (EntryPhone.Text != null && EntryPass.Text != null && EntryRePass.Text != null && EntryFullName.Text != null)
                {
                    if (EntryPhone.Text == "" || EntryPass.Text == "" || EntryRePass.Text == "" || EntryFullName.Text == "" || txtPhone.IsVisible == true
                    || txtName.IsVisible == true || txtPass.IsVisible == true || txtRePass.IsVisible == true)
                    {
                        txtContinue.TextColor = Color.Default;
                        txtContinue.BackgroundColor = Color.FromRgb(230, 230, 230);
                        txtContinue.IsEnabled = false;
                    }
                    else
                    {
                        txtContinue.IsEnabled = true;
                        txtContinue.BackgroundColor = Color.FromHex("#009dfe");
                        txtContinue.TextColor = Color.White;
                    }
                }
                else
                {
                    txtContinue.TextColor = Color.Default;
                    txtContinue.BackgroundColor = Color.FromRgb(230, 230, 230);
                    txtContinue.IsEnabled = false;
                }
                return checkbool;
            });
        }
        /// <summary>
        /// kiểm tra số điện thoại
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryPhone_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (EntryPhone.Text == null || EntryPhone.Text == "")
            {
                txtPhone.IsVisible = false;
            }
            else
            {
                if (EntryPhone.Text.Length > 16 || EntryPhone.Text.Length < 10)
                {
                    txtPhone.IsVisible = true;
                    return;
                }
                string[] newPhone = null;
                newPhone = EntryPhone.Text.Split(new char[] { ' ', '*', '#', '(', '/', ')', ',', '.', ';', '-' });
                if (newPhone.Length > 1)
                {
                    txtPhone.IsVisible = true;
                    return;
                }
                txtPhone.IsVisible = false;
            }
        }
        /// <summary>
        /// Kiểm tra string trong Full Name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryFullName_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (EntryFullName.Text == null || EntryFullName.Text == "" || string.IsNullOrWhiteSpace(EntryFullName.Text))
            {
                txtName.IsVisible = false;
            }
            else
            {
                bool checkSpecChar = false;
                if (Helper.Instance().CheckSpecChar(EntryFullName.Text))
                    checkSpecChar = true;
                if (String.IsNullOrEmpty(EntryFullName.Text) || EntryFullName.Text?.Length > 50 || checkSpecChar)
                    txtName.IsVisible = true;
                else txtName.IsVisible = false;
            }
        }
        /// <summary>
        /// Kiểm tra string trong EntryPass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryPass_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (EntryPass.Text == null || EntryPass.Text == "")
            {
                txtPass.IsVisible = false;
            }
            else
            {
                if (EntryPass.Text.Length < 8 || EntryPass.Text.Length > 20)
                {
                    txtPass.IsVisible = true;
                    return;
                }
                bool checkSpecChar = false;
                if (Helper.Instance().CheckSpecChar(EntryPass.Text))
                    checkSpecChar = true;
                if (checkSpecChar)
                {
                    txtPass.IsVisible = true;
                    return;
                }
                txtPass.IsVisible = false;
            }
            EntryRePass_PropertyChanged(sender,e);
        }
        /// <summary>
        /// Kiểm tra string trong EntryRePass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryRePass_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (EntryRePass.Text == null || EntryRePass.Text == "")
            {
                txtRePass.IsVisible = false;
            }
            else
            {
                if (EntryPass.Text != null & EntryRePass.Text != null)
                {
                    if (EntryRePass.Text.Equals(EntryPass.Text))
                    {
                        txtRePass.IsVisible = false;
                    }
                    else
                    {
                        txtRePass.IsVisible = true;
                    }
                }
            }
        }
        /// <summary>
        /// Register account to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapRegister(object sender, EventArgs e)
        {
            if (model != null)
            {
                model.Register(EntryPhone.Text, EntryFullName.Text, EntryPass.Text);
            }
        }

    }
}



