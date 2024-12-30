using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GPHive.Core
{
    public class LevelSettings : MonoBehaviour
    {
        public LevelConfig levelConfig;
        [SerializeField] private MeshRenderer[] Builds;
        [SerializeField] private Material PlatAtlas;
        [SerializeField] private List<LevelConfig> levelConsepts = new();
        public YourBest yourBest;

        private void OnEnable()
        {
            yourBest.player = GameManager.Instance.player;
            yourBest.StartMeth();

            var conceptLevel = PlayerPrefs.GetInt("conceptLevel", 0);
            if (conceptLevel < 3)
            {
                levelConfig = levelConsepts[0];
            }
            else if (conceptLevel < 6)
            {
                levelConfig = levelConsepts[1];
            }
            else if (conceptLevel < 9)
            {
                levelConfig = levelConsepts[2];
            }
            else
            {
                PlayerPrefs.SetInt("conceptLevel", 0);
                levelConfig = levelConsepts[0];
            }


            if (levelConfig.enableVolume)
                FindObjectOfType<Volume>().profile = levelConfig.volumeProfile;


            if (levelConfig.changeSkybox)
                RenderSettings.skybox = levelConfig.skybox;

            RenderSettings.fog = levelConfig.fog;

            if (levelConfig.fog)
            {
                RenderSettings.fogColor = levelConfig.fogColor;

                if (levelConfig.fogMode == FogMode.Linear)
                {
                    RenderSettings.fogStartDistance = levelConfig.fogStart;
                    RenderSettings.fogEndDistance = levelConfig.fogEnd;
                }
                else
                {
                    RenderSettings.fogDensity = levelConfig.fogDensity;
                }
            }


            if (levelConfig.ChangeBuildColors)
            {
                foreach (var _build in Builds) _build.material = levelConfig.BuildMat;

                PlatAtlas.mainTexture = levelConfig.PlatAtlas;
                PlatAtlas.color = levelConfig.PlatformColor;
            }
        }

        private void Reset()
        {
            levelConfig.fogMode = FogMode.Linear;
        }
    }
}