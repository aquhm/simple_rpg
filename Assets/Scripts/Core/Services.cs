using System;
using System.Collections.Generic;

namespace Client.Service
{
    public class Services
    {
        private readonly Dictionary<Type, IService> _services = new();

        private bool _isInitialized;

        public ActorService ActorService { get; private set; }
        public InputService InputService { get; private set; }
        public CameraService CameraService { get; private set; }


        public void Initialize()
        {
            if (_isInitialized) return;

            RegisterServices();
            InitializeServices();
            _isInitialized = true;
        }

        private void RegisterServices()
        {
            InputService = RegisterService(new InputService());
            CameraService = RegisterService(new CameraService());
            ActorService = RegisterService(new ActorService());
        }

        private T RegisterService<T>(T service) where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var value) == false)
            {
                _services[typeof(T)] = service;
                return service;
            }

            return (T)value;
        }

        private void InitializeServices()
        {
            foreach (var service in _services.Values) service.Initialize();
        }

        public void DoUpdate(float deltaTime)
        {
            foreach (var service in _services.Values) service.DoUpdate(deltaTime);
        }

        public void Release()
        {
            foreach (var service in _services.Values) service.Release();
            _services.Clear();
        }

        public T GetService<T>() where T : class, IService
        {
            return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
        }

        public void EnableService<T>() where T : class, IService
        {
            var service = GetService<T>();
            if (service is { IsActive: false }) service.Initialize();
        }

        public void DisableService<T>() where T : class, IService
        {
            var service = GetService<T>();
            if (service is { IsActive: true }) service.Release();
        }
    }
}