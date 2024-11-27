using System;
using System.Collections.Generic;
using Client.Actor;
using Client.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Client.Service
{
    public class ActorService : IService
    {
        private readonly Dictionary<string, ActorEntity> _entities = new();
        private ActorEntity _localActorEntity;

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            CreateLocalActor2();
            IsActive = true;
        }

        public void DoUpdate(float deltaTime)
        {
            if (!IsActive)
            {
                return;
            }

            foreach (var entity in _entities.Values)
            {
                entity.Update();
            }
        }

        public void Release()
        {
            foreach (var entity in _entities.Values)
            {
                entity.Release();
            }

            _entities.Clear();
            _localActorEntity = null;
            IsActive = false;
        }

        private void CreateLocalActor()
        {
            var id = Guid.NewGuid().ToString();
            var actor = new LocalActor(id);
            var view = CreateView("Prefabs/LocalPlayer");

            _localActorEntity = new ActorEntity(id, actor, view);
            _localActorEntity.Initialize();

            _entities.Add(id, _localActorEntity);
        }

        private void CreateLocalActor2()
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                var actor = new LocalActor(id);
                var view = GameApplication.Instance.LocalActor.GetOrAddComponent<ActorView>();

                _localActorEntity = new ActorEntity(id, actor, view);
                _localActorEntity.Initialize();

                _entities.Add(id, _localActorEntity);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public T GetLocalActor<T>() where T : class, IActor
        {
            return _localActorEntity?.GetActor<T>();
        }

        public ActorEntity GetEntity(string id)
        {
            return _entities.GetValueOrDefault(id);
        }

        private IActorView CreateView(string prefabPath)
        {
            var prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab != null)
            {
                var instance = GameObject.Instantiate(prefab);
                return instance.GetOrAddComponent<ActorView>();
            }

            return null;
        }
    }
}
