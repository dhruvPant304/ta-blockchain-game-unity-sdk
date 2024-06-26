using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TA.Components{
public class MessagePopupPresenter : MonoBehaviour{
    [SerializeField] TextMeshProUGUI header;
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Transform exitButtonContent;

    [SerializeField] GameObject dangerBanner;
    [SerializeField] GameObject rewardBanner;

    [Header("Button Prefabs")]
    [SerializeField] DynamicButton confirmationPrefab;
    [SerializeField] DynamicButton regularPrefab;
    [SerializeField] DynamicButton dangerPrefab;

    List<DynamicButton> _activeExitButtons = new();

    void Start(){
        Hide();
    }

    public void ShowMessagePopup(MessagePopup popup){
        Show();

        header.text = popup.header;
        message.text = popup.message;

        dangerBanner.SetActive(false);
        rewardBanner.SetActive(false);

        switch(popup.banner){
            case BannerType.Danger:
                dangerBanner.SetActive(true);
                break;
            case BannerType.Reward:
                rewardBanner.SetActive(true);
                break;
        }

        CreateExits(popup.exits.ToArray());
    }

    public void HidePopup(){
        Hide();
        DisposeActiveButtons();
    }

    void CreateExits(MessagePopupExit[] exits){
        DisposeActiveButtons();
        foreach(var exit in exits){
            CreateExit(exit);
        }
    }

    DynamicButton CreateExit(MessagePopupExit popupExit){
        var buttonPrefab = GetPrefabForStyle(popupExit.exitStyle);
        var button = Instantiate(buttonPrefab, exitButtonContent);

        //Appending Hide pop up function to exit action
        popupExit.exitAction += HidePopup;
        button.CreateButton(popupExit.name, popupExit.exitAction);

        _activeExitButtons.Add(button);
        return button;
    }

    void DisposeActiveButtons(){
        foreach(var button in _activeExitButtons){
            Destroy(button.gameObject);
        }
        _activeExitButtons.Clear();
    }

    DynamicButton GetPrefabForStyle(MessagePopupExit.ExitStyle style){
        switch(style){
            case MessagePopupExit.ExitStyle.Regular:
                return regularPrefab;
            case MessagePopupExit.ExitStyle.Danger:
                return dangerPrefab;
            case MessagePopupExit.ExitStyle.Confirmation:
                return confirmationPrefab;
        }
        return regularPrefab;
    }

    void Show(){
        gameObject.SetActive(true);
    }

    void Hide(){
        gameObject.SetActive(false);
    }
}

public class MessagePopup{
    public string message = "";
    public string header = "";
    public BannerType banner;
    public List<MessagePopupExit> exits;
}

public class MessagePopupExit{
    public string name;
    public Action exitAction;

    public enum ExitStyle{
        Regular,
        Confirmation,
        Danger
    }
    public ExitStyle exitStyle;
}

public enum BannerType{
    Danger,
    Good,
    Warning,
    Reward
}
}
