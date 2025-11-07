using UnityEngine;

/// <summary>
/// Karakterin (Player veya AI) item alma ve býrakma seslerini çalmasýný saðlar.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CharacterAudio : MonoBehaviour
{
    [Header("Ses Klipleri")]
    [SerializeField]
    private AudioClip pickupSound; // Item alma (sýrtýna koyma) sesi

    [SerializeField]
    private AudioClip dropSound; // Item býrakma (depoya koyma) sesi

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Item alma sesini çalar.
    /// </summary>
    public void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }

    /// <summary>
    /// Item býrakma sesini çalar.
    /// </summary>
    public void PlayDropSound()
    {
        if (dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
    }
}