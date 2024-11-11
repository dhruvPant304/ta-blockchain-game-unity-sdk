using System;
using TA.APIClient;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardTypeSelector : MonoBehaviour {
    [SerializeField] GameObject totalSelectionHighlight;
    [SerializeField] GameObject highSelectionHighlight;

    [SerializeField] Button total;
    [SerializeField] Button high;

    public string Selection {get; private set;}
    public Action<string> OnSelection;
    public bool interactable = true;

    void OnHigh(){
        if(!interactable) return;
        Selection = "high";
        totalSelectionHighlight.SetActive(false);
        highSelectionHighlight.SetActive(true);
        OnSelection?.Invoke(Selection);
    }

    void OnTotal(){
        if(!interactable) return;
        Selection = "total";
        highSelectionHighlight.SetActive(false);
        totalSelectionHighlight.SetActive(true);
        OnSelection?.Invoke(Selection);
    }

    void Start(){
        var config = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;

        if(!config.hasHighScoreLeaderboard && !config.hasTotalScoreLeaderboard) {
            gameObject.SetActive(false);
            return;
        }

        total.gameObject.SetActive(config.hasTotalScoreLeaderboard);
        high.gameObject.SetActive(config.hasHighScoreLeaderboard);

        total.onClick.AddListener(OnTotal);
        high.onClick.AddListener(OnHigh);

        OnHigh();
    }
}
