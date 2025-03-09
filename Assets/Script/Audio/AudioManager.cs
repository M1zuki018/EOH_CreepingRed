using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Audioを管理するManagerクラス
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AudioMixer")] 
    [SerializeField, HighlightIfNull] private AudioMixer _mixer;
    
    [Header("AudioData")]
    private Dictionary<BGMEnum, AudioClip> _bgmClip = AudioLoader.LoadAudioClips<BGMEnum>("Audio/BGM");
    private Dictionary<SEEnum, AudioClip> _seClip = AudioLoader.LoadAudioClips<SEEnum>("Audio/SE");
    private Dictionary<AmbienceEnum, AudioClip> _ambienceClip = AudioLoader.LoadAudioClips<AmbienceEnum>("Audio/Ambience");
    private Dictionary<VoiceEnum, AudioClip> _voiceClip = AudioLoader.LoadAudioClips<VoiceEnum>("Audio/Voice");
    
    [Header("AudioSource")]
    private AudioSource _bgmSource; // BGM用
    private AudioSource _ambienceSource; // 環境音用
    private List<AudioSource> _seSource; // SE用
    private List<AudioSource>  _voiceSource; // ボイス用
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    public void PlayBGM(BGMEnum bgm)
    {
        _bgmSource.clip = _bgmClip[bgm];
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    public void PlaySE(SEEnum se)
    {
        _seSource[0].clip = _seClip[se];
    }
    
    /// <summary>
    /// 環境音を再生する
    /// </summary>
    public void PlayAmbience(AmbienceEnum ambience)
    {
        _ambienceSource.clip = _ambienceClip[ambience];
    }

    /// <summary>
    /// ボイスを再生する
    /// </summary>
    public void PlayVoice(VoiceEnum voice)
    {
        _voiceSource[0].clip = _voiceClip[voice];
    }
}
