using UnityEngine;
using UnityEngine.UI;

public class AIUnlockPad : MonoBehaviour
{
    [Header("Satýn Alma Ayarlarý")]
    [SerializeField]
    private float totalCost = 200; 

    [SerializeField]
    private int moneyPerTick = 5; 

    [SerializeField]
    private float tickInterval = 0.05f; 

    [Header("Görsel Referanslar")]
    [Tooltip("Dediðin o 'yeþil dolan' UI Image'ý")]
    [SerializeField]
    private Image fillImage;

    [Header("Spawning")]
    [SerializeField]
    private GameObject aiWorkerPrefab;  

    [SerializeField]
    private Transform spawnPoint; 

    private float currentAmount = 0;
    private float tickTimer = 0f;
    private bool isPlayerOnPad = false;
    private bool isUnlocked = false;

    private void Start()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 0;
        }
    }

    private void Update()
    {
        if (!isUnlocked && isPlayerOnPad)
        {
            // Zamanlayýcýyý artýr
            tickTimer += Time.deltaTime;

            // Zamaný geldiyse para harcamayý dene
            if (tickTimer >= tickInterval)
            {
                // Para harcamayý dene
                if (MoneyManager.Instance.SpendMoney(moneyPerTick))
                {
                    // Baþarýlý!
                    currentAmount += moneyPerTick;
                    UpdateFillAmount(); // Görseli güncelle
                }

                tickTimer = 0f; // Zamanlayýcýyý sýfýrla
            }
        }
    }

    private void UpdateFillAmount()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentAmount / totalCost;
        }

        if (currentAmount >= totalCost && !isUnlocked)
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        isUnlocked = true;
        Debug.Log("AI Ýþçi Kilidi Açýldý!");
        Instantiate(aiWorkerPrefab, spawnPoint.position, spawnPoint.rotation);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPad = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPad = false;
        }
    }
}