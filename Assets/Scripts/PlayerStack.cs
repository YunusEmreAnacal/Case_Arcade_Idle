using System.Collections.Generic;
using UnityEngine;

public class PlayerStack : MonoBehaviour
{
    [Header("Stack Ayarlarý")]
    [SerializeField]
    private Transform stackParent;

    [SerializeField]
    private int maxCapacity = 10;

    [Header("Ölçüm Referansý")]
    [Tooltip("Sýrtta taþýnacak item'ýn PREFAB'ý. Yükseklik buna göre hesaplanacak.")]
    [SerializeField]
    private GameObject itemPrefabForMeasuring;

    private Stack<GameObject> stackedItems = new Stack<GameObject>();
    private float measuredItemHeight = 0.3f;

    public bool IsFull => stackedItems.Count >= maxCapacity;
    public bool IsEmpty => stackedItems.Count == 0;

    private void Awake()
    {
        MeasureHeight();
    }

    private void MeasureHeight()
    {
        if (itemPrefabForMeasuring == null)
        {
            Debug.LogWarning("PlayerStack: 'itemPrefabForMeasuring' atanmamýþ! Varsayýlan yükseklik kullanýlýyor.", this);
            return;
        }

        Renderer itemRenderer = itemPrefabForMeasuring.GetComponentInChildren<Renderer>();
        if (itemRenderer != null)
        {
            measuredItemHeight = itemRenderer.bounds.size.y;
        }
        else
        {
            Debug.LogWarning("Item'ýn boyutu ölçülemedi, varsayýlan yükseklik kullanýlýyor.", this);
        }
    }

    public void AddItem(GameObject item)
    {
        if (IsFull)
        {
            Debug.LogWarning("PlayerStack dolu. Item eklenemiyor.");
            Destroy(item);
            return;
        }

        item.transform.SetParent(stackParent);
        float newYPosition = stackedItems.Count * measuredItemHeight;
        item.transform.localPosition = new Vector3(0, newYPosition, 0);
        item.transform.localRotation = Quaternion.identity;

        stackedItems.Push(item);
    }

    public GameObject RemoveItem()
    {
        if (IsEmpty) return null;

        GameObject itemToRemove = stackedItems.Pop();
        itemToRemove.transform.SetParent(null);

        return itemToRemove;
    }
}