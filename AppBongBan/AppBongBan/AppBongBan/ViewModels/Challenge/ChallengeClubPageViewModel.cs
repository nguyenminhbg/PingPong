using Acr.UserDialogs;
using AppBongBan.Dependency;
using AppBongBan.Helpers;
using AppBongBan.Models;
using AppBongBan.Models.PingPongs;
using AppBongBan.ViewModels.Actions;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AppBongBan.ViewModels.Challenge
{
    public class ChallengeClubPageViewModel: BaseViewModel
    {
        private Club _clubTarget;
        private string _startTime;
        private string _endTime;
        private string _endDate;
        private string _startDate;

        public Club ClubTarget { get => _clubTarget; set { SetProperty(ref _clubTarget, value); } }
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

        public ChallengeClubPageViewModel(INavigationService navigationService) : base(navigationService)
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
        /// Gửi thách đấu club
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
            //thực hiện chuẩn bị bản tin gửi thách đấu
            if (!content.Equals(""))
            {
                if (Helper.Instance().CheckLogin())
                {
                    var acc = Helper.Instance().MyAccount;
                    ChallengeAction.ChallengeClub(acc.Number_Id, ClubTarget.ClubID, content, Challenge.DateStartTime, Challenge.DateEndTime);
                }
            }
            else
            {
                UserDialogs.Instance.Toast("Hãy thêm nội dung thách đấu");
            }
        }
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("Club"))
            {
                var NumberID = long.Parse(parameters["Club"].ToString());
                if (NumberID > 0)
                    ClubTarget = Helpers.Helper.Instance().ListClub[NumberID];
            }
        }
    }
}
