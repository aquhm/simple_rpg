using System.Collections.Generic;
using UnityEngine;

namespace Client.Actor
{
    public class ActorAnimationHandler : IAnimationHandler
    {
        private readonly Animator _animator;
        private readonly Dictionary<string, int> _hashCache = new();

        public ActorAnimationHandler(Animator animator)
        {
            _animator = animator;
        }

        public void SetBool(string name, bool value)
        {
            _animator.SetBool(GetHash(name), value);
        }

        public void SetTrigger(string name)
        {
            _animator.SetTrigger(GetHash(name));
        }

        public void SetInteger(string name, int value)
        {
            _animator.SetInteger(GetHash(name), value);
        }

        public void SetLayerWeight(int layerIndex, float weight)
        {
            _animator.SetLayerWeight(layerIndex, weight);
        }

        public void SetFloat(string name, float value)
        {
            _animator.SetFloat(GetHash(name), value);
        }

        private int GetHash(string name)
        {
            if (!_hashCache.ContainsKey(name)) _hashCache[name] = Animator.StringToHash(name);
            return _hashCache[name];
        }
    }
}