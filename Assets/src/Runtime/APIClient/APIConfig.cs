using UnityEngine;

namespace TA.APIClient{
[CreateAssetMenu(menuName="TA/APIService", fileName="APIConfig")]
public class APIConfig : ScriptableObject {

    [Header("Env")]
    public string serverUrl;

    [Header("Keys")]
    public string encryptionPublicKey;
    public string gameId;

    [Header("Leaderboard")]
    public int entriesPerPage = 20;
    public bool hasHighScoreLeaderboard = true;
    public bool hasTotalScoreLeaderboard = true;

    [Header("DebugSettings")]
    public bool logResponses;
    public bool logRequest;
}
}
