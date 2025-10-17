using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    [Range(0, 10)] public int sfxVoices = 4;
    
    private AudioSource bgm;
    private List<AudioSource> sfxV;
    private int sfxIndex;

    private void Awake()
    {
        
        // sfx 보이스를 여러 개 만드는 초기화 루프
        sfxV = new List<AudioSource>(sfxVoices);
        var go = new GameObject($"SFX_Voice_");
        go.transform.SetParent(bgm.transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.volume = Mathf.Clamp01(bgmVolume);
        sfxV.Add(src);
    }

    //BGM 플레이
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        StopBGM();
        var bgmGO = new GameObject("BGM");
        bgmGO.transform.SetParent(transform);
        bgm = bgmGO.AddComponent<AudioSource>();
        bgm.clip = clip;
        bgm.playOnAwake = false;
        bgm.loop = true;
        bgm.volume = Mathf.Clamp01(bgmVolume);
        bgm.Play();
    }
    //BGM 멈춰
    private void StopBGM()
    {
        if(bgm == null) return;
        Destroy(bgm.gameObject);
        bgm.Stop();
    }


    public void PlaySfx(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) return;
        var srcGo = new GameObject("SFX");
        var src = srcGo.AddComponent<AudioSource>();
        src.transform.SetParent(transform);
        src.transform.localPosition = Vector3.zero;
        src.spatialBlend = 0;
        src.pitch = pitch;
        src.clip = clip;
        src.volume = Mathf.Clamp01(sfxVolume);
        src.Play();
    }
    

}











