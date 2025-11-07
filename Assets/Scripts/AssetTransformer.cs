using System.Collections;
using UnityEngine;


public class AssetTransformer : MonoBehaviour
{
    [Header("Dönüþüm Ayarlarý")]
    [SerializeField]
    private GameObject outputPrefab; // Dönüþüm sonrasý oluþacak item (Çatý Kiremiti)

    [SerializeField]
    private float processingTime = 2.0f; // Bir item'ý dönüþtürme süresi 

    [Header("Depo Referanslarý")]
    [SerializeField]
    private GridDepot inputDepot; // GÝRÝÞ deposu

    [SerializeField]
    private GridDepot outputDepot; // ÇIKIÞ deposu 

    private void Start()
    {
        if (inputDepot == null || outputDepot == null || outputPrefab == null)
        {
            Debug.LogError("AssetTransformer: Depo referanslarý veya Output Prefab atanmamýþ!", this);
            return;
        }

        // Dönüþüm döngüsünü baþlat
        StartCoroutine(ProcessLoop());
    }

    private IEnumerator ProcessLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => !inputDepot.IsEmpty && !outputDepot.IsFull);

            // 2. Girdi deposundan bir item al ve yok et
            GameObject inputItem = inputDepot.RemoveItem();
            if (inputItem != null)
            {
                Destroy(inputItem);
            }

            yield return new WaitForSeconds(processingTime);

            // 4. Yeni "dönüþmüþ" item'ý oluþtur
            GameObject outputItem = Instantiate(outputPrefab);

            outputDepot.AddItem(outputItem);
        }
    }
}