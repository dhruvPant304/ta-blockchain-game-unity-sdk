using TA.Menus;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CloseAllPageButton : MonoBehaviour{
    TAMenuService _taMenuService;

    void Start(){
        _taMenuService = ServiceLocator.Instance.GetService<TAMenuService>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick(){
        _taMenuService.CloseAll();
    }
}
