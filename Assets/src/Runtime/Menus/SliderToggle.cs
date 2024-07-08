using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        StartCoroutine(TransformColor(sliderBase,baseEnabledColor, 0.1f));
        StartCoroutine(TransformColor(slider, sliderEnabledColor, 0.1f));
        StartCoroutine(TransformColor(sliderText, sliderTextEnabledColor, 0.1f));
        StartCoroutine(DoLocalMoveX(slider.rectTransform, enabledX, 0.1f));

        sliderText.text = "ON";
    }

    void Disable(){
        StartCoroutine(TransformColor(sliderBase,baseDisabledColor, 0.1f));
        StartCoroutine(TransformColor(slider, sliderDisableColor, 0.1f));
        StartCoroutine(TransformColor(sliderText, sliderTextDisableColor, 0.1f));
        StartCoroutine(DoLocalMoveX(slider.rectTransform, disabledX, 0.1f));

        sliderText.text = "OFF";
    }

    void OnClick(){
        Toggle();
    }

    void Toggle(){
        Value = !Value;
    }

    IEnumerator TransformColor(Graphic graphic, Color endColor, float duration){
        var startTime = Time.time;
        var startColor = graphic.color;
        while(Time.time - startTime < duration){
            var t = (Time.time - startTime) / duration;
            graphic.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        graphic.color = endColor;
    } 

    IEnumerator DoLocalMoveX(Transform transform, float endValue, float duration){
        var startPos = transform.localPosition;
        var endPos = new Vector3(endValue, startPos.y ,startPos.z);
        var startTime = Time.time;
        while(Time.time - startTime < duration){
            var t = (Time.time - startTime) / duration;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        transform.localPosition = endPos;
    }
}
