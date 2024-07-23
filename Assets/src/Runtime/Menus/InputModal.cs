using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Menus{
public class InputModal : MonoBehaviour {
    [SerializeField] Button discardButton;
    [SerializeField] Button updateButton;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI heading;

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

    public async UniTask<Result> WaitInput(string header) {
        Show();

        _confirmed = false;
        _cancelled = false;

        updateButton.onClick.AddListener(Confirm);
        discardButton.onClick.AddListener(Cancel);

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

        Hide();

        return new Result {
            option = Option.Some,
            result = inputField.text
        };
    }
}

public class Result{
    public Option option;
    public string result;
}

public enum Option{
    Some,
    None
}}
