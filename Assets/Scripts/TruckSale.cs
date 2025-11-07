using System.Collections;
using UnityEngine;

/// <summary>
/// Kamyonun deposunu (GridDepot) yönetir.
/// Depo dolduðunda item'larý "satar" (yok eder), para verir ve boþalýr.
/// </summary>
public class TruckSale : MonoBehaviour
{
    [Header("Satýþ Ayarlarý")]
    [SerializeField]
    private int pricePerItem = 5; // Her "Çatý Kiremiti" için kazanýlacak para

    [SerializeField]
    private float saleAnimationTime = 2.0f; // Kamyonun gidip gelmesini simüle eden süre

    [Header("Referanslar")]
    [SerializeField]
    private GridDepot depot; // Kamyonun kasasý olan GridDepot

    private bool isSelling = false; // Zaten satýþta mý?

    private void Update()
    {
        // Eðer depo doluysa VE zaten satýþ iþlemi baþlamamýþsa
        if (depot.IsFull && !isSelling)
        {
            StartCoroutine(SellItems());
        }
    }

    /// <summary>
    /// Satýþ iþlemini, animasyonu ve depo boþaltmayý simüle eder.
    /// </summary>
    private IEnumerator SellItems()
    {
        isSelling = true;

        // 1. Parayý Hesapla
        int itemsSold = depot.CurrentItemCount;
        int moneyEarned = itemsSold * pricePerItem;

        // TODO: UI Manager'a parayý ekle
        Debug.Log($"KAMYON DOLDU! {itemsSold} item satýldý. Kazanýlan para: {moneyEarned}");

        // 2. Satýþ Animasyonunu Simüle Et (Kamyonun gitmesi)
        // Gerçek oyunda burada kamyonu hareket ettiririz.
        yield return new WaitForSeconds(saleAnimationTime);

        // 3. Depoyu Boþalt
        while (!depot.IsEmpty)
        {
            // Depodaki tüm item'larý tek tek alýp yok et
            GameObject item = depot.RemoveItem();
            if (item != null)
            {
                Destroy(item);
            }
        }

        Debug.Log("Satýþ tamamlandý. Yeni kamyon hazýr.");

        // 4. Yeni satýþ için hazýr ol
        isSelling = false;
    }
}