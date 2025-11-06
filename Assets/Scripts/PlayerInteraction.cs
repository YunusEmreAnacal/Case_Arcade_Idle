using UnityEngine;

[RequireComponent(typeof(PlayerStack))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Etkileþim Ayarlarý")]
    [SerializeField]
    private float transferInterval = 0.1f;

    private PlayerStack playerStack;
    private float transferTimer = 0f;

    private int heldItemType = 0;

    private void Awake()
    {
        playerStack = GetComponent<PlayerStack>();
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
        if (transferTimer < transferInterval)
        {
            return;
        }

        // Girdiðimiz alanda bir GridDepot var mý?
        if (other.TryGetComponent<GridDepot>(out GridDepot depot))
        {
            string tag = other.tag;
            if (tag == "SpawnerDepot")
            {
                // Sadece sýrtýmýz boþsa VEYA zaten Kiremit taþýyorsak al
                if (!playerStack.IsFull && !depot.IsEmpty && (heldItemType == 0 || heldItemType == 1))
                {
                    GameObject item = depot.RemoveItem();
                    if (item != null)
                    {
                        playerStack.AddItem(item);
                        heldItemType = 1; // Artýk Kiremit taþýyoruz
                        transferTimer = 0f;
                    }
                }
            }

            else if (tag == "InputDepot")
            {
                // Sadece Kiremit taþýyorsak ve depo dolu deðilse
                if (!playerStack.IsEmpty && !depot.IsFull && heldItemType == 1)
                {
                    GameObject item = playerStack.RemoveItem();
                    if (item != null)
                    {
                        depot.AddItem(item);
                        transferTimer = 0f;
                    }
                }
            }
            else if (tag == "OutputDepot")
            {
                // Sadece sýrtýmýz boþsa VEYA zaten Çatý Kiremiti taþýyorsak
                if (!playerStack.IsFull && !depot.IsEmpty && (heldItemType == 0 || heldItemType == 2))
                {
                    GameObject item = depot.RemoveItem();
                    if (item != null)
                    {
                        playerStack.AddItem(item);
                        heldItemType = 2; // Artýk Çatý Kiremiti taþýyoruz
                        transferTimer = 0f;
                    }
                }
            }
            else if (tag == "TruckDepot")
            {
                if (!playerStack.IsEmpty && !depot.IsFull && heldItemType == 2)
                {
                    GameObject item = playerStack.RemoveItem();
                    if (item != null)
                    {
                        depot.AddItem(item);
                        transferTimer = 0f;
                    }
                }
            }
        }
    }
}