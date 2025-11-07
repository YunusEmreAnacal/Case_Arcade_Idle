using System.Collections;
using UnityEngine;

public class TruckManager : MonoBehaviour
{
    [Header("Kamyon Ayarlarý")]
    [SerializeField]
    private GameObject truckPrefab; // GridDepot'lu kamyon prefab'ý
    [SerializeField]
    private float truckSpeed = 10f;

    [Header("Satýþ Ayarlarý")]
    [SerializeField]
    private int pricePerItem = 5;

    [Header("Konumlar")]
    [SerializeField]
    private Transform spawnPoint; // Kamyonun doðacaðý yer (ekran dýþý)
    [SerializeField]
    private Transform sellPoint; // Kamyonun durup yükleneceði yer
    [SerializeField]
    private Transform despawnPoint; // Kamyonun gidip kaybolacaðý yer (ekran dýþý)

    private GridDepot currentTruckDepot;
    private bool isSelling = false;

    private void Start()
    {
        // Baþlangýçta bir kamyon getir
        StartCoroutine(SpawnNewTruck(spawnPoint.position, sellPoint.position));
    }

    private void Update()
    {
        // Eðer bir kamyon varsa, dolu mu diye kontrol et
        if (currentTruckDepot != null && currentTruckDepot.IsFull && !isSelling)
        {
            StartCoroutine(SellAndReplaceTruck());
        }
    }

    private IEnumerator SellAndReplaceTruck()
    {
        isSelling = true;

        // 1. Para kazan
        int itemsSold = currentTruckDepot.CurrentItemCount;
        int moneyEarned = itemsSold * pricePerItem;
        Debug.Log($"KAMYON DOLDU! {itemsSold} item satýldý. Kazanýlan para: {moneyEarned}");

        // 2. Dolu kamyonu gönder (Despawn)
        GameObject oldTruck = currentTruckDepot.transform.parent.gameObject; // GridDepot'un parent'ý kamyondur
        yield return MoveTruck(oldTruck.transform, sellPoint.position, despawnPoint.position);

        // 3. Eski kamyonu ve içindekileri yok et
        while (!currentTruckDepot.IsEmpty)
        {
            Destroy(currentTruckDepot.RemoveItem());
        }
        Destroy(oldTruck);

        // 4. Yeni kamyonu getir (Spawn)
        yield return SpawnNewTruck(spawnPoint.position, sellPoint.position);

        isSelling = false;
    }

    private IEnumerator SpawnNewTruck(Vector3 from, Vector3 to)
    {
        GameObject newTruckObj = Instantiate(truckPrefab, from, spawnPoint.rotation);
        currentTruckDepot = newTruckObj.GetComponentInChildren<GridDepot>();
        if (currentTruckDepot == null)
        {
            Debug.LogError("TruckManager: Kamyon prefab'ýnýn child'ýnda GridDepot bulunamadý!");
            yield break;
        }

        yield return MoveTruck(newTruckObj.transform, from, to);
    }

    private IEnumerator MoveTruck(Transform truck, Vector3 from, Vector3 to)
    {
        float timer = 0f;
        float duration = Vector3.Distance(from, to) / truckSpeed;

        while (timer < duration)
        {
            truck.position = Vector3.Lerp(from, to, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        truck.position = to; // Tam olarak hedefe oturt
    }
}