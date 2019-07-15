
using AppBongBan.Database;
using AppBongBan.Helpers;
using AppBongBan.ViewModels.Actions;
using AppBongBan.ViewModels.Challenge;
using AppBongBan.ViewModels.Checkins;
using AppBongBan.ViewModels.Clubs;
using AppBongBan.ViewModels.Group;
using AppBongBan.ViewModels.Groups;
using AppBongBan.ViewModels.Groups.Images;
using AppBongBan.ViewModels.Groups.ManageMember;
using AppBongBan.ViewModels.Groups.MyClubs;
using AppBongBan.ViewModels.Login;
using AppBongBan.ViewModels.News;
using AppBongBan.ViewModels.Notify;
using AppBongBan.Views.Groups;
using PingPong;
using Plugin.DeviceInfo;
using Prism.Mvvm;
using qhmono;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppBongBan.Services
{
    public enum StatueGetAccount
    {
        Content = 1,
        Comment = 2,
        Reply = 3,
        Members = 4,
        Request = 5,
        Like = 6,
        LikeComment = 7,
        SearchAcc = 8,
        DetailImage = 9,
    }
    public enum StatueLike
    {
        Content = 1,
        Comment = 2,
        Reply = 3,
    }
    public enum StatueLikeInd
    {
        Content = 1,
        Comment = 2,
        Replay = 3,
    }
    public enum StatueUpdatePosition
    {
        AddClub = 1,
        UpdateClub = 2,
        UpdatePerson = 3
    }
    public enum StatueTyping
    {
        Comment,
        ReplyComment
    }
    public enum StatuePostContent
    {
        AddContent,
        UpLoadImage,
        Checkin
    }
    public class Service : BindableBase
    {
        private static Service _service;
        /// <summary>
        /// nếu bằng false nghĩa là trạng thái lấy content. trạng thái là true nghĩa là lấy nội dung account từ danh sách account
        /// </summary>
        public StatueGetAccount statue;
        public StatueLike Like;
        public StatueLikeInd LikeInd;
        public StatueUpdatePosition Position;
        public StatueTyping stTyping;
        public StatuePostContent StPostContent;
        public string Notification { get => _notification; set { SetProperty(ref _notification, value); } }

        protected Service()
        {
            AppChat.AppInstance.Instance();
            AppChat.Services.Service.IsChat = false;
            AppChat.Services.Service.Instiance().OnGetMsg += OnReceiveMessage;
            AppChat.Services.Service.Instiance().websocket.Ping = PingServer;
            AppChat.Services.Service.Instiance().websocket.PingInterval(TimeSpan.FromSeconds(15));
            //   PingServer();

            UpdateLocationPerson();
            Notification = "notification.png";

            //AppChat.Helpers.Helper.Instiance().navigateProfilePingPong+=
        }

        /// <summary>
        /// Cơ chế ping pong lên server
        /// </summary>
        public void PingServer()
        {
            QHMessage msg = new QHMessage((ushort)Chat.ChatMessage.MSG_PING);
            if (Helper.Instance().MyAccount == null) return;
            Debug.WriteLine("id thiết bị: " + Helper.Instance().MyAccount.Number_Id);
            msg.SetAt((byte)Chat.MsgPingArg.NumberID, new QHNumber(Helper.Instance().MyAccount.Number_Id));
            msg.SetAt((byte)Chat.MsgPingArg.DeviceID, CrossDeviceInfo.Current.Id);
            SendMessage(msg);
        }
        /// <summary>
        /// Gửi bản tin cho server
        /// </summary>
        /// <param name="msg"></param>
        public bool SendMessage(QHMessage msg)
        {
            try
            {
                AppChat.Services.Service.Instiance().SendMessage(msg);
            }
            catch (Exception) { }
            return true;
        }

        /// <summary>
        /// Thực hiện nhận bản tin từ server trả về
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="msg"></param>
        public virtual void OnReceiveMessage(QHMessage msg)
        {
            Debug.WriteLine("ThreadIdOnRec: " + Thread.CurrentThread.ManagedThreadId);
            Debug.WriteLine("trả về tin từ server: " + msg.JSONString());
            //phần liên quan đến chat message
            switch ((Chat.ChatMessage)msg.MsgType)
            {
                case Chat.ChatMessage.MSG_LOGIN_ACK:
                    {
                        LoginViewModel.LoginAckProcess(msg);
                        break;
                    }
                case Chat.ChatMessage.MSG_REGISTER_ACK:
                    {
                        RegisViewModel.RegisterProcess(msg);
                        break;
                    }
                case Chat.ChatMessage.MSG_PONG:
                    {
                        Debug.WriteLine("Pong thành công");
                    }
                    break;
                case Chat.ChatMessage.MSG_PROFILE_INFO_ACK:
                    Helper.Instance().GetAccount(msg);

                    break;

            }
            //phần liên quan đến pingpong bình luận
            switch ((PingPongMsg)msg.MsgType)
            {
                case PingPongMsg.MSG_CLUB_ADD_ACK:
                    {
                        // if (Helper.Instance().CheckLogin())
                        AddClubModel.AddClubAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_INFO_UPDATE_ACK:
                    {
                        // if (Helper.Instance().CheckLogin())
                        AddClubModel.UpdateAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_SEARCH_ACK:
                    {
                        //thực hiện hiển thị danh sách club
                        if (AppChat.Helpers.Helper.Instiance().myAccount != null)
                            clubSearchVM.SearchClubAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_HOMEPAGE_ACK:
                    {
                        //hiển thị chi tiết club
                        if (AppChat.Helpers.Helper.Instiance().myAccount != null)
                            ClubModel.GetDetailClub(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_CONTENT_ADD_ACK:
                    {
                        switch (StPostContent)
                        {
                            case StatuePostContent.AddContent:
                                // if (Helper.Instance().CheckLogin())
                                AddNewModel.OnMsgContentAddAck(msg);
                                break;
                            case StatuePostContent.UpLoadImage:
                                //   if (Helper.Instance().CheckLogin())
                                UpLoadVM.OnMsgContentAddAck(msg);
                                break;
                        }

                    }
                    break;
                case PingPongMsg.MSG_CLUB_JOIN_ACK:
                    {
                        // if (Helper.Instance().CheckLogin())
                        ClubModel.OnMsgClubJoinAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_JOIN_REQ_IND:
                    {
                        // if (Helper.Instance().CheckLogin())
                        ClubModel.OnMsgClubJoinInd(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_JOIN_ACCEPT_ACK:
                    {
                        //if (Helper.Instance().CheckLogin())
                        listUserReqModel.AcceptAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_JOIN_IND:
                    {
                        //   if (Helper.Instance().CheckLogin())
                        listUserReqModel.ClubJoinAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_CLUB_CONTENT_ADD_IND:
                    {
                        //  if (Helper.Instance().CheckLogin())
                        ClubModel.OnMsgContentIn(msg);
                    }
                    break;
                case PingPongMsg.MSG_LIKE_ACK:
                    LikeAction.LikeContentAck(msg);
                    break;
                case PingPongMsg.MSG_LIKE_IND:
                    {
                        LikeAction.LikeInd(msg);
                    }
                    break;
                case PingPongMsg.MSG_UNLIKE_ACK:
                    {
                        LikeAction.UnlikeContentAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_UNLIKE_IND:
                    {
                        LikeAction.UnLikeInd(msg);
                    }
                    break;
                case PingPongMsg.MSG_COMMENT_ADD_ACK:
                    {
                        //  if (Helper.Instance().CheckLogin())
                        CommentModel.OnMsgAddCommentAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_COMMENT_ADD_IND:
                    {
                        //  if (Helper.Instance().CheckLogin())
                        CommentModel.OnMsgAddCommentInd(msg);
                    }
                    break;
                case PingPongMsg.MSG_GET_CONTENT_ACK:
                    {
                        if (AppChat.Helpers.Helper.Instiance().myAccount != null)
                            Helper.Instance().GetContentAsync(msg);
                    }
                    break;
                case PingPongMsg.MSG_GET_COMMENT_ACK:
                    if (AppChat.Helpers.Helper.Instiance().myAccount != null)
                        Helper.Instance().GetComment(msg);
                    break;
                case PingPongMsg.MSG_REPLY_ADD_ACK:
                    {
                        //  if (Helper.Instance().CheckLogin())
                        ReplyModel.OnMsgReplyAddAck(msg);
                    }
                    break;
                case PingPongMsg.MSG_REPLY_ADD_IND:
                    {
                        // if (Helper.Instance().CheckLogin())
                        ReplyModel.OnMsgReplyAddInd(msg);
                    }
                    break;
                case PingPongMsg.MSG_GET_REPLY_ACK:
                    {
                        //  if (Helper.Instance().CheckLogin())
                        Helper.Instance().GetReplyComment(msg);
                    }
                    break;
                case PingPongMsg.MSG_LOCATION_UPDATE_ACK:
                    switch (Position)
                    {
                        case StatueUpdatePosition.AddClub:
                            AddClubModel.OnMsgLocationUpdateAck(msg);
                            break;
                        case StatueUpdatePosition.UpdateClub:
                            RepairClub.OnMsgLocationUpdateAck(msg);
                            break;
                        case StatueUpdatePosition.UpdatePerson:

                            break;
                    }
                    break;
                case PingPongMsg.MSG_SCAN_LOCATION_ACK:
                    //  SearchLocationModel.OnMsgScanLocationAc(msg);
                    clubSearchVM.OnMsgScanLocationAc(msg);
                    break;
                case PingPongMsg.MSG_GET_CLUB_INFO_ACK:
                    Helper.Instance().GetClub(msg);
                    break;
                case PingPongMsg.MSG_GET_MYCLUB_ACK:
                    Helper.Instance().GetReplyMyClub(msg);
                    //myClubsVM.OnReciveMsg(msg);
                    break;
                case PingPongMsg.MSG_GET_CLUB_IMAGES_ACK:
                    ListImageVM.OnReciveMsg(msg);
                    break;
                case PingPongMsg.MSG_CHECK_IN_ACK:
                    CheckinVM.OnReciveMsg(msg);
                    break;
                case PingPongMsg.MSG_CHECK_IN_IND:
                    CheckinVM.OnReciveMsgInd(msg);
                    break;
                case PingPongMsg.MSG_GET_CLUB_CHECK_IN_LIST_ACK:
                    ListCheckinVM.OnReciveMsg(msg);
                    break;
                //typing cho comment
                //typing cho reply comment
                case PingPongMsg.MSG_USER_TYPING_IND_RELAY:
                    switch (stTyping)
                    {
                        case StatueTyping.Comment:
                            CommentModel.OnReciveTyping(msg);
                            break;
                        case StatueTyping.ReplyComment:
                            ReplyModel.OnReciveTyping(msg);
                            break;
                    }
                    break;
                case PingPongMsg.MSG_LIST_USER_POST_CONTENTS_ACK:
                    //  if (Helper.Instance().CheckLogin())
                    NewsSiteVM.OnReciveListContent(msg);
                    break;
                case PingPongMsg.MSG_INVITE_CHALLENGE_ACK:
                    ChallengeAction.OnReciveAckChall(msg);
                    break;
                case PingPongMsg.MSG_INVITE_CHALLENGE_IND:
                    NotifiChall.OnReciveChallengeInd(msg);
                    break;
                case PingPongMsg.MSG_INVITE_CHALLENGE_CNF_IND:
                    Debug.WriteLine("message: " + msg.JSONString());
                    ListChallenge.OnReciveCancelChallengeInd(msg);
                    break;
                case PingPongMsg.MSG_LIST_CHALLENGES_ACK:
                    //if (Helper.Instance().CheckLogin())
                    NotifiChall.OnReciveListChallenge(msg);
                    break;
                case PingPongMsg.MSG_HOMEPAGE_INFO_ACK:
                    // if (Helper.Instance().CheckLogin())
                    Service.Instiance().NewsSiteVM.OnRecHomePageInfo(msg);
                    break;
                case PingPongMsg.MSG_HOMEPAGE_LIST_CONTENT_ACK:
                    if (AppChat.Helpers.Helper.Instiance().myAccount != null)
                        NewsSiteVM.OnRecMsgHomePageGetContentsAck(msg);
                    break;
                case PingPongMsg.MSG_HOMEPAGE_ADD_CONTENT_ACK:
                    //  if (Helper.Instance().CheckLogin())
                    personalPostViewModelInstance.UpdateContent(msg);
                    break;
                case PingPongMsg.MSG_HOMEPAGE_ADD_CONTENT_IND:
                    // if (Helper.Instance().CheckLogin())
                    NewsSiteVM.MsgAddContentIndicator(msg);
                    break;


            }
        }
        public static Service Instiance()
        {
            if (_service == null)
            {
                _service = new Service();
            }
            return _service;
        }
        /// <summary>
        /// Update vị trí của người dùng để gửi lên Server
        /// </summary>
        public void UpdateLocationPerson()
        {
            Device.StartTimer(TimeSpan.FromSeconds(20), () =>
            {
                QHMessage msg = new QHMessage((ushort)PingPongMsg.MSG_LOCATION_UPDATE_REQ);
                if (Helper.Instance().MyAccount != null)
                {
                    var id = Helper.Instance().MyAccount.Number_Id;
                    if (id > 0)
                    {
                        msg.SetAt((byte)MsgLocationUpdateReqArg.NumberID, new QHNumber(id));
                        msg.SetAt((byte)MsgLocationUpdateReqArg.Kind, new QHNumber(0));
                        Task.Run(async () =>
                        {
                            var position = await Helper.Instance().MyPosition();
                            if (position.Latitude > 0 || position.Longitude > 0)
                            {
                                msg.SetAt((byte)MsgLocationUpdateReqArg.Latitude, new QHNumber((long)(position.Latitude * Math.Pow(10, 6))));
                                msg.SetAt((byte)MsgLocationUpdateReqArg.Longitude, new QHNumber((long)(position.Longitude * Math.Pow(10, 6))));
                                Position = StatueUpdatePosition.UpdatePerson;
                                if (!SendMessage(msg))
                                {

                                }
                                else
                                {
                                    Debug.WriteLine("LocationUpdate: " + msg.JSONString());
                                }
                            }
                        });

                    }
                }
                return true;
            });
        }
        public static long SessionID;
        public static Dictionary<string, DatabaseSQLite> dt_compare = new Dictionary<string, DatabaseSQLite>();
        private DatabaseSQLite _database;
        // Khi nao duoc goi se duoc khoi tao
        public DatabaseSQLite DatabaseInstanc(string namefile)
        {
            if (dt_compare.TryGetValue(namefile, out _database))
            {
                return _database;
            }
            else
            {
                _database = new DatabaseSQLite(namefile);
                dt_compare.Add(namefile, _database);
                return _database;
            }
        }
        private ClubSearchVM _clubSearchVM;
        public ClubSearchVM clubSearchVM
        {
            get
            {
                if (_clubSearchVM == null)
                    _clubSearchVM = new ClubSearchVM();
                return _clubSearchVM;
            }
        }
        private PersonalSearchPageVM _personalSearchPageVM;
        public PersonalSearchPageVM personalSearchPageVM
        {
            get
            {
                if (_personalSearchPageVM == null)
                {
                    _personalSearchPageVM = new PersonalSearchPageVM();
                }
                return _personalSearchPageVM;
            }
        }
        //chatmsg
        private LoginPageViewModel _loginViewModel;
        public LoginPageViewModel LoginViewModel
        {
            get
            {
                if (_loginViewModel == null)
                {
                    _loginViewModel = new LoginPageViewModel();
                }
                return _loginViewModel;

            }
        }

        private RegisterPageViewModel _regisViewModel;
        public RegisterPageViewModel RegisViewModel
        {
            get
            {
                if (_regisViewModel == null)
                {
                    _regisViewModel = new RegisterPageViewModel();
                }
                return _regisViewModel;
            }
        }

        //pingpongmsg
        private SearchLocationVM _searchLocationPageViewModel;
        /// <summary>
        /// ViewModel của lớp Search dữ liệus
        /// </summary>
        public SearchLocationVM SearchLocationModel
        {
            get
            {
                if (_searchLocationPageViewModel == null)
                {
                    _searchLocationPageViewModel = new SearchLocationVM();
                }
                return _searchLocationPageViewModel;
            }
        }
        private ClubPageVM _clubViewModel;
        public ClubPageVM ClubModel
        {
            get
            {
                if (_clubViewModel == null)
                {
                    _clubViewModel = new ClubPageVM();
                }
                return _clubViewModel;
            }
        }
        private AddNewsVM _AddNewsViewModel;
        public AddNewsVM AddNewModel
        {
            get
            {
                if (_AddNewsViewModel == null)
                {
                    _AddNewsViewModel = new AddNewsVM();
                }
                return _AddNewsViewModel;
            }
        }
        private AddClubViewModel _addClubModel;
        public AddClubViewModel AddClubModel
        {
            get
            {
                if (_addClubModel == null)
                {
                    _addClubModel = new AddClubViewModel();
                }
                return _addClubModel;
            }
        }
        private ListUserReqVM _listUserReqModel;
        public ListUserReqVM listUserReqModel
        {
            get
            {
                if (_listUserReqModel == null)
                {
                    _listUserReqModel = new ListUserReqVM();
                }
                return _listUserReqModel;
            }
        }
        private ListMemberVM _memberPage;
        public ListMemberVM MemberPage
        {
            get
            {
                if (_memberPage == null)
                {
                    _memberPage = new ListMemberVM();
                }
                return _memberPage;
            }


        }
        private CommentNewsVM _commentModel;
        public CommentNewsVM CommentModel
        {
            get
            {
                if (_commentModel == null)
                {
                    _commentModel = new CommentNewsVM();
                }
                return _commentModel;
            }
        }
        private ReplayCommentVM _replyModel;
        public ReplayCommentVM ReplyModel
        {
            get
            {
                if (_replyModel == null)
                {
                    _replyModel = new ReplayCommentVM();
                }
                return _replyModel;
            }
        }
        private RepairClubVM _repairClub;
        public RepairClubVM RepairClub
        {
            get
            {
                if (_repairClub == null)
                {
                    _repairClub = new RepairClubVM();
                }
                return _repairClub;
            }
        }
        private MyClubsVM _myClubsVM;
        public MyClubsVM myClubsVM
        {
            get
            {
                if (_myClubsVM == null)
                {
                    _myClubsVM = new MyClubsVM();
                }
                return _myClubsVM;
            }
        }
        private ListImageClubVM _listImageVM;
        public ListImageClubVM ListImageVM
        {
            get
            {
                if (_listImageVM == null)
                {
                    _listImageVM = new ListImageClubVM();
                }
                return _listImageVM;
            }
        }
        private DetailListImagePageVM _detailImgVM;
        public DetailListImagePageVM DetailImgVM
        {
            get
            {
                if (_detailImgVM == null)
                {
                    _detailImgVM = new DetailListImagePageVM();
                }
                return _detailImgVM;
            }
        }
        private UpLoadImageVM _uploadVM;
        private string _notification;

        public UpLoadImageVM UpLoadVM
        {
            get
            {
                if (_uploadVM == null)
                {
                    _uploadVM = new UpLoadImageVM();
                }
                return _uploadVM;
            }
        }

        private CheckinPageVM _checkinVM;
        public CheckinPageVM CheckinVM
        {
            get
            {
                if (_checkinVM == null)
                    _checkinVM = new CheckinPageVM();
                return _checkinVM;
            }
        }

        private ListCheckPageVM _listCheckinVM;
        public ListCheckPageVM ListCheckinVM
        {
            get
            {
                if (_listCheckinVM == null)
                    _listCheckinVM = new ListCheckPageVM();
                return _listCheckinVM;
            }
        }

        private NewsSitePageVM _newsSitePage;
        public NewsSitePageVM NewsSiteVM
        {
            get
            {
                if (_newsSitePage == null)
                    _newsSitePage = new NewsSitePageVM();
                return _newsSitePage;
            }
        }

        private ListChallengePageVM _notifichall;
        public ListChallengePageVM NotifiChall
        {
            get
            {
                if (_notifichall == null)
                    _notifichall = new ListChallengePageVM();
                return _notifichall;
            }
        }

        private ListChallengeVM listChallenge;
        public ListChallengeVM ListChallenge
        {
            get
            {
                if (listChallenge == null)
                    listChallenge = new ListChallengeVM();
                return listChallenge;
            }
        }

        private PersonalPostPageViewModel personalPostViewModelInstance;
        public PersonalPostPageViewModel PersonalPostViewModelInstance
        {
            get
            {
                if (personalPostViewModelInstance == null)
                    personalPostViewModelInstance = new PersonalPostPageViewModel();
                return personalPostViewModelInstance;
            }
        }
    }
}
