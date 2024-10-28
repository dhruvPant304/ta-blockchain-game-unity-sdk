using UnityEngine.Purchasing;
using TA.Services;
using TA.Components;
using TA.APIClient;
using System.Collections.Generic;
using UnityEngine;
using TA.Menus;
using UnityEngine.Purchasing.Extension;

namespace TA.IAP{
    public class IAPService : Service<IAPService>, IDetailedStoreListener {
        [SerializeField] CreditPurchasePackageData purchasePackageData; //Create a Data provider service for this

        BlockchainGameCanvas _canvas;
        APIService _apiService;

        IStoreController _storeController;
        IExtensionProvider _extensionProvider;

        public bool Initialized => _storeController != null && _extensionProvider != null;

        void Start(){
            _canvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
            _apiService = ServiceLocator.Instance.GetService<APIService>();

            if(!Initialized) InitializePurchasing();
        }

        void InitializePurchasing(){
            var targetAppStore = Application.platform == RuntimePlatform.Android ? AppStore.GooglePlay : AppStore.AppleAppStore;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(targetAppStore));

            foreach(var package in purchasePackageData.packages){
                builder.AddProduct(package.storeProductId, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyProductID(string productId){
            if(!Initialized) {
                _canvas.ShowMessagePopup(CreatePurchaseErrorPopUp("Purchasing service failed to initialize"));
                return;
            }
            var product = _storeController.products.WithID(productId);
            if(product != null && product.availableToPurchase){
                _storeController.InitiatePurchase(product);
                return;
            }
            _canvas.ShowMessagePopup(CreatePurchaseErrorPopUp("Selected product not available for purchase"));
        }

        //=====================
        // MESSAGE POP-UP 
        //=====================

        MessagePopup CreateInitializationErrorPopUp(string errorMessage){
            return new MessagePopup(){
                banner = BannerType.Danger,
                header = "Failed To Initialize IAP",
                message = errorMessage,
                exits = new List<MessagePopupExit>(){
                    new MessagePopupExit(){
                        name = "Okay",
                        exitAction = () => {},
                        exitStyle = MessagePopupExit.ExitStyle.Confirmation
                    }
                }
            };
        }

        MessagePopup CreatePurchaseErrorPopUp(string errorMessage){
            return new MessagePopup(){
                banner = BannerType.Danger,
                header = "Purchase Failed",
                message = errorMessage,
                exits = new List<MessagePopupExit>(){
                    new MessagePopupExit(){
                        name = "Okay",
                        exitAction = () => {},
                        exitStyle = MessagePopupExit.ExitStyle.Confirmation
                    }
                }
            };
        }

        //=====================
        // STORE LISTENER IMPLEMENTATION
        //=====================


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions){
            // throw new System.NotImplementedException();
        }

        public void OnInitializeFailed(InitializationFailureReason error){
            // throw new System.NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)        {
            // throw new System.NotImplementedException();
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message){
            _canvas.ShowMessagePopup(CreateInitializationErrorPopUp(message));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent){
            throw new System.NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)        {
            _canvas.ShowMessagePopup(CreatePurchaseErrorPopUp(failureDescription.message));
        }

    }
}

