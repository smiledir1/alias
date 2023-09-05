using System;
using System.Threading;
using Common.UniTaskAnimations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.UI
{
    //TODO: обдумать насчет модели / контроллера / презентера / етц
    public abstract class UIObject : MonoBehaviour
    {
        #region View

        [Header("UIObject Main")]
        [SerializeReference]
        protected ITween OpenTween;

        [SerializeReference]
        protected ITween CloseTween;

        #endregion

        #region Properties

        public UIModel UIModel { get; private set; }
        public int Group { get; private set; }
        public UIObjectState State { get; private set; }

        #endregion /Properties

        #region Events

        internal event Action CloseAction;

        public event Action UIObjectOpen;
        public event Action UIObjectClose;
        public event Action UIObjectStart;
        public event Action UIObjectStop;
        public event Action UIObjectLoad;
        public event Action UIObjectUnload;

        #endregion /Events

        #region Cache

        private CancellationTokenSource _animationTokenSource;

        #endregion

        #region Internal Methods

        internal virtual void Initialize(UIModel model, int group)
        {
            UIModel = model;
            Group = group;
        }

        internal async UniTask LoadAsync()
        {
            CloseAction = null;
            UIObjectLoad?.Invoke();
            await OnLoadAsync();
            State = UIObjectState.Loaded;
        }

        internal async UniTask UnLoadAsync()
        {
            UIObjectUnload?.Invoke();
            await OnUnLoadAsync();
            State = UIObjectState.Unloaded;
        }

        internal async UniTask OpenAsync()
        {
            UIObjectOpen?.Invoke();
            await OnOpenAsync();
            State = UIObjectState.Opened;
            if (OpenTween != null)
            {
                var hasActiveAnimation = _animationTokenSource != null;
                if (hasActiveAnimation)
                {
                    _animationTokenSource.Cancel();
                    OpenTween.ResetValues();
                }

                _animationTokenSource = new CancellationTokenSource();
                await OpenTween.StartAnimation(false, false, _animationTokenSource.Token);
                _animationTokenSource = null;
            }
        }

        internal async UniTask CloseAsync()
        {
            UIObjectClose?.Invoke();
            await OnCloseAsync();
            State = UIObjectState.Loaded;
            if (CloseTween != null)
            {
                var hasActiveAnimation = _animationTokenSource != null;
                if (hasActiveAnimation)
                {
                    _animationTokenSource.Cancel();
                    CloseTween.ResetValues();
                }

                _animationTokenSource = new CancellationTokenSource();
                await CloseTween.StartAnimation(false, false, _animationTokenSource.Token);
                _animationTokenSource = null;
            }

            UIModel?.Close();
        }

        internal async UniTask StartAsync()
        {
            UIObjectStart?.Invoke();
            await OnStartAsync();
            State = UIObjectState.Started;
        }

        internal async UniTask StopAsync()
        {
            UIObjectStop?.Invoke();
            await OnStopAsync();
            State = UIObjectState.Opened;
        }

        internal void SetOpenAnimation(ITween openTween)
        {
            OpenTween = openTween;
        }

        internal void SetCloseAnimation(ITween closeTween)
        {
            CloseTween = closeTween;
        }

        #endregion /Internal Methods

        #region Protected Methods

        /// <summary>
        /// After Instantiate Object
        /// </summary>
        protected virtual UniTask OnLoadAsync() => UniTask.CompletedTask;

        /// <summary>
        /// Before Destroy
        /// </summary>
        protected virtual UniTask OnUnLoadAsync() => UniTask.CompletedTask;

        /// <summary>
        /// After Open (Show View)
        /// </summary>
        protected virtual UniTask OnOpenAsync() => UniTask.CompletedTask;

        /// <summary>
        /// Before Close (Close View)
        /// </summary>
        protected virtual UniTask OnCloseAsync() => UniTask.CompletedTask;

        /// <summary>
        /// After Active
        /// </summary>
        protected virtual UniTask OnStartAsync() => UniTask.CompletedTask;

        /// <summary>
        /// Before Deactivate
        /// </summary>
        protected virtual UniTask OnStopAsync() => UniTask.CompletedTask;

        protected void Close()
        {
            CloseAction?.Invoke();
        }

        #endregion /Protected Methods
    }

    public abstract class UIObject<T> : UIObject where T : UIModel
    {
        protected T Model { get; private set; }

        internal override void Initialize(UIModel uiModel, int group)
        {
            base.Initialize(uiModel, group);
            Model = (T) uiModel;
            if (Model == null) Debug.LogError($"Wrong Model {typeof(T)} in {GetType()}");
        }
    }
}