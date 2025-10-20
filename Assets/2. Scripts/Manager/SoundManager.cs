using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Range(0,1)] public float masterVolume = 1f;
    [Range(0,1)] public float bgmVolume = 1f;
    [Range(0,1)] public float sfxVolume = 1f;

    AudioSource bgm;   // 루프용 (BGM)
    AudioSource sfx;   // 단발용 (SFX)
    
    private AudioClip uiClip;
    private AudioClip cardClip;

    void Awake()
    {
        uiClip = GameManager.Resource.Load<AudioClip>(Path.Sound + "Laptop_Keystroke_82");
        cardClip = GameManager.Resource.Load<AudioClip>(Path.Sound + "Laptop_Keystroke_82");
        ApplyVolumes();
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

    void ApplyVolumes()
    {
        if (bgm != null)
            bgm.volume = Mathf.Clamp01(bgmVolume * masterVolume);
        if (sfx != null)
            sfx.volume = Mathf.Clamp01(sfxVolume * masterVolume);
    }
    
    // 마스터
    public void SetMasterVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        ApplyVolumes();
    }
    // 브금
    public void SetBgmVolume(float v)
    {
        bgmVolume = Mathf.Clamp01(v);
        ApplyVolumes();
    }
    // 효과음
    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        ApplyVolumes();
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
    // BGM 멈추고 싶을 때
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
        sfx.volume = sfxVolume;
        sfx.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        sfx.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * volumeMul));
    }

    public void PlayUISfx()
    {
        sfx.clip = cardClip;
        if (sfx.clip == null || cardClip == null) return;
        sfx.volume = sfxVolume;
        sfx.pitch = Mathf.Clamp(sfx.pitch, 0.1f, 3f);
        sfx.PlayOneShot(sfx.clip, Mathf.Clamp01(sfx.pitch));
    }

    public void PlayCardSelectSfx()
    {
        sfx.clip = GameManager.Resource.Load<AudioClip>(Path.Sound + "");
        if (sfx.clip == null) return;
        sfx.pitch = Mathf.Clamp(sfx.pitch, 0.1f, 3f);
        sfx.volume = sfxVolume;
        sfx.PlayOneShot(sfx.clip, Mathf.Clamp01(sfx.pitch));
    }

    public void PlayShopSfx()
    {
        sfx.clip = uiClip;
        if (sfx.clip == null || uiClip == null) return;
        sfx.pitch = Mathf.Clamp(sfx.pitch, 0.1f, 3f);
        sfx.volume = sfxVolume;
        sfx.PlayOneShot(sfx.clip, Mathf.Clamp01(sfx.pitch));
    }
}






