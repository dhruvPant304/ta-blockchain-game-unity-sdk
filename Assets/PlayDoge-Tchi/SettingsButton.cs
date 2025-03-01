using UnityEngine;
using TA.Menus;
using TA.Services;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsButton : MonoBehaviour {
    TAMenuService _taMenuService;

    void Start(){
        _taMenuService = ServiceLocator.Instance.GetService<TAMenuService>();
        GetComponent<Button>().onClick.AddListener(_taMenuService.OpenSettingsPage);
    }
}
