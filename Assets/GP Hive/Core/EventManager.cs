using UnityEngine;

namespace GPHive.Core
{
    public class EventManager : MonoBehaviour
    {
        public delegate void OnLevelStart();
        public static event OnLevelStart LevelStarted;

        public delegate void OnLevelSuccess();
        public static event OnLevelSuccess LevelSuccessed;

        public delegate void OnLevelFail();
        public static event OnLevelFail LevelFailed;

        public delegate void OnLevelRestart();
        public static event OnLevelRestart EnteredNextLevel;

        public delegate void OnNextLevelCreated();
        public static event OnNextLevelCreated NextLevelCreated;


        public static void StartLevel(int level)
        {
            LevelStarted?.Invoke();
        }

        public static void SuccessLevel(int level)
        {
            LevelSuccessed?.Invoke();
        }

        public static void FailLevel(int level)
        {
            LevelFailed?.Invoke();
        }

        public static void NextLevel(int level)
        {
            EnteredNextLevel?.Invoke();
        }

        public static void CreateNextLevel()
        {
            NextLevelCreated?.Invoke();
        }
    }
}
