using System;
using System.Diagnostics;
using System.Windows.Input;
using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.Db.Content;
using AppBongBan.ViewModels.Actions;
using Prism.Commands;
using Prism.Navigation;


namespace AppBongBan.ViewModels.Challenge
{
    public class ChallengePageViewModel : BaseViewModel
    {
        private Accountlocal _accTarget;
        private string _startTime;
        private string _endTime;
        private string _endDate;
        private string _startDate;
        public Accountlocal AccTarget { get => _accTarget; set { SetProperty(ref _accTarget, value); } }
        //public ICommand SendChallCmd { get; set; }
        public ICommand SelectStartTime { get; set; }
        public ICommand SelectStartDate { get; set; }
        public ICommand SelectEndTime { get; set; }
        public ICommand SelectEndDate { get; set; }

        public string StartTime { get => _startTime; set { SetProperty(ref _startTime, value); } }
        public string EndTime { get => _endTime; set { SetProperty(ref _endTime, value); } }
        public string EndDate { get => _endDate; set { SetProperty(ref _endDate, value); } }
        public string StartDate { get => _startDate; set { SetProperty(ref _startDate, value); } }

        public ChallengeInfo Challenge { get; set; }

        public ChallengePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //SendChallCmd = new DelegateCommand(SendChall);
            StartTime = "hh:mm";
            EndTime = "hh:mm";
            EndDate = "dd/MM/yyyy";
            StartDate = "dd/MM/yyyy";
            SelectStartTime = new DelegateCommand(StartTimeExe);
            SelectEndTime = new DelegateCommand(EndTimeExe);
            SelectEndDate = new DelegateCommand(EndDateExe);
            SelectStartDate = new DelegateCommand(StartDateExe);
            Challenge = new ChallengeInfo();
        }

        private void StartTimeExe()
        {
            CrossXDateTimeNumberPicker.Current.ShowTimePicker("Choose a time", DateTime.Now, seletedTime =>
            {
                StartTime = $"{seletedTime.Hour}:{seletedTime.Minute}";
                Challenge.StartTime = seletedTime.Hour * 3600 + seletedTime.Minute * 60;
            });
        }
        private void EndTimeExe()
        {
            CrossXDateTimeNumberPicker.Current.ShowTimePicker("Choose a time", DateTime.Now, seletedTime =>
            {
                EndTime = $"{seletedTime.Hour}:{seletedTime.Minute}";
                Challenge.EndTime = seletedTime.Hour * 3600 + seletedTime.Minute * 60;
            });
        }
        private void StartDateExe()
        {
            var now = DateTime.Now;
            var minDate = DateTime.Now;
            var maxDate = DateTime.Now.AddYears(20);
            CrossXDateTimeNumberPicker.Current.ShowDatePicker(now, minDate, maxDate, "Choose a date", selectedDate =>
            {
                StartDate = $"{selectedDate:dd/MM/yyyy}";
                Challenge.StartDate = Helper.ConvertToUnixTime(selectedDate);
            });
        }
        private void EndDateExe()
        {
            var now = DateTime.Now;
            var minDate = DateTime.Now;
            var maxDate = DateTime.Now.AddYears(20);
            CrossXDateTimeNumberPicker.Current.ShowDatePicker(now, minDate, maxDate, "Choose a date", selectedDate =>
            {
                EndDate = $"{selectedDate:dd/MM/yyyy}";
                Challenge.EndDate = Helper.ConvertToUnixTime(selectedDate);
            });
        }
        /// <summary>
        /// Gửi thách đấu cá nhân
        /// </summary>
        public void SendChall(string content)
        {
            if (Challenge.StartTime == 0)
            {
                UserDialogs.Instance.Toast("Hãy thêm thời gian bắt đầu");
                return;
            }

            if (Challenge.EndTime == 0)
            {
                UserDialogs.Instance.Toast("Hãy thêm thời gian kết thúc");
                return;
            }

            if (Challenge.StartDate == 0)
            {
                UserDialogs.Instance.Toast("Hãy thêm ngày bắt đầu");
                return;
            }
            if (Challenge.EndDate == 0)
            {
                UserDialogs.Instance.Toast("Hãy thêm ngày kết thúc");
                return;
            }
            if(Challenge.EndTime <= Challenge.StartTime)
            {
                UserDialogs.Instance.Toast("Thời gian kết thúc phải sau thời gian bắt đầu");
                return;
            }
            if(Challenge.DateEndTime < Challenge.DateStartTime)
            {
                UserDialogs.Instance.Toast("Ngày kết thúc thi đấu phải lớn hơn hoặc bằng ngày bắt đầu");
                return;
            }
            //thực hiện chuẩn bị bản tin gửi thách đấu
            if (!content.Equals(""))
            {
                if (Helper.Instance().CheckLogin())
                {
                    var acc = Helper.Instance().MyAccount;
                    Debug.WriteLine("Time: " + Helpers.Helper.ConvertToHourTime(Challenge.StartTime));
                    Debug.WriteLine("Time: " + Helpers.Helper.ConvertToHourTime(Challenge.EndTime));
                    ChallengeAction.ChallengePersionReq(acc.Number_Id, AccTarget.Number_Id, content, Challenge.DateStartTime, Challenge.DateEndTime);
                }
            }
            else
            {
                UserDialogs.Instance.Toast("Hãy thêm nội dung thách đấu");
            }
        }
        public  override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Account"))
            {
                var NumberID = long.Parse(parameters["Account"].ToString());
                if (NumberID > 0)
                {
                    if (Helper.Instance().ListAcclocal.ContainsKey(NumberID))
                    {
                        AccTarget = Helper.Instance().ListAcclocal[NumberID];
                    }
                    else
                    {
                        if (!Helper.Instance().ListAccounts.ContainsKey(NumberID.ToString()))
                        {
                            var account = new Accounts() { Number_Id = NumberID};
                            Helper.Instance().ListAccounts.Add(NumberID.ToString(), account);
                        }
                        var accounOwner = Helper.Instance().ListAccounts[NumberID.ToString()];
                        if(string.IsNullOrEmpty(accounOwner.fullname) || string.IsNullOrEmpty(accounOwner.Avatar_Uri))
                        {
                         Helper.Instance().CheckExistAccountAsync(accounOwner.Number_Id);
                        }
                        if (accounOwner != null)
                        {
                            var acccount = new Accountlocal();
                            acccount.Challenge = "pingpong.png";
                            acccount.Blade = "Cốt A - Hãng A";
                            acccount.Facebat = "Mặt A - Hãng A";
                            acccount.Level = "Hạng A";
                            acccount.AccepLevel = "Đã Duyệt";
                            acccount.AddFriend = Helper.Instance().IsFriendImg(accounOwner.Number_Id);
                            acccount.TextStatusFriend = Helper.Instance().IsFriend(accounOwner.Number_Id);
                            acccount.TextAcceptFriend = Helper.Instance().TextAcceptFriend;
                            acccount.Number_Id = accounOwner.Number_Id;
                            acccount.fullname = accounOwner.fullname;
                            acccount.Avatar_Uri = accounOwner.Avatar_Uri;

                        }
                    }
                }

            }
        }

    }
}
