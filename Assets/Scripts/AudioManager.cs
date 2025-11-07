using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }
    // --- Bitiþ ---

    [Header("Ses Kaynaklarý (AudioSources)")]
    [SerializeField]
    private AudioSource musicSource; 

    [SerializeField]
    private AudioSource sfxSource;

    [Header("Ses Klipleri")]
    [SerializeField]
    private AudioClip backgroundMusic;

    [SerializeField]
    private AudioClip cashRegisterSound; 

    [SerializeField]
    private AudioClip truckHonkSound;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayCashSound()
    {
        if (cashRegisterSound != null)
        {
            sfxSource.PlayOneShot(cashRegisterSound);
        }
    }

    public void PlayTruckHonk()
    {
        if (truckHonkSound != null)
        {
            sfxSource.PlayOneShot(truckHonkSound);
        }
    }
}