using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Level Config", menuName = "GP Hive Objects/Level Settings/Level Config", order = 1)]
public class LevelConfig : ScriptableObject
{
    public bool changeSkybox;
    [ShowIf("changeSkybox")] public Material skybox;

    public bool enableVolume;
    [ShowIf("enableVolume")] public Volume volume;
    [ShowIf("enableVolume")] public VolumeProfile volumeProfile;

    public bool fog;
    [ShowIf("fog")] public FogMode fogMode;
    [ShowIf("fog")] public Color fogColor;
    [ShowIf("fog")] public float fogDensity, fogStart, fogEnd;
    
    public bool ChangeBuildColors;
    [ShowIf("ChangeBuildColors")] public Material BuildMat;
    [ShowIf("ChangeBuildColors")] public Color PlatformColor;
    [ShowIf("ChangeBuildColors")] public Texture2D PlatAtlas;
}
