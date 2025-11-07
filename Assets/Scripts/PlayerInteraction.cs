using UnityEngine;

[RequireComponent(typeof(PlayerStack))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Etkileþim Ayarlarý")]
    [SerializeField]
    private float transferInterval = 0.1f;
    private CharacterAudio myAudio;
    private PlayerStack playerStack;
    private float transferTimer = 0f;

    private int heldItemType = 0;

    private void Awake()
    {
        playerStack = GetComponent<PlayerStack>();
        myAudio = GetComponent<CharacterAudio>();
    }

    private void Update()
    {
        if (transferTimer < transferInterval)
        {
            transferTimer += Time.deltaTime;
        }

        // Sýrtýmýz boþaldýysa item tipini sýfýrla
        if (playerStack.IsEmpty)
        {
            heldItemType = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (transferTimer < transferInterval) return;

        string tag = other.tag;

        // --- 5. ÇÖP KUTUSU ---
        if (tag == "TrashDepot")
        {
            if (!playerStack.IsEmpty)
            {
                GameObject itemToTrash = playerStack.RemoveItem();
                myAudio.PlayPickupSound();
                if (itemToTrash != null)
                {
                    Destroy(itemToTrash);  
                    transferTimer = 0f;
                }
                return;
            }
        }

        // Depo gerektiren diðer etkileþimler
        if (other.TryGetComponent<GridDepot>(out GridDepot depot))
        {
            // 1. SPANWER'DAN ALMA
            if (tag == "SpawnerDepot")
            {
                if (!playerStack.IsFull && !depot.IsEmpty && (heldItemType == 0 || heldItemType == 1))
                {
                    GameObject item = depot.RemoveItem();
                    myAudio.PlayPickupSound();
                    if (item != null)
                    {
                        playerStack.AddItem(item);
                        heldItemType = 1;
                        transferTimer = 0f;
                    }
                }
            }
            // 2. TRANSFORMER GÝRÝÞÝNE BIRAKMA
            else if (tag == "InputDepot")
            {
                if (!playerStack.IsEmpty && !depot.IsFull && heldItemType == 1)
                {
                    GameObject item = playerStack.RemoveItem();
                    myAudio.PlayPickupSound();
                    if (item != null) { depot.AddItem(item); transferTimer = 0f; }
                }
            }
            // 3. TRANSFORMER ÇIKIÞINDAN ALMA
            else if (tag == "OutputDepot")
            {
                if (!playerStack.IsFull && !depot.IsEmpty && (heldItemType == 0 || heldItemType == 2))
                {
                    GameObject item = depot.RemoveItem();
                    myAudio.PlayPickupSound();
                    if (item != null)
                    {
                        playerStack.AddItem(item);
                        myAudio.PlayPickupSound();
                        heldItemType = 2;
                        transferTimer = 0f;
                    }
                }
            }
            // 4. KAMYONA BIRAKMA
            else if (tag == "TruckDepot")
            {
                if (!playerStack.IsEmpty && !depot.IsFull && heldItemType == 2)
                {
                    GameObject item = playerStack.RemoveItem();
                    myAudio.PlayPickupSound();
                    if (item != null) { depot.AddItem(item); transferTimer = 0f; }
                }
            }
        }
    }
}