using System.Collections.Generic;
using UnityEngine;

public class GridDepot : MonoBehaviour
{
    [Header("Grid Ayarlarý")]
    [SerializeField]
    private Transform depotParent;

    [SerializeField]
    private int columns = 3;

    [SerializeField]
    private int rows = 2;

    [SerializeField]
    private int maxLayers = 10;

    [Header("Ölçüm Referansý")]
    [Tooltip("Bu depoda saklanacak item'ýn PREFAB'ý. Boyutlar buna göre hesaplanacak.")]
    [SerializeField]
    private GameObject itemPrefabForMeasuring; // YENÝ EKLENDÝ

    // --- Dahili Deðiþkenler ---
    private Stack<GameObject> storedItems = new Stack<GameObject>();
    private List<Vector3> slotPositions = new List<Vector3>();
    private Vector3 measuredItemSize;
    private int maxCapacity;

    // --- Public Özellikler ---
    public bool IsFull => storedItems.Count >= maxCapacity;
    public bool IsEmpty => storedItems.Count == 0;
    public int CurrentItemCount => storedItems.Count;

    private void Awake()
    {
        if (depotParent == null)
        {
            depotParent = this.transform;
        }
        maxCapacity = columns * rows * maxLayers;

        MeasureAndCalculateSlots();
    }

    public bool AddItem(GameObject item)
    {
        if (IsFull)
        {
            Debug.LogWarning("Depo dolu, item eklenemiyor!");
            return false;
        }

        item.transform.localScale = itemPrefabForMeasuring.transform.localScale;

        item.transform.SetParent(depotParent);
        item.transform.localPosition = GetNextSlotPosition();
        item.transform.localRotation = Quaternion.identity;

        storedItems.Push(item);
        return true;
    }

    public GameObject RemoveItem()
    {
        if (IsEmpty) return null;

        GameObject itemToRemove = storedItems.Pop();
        itemToRemove.transform.SetParent(null);

        return itemToRemove;
    }

    private void MeasureAndCalculateSlots() // Parametre kaldýrýldý
    {
        if (itemPrefabForMeasuring == null)
        {
            Debug.LogError("GridDepot: 'itemPrefabForMeasuring' atanmamýþ! Ölçüm yapýlamýyor.", this);
            return;
        }

        Renderer itemRenderer = itemPrefabForMeasuring.GetComponentInChildren<Renderer>();
        if (itemRenderer == null)
        {
            Debug.LogError("GridDepot: Prefab'in Renderer'ý bulunamadý.", this);
            measuredItemSize = Vector3.one;
        }
        else
        {
            measuredItemSize = itemRenderer.bounds.size;
        }

        CalculateSlotPositions();
    }

    private void CalculateSlotPositions()
    {
        slotPositions.Clear();
        Vector3 startOffset = new Vector3(
            -measuredItemSize.x * (columns - 1) / 2.0f,
            0,
            -measuredItemSize.z * (rows - 1) / 2.0f
        );

        for (int l = 0; l < maxLayers; l++)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    float x = c * measuredItemSize.x;
                    float y = l * measuredItemSize.y;
                    float z = r * measuredItemSize.z;
                    slotPositions.Add(startOffset + new Vector3(x, y, z));
                }
            }
        }
    }

    private Vector3 GetNextSlotPosition()
    {
        return slotPositions[storedItems.Count];
    }
}