using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Menus{
public class InputModal : MonoBehaviour {
    [SerializeField] Button discardButton;
    [SerializeField] Button updateButton;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI lable;
    [SerializeField] TextMeshProUGUI heading;
    [SerializeField] TextMeshProUGUI validationErrorMessage;

    bool _confirmed = false;
    bool _cancelled = false;

    public bool IsOpen {get; private set;}
    
    void Start(){
        Hide();
    }

    void Hide(){
        gameObject.SetActive(false);
        IsOpen = false;
    }

    void Show(){
        gameObject.SetActive(true);
        IsOpen = true;
    }

    void Confirm(){
        _confirmed = true;
    }

    void Cancel(){
        _cancelled = true;
    }

    public async UniTask<Result> WaitInput(string header, string lable, Func<string, UniTask<ValidationResult>> validation, string validationError = "") {
        if(IsOpen) throw new Exception("Input modal is alread awaiting user input, make sure previous input is handled before making another, input request using this input modal");
        Show();

        _confirmed = false;
        _cancelled = false;

        updateButton.onClick.AddListener(Confirm);
        discardButton.onClick.AddListener(Cancel);

        this.heading.text = header;
        this.lable.text = lable;

        if(validationError == ""){
            validationErrorMessage.gameObject.SetActive(false);
            updateButton.interactable = true;
        }
        else{
            validationErrorMessage.gameObject.SetActive(true);
            updateButton.interactable = false;
        }

        validationErrorMessage.text = validationError;

        async void OnValueChanged(string val) {
            var response = await validation(val);
            if (!response.IsValid) {
                validationErrorMessage.text = response.ErrorMessage;
                validationErrorMessage.gameObject.SetActive(true);
                updateButton.interactable = false;
            }
            else {
                validationErrorMessage.gameObject.SetActive(false);
                updateButton.interactable = true;
            }
        }

        inputField.onValueChanged.AddListener(OnValueChanged);

        await UniTask.WaitUntil(() => _confirmed || _cancelled);

        updateButton.onClick.RemoveListener(Confirm);
        discardButton.onClick.RemoveListener(Cancel);
        inputField.onValueChanged.RemoveListener(OnValueChanged);

        if(_cancelled){
            Hide();

            return new Result{
                option = Option.None,
                result = null
            };
        }

        var input = inputField.text;
        var result = await validation(input);
        Hide();

        if(!result.IsValid){
            return await WaitInput(header, lable, validation, validationError);
        }

        return new Result {
            option = Option.Some,
            result = input
        };
    }
}

public class ValidationResult{
    public bool IsValid;
    public string ErrorMessage;
}

public class Result{
    public Option option;
    public string result;
}

public enum Option{
    Some,
    None
}}
