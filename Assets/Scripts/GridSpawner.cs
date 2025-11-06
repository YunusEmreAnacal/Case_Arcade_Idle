using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridDepot))] 
public class GridSpawner : MonoBehaviour
{
    [Header("Üretim Ayarlarý")]
    [SerializeField]
    private GameObject itemPrefab; // Üretilecek item (Kiremit prefab'ý)

    [SerializeField]
    private float spawnInterval = 0.5f;

    private GridDepot depot;

    private void Awake()
    {
  
        depot = GetComponent<GridDepot>();

        if (itemPrefab == null)
        {
            Debug.LogError("GridSpawner: 'itemPrefab' atanmamýþ!");
        }
    }

    private void Start()
    {
        if (itemPrefab != null)
        {
            StartCoroutine(SpawnLoop());
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Depo dolana kadar bekle 
            yield return new WaitWhile(() => depot.IsFull);

            // Yeni item'ý oluþtur
            GameObject newItem = Instantiate(itemPrefab);


            depot.AddItem(newItem);

            // Bir sonraki üretim için bekle
            yield return new WaitForSeconds(spawnInterval);

            // Boþalýnca döngü baþa döner ve WaitWhile'ý geçer 
        }
    }

}