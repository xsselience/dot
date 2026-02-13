using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;   // 主BGM
    public AudioSource ambientSource; // 环境音
    public AudioSource sfxSource;     // 玩家射击、跑步音效等

    [Header("场景音效配置")]
    public SceneAudio[] sceneAudios;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneAudio(scene.name);
    }

    void PlaySceneAudio(string sceneName)
    {
        SceneAudio config = null;
        foreach (var s in sceneAudios)
        {
            if (s.sceneName == sceneName)
            {
                config = s;
                break;
            }
        }

        if (config == null) return;

        // 播放主BGM
        if (config.mainMusic != null)
        {
            musicSource.clip = config.mainMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }

        // 播放环境音
        if (config.ambientMusic != null)
        {
            ambientSource.clip = config.ambientMusic;
            ambientSource.loop = true;
            ambientSource.Play();
        }
        else
        {
            ambientSource.Stop();
        }

        // SFX 不循环，由行为触发
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }
}

[System.Serializable]
public class SceneAudio
{
    public string sceneName;
    public AudioClip mainMusic;
    public AudioClip ambientMusic;
}
