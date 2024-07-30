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
    
    void Start(){
        Hide();
    }

    public void Hide(){
        gameObject.SetActive(false);
    }

    public void Show(){
        gameObject.SetActive(true);
    }

    void Confirm(){
        _confirmed = true;
    }

    void Cancel(){
        _cancelled = true;
    }

    public async UniTask<Result> WaitInput(string header, string lable, Func<string, UniTask<ValidationResult>> validation, string validationError = "") {
        Show();

        _confirmed = false;
        _cancelled = false;

        updateButton.onClick.AddListener(Confirm);
        discardButton.onClick.AddListener(Cancel);

        this.heading.text = header;
        this.lable.text = lable;

        if(validationError == ""){
            validationErrorMessage.gameObject.SetActive(false);
        }
        validationErrorMessage.text = validationError;

        inputField.onValueChanged.AddListener(async (val) => {
            var response = await validation(val);
            if(!response.IsValid){
                validationErrorMessage.text = response.ErrorMessage;
            }
        });

        await UniTask.WaitUntil(() => _confirmed || _cancelled);

        updateButton.onClick.RemoveListener(Confirm);
        discardButton.onClick.RemoveListener(Cancel);

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
