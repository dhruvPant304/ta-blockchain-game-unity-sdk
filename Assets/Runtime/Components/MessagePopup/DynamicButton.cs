using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Components{
[RequireComponent(typeof(Button))]
public class DynamicButton : MonoBehaviour {
    [SerializeField] TextMeshProUGUI buttonText;
    Action onClick;

    public void CreateButton(string name, Action onClick){
        this.onClick = onClick;
        GetComponent<Button>().onClick.AddListener(OnClick);
        buttonText.text = name;
    }

    void OnClick(){
        onClick?.Invoke();
    }
}
}
