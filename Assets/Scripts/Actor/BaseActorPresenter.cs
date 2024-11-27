using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Client.Actor
{
    public abstract class BaseActorPresenter : IActorPresenter
    {
        protected readonly Dictionary<Type, IActorController> _controllers = new();
        protected readonly CompositeDisposable _disposables = new();
        protected readonly IActorModel _model;
        protected readonly ActorStateModel _stateModel;
        protected readonly IActorView _view;

        protected BaseActorPresenter(IActorView view, IActorModel model)
        {
            _view = view;
            _model = model;
            _stateModel = new ActorStateModel();
        }

        public ActorStateModel StateModel
        {
            get => _stateModel;
        }

        public virtual void Initialize()
        {
            InitializeControllers();
            SetupBaseBindings();
            SetupSpecificBindings();
        }

        public virtual void Update()
        {
            foreach (var controller in _controllers.Values.Where(c => c.IsActive))
            {
                controller.Update();
            }
        }

        public virtual void Release()
        {
            _disposables.Dispose();
            foreach (var controller in _controllers.Values)
            {
                controller.Release();
            }

            _controllers.Clear();
        }

        protected virtual void SetupBaseBindings()
        {
        }

        public T GetController<T>() where T : class, IActorController
        {
            var type = typeof(T);
            return _controllers.TryGetValue(type, out var controller) ? controller as T : null;
        }

        protected abstract void InitializeControllers();
        protected abstract void SetupSpecificBindings();
    }
}
