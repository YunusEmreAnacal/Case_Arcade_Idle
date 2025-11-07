using System.Collections;
using UnityEngine;

public class TruckSale : MonoBehaviour
{
    [Header("Satýþ Ayarlarý")]
    [SerializeField]
    private int pricePerItem = 5; 

    [SerializeField]
    private float saleAnimationTime = 2.0f;

    [Header("Referanslar")]
    [SerializeField]
    private GridDepot depot; 

    private bool isSelling = false; 

    private void Update()
    {
        if (depot.IsFull && !isSelling)
        {
            StartCoroutine(SellItems());
        }
    }

    private IEnumerator SellItems()
    {
        isSelling = true;

        int itemsSold = depot.CurrentItemCount;
        int moneyEarned = itemsSold * pricePerItem;

        Debug.Log($"KAMYON DOLDU! {itemsSold} item satýldý. Kazanýlan para: {moneyEarned}");

        yield return new WaitForSeconds(saleAnimationTime);

        while (!depot.IsEmpty)
        {
            GameObject item = depot.RemoveItem();
            if (item != null)
            {
                Destroy(item);
            }
        }

        Debug.Log("Satýþ tamamlandý. Yeni kamyon hazýr.");
        isSelling = false;
    }
}