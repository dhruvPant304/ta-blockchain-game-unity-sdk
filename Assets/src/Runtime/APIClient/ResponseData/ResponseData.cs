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
    public class ProgressResponse<T> : APIResponse<ProgressData<T>> where T : class{};

    [Serializable]
    public class CoinEarnedResponse : APIResponse<CoinEarnData> {};

    [Serializable]
    public class FailedResponse{
        public string message;
        public string error;
    }

    //=====================
    // DATA CLASSES
    //=====================

    public class SerializedClass{
        public T? GetFild<T>(string fieldName) where T : struct{
            var fieldInfo = this.GetType().GetField(fieldName);
            if(fieldInfo != null && fieldInfo.FieldType == typeof(T)){
                return (T)fieldInfo.GetValue(this);
            }else{
                return null;
            }
        }
    }

    [Serializable]
    public class UserData : SerializedClass {
        public string username;
        public string walletAddress;
        public string createdAt;
        public string email;
        public AppSettings[] appSettings;
    }

    [Serializable]
    public class GameSessionData : SerializedClass {
        public string token;
    }

    [Serializable]
    public class LoginSessionData : UserData{
        public string token;
        public bool isFirstTimeUser;
    }

    [Serializable]
    public class ScoreUpdateData: SerializedClass{
        public GameResult result;
    }

    [Serializable]
    public class GameResult : SerializedClass{
        public UserLeaderboardData userLeaderboard;
    }

    [Serializable]
    public class UserBalanceData : SerializedClass{
        public int credits;
        public int gameCoin;
        public int freeCredits;
        public int boughtCredits;
        public float tokens;
        public float xpBalance;
        public float native;
    }

    [Serializable]
    public class UserLeaderboardData : SerializedClass{
        public LeaderBoardEntry high;
        public LeaderBoardEntry total;
    }

    [Serializable]
    public class LeaderBoardEntry : SerializedClass{
        public int score;
        public int rank;
        public float reward;
        public UserData user;
    }

    [Serializable]
    public class AppSettings : SerializedClass{
        public string id;
        public bool isMusic;
        public bool isSound;
        public bool isVibrate;
    }

    [Serializable]
    public class CRUDDBData : SerializedClass{
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
    public class MinimumPrizePoolData : SerializedClass{
        public float prizePool;
    }

    [Serializable]
    public class InitiatePaymentData : SerializedClass{
        public string uuid;
    }

    [Serializable]
    public class ProgressData<T> : CRUDDBData where T : class{
        public int userLevel;
        public T progress;
    }

    [Serializable]
    public class CoinEarnData : SerializedClass{
        public int coinEarned;
        public int totalCoinEarned;
    }
}
