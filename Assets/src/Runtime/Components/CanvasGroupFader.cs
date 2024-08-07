using Cysharp.Threading.Tasks;
using TA.Services;
using UnityEngine;

namespace TA.Components{
    public class CanvasGroupFader : Service<CanvasGroupFader> {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] float fadeDuration;
        bool _animating;
        bool _fadingIn;
        bool _fadingOut;
        bool _faddedIn;
        bool _faddedOut = true;

        protected override void OnInitialize(){
        }

        void Start(){
            //FadeIn().Forget();
        }

        public async UniTask FadeIn(){
            if(_faddedIn) return;
            if(_fadingIn) return;
            Debug.Log("fading in");
            _fadingIn = true;
            _faddedOut = false;
            await LerpToAlpha(fadeDuration,1, 0);
            _fadingIn = false;
            _faddedIn = true;
        }

        public async UniTask FadeOut(){
            if(_faddedOut) return;
            if(_fadingOut) return;
            Debug.Log("fading out");
            _fadingOut = true;
            _faddedIn = false;
            await LerpToAlpha(fadeDuration,0, 1);
            _fadingOut = false;
            _faddedOut = true;
        }

        async UniTask LerpToAlpha(float duration, float startAlpha, float finalAlpha){
            await UniTask.WaitWhile(() => _animating);
            _animating = true;
            var startTime = Time.time;
            while(Time.time - startTime <= fadeDuration){
                var t = (Time.time - startTime)/fadeDuration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, finalAlpha, t);
                await UniTask.Yield();
            }
            canvasGroup.alpha = finalAlpha;
            _animating = false;
        }
    }
}
