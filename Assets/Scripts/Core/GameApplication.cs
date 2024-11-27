using Client.Model;
using Client.Service;
using UnityEditor;
using UnityEngine;

namespace Client.Core
{
    public class GameApplication : MonoBehaviour
    {
        [SerializeField] public GameObject LocalActor;

        [Header("Animation Controllers")]
        [SerializeField] private RuntimeAnimatorController _normalStateController;

        [SerializeField] private RuntimeAnimatorController _combatStateController;
        [SerializeField] private Avatar _characterAvatar;

        // 애니메이션 컨트롤러 접근자
        public RuntimeAnimatorController NormalStateController
        {
            get => _normalStateController;
        }

        public RuntimeAnimatorController CombatStateController
        {
            get => _combatStateController;
        }

        public Avatar CharacterAvatar
        {
            get => _characterAvatar;
        }

        public static GameApplication Instance { get; private set; }

        public Services Services { get; } = new();
        public Models Models { get; } = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Initialize();
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void Update()
        {
            Services.DoUpdate(Time.deltaTime);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
            }
        }

        private void OnApplicationQuit()
        {
        }

        private void Initialize()
        {
            // 본인에 대한 설정
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Models.Initialize();
            Services.Initialize();

            Application.wantsToQuit += WantsToQuit;
            Application.quitting += OnQuitting;
        }

        private void Release()
        {
            Services.Release();
            Models.Release();
        }

        private bool WantsToQuit()
        {
            return true;
        }

        private void OnQuitting()
        {
            Release();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                EditorApplication.isPlaying = false;
            }
            else
#endif
            {
                Application.Quit();
            }
        }
    }
}
