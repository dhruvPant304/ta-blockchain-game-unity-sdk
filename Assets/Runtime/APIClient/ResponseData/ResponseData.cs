using System;

namespace TA.APIClient.ResponseData{
    [Serializable]
    public class APIResponse<T>{
        public string status;
        public string message;
        public T data;

        public bool IsSuccess => status == "success";
    }
    
    //=====================
    // RESPONSE CLASSES
    //=====================

    [Serializable]
    public class LoginResponse : APIResponse<LoginUserData>{}

    [Serializable]
    public class UserBalanceResponse : APIResponse<UserBalanceData>{}

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
    public class LoginUserData : UserData{
        public string token;
        public bool isFirstTimeUser;
    }

    [Serializable]
    public class UserBalanceData {
        public int credits;
        public int gameCoin;
        public int freeCredits;
        public int boughtCredits;
        public int tokens;
        public int xpBalance;
        public int native;
    }

    [Serializable]
    public class AppSettings{
        public bool isMusic;
        public bool isSound;
        public bool isVibrate;
    }
}