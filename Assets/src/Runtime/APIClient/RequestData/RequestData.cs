using System;

namespace TA.APIClient.RequestData{

    [Serializable]
    public class LoginParams{
        public string verifierId;
        public string loginType;
        public string walletAddress;
        public string signature;
        public string email;
        public string platform;
        public string appType;
    }
}

