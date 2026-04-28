using UnityEngine;
using UnityEngine.Audio;

public enum AudioMixerType { Master, BGM, SFX }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init() => Instance = null;

    public AudioMixer audioMixer;
    bool[] isMute = new bool[3];
    float[] audioVolumes = new float[3];

    public AudioSource bgmSource;
    public AudioSource[] sfxSources = new AudioSource[8]; int sfxIndex;

#if UNITY_EDITOR
    private void Reset()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:AudioMixer");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            audioMixer = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>(path);
        }
    }
#endif
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; } Instance = this; //싱글톤
    }

    public void PlayBGM(AudioResource bgmClip)
    {
        if (bgmSource.clip == bgmClip) return;

        bgmSource.resource = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void PlaySfx(AudioResource clip)
    {
        sfxSources[sfxIndex].resource = clip;
        sfxSources[sfxIndex].Play();
        sfxIndex = (sfxIndex + 1) % sfxSources.Length;
    }


    public void SetAudioVolume(AudioMixerType audioMixerType, float volume)
    {
        // 오디오 믹서의 값은 -80 ~ 0까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }

    public void SetAudioMute(AudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType;
        if (!isMute[type]) // 뮤트 
        {
            isMute[type] = true;
            audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
            audioVolumes[type] = curVolume;
            SetAudioVolume(audioMixerType, 0.001f);
        }
        else
        {
            isMute[type] = false;
            SetAudioVolume(audioMixerType, audioVolumes[type]);
        }
    }

    // 버튼 클릭 이벤트에 연결할 함수들
    private void Mute()
    {
        AudioManager.Instance.SetAudioMute(AudioMixerType.BGM);
    }
    private void ChangeVolume(float volume)
    {
        AudioManager.Instance.SetAudioVolume(AudioMixerType.BGM, volume);
    }
}