using System;
using Client.Core;
using UniRx;

namespace Client.Actor
{
    public class ActorEntity : IEntity
    {
        private readonly IActor _actor;

        private readonly CompositeDisposable _disposables = new();
        private readonly IActorModel _model;
        private readonly IActorPresenter _presenter;

        public ActorEntity(string id, IActor actor, IActorView view)
        {
            Id = id;
            _actor = actor;
            _model = CreateModel(actor, view.Settings);
            _presenter = CreatePresenter(view, _model);
        }

        public string Id { get; }

        public void Initialize()
        {
            _presenter.Initialize();
        }

        public void Update()
        {
            _presenter.Update();
        }

        public void Release()
        {
            _presenter.Release();
            _disposables.Dispose();
        }

        private IActorModel CreateModel(IActor actor, ActorSettings settings)
        {
            // Actor 타입에 따라 적절한 Model 생성
            return actor.Type switch
            {
                    ActorType.LocalPlayer => new LocalActorModel(settings),
                    // ActorType.RemotePlayer => new RemoteActorModel(actor, settings),
                    // ActorType.NPC => new NPCActorModel(actor, settings),
                    _ => new ActorModel(settings)
            };
        }

        private IActorPresenter CreatePresenter(IActorView view, IActorModel model)
        {
            return model switch
            {
                    LocalActorModel localModel => new LocalActorPresenter(view, localModel),
                    // RemoteActorModel remoteModel => new RemoteActorPresenter(view, remoteModel),
                    _ => throw new ArgumentException("Unknown actor model type")
            };
        }

        public T GetModel<T>() where T : class, IActorModel
        {
            return _model as T;
        }

        public T GetActor<T>() where T : class, IActor
        {
            return _actor as T;
        }
    }
}