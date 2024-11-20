using System;
using Newtonsoft.Json.Linq;

namespace TA.APIClient.ResponseData{
    [Serializable]
    public class BaseAPIResponse{
        public string status;
        public string message;
        public bool IsSuccess => status == "success";
    }

    [Serializable]
    public class APIResponse<T> : BaseAPIResponse {
        public T data;
    }

    //=====================
    // RESPONSE CLASSES
    //=====================

    [Serializable]
    public class LoginResponse : APIResponse<LoginSessionData>{}

    [Serializable]
    public class UserBalanceResponse : APIResponse<UserBalanceData>{}

    [Serializable]
    public class GameSessionResponse : APIResponse<GameSessionData>{}

    [Serializable]
    public class ScoreUpdateResponse : APIResponse<ScoreUpdateData>{}

    [Serializable]
    public class UserDataResponse : APIResponse<UserData>{}

    [Serializable]
    public class MasterLeaderboardResponse : APIResponse<LeaderBoard[]>{}

    [Serializable]
    public class LeaderBoardResponse : APIResponse<LeaderBoardEntry[]>{}

    [Serializable]
    public class UserLeaderBoardStatsResponse : APIResponse<LeaderBoardEntry> {};

    [Serializable]
    public class InitiatePaymentResponse : APIResponse<InitiatePaymentData> {};

    [Serializable]
    public class JSONStringResponse : APIResponse<string> {};

    [Serializable]
    public class ProgressResponse : APIResponse<ProgressData> {};

    [Serializable]
    public class FailedResponse{
        public string message;
        public string error;
    }

    //=====================
    // DATA CLASSES
    //=====================

    [Serializable]
    public class UserData{
        public string username;
        public string walletAddress;
        public string createdAt;
        public string email;
        public AppSettings[] appSettings;
    }

    [Serializable]
    public class GameSessionData {
        public string token;
    }

    [Serializable]
    public class LoginSessionData : UserData{
        public string token;
        public bool isFirstTimeUser;
    }

    [Serializable]
    public class ScoreUpdateData{
        public GameResult result;
    }

    [Serializable]
    public class GameResult{
        public UserLeaderboardData userLeaderboard;
    }

    [Serializable]
    public class UserBalanceData {
        public int credits;
        public int gameCoin;
        public int freeCredits;
        public int boughtCredits;
        public float tokens;
        public float xpBalance;
        public float native;
    }

    [Serializable]
    public class UserLeaderboardData{
        public LeaderBoardEntry high;
        public LeaderBoardEntry total;
    }

    [Serializable]
    public class LeaderBoardEntry{
        public int score;
        public int rank;
        public float reward;
        public UserData user;
    }

    [Serializable]
    public class AppSettings{
        public string id;
        public bool isMusic;
        public bool isSound;
        public bool isVibrate;
    }

    [Serializable]
    public class CRUDDBData{
        public string id;
        public string createdAt;
        public string updatedAt;
        public bool isActive;
        public bool isDeleted;
    }

    [Serializable]
    public class LeaderBoard : CRUDDBData{
        public string startTime;
        public string endTime;
        public MinimumPrizePoolData minimumPrizePoolData;
    } 

    [Serializable]
    public class MinimumPrizePoolData{
        public float prizePool;
    }

    [Serializable]
    public class InitiatePaymentData{
        public string uuid;
    }

    [Serializable]
    public class ProgressData : CRUDDBData{
        public string userLevel;
        public object progress;
    }
}
