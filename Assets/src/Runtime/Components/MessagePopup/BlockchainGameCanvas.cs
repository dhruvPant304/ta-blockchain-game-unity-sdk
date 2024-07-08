using TA.Services;
using UnityEngine;

namespace TA.Components{
public class BlockchainGameCanvas : Service<BlockchainGameCanvas>{
    [SerializeField] MessagePopupPresenter presenter;

    protected override void OnInitialize(){
    }

    public void ShowMessagePopup(MessagePopup messagePopup){
        presenter.ShowMessagePopup(messagePopup);
    }
}
}
