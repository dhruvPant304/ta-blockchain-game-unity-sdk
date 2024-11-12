using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TA.Services;
using UnityEngine;

namespace TA.Components{
public class BlockchainGameCanvas : Service<BlockchainGameCanvas>{
    [SerializeField] MessagePopupPresenter presenter;

    protected override void OnInitialize(){
    }

    void Start(){
        StartCoroutine(ProcessRequestStack().ToCoroutine());
    }

    private Stack<PopUpRequest> requests;

    public void ShowMessagePopup(MessagePopup messagePopup, int styleIndex = 0){
        requests.Push(new PopUpRequest{popup = messagePopup, styleIndex = styleIndex});
    }

    public async UniTask ProcessRequestStack(){
        while(true){
            await UniTask.WaitWhile(() => requests.Count == 0);
            var req = requests.Peek();
            presenter.ShowMessagePopup(req.popup, req.styleIndex);
            await UniTask.WaitUntil(() => presenter.IsHidden);
            requests.Pop();
        }
    }

    public struct PopUpRequest{
        public MessagePopup popup;
        public int styleIndex;
    }
}
}
