using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Components{
public class MessagePopupPresenter : MonoBehaviour{
    [SerializeField] TextMeshProUGUI header;
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Image popupBG;
    [SerializeField] Transform exitButtonContent;

    [SerializeField] GameObject dangerBanner;
    [SerializeField] GameObject rewardBanner;
    [SerializeField] GameObject bG;

    [Header("Button Prefabs")]
    [SerializeField] DynamicButton confirmationPrefab;
    [SerializeField] DynamicButton regularPrefab;
    [SerializeField] DynamicButton dangerPrefab;

    [Header("PopUp Styles")]
    [SerializeField] List<PopUpStyle> style;

    List<DynamicButton> _activeExitButtons = new();
    public bool IsHidden {get; private set;}

    void Start(){
        Hide();
    }

    public void ShowMessagePopup(MessagePopup popup, int styleIndex = 0){
        ApplyStyle(style[styleIndex]);
        Show();

        header.text = popup.header;
        message.text = popup.message;

        dangerBanner.SetActive(false);
        rewardBanner.SetActive(false);
        bG.SetActive(popup.hasBackground);

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

    void ApplyStyle(PopUpStyle style){
        popupBG.sprite = style.BgImage;
        header.font = style.headerFont;
        message.font = style.messageFont;
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
        IsHidden = false;
        gameObject.SetActive(true);
    }

    void Hide(){
        IsHidden = true;
        gameObject.SetActive(false);
    }
}

[Serializable]
public class PopUpStyle{
    public Sprite BgImage;
    public TMP_FontAsset headerFont;
    public TMP_FontAsset messageFont;
}

public class MessagePopup{
    public string message = "";
    public string header = "";
    public bool hasBackground = false;
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
    public ExitStyle exitStyle = ExitStyle.Regular;
}

public enum BannerType{
    None,
    Danger,
    Good,
    Warning,
    Reward
}
}
