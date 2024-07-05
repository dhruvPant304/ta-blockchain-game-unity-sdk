using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SliderToggle : MonoBehaviour{
    [Header("Image Components")]
    [SerializeField] Image sliderBase;
    [SerializeField] Image slider;
    [SerializeField] TextMeshProUGUI sliderText;

    [Header("Colors")]
    [SerializeField] Color baseDisabledColor;
    [SerializeField] Color baseEnabledColor;
    [SerializeField] Color sliderEnabledColor;
    [SerializeField] Color sliderDisableColor;
    [SerializeField] Color sliderTextEnabledColor;
    [SerializeField] Color sliderTextDisableColor;

    [Header("Slider Position")]
    [SerializeField] float enabledX;
    [SerializeField] float disabledX;

    [Header("Toggle")]
    [SerializeField] private bool value;

    public UnityAction<bool> OnValueChanged;
    public bool Value {
        get =>  value;
        set {
            this.value = value;
            SetValue(this.value);
        }
    }

    void Start(){
        SetValueWithoutNotify(value);

        var button = GetComponent<Button>();
        if(button == null) return;
        button.onClick.AddListener(OnClick);
    }

    void SetValue(bool val, bool notify = true){
        value = val;
        if(val) Enable();
        else Disable();
        if(notify) OnValueChanged?.Invoke(val);
    }

    public void SetValueWithoutNotify(bool val){
        SetValue(val, false);
    }

    void Enable(){
        sliderBase.DOColor(baseEnabledColor, 0.1f);
        slider.DOColor(sliderEnabledColor, 0.1f);
        sliderText.DOColor(sliderTextEnabledColor, 0.1f);
        slider.rectTransform.DOLocalMoveX(enabledX, 0.1f);

        sliderText.text = "ON";
    }

    void Disable(){
        sliderBase.DOColor(baseDisabledColor, 0.1f);
        slider.DOColor(sliderDisableColor, 0.1f);
        sliderText.DOColor(sliderTextDisableColor, 0.1f);
        slider.rectTransform.DOLocalMoveX(disabledX, 0.1f);

        sliderText.text = "OFF";
    }

    void OnClick(){
        Toggle();
    }

    void Toggle(){
        Value = !Value;
    }
}
