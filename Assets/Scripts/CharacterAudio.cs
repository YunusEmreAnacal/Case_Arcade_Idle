using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterAudio : MonoBehaviour
{
    [Header("Ses Klipleri")]
    [SerializeField]
    private AudioClip pickupSound; 

    [SerializeField]
    private AudioClip dropSound; 

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }


    public void PlayDropSound()
    {
        if (dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
    }
}