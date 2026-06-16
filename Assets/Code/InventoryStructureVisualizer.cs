using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryStructureVisualizer : MonoBehaviour
{
    private class InventoryItem
    {
        public int ItemId { get; }
        public string ItemName { get; }
        public Color SlotColor { get; }

        public InventoryItem(int itemId, string itemName, Color slotColor)
        {
            ItemId = itemId;
            ItemName = itemName;
            SlotColor = slotColor;
        }
    }

    [Header("Inventory")]
    [Tooltip("인벤토리에 들어갈 수 있는 최대 슬롯 수입니다.")]
    [SerializeField] private int maxSlotCount = 8;

    [Tooltip("Scene 뷰에 그릴 슬롯 한 칸의 크기입니다.")]
    [SerializeField] private float slotSize = 0.8f;

    [Tooltip("슬롯 사이의 간격입니다.")]
    [SerializeField] private float slotGap = 0.15f;

    private readonly List<InventoryItem> inventory = new List<InventoryItem>();

    private readonly Dictionary<int, int> slotIndexByItemId = new Dictionary<int, int>();

    private int nextItemId = 1000;
    private int selectedSlotIndex;
    private int highlightedItemId = -1;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            AddItem();
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            RemoveSelectedItem();
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            selectedSlotIndex--;
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            selectedSlotIndex++;
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            HighlightNewestItemByDictionary();
        }

        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, Mathf.Max(0, inventory.Count - 1));
    }

    private void AddItem()
    {
        if (inventory.Count >= maxSlotCount)
        {
            return;
        }

        int itemId = nextItemId;
        nextItemId++;

        string itemName = "Item " + itemId;
        Color slotColor = GetColorBySlot(inventory.Count);
        InventoryItem item = new InventoryItem(itemId, itemName, slotColor);

        inventory.Add(item);
        slotIndexByItemId[item.ItemId] = inventory.Count - 1;

        selectedSlotIndex = inventory.Count - 1;
        highlightedItemId = item.ItemId;
    }

    private void RemoveSelectedItem()
    {
        if (inventory.Count == 0)
        {
            return;
        }

        InventoryItem removedItem = inventory[selectedSlotIndex];
        inventory.RemoveAt(selectedSlotIndex);
        slotIndexByItemId.Remove(removedItem.ItemId);

        RebuildDictionary();
        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, Mathf.Max(0, inventory.Count - 1));
        highlightedItemId = -1;
    }

    private void HighlightNewestItemByDictionary()
    {
        int newestItemId = nextItemId - 1;

        if (slotIndexByItemId.TryGetValue(newestItemId, out int slotIndex))
        {
            selectedSlotIndex = slotIndex;
            highlightedItemId = newestItemId;
        }
    }

    private void RebuildDictionary()
    {
        slotIndexByItemId.Clear();

        for (int i = 0; i < inventory.Count; i++)
        {
            slotIndexByItemId[inventory[i].ItemId] = i;
        }
    }

    private Color GetColorBySlot(int index)
    {
        Color[] colors =
        {
            new Color(0.2f, 0.7f, 1f),
            new Color(0.3f, 0.9f, 0.45f),
            new Color(1f, 0.75f, 0.25f),
            new Color(0.9f, 0.45f, 1f)
        };

        return colors[index % colors.Length];
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < maxSlotCount; i++)
        {
            Vector3 slotPosition = transform.position + Vector3.right * i * (slotSize + slotGap);

            bool hasItem = Application.isPlaying && i < inventory.Count;

            Gizmos.color = hasItem ? inventory[i].SlotColor : Color.gray;
            Gizmos.DrawCube(slotPosition, Vector3.one * slotSize);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(slotPosition, Vector3.one * slotSize);

            if (!Application.isPlaying || !hasItem)
            {
                continue;
            }

            if (i == selectedSlotIndex)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(slotPosition, Vector3.one * (slotSize + 0.18f));
            }

            if (inventory[i].ItemId == highlightedItemId)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(slotPosition + Vector3.up * 0.65f, 0.18f);
            }
        }
    }
}