using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioSource audioSource; // 负责播放背景音乐的音频源
    public List<AudioClip> bgms; // 背景音乐列表
    public int nowPlaying = -1; // 当前播放的背景音乐索引
    private IEnumerator fadeInCoroutine; // 用于控制淡入的协程

    // Update is called once per frame
    void Update()
    {
        if (nowPlaying == -1)
        {
            if (audioSource.isPlaying) audioSource.Stop(); // 停止当前播放
            return;
        }
        // 检查是否正在播放，若停止则播放当前音乐
        if (!audioSource.isPlaying && bgms.Count > 0)
        {
            PlayCurrentBGM();
        }
        int v = GlobalVar.instance.globalExposureValue;
        int half = GlobalVar.instance.maxGlobalExposureValue / 2;
        int _3quarter = half / 2 + half;
        audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp(1 - (float)(v+5 - _3quarter) / ((float)_3quarter / 3),0,1), 5*Time.deltaTime);
    }

    private void PlayCurrentBGM()
    {
        if (bgms.Count == 0 || nowPlaying < 0) return; // 没有音频时直接返回
        nowPlaying %= bgms.Count; // 防止索引超出范围
        audioSource.clip = bgms[nowPlaying]; // 设置当前音频片段
        audioSource.loop = false; // 禁止音频源自带循环
        audioSource.Play(); // 开始播放
    }
    public void func()
    {
        StartCoroutine(FadeAndSwitchBGM((nowPlaying + 2)%4-1, 1f));
    }
    
    
    private IEnumerator FadeAndSwitchBGM(int nextIndex, float fadeDuration)
    {
        Debug.Log(1);
        if (audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = startVolume; // 重置音量
        }

        // 切换并播放新的音频
        nowPlaying = nextIndex;
        PlayCurrentBGM();
    }

}
