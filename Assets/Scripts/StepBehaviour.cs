using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StepBehaviour : MonoBehaviour
    {

        [SerializeField] private AudioSource sourceStep;
        [SerializeField] private List<AudioClip> _tileAudioClips;
        [SerializeField] private List<AudioClip> _waterAudioClips;

        private AudioClip _currentSource;
        private List<AudioClip> _audioClipList;

        public void Step()
        {
            if (_audioClipList != default)
            {
                _currentSource = _audioClipList[Random.Range(0, _audioClipList.Count)];
                sourceStep.PlayOneShot(_currentSource);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Floor"))
            {
                _audioClipList = _tileAudioClips;
            }
            
            if (hit.gameObject.CompareTag("Water"))
            {
                _audioClipList = _waterAudioClips;
            }
        }
    }


