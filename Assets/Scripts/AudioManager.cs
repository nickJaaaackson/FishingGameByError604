using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    
    [Header("BGM")]
    public AudioSource bgmSource;
    public List<AudioClip> bgmClips;

   
    [Header("SFX One Shot")]
    public AudioSource sfxSource;
    public List<AudioClip> sfxList;   

    private Dictionary<string, AudioClip> sfxDict;

    
    [Header("Ambient - Waves")]
    public AudioSource waveSource;
    public AudioClip waveClip;
    public Vector2 waveInterval = new Vector2(2f, 4f);

    [Header("Ambient - Seagulls")]
    public AudioSource seagullSource;
    public List<AudioClip> seagullClips;
    public Vector2 seagullInterval = new Vector2(5f, 12f);

    private Coroutine waveRoutine;
    private Coroutine seagullRoutine;


   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            
            sfxDict = new Dictionary<string, AudioClip>();
            foreach (var clip in sfxList)
                sfxDict[clip.name] = clip;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartWaveLoop();
        StartSeagullLoop();
        PlayBGM("Village by the Shore");
    }

    public void PlayBGM(string bgmName)
    {
        AudioClip clip = bgmClips.Find(c => c.name == bgmName);
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();


   
    public void PlaySFX(string name, float volume = 0.5f)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip,volume);
        }
        else
        {
            Debug.LogWarning("❗ SFX not found : " + name);
        }
    }


    
    public void StartWaveLoop()
    {
        if (waveRoutine == null)
            waveRoutine = StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            if (waveClip != null)
                waveSource.PlayOneShot(waveClip);

            yield return new WaitForSeconds(Random.Range(waveInterval.x, waveInterval.y));
        }
    }


   
    public void StartSeagullLoop()
    {
        if (seagullRoutine == null)
            seagullRoutine = StartCoroutine(SeagullLoop());
    }

    IEnumerator SeagullLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(seagullInterval.x, seagullInterval.y));

            if (seagullClips.Count > 0)
            {
                var clip = seagullClips[Random.Range(0, seagullClips.Count)];
                seagullSource.pitch = Random.Range(0.95f, 1.05f);
                seagullSource.PlayOneShot(clip);
            }
        }
    }


    
    public void StopAmbient()
    {
        if (waveRoutine != null) StopCoroutine(waveRoutine);
        if (seagullRoutine != null) StopCoroutine(seagullRoutine);
    }
}
