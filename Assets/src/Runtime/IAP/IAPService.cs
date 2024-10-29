using UnityEngine.Purchasing;
using TA.Services;
using TA.Components;
using TA.APIClient;
using System.Collections.Generic;
using UnityEngine;
using TA.Menus;
using UnityEngine.Purchasing.Extension;
using System;
using Cysharp.Threading.Tasks;
using TA.APIClient.ResponseData;
using System.Linq;
using TA.UserProfile;

namespace TA.IAP{
    public class IAPService : Service<IAPService>, IDetailedStoreListener {
        [SerializeField] CreditPurchasePackageData purchasePackageData; //Create a Data provider service for this

        BlockchainGameCanvas _canvas;
        APIService _apiService;
        UserProfileService _userProfile; 

        IStoreController _storeController;
        IExtensionProvider _extensionProvider;

        public bool Initialized => _storeController != null && _extensionProvider != null;
        public string Platform => Application.platform == RuntimePlatform.Android? "android" : "ios";

        void Start(){
            _canvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
            _apiService = ServiceLocator.Instance.GetService<APIService>();
            _userProfile = ServiceLocator.Instance.GetService<UserProfileService>();

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

        MessagePopup CreatePurchaseSuccessPopUp(string message){
            return new MessagePopup(){
                banner = BannerType.Reward,
                header = "Purchase Successful",
                message = message,
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
        // PAYMENT VERIFICATION 
        //=====================

        async UniTask<InitiatePaymentResponse> InitiatePayment(PurchaseEventArgs purchaseEvent, PurchasePackage creditPackage){
            

            var initiateData = new InitiatePurchaseData(){
                amount = purchaseEvent.purchasedProduct.metadata.localizedPrice,
                credits = creditPackage.creditAmount,
                platform = Platform,
                currency = purchaseEvent.purchasedProduct.metadata.isoCurrencyCode
            };

            var response = await _apiService.SendInitiatePaymentRequest(initiateData,_userProfile.LoginToken);
            if(response.IsSuccess) return response.SuccessResponse;
            
            return new InitiatePaymentResponse(){
                status = "FAILED",
                message = response.FailureResponse.message
            };
        }

        async UniTask<BaseAPIResponse> VerifyPayment(string uuid, PurchasePackage creditPackage, PurchaseEventArgs purchaseEvent){
            var verificationData = new PurhcaseVerificationData(){
                amount = purchaseEvent.purchasedProduct.metadata.localizedPrice,
                credits = creditPackage.creditAmount,
                verifyUUID = uuid,
                currency = purchaseEvent.purchasedProduct.metadata.isoCurrencyCode,
                receipt = purchaseEvent.purchasedProduct.receipt
            };

            var response =await _apiService.SendVerificationRequest(verificationData, _userProfile.LoginToken);
            if(response.IsSuccess) return response.SuccessResponse;

            return new BaseAPIResponse(){
                status = "FAILED",
                message = response.FailureResponse.message
            };
        }


        //=====================
        // STORE LISTENER IMPLEMENTATION
        //=====================


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions){
            _storeController = controller;
            _extensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error){
            // throw new System.NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason){
            // throw new System.NotImplementedException();
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message){
            _canvas.ShowMessagePopup(CreateInitializationErrorPopUp(message));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent){
            VerifyPaymentFromServer(purchaseEvent).Forget();
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription){
            _canvas.ShowMessagePopup(CreatePurchaseErrorPopUp(failureDescription.message));
        }

        async UniTask VerifyPaymentFromServer(PurchaseEventArgs purchaseEvent){
            PurchasePackage creditPackage;
            try{
                creditPackage = purchasePackageData.packages.First((p) => p.storeProductId == purchaseEvent.purchasedProduct.definition.id);
            }
            catch{
                _canvas.ShowMessagePopup(CreatePurchaseErrorPopUp($"Cannot identify package in game id:{purchaseEvent.purchasedProduct.definition.id}"));
                return;
            }

            var response = await InitiatePayment(purchaseEvent, creditPackage);
            
            if(!response.IsSuccess){
                _canvas.ShowMessagePopup(CreatePurchaseErrorPopUp(response.message));
            }

            var verifyResponse = await VerifyPayment(response.data.uuid, creditPackage, purchaseEvent);

            if(!response.IsSuccess){
                _canvas.ShowMessagePopup(CreatePurchaseErrorPopUp(verifyResponse.message));
            }

            _canvas.ShowMessagePopup(CreatePurchaseSuccessPopUp("Credits purchased successfully"));
        }

    }

    [Serializable]
    public class InitiatePurchaseData{
        public decimal amount;
        public int credits;
        public string platform;
        public string currency;
    }

    [Serializable]
    public class PurhcaseVerificationData{
        public decimal amount;
        public int credits;
        public string verifyUUID;
        public string currency;
        public string receipt;
    }
}

