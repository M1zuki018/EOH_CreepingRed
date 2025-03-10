using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    [SerializeField, HighlightIfNull] private GameSettings _gameSettings;
    
    [Header("AudioData")]
    private Dictionary<BGMEnum, AudioClip> _bgmClip = new Dictionary<BGMEnum, AudioClip>();
    private Dictionary<SEEnum, AudioClip> _seClip = new Dictionary<SEEnum, AudioClip>();
    private Dictionary<AmbienceEnum, AudioClip> _ambienceClip = new Dictionary<AmbienceEnum, AudioClip>();
    private Dictionary<VoiceEnum, AudioClip> _voiceClip = new Dictionary<VoiceEnum, AudioClip>();
    
    [Header("AudioSource")]
    private AudioSource _bgmSource; // BGM用
    private AudioSource _ambienceSource; // 環境音用
    private IObjectPool<AudioSource> _seSourcePool; // SE用のAudioSource
    private IObjectPool<AudioSource> _voiceSourcePool; // Voice用のAudioSource
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _bgmClip = AudioLoader.LoadAudioClips<BGMEnum>("Audio/BGM");
        _seClip = AudioLoader.LoadAudioClips<SEEnum>("Audio/SE");
        _ambienceClip = AudioLoader.LoadAudioClips<AmbienceEnum>("Audio/Ambience");
        _voiceClip = AudioLoader.LoadAudioClips<VoiceEnum>("Audio/Voice");
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _bgmSource = CreateAudioSource(AudioType.BGM);
        _ambienceSource = CreateAudioSource(AudioType.Ambience);
        _seSourcePool = CreateAudioSourcePool(AudioType.SE, 3, 100); // SE用のオブジェクトプール初期化
        _voiceSourcePool = CreateAudioSourcePool(AudioType.Voice, 3, 20); // Voice用のオブジェクトプール初期化

        for (int i = 0; i < 3; i++)
        {
            _seSourcePool.Get();
            _voiceSourcePool.Get();
        }
        
        SetVolume("Master", _gameSettings.MasterVolume);
        SetVolume("BGM", _gameSettings.BGMVolume);
        SetVolume("SE", _gameSettings.SEVolume);
        SetVolume("Ambience", _gameSettings.AmbientVolume);
        SetVolume("Voice", _gameSettings.VoiceVolume);
    }
    
    /// <summary>
    /// AudioMixerの音量を調整する
    /// </summary>
    private void SetVolume(string type, float volume)
    {
        float volumeInDb = volume > 0 ? Mathf.Log10(volume) * 20 : -80;
        _mixer.SetFloat($"{type}Volume", volumeInDb);
        
        // Debug用
        _mixer.GetFloat($"{type}Volume", out volume);
        Debug.Log($"{type}Volume: {volume}");
    }
    
    /// <summary>
    /// AudioSourceのオブジェクトプールを作成
    /// </summary>
    private IObjectPool<AudioSource> CreateAudioSourcePool(AudioType type, int defaultCapacity, int maxSize)
    {
        return new ObjectPool<AudioSource>(
            createFunc: () => CreateAudioSource(type),
            actionOnGet: source => source.gameObject.SetActive(true),
            actionOnRelease: source => source.gameObject.SetActive(false),
            actionOnDestroy: Destroy,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    /// <summary>
    /// 新しくGameObjectとAudioSourceを生成する
    /// </summary>
    private AudioSource CreateAudioSource(AudioType type)
    {
        GameObject obj = new GameObject(type.ToString());
        obj.transform.SetParent(transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = _mixer.FindMatchingGroups(type.ToString())[0];
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
        if (_bgmClip.TryGetValue(bgm, out AudioClip clip))
        {
            _bgmSource.gameObject.SetActive(true);
            _bgmSource.clip = clip;
            _bgmSource.Play();   
        }
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    public async UniTask PlaySE(SEEnum se)
    {
        if (_seClip.TryGetValue(se, out AudioClip clip))
        {
            AudioSource source = _seSourcePool.Get();
            source.PlayOneShot(clip);

            await UniTask.WaitForSeconds(clip.length);
        
            _seSourcePool.Release(source);
        }
    }
    
    /// <summary>
    /// 環境音を再生する
    /// </summary>
    public void PlayAmbience(AmbienceEnum ambience)
    {
        if (_ambienceClip.TryGetValue(ambience, out AudioClip clip))
        {
            _ambienceSource.clip = clip;
            _ambienceSource.Play();
        }
    }

    /// <summary>
    /// ボイスを再生する
    /// </summary>
    public async UniTask PlayVoice(VoiceEnum voice)
    {
        if (_voiceClip.TryGetValue(voice, out AudioClip clip))
        {
            AudioSource source = _voiceSourcePool.Get();
            source.PlayOneShot(clip);
        
            await UniTask.WaitForSeconds(clip.length);
        
            _voiceSourcePool.Release(source);
        }
    }
}
