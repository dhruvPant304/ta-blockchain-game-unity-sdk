using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.Components;
using TA.Menus;
using TA.Services;
using TA.UserProfile;
using LeaderBoardEntry = TA.APIClient.ResponseData.LeaderBoardEntry;

namespace TA.Leaderboard {
    public class LeaderboardService : Service<LeaderboardService>
    {
        protected override void OnInitialize() {}

        APIService _apiService;
        APIConfigProviderService _apiConfigProvider;
        BlockchainGameCanvas _gameCanvas;
        TAMenuService _menuService;

        Dictionary<string, LeaderBoard> _totalScoreLeaderBoards;
        Dictionary<string, LeaderBoard> _highScoreLeaderBoards;

        public void Start(){
            _apiService = ServiceLocator.Instance.GetService<APIService>();
            _apiConfigProvider = ServiceLocator.Instance.GetService<APIConfigProviderService>();
            _gameCanvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
            _menuService = ServiceLocator.Instance.GetService<TAMenuService>();
        }

        public async UniTask FetchMaster(){

            _totalScoreLeaderBoards = new();
            _highScoreLeaderBoards = new();

            var response = await _apiService.SendFetchMasterLeaderboardRequest(_apiConfigProvider.APIConfig.gameId);
            if(response.IsSuccess){
                foreach(var leaderboard in response.SuccessResponse.data){
                    var totalBoard = new LeaderBoard(){
                        id = leaderboard.id,
                        type = "total",
                        isActive = leaderboard.isActive,
                        startDate = leaderboard.startTime,
                        endDate = leaderboard.endTime
                    };

                    _totalScoreLeaderBoards[totalBoard.DisplayName()] = totalBoard;

                    var highBoard = new LeaderBoard(){
                        id = leaderboard.id,
                        type = "high",
                        isActive = leaderboard.isActive,
                        startDate = leaderboard.startTime,
                        endDate = leaderboard.endTime
                    };

                    _highScoreLeaderBoards[highBoard.DisplayName()] = highBoard;
                }
            }
            else{
                _gameCanvas.ShowMessagePopup(GetFetchErrorPopUp(response.FailureResponse.message));
                throw new System.Exception("Failed to get master leader baord");
            }
        }

        public List<string> GetAllLeaderBoardNames(){
            return _totalScoreLeaderBoards.Values.Select((board) => board.DisplayName()).ToList();
        }

        public string GetActiveName(){
            return _totalScoreLeaderBoards.Values.First((board) => board.isActive)?.DisplayName();
        }

        public LeaderBoard GetHighScoreLeaderBoard(string displayName){
            if(!_highScoreLeaderBoards.ContainsKey(displayName)){
                throw new Exception($"High score leaderboard with display name: \"{displayName}\" not found");
            }

            return _highScoreLeaderBoards[displayName];
        }

        public LeaderBoard GetTotalScoreLeaderBoard(string displayName){
            if(!_totalScoreLeaderBoards.ContainsKey(displayName)){
                throw new Exception($"Total score leaderboard with display name: \"{displayName}\" not found");
            }

            return _totalScoreLeaderBoards[displayName];
        }

        MessagePopup GetFetchErrorPopUp(string message){
            return new MessagePopup{
                header = "Failed to fetch leaderboard",
                message = message,
                banner = BannerType.Danger,
                exits = new List<MessagePopupExit>(){
                    new MessagePopupExit{
                        exitStyle = MessagePopupExit.ExitStyle.Regular,
                        name = "okay",
                        exitAction = _menuService.CloseAll
                    }
                }
            };
        }
    }

    public class LeaderBoard{
        APIService _apiService;
        APIConfigProviderService _apiConfigProvide;
        UserProfileService _userProfileService;

        public string id;
        public string type;
        public string startDate;
        public string endDate;
        public bool isActive;
        public LeaderBoardEntry userEntry = null;
        public Dictionary<int, List<LeaderBoardEntry>> pages = new();

        public LeaderBoard(){
            _apiService = ServiceLocator.Instance.GetService<APIService>();
            _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>(); 
            _apiConfigProvide = ServiceLocator.Instance.GetService<APIConfigProviderService>();
        }

        public string DisplayName(){
            DateTime startDateTime = DateTime.Parse(startDate, null, DateTimeStyles.RoundtripKind);
            DateTime endDateTime = DateTime.Parse(endDate, null, DateTimeStyles.RoundtripKind);

            string startDateFormatted = startDateTime.ToString("dd MMMM", CultureInfo.InvariantCulture);
            string endDateFormatted = endDateTime.ToString("dd MMMM", CultureInfo.InvariantCulture);

            string displayName = $"{startDateFormatted} - {endDateFormatted}";
            return displayName;
        }

        public async UniTask<LeaderBoardEntry> GetUserStats(){
            if(userEntry != null) return userEntry;
            var response = await _apiService.SendFetchUserLeaderBoardStatsRequest(
                    _apiConfigProvide.APIConfig.gameId, 
                    id, 
                    type,
                    _userProfileService.LoginToken);
            if(response.IsSuccess){
                userEntry = response.SuccessResponse.data;
            }

            return userEntry;
        }

        public async UniTask<List<LeaderBoardEntry>> GetPage(int page){
            if(pages.ContainsKey(page)) return pages[page];
            await LoadPage(page);
            return pages[page];
        }

        public async UniTask LoadPage(int page){
            var response = await _apiService.SendFetchLeaderBoardResponse(
                    _apiConfigProvide.APIConfig.gameId,
                    id,
                    type,
                    _apiConfigProvide.APIConfig.entriesPerPage,
                    page
                );

            if(response.IsSuccess){
                pages[page] = response.SuccessResponse.data.ToList();
            }
            else{
                throw new System.Exception($"Failed to get page: \"{page}\" for leaderboardID: \"{id}\" ");
            }
        }
    }
}
