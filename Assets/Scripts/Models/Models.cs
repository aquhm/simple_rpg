using System;
using System.Collections.Generic;
using Client.Core;

namespace Client.Model
{
    public class Models
    {
        private readonly Dictionary<Type, IModel> _models = new();

        public readonly InputStateModel InputStateModel = new();

        private bool _isInitialized;

        public IEnumerable<IModel> All
        {
            get
            {
                yield return InputStateModel;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            InitializeModels();
            _isInitialized = true;
        }


        private void InitializeModels()
        {
            foreach (var model in All)
            {
                model.Initialize();
            }
        }

        public void Reset()
        {
            foreach (var model in All)
            {
                model.Reset();
            }
        }

        public void Release()
        {
            foreach (var model in All)
            {
                model.Release();
            }

            _isInitialized = false;
        }
    }
}
