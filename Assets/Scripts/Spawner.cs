using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridDepot))]
public class Spawner : MonoBehaviour
{
    [Header("Üretim Ayarlarý")]
    [SerializeField]
    private GameObject itemPrefab; // Üretilecek item (Kiremit prefab'ý)

    [SerializeField]
    private float spawnInterval = 1.0f; // Saniyede bir item

    [Header("Referanslar")]
    [SerializeField]
    private Transform spawnPoint; // Item'ýn nerede doðacaðý

    private GridDepot depot;

    private void Awake()
    {
        depot = GetComponent<GridDepot>();

        if (spawnPoint == null)
        {
            spawnPoint = this.transform;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("Spawner'a üretilecek 'itemPrefab' atanmamýþ!", this);
        }
        else if (itemPrefab.GetComponent<StackableItem>() == null)
        {
            // GridDepot ve PlayerStack'in çalýþmasý için bu þart
            Debug.LogError("Atanan 'itemPrefab' bir 'StackableItem' component'ine sahip deðil!", itemPrefab);
        }
    }

    private void Start()
    {
        if (itemPrefab != null)
        {
            // Üretim döngüsünü baþlat
            StartCoroutine(SpawnLoop());
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {

            yield return new WaitWhile(() => depot.IsFull);

            ProduceItem();

            yield return new WaitForSeconds(spawnInterval);

        }
    }

    private void ProduceItem()
    {
        // Prefab'ý oluþtur
        GameObject newItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);


        bool added = depot.AddItem(newItem);

        if (!added)
        {
            Debug.LogWarning("Depo doluyken item üretilmeye çalýþýldý. Item yok ediliyor.");
            Destroy(newItem);
        }
    }
}