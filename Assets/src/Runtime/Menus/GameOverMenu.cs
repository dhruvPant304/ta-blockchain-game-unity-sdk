using TA.Game;
using TA.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Menus{
public class GameOverMenu : MonoBehaviour {
    [Header("Text Fields")]
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI continueCost;
    [SerializeField] TextMeshProUGUI position;

    [Header("Buttons")]
    [SerializeField] Button endGameButton;
    [SerializeField] Button ContinueButton;
    [SerializeField] Button buyCreditsButton;

    [Header("Panels")]
    [SerializeField] GameObject panel;

    [Header("Text options")]
    [SerializeField] string scoreSuffix;
    [SerializeField] string scorePrefix;
    [SerializeField] string positionSuffix;
    [SerializeField] string positionPrefix;

    GameService _gameService;
    TAMenuService _taMenuService;

    void Start(){
        _gameService = ServiceLocator.Instance.GetService<GameService>();
        _taMenuService = ServiceLocator.Instance.GetService<TAMenuService>();

        endGameButton.onClick.AddListener(OnEndGame);
        ContinueButton.onClick.AddListener(OnContinueGame);
        buyCreditsButton.onClick.AddListener(OnBuyCredits);

        Hide();
    }

    public async void ShowEndGameMenu(){
        panel.SetActive(true);
        score.text = scorePrefix + _gameService.SavedTotalScore.ToString() + scoreSuffix;
        continueCost.text = $"Continue {_gameService.NextContinueCost} credit?";

        var rank = await _gameService.FetchCurrentUserRank();
        position.text = positionPrefix + rank.ToString() + positionSuffix;
    }

    void Hide(){
        panel.SetActive(false);
    }

    void OnEndGame(){
        Hide();
        _gameService.MakeExitGameRequest();
    }

    void OnContinueGame(){
        Hide();
        _gameService.ContinueGame();
    }

    void OnBuyCredits(){
        Hide();
        _taMenuService.OpenInGameCreditShop();
        _taMenuService.OnInGameCreditShopClosed += ReOpenOnBuyCreditClose;
    }

    void ReOpenOnBuyCreditClose(){
        ShowEndGameMenu();
        _taMenuService.OnInGameCreditShopClosed -= ReOpenOnBuyCreditClose;
    }
}}
