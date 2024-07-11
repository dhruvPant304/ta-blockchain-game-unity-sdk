using TA.Menus;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuyCreditsButton : MonoBehaviour{
    TAMenuService _taMenuService;

    [SerializeField] GameObject TitleGroup;
    [SerializeField] GameObject NavButtonGroup;

    void Start(){
        _taMenuService = ServiceLocator.Instance.GetService<TAMenuService>();
        GetComponent<Button>().onClick.AddListener(OpenBuyCreditsPanel);

        _taMenuService.OnBuyCreditsMenuOpen += OnBuyCreditOpen;
        _taMenuService.OnBuyCreditsMenuClosed += OnCloseBuyCredit;
    }

    void OpenBuyCreditsPanel(){
        Debug.Log("OpenBuyCreditsMenu");
        _taMenuService.OpenBuyCreditsMenu();
    }

    void OnBuyCreditOpen(){
        TitleGroup.SetActive(false);
        NavButtonGroup.SetActive(false);
    }

    void OnCloseBuyCredit(){
        TitleGroup.SetActive(true);
        NavButtonGroup.SetActive(true);
    }
}
