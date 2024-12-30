using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GPHive.Core
{
    public enum GameState
    {
        Idle,
        Playing,
        End
    }

    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private TextMeshProUGUI levelText;

        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject loseScreen;

        [SerializeField] private float OpenFinalUIAfterTime;

        private GameState gameState;
        [SerializeField] private SwerveControllerConfig swerveControllerConfig;
        public Transform player;
        public GameState GameState => gameState;

        private void Start()
        {
            Application.targetFrameRate = 60;
            SetLevelText();
        }

        private void SetLevelText()
        {
            levelText.SetText($"LEVEL {SaveLoadManager.GetLevel()}");
        }

        public void StartLevel()
        {
            swerveControllerConfig.DefoultSpeed();
            EventManager.StartLevel(SaveLoadManager.GetLevel());

            gameState = GameState.Playing;
        }

        public void NextLevel()
        {
            EventManager.NextLevel(SaveLoadManager.GetLevel());
            StartCoroutine(CO_NextLevel());
        }

        private IEnumerator CO_NextLevel()
        {
            Destroy(LevelManager.Instance.ActiveLevel);
            yield return new WaitForEndOfFrame();
            LevelManager.Instance.ActiveLevel = LevelManager.Instance.CreateLevel();

            gameState = GameState.Idle;

            SetLevelText();
        }

        /// <summary>
        /// Call when level is successfully finished.
        /// </summary>
        public void WinLevel()
        {
            EventManager.SuccessLevel(SaveLoadManager.GetLevel());
            SaveLoadManager.IncreaseLevel();
            PlayerPrefs.SetInt("conceptLevel", PlayerPrefs.GetInt("conceptLevel", 0) + 1);

            gameState = GameState.End;

            StartCoroutine(CO_OpenUIDelayed(winScreen));
        }

        /// <summary>
        /// Call when level is failed.
        /// </summary>
        public void LoseLevel()
        {
            EventManager.FailLevel(SaveLoadManager.GetLevel());

            gameState = GameState.End;

            StartCoroutine(CO_OpenUIDelayed(loseScreen));
        }

        private IEnumerator CO_OpenUIDelayed(GameObject UI)
        {
            yield return BetterWaitForSeconds.Wait(OpenFinalUIAfterTime);
            UI.SetActive(true);
        }
    }
}