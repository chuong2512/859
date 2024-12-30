using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip Clip;

    public float Volume;
    public float Pitch;
    

    public override void Play(AudioSource source)
    {
        if (Clip == null) return;

        source.clip = Clip;
        source.volume = Volume;
        source.pitch = Pitch;

        source.Play();
    }
}