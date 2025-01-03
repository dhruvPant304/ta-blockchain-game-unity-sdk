using System;
using Newtonsoft.Json;
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
    public class BoosterResponse<T> : APIResponse<BoosterData<T>> where T: class, IShopItem {};

    [Serializable]
    public class InventoryResponse : APIResponse<InventoryEntry[]> {};

    [Serializable]
    public class CheckFreeBoosterResponse : APIResponse<FreeBoosterAvailableData>{};

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
        public int xpBalance;
        public float native;

        public static UserBalanceData operator+(UserBalanceData data1, UserBalanceData data2){
            return new UserBalanceData{
                credits = data1.credits + data2.credits,
                gameCoin = data1.gameCoin + data2.gameCoin,
                freeCredits = data1.freeCredits + data2.freeCredits,
                boughtCredits = data1.boughtCredits + data2.boughtCredits,
                tokens = data1.tokens + data2.tokens,
                xpBalance = data1.xpBalance + data2.xpBalance,
                native = data1.native + data2.native
            };
        }
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
    public class BoosterData<T> where T: class{
        public T[] boosters;
        public T freeBooster;
        public string boosterRenewTimeInMinutes;
    }

    [Serializable]
    public class InventoryEntry{
        public int quantity;
        public string lastPurchaseTime;
        public JObject item;

        public InventoryEntry<TConvert> TryParse<TConvert>(ref bool success){
            try{
                success = true;
                return new InventoryEntry<TConvert>(){
                    quantity = quantity,
                    lastPurchaseTime = lastPurchaseTime,
                    item = JsonConvert.DeserializeObject<TConvert>(JsonConvert.SerializeObject(item))
                };
            } catch {
                success = false;
                return null;
            }
        }

        public InventoryEntry<TConvert> Parse<TConvert>(){
            bool success = false;
            return TryParse<TConvert>(ref success);
        }
    }

    [Serializable]
    public class InventoryEntry<T>{
        public int quantity;
        public string lastPurchaseTime;
        public T item;
    }

    [Serializable]
    public class FreeBoosterAvailableData{
        public bool isClaimed;
        public string lastPurchaseTime;
    }

    [Serializable]
    public class CoinEarnData : SerializedClass{
        public int coinEarned;
        public int totalCoinEarned;
    }
}
