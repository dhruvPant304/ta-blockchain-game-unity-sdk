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
    }

    [Serializable]
    public class LoginUserData : UserData{
        public string token;
        public bool isFirstTimeUser;
    }
}
