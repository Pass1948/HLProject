using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Range(0,1)] public float bgmVolume = 1f;
    [Range(0,1)] public float sfxVolume = 1f;

    AudioSource bgm;   // 루프용 (BGM)
    AudioSource sfx;   // 단발용 (SFX)

    void Awake()
    {
        // BGM 소스
        var bgmGO = new GameObject("BGM_Source");
        bgmGO.transform.SetParent(transform, false);
        bgm = bgmGO.AddComponent<AudioSource>();
        bgm.playOnAwake = false;
        bgm.loop = true;
        bgm.spatialBlend = 0f;
        bgm.volume = bgmVolume;

        // SFX 소스
        var sfxGO = new GameObject("SFX_Source");
        sfxGO.transform.SetParent(transform, false);
        sfx = sfxGO.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.loop = false;
        sfx.spatialBlend = 0f;
        sfx.volume = sfxVolume;
    }

    // BGM 
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgm.clip == clip && bgm.isPlaying) return; // 같은 곡이면 무시
        bgm.clip = clip;
        bgm.volume = Mathf.Clamp01(bgmVolume);
        bgm.Play();
    }

    public void StopBGM()
    {
        if (!bgm) return;
        bgm.Stop();
        bgm.clip = null;
    }

    // SFX겹침 거의 없으니 OneShot
    public void PlaySfx(AudioClip clip, float volumeMul = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        sfx.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        sfx.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * volumeMul));
    }
}