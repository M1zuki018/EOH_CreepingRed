using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

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
    
    private IObjectPool<AudioSource> _seSourcePool;
    private IObjectPool<AudioSource> _voiceSourcePool;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // SE用のオブジェクトプール初期化
        _seSourcePool = new ObjectPool<AudioSource>(
            createFunc: CreateAudioSource,
            actionOnGet: source => source.gameObject.SetActive(true),
            actionOnRelease: source => source.gameObject.SetActive(false),
            actionOnDestroy: Destroy,
            defaultCapacity: 5,
            maxSize: 100);
        
        // Voice用のオブジェクトプールの初期化
        _voiceSourcePool = new ObjectPool<AudioSource>(
            createFunc: CreateAudioSource,
            actionOnGet: source => source.gameObject.SetActive(true),
            actionOnRelease: source => source.gameObject.SetActive(false),
            actionOnDestroy: Destroy,
            defaultCapacity: 5,
            maxSize: 20);
    }

    /// <summary>
    /// オブジェクトプール用
    /// 新しくGameObjectとAudioSourceを生成する
    /// </summary>
    private AudioSource CreateAudioSource()
    {
        GameObject obj = new GameObject("PooledAudioSource");
        obj.transform.SetParent(transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        obj.SetActive(false);
        return source;
    }

    /// <summary>
    /// SEのオブジェクトプールからAudioSourceを取得する
    /// </summary>
    public AudioSource GetSEAudioSource() => _seSourcePool.Get();
    
    /// <summary>
    /// VoiceのオブジェクトプールからAudioSourceを取得する
    /// </summary>
    public AudioSource GetVoiceAudioSource() => _voiceSourcePool.Get();

    /// <summary>
    /// SEのオブジェクトプールから引数で渡したAudioSourceを解除する
    /// </summary>
    public void SESourceRelease(AudioSource source) => _seSourcePool.Release(source);
    
    /// <summary>
    /// Voiceのオブジェクトプールから引数で渡したAudioSourceを解除する
    /// </summary>
    public void VoiceSourceRelease(AudioSource source) => _voiceSourcePool.Release(source);


    /// <summary>
    /// BGMを再生する
    /// </summary>
    public void PlayBGM(BGMEnum bgm)
    {
        _bgmSource.clip = _bgmClip[bgm];
        _bgmSource.Play();
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
