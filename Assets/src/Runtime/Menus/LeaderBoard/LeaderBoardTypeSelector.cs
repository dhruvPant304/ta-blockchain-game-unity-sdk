using System;
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
        total.onClick.AddListener(OnTotal);
        high.onClick.AddListener(OnHigh);

        OnHigh();
    }
}
