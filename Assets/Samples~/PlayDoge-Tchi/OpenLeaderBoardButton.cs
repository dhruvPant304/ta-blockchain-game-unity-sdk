using TA.Menus;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenLeaderBoardButton : MonoBehaviour {
    TAMenuService _menuService;

    void Start(){
        _menuService = ServiceLocator.Instance.GetService<TAMenuService>();
        GetComponent<Button>().onClick.AddListener(_menuService.OpenLeaderBoardPage);
    }
}
