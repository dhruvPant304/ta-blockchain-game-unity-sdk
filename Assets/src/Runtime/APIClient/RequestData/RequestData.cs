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
    public class UpdateScoreRequest{
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

    [Serializable]
    public class ProgressParams{
        public object progress;
    }

    [Serializable]
    public class AddCoinParams{
        public int coinEarned;
    }

    [Serializable]
    public class BuyItemParams{
        public int itemId;
        public int quantity;
        public string itemType;
    }

    [Serializable]
    public class ConsumeItemParams{
        public int id;
    }
}

