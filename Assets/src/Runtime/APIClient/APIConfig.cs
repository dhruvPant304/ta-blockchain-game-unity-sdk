using UnityEngine;

namespace TA.APIClient{
[CreateAssetMenu(menuName="TA/APIService", fileName="APIConfig")]
public class APIConfig : ScriptableObject {

    [Header("Env")]
    public string serverUrl;

    [Header("Keys")]
    public string encryptionPublicKey;
    public string gameId;

    [Header("Login Config")]
    public string signatureString;

    [Header("Game Config")]
    public bool requireCreditsToPlay = true;

    [Header("Leaderboard")]
    public int entriesPerPage = 20;
    public bool hasHighScoreLeaderboard = true;
    public bool hasTotalScoreLeaderboard = true;

    [Header("DebugSettings")]
    public bool logResponses;
    public bool logRequest;
}
}
