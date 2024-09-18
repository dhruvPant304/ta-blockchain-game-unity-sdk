using System;

namespace TA.APIClient.RequestData{
    [Serializable]
    public class StartGameParams {
        public bool isFreeToPlay;
    }

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

    [Serializable]
    public class UpdateScoreParams{
        public string sessionScore;
        public string startTime;
        public string endTime;
        public string duration;
    }

    [Serializable]
    public class FinalScoreParams {
        public string totalScore;
    }

    [Serializable]
    public class UpdateProfileParams{
        public string username;
    }
}

