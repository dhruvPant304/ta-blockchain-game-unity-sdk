using TMPro;
using UnityEngine;

namespace TA.Menus{
public class LeaderBoardEntry : MonoBehaviour {
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI reward;

    public void SetValues(string rank, string playerName, string score, string reward){
        this.rank.text = rank;
        this.playerName.text = playerName;
        this.reward.text = reward;
        this.score.text = score;
    }
}
}
