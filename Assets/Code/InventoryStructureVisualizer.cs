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

    // List는 슬롯 순서를 그대로 보관합니다. 0번, 1번, 2번처럼 인덱스로 접근할 수 있습니다.
    private readonly List<InventoryItem> inventory = new List<InventoryItem>();

    // Dictionary는 ItemId를 Key로 사용해 해당 아이템이 몇 번째 슬롯에 있는지 빠르게 찾습니다.
    private readonly Dictionary<int, int> slotIndexByItemId = new Dictionary<int, int>();

    private int nextItemId = 1000;
    private int selectedSlotIndex;
    private int highlightedItemId = -1;

    private void Update()
    {
        // Keyboard.current는 현재 연결된 키보드 장치를 가져오는 Input System 프로퍼티입니다.
        if (Keyboard.current == null)
        {
            return;
        }

        // wasPressedThisFrame은 이번 프레임에 막 눌린 순간에만 true가 됩니다.
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

        // Mathf.Clamp는 선택 번호가 0보다 작거나 마지막 슬롯을 넘지 않도록 막습니다.
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

        // Add는 List의 맨 뒤에 새 데이터를 추가합니다.
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
        // RemoveAt은 지정한 인덱스의 데이터를 제거하고, 뒤쪽 데이터를 앞으로 당깁니다.
        inventory.RemoveAt(selectedSlotIndex);
        slotIndexByItemId.Remove(removedItem.ItemId);

        RebuildDictionary();
        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, Mathf.Max(0, inventory.Count - 1));
        highlightedItemId = -1;
    }

    private void HighlightNewestItemByDictionary()
    {
        int newestItemId = nextItemId - 1;

        // Dictionary는 Key가 있는지 빠르게 확인하고, 있으면 슬롯 번호를 바로 가져올 수 있습니다.
        // TryGetValue는 Key가 있으면 true를 반환하고, 찾은 Value를 out 변수에 넣어 줍니다.
        if (slotIndexByItemId.TryGetValue(newestItemId, out int slotIndex))
        {
            selectedSlotIndex = slotIndex;
            highlightedItemId = newestItemId;
        }
    }

    private void RebuildDictionary()
    {
        // Clear는 Dictionary 안에 들어 있던 Key-Value 쌍을 모두 지웁니다.
        slotIndexByItemId.Clear();

        for (int i = 0; i < inventory.Count; i++)
        {
            // List의 현재 순서를 기준으로 Dictionary의 슬롯 번호를 다시 맞춥니다.
            slotIndexByItemId[inventory[i].ItemId] = i;
        }
    }

    private Color GetColorBySlot(int index)
    {
        // 같은 색만 반복되면 슬롯 구분이 어려우므로 몇 가지 색을 번갈아 사용합니다.
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
        // OnDrawGizmos는 Scene 뷰에 개발용 시각 표시를 그릴 때 사용하는 Unity 메시지 메서드입니다.
        for (int i = 0; i < maxSlotCount; i++)
        {
            Vector3 slotPosition = transform.position + Vector3.right * i * (slotSize + slotGap);

            // Application.isPlaying은 현재 Play 모드인지 확인하는 프로퍼티입니다.
            bool hasItem = Application.isPlaying && i < inventory.Count;

            // Gizmos.DrawCube는 Scene 뷰에 색이 채워진 정육면체를 그립니다.
            Gizmos.color = hasItem ? inventory[i].SlotColor : Color.gray;
            Gizmos.DrawCube(slotPosition, Vector3.one * slotSize);

            // DrawWireCube는 채워지지 않은 테두리 상자를 그립니다.
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