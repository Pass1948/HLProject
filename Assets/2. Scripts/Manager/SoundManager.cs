using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioClip defaultBgm;
    
    public float bgmVolume;
    public float sfxVolume;

    [Range(0, 10)] public int sfxVoices = 4;
    
    private AudioSource bgm;
    private List<AudioSource> sfxV;
    private int sfxIndex;

    private void Awake()
    {
        var bgmGO = GameManager.Resource.Create<GameObject>(Path.Sound +"BGM");
        bgmGO.transform.SetParent(transform);
        bgm = bgmGO.GetComponent<AudioSource>();
        
        bgm.playOnAwake = false;
        bgm.loop = true;
        bgm.volume = Mathf.Clamp01(bgmVolume);
        
        bgm.clip = defaultBgm;
        bgm.Play();

        for (int i = 0; i < sfxVoices; i++)
        {
            var go = new GameObject($"SFX_Voice_{i}");
            go.transform.SetParent(bgm.transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.volume = Mathf.Clamp01(bgmVolume);
            sfxV.Add(src);
        }
    }

    //BGM 플레이
    public void PlayerBGM(AudioClip clip)
    {
        if (clip == null) return;
        bgm.clip = clip;
        bgm.volume = Mathf.Clamp01(bgm.volume);
        bgm.Play();
    }
    //BGM 멈춰
    public void StopBGM() => bgm.Stop();
    

    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        
    }
}
