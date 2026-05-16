using TMPro;
using System;
using UnityEngine;

public abstract class SlotController : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Vector2 _dropSize = Vector2.one;
    [SerializeField] private Vector2 _offset = Vector2.zero;

    protected Action<Vector2> onDrag;
    protected Func<int, Vector3, bool> onDropped;

    protected Slot[] slots;

    private Vector2 _dropHalfSize;
    private Camera _mainCamera;
    private int _lastHoveredIdx = -1;

    protected void InitializeSlots(int capacity, Action<Vector2> onDrag, Func<int, Vector3, bool> onDropped)
    {
        this.onDrag = onDrag;
        this.onDropped = onDropped;

        slots = new Slot[capacity];

        _dropHalfSize = _dropSize * 0.5f;
        _mainCamera = Camera.main;

        for (int i = 0; i < capacity; i++)
        {
            var slotInstance = Instantiate(_slotPrefab, transform);
            var slot = CreateSlot();
            slot.Root = slotInstance.transform;
#if UNITY_EDITOR
            var label = slot.Root.Find("Label")?.GetComponentInChildren<TextMeshPro>(true);
            if (label != null)
            {
                label.gameObject.SetActive(true);
                label.text = i.ToString();
            }
#endif
            slot.Frame = slot.Root.Find("Frame")?.GetComponentInChildren<SpriteRenderer>(true);
            slots[i] = slot;
        }

        if (TryGetComponent<SlotLayout>(out var slotLayout))
        {
            slotLayout.Arrange();
        }
    }

    protected abstract Slot CreateSlot();

    public virtual bool IsSlotEmpty(int idx) => false;

    protected abstract void Refresh();

    public int GetSlotIndexAtWorldPos(Vector3 worldPos)
    {
        for (int idx = 0; idx < slots.Length; idx++)
        {
            var diffVector = (Vector2)slots[idx].Root.position + _offset - (Vector2)worldPos;
            var withinX = Mathf.Abs(diffVector.x) <= _dropHalfSize.x;
            var withinY = Mathf.Abs(diffVector.y) <= _dropHalfSize.y;

            if (withinX && withinY)
            {
                return idx;
            }
        }

        return -1;
    }

    /// <summary>
    /// ...
    /// </summary>
    public void TryHoverAt(Vector2 screenPos, bool excludeEmpty = false)
    {
        var worldPos = (Vector3)_mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        var currHoverIdx = GetSlotIndexAtWorldPos(worldPos);

        if (excludeEmpty)
        {
            if (currHoverIdx != -1 && IsSlotEmpty(currHoverIdx))
            {
                currHoverIdx = -1;
            }
        }

        // 같은 슬롯에서의 드래그는 무시
        if (currHoverIdx == _lastHoveredIdx) return;

        // 이전에 호버되고 있던 슬롯이 있었다면 해당 슬롯을 Exit
        if (_lastHoveredIdx != -1)
        {
            slots[_lastHoveredIdx]?.SetActiveFrame(false);
        }

        // 호버 인덱스 갱신
        _lastHoveredIdx = currHoverIdx;

        // 새롭게 호버된 슬롯이 있다면 해당 슬롯을 Enter
        if (_lastHoveredIdx != -1)
        {
            slots[_lastHoveredIdx]?.SetActiveFrame(true);
        }
    }

    public void ClearHover()
    {
        if (_lastHoveredIdx == -1) return;

        slots[_lastHoveredIdx]?.SetActiveFrame(false);
        _lastHoveredIdx = -1;
    }

    private void OnDrawGizmos()
    {
        if (slots == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.4f);

        foreach (var slot in slots)
        {
            if (slot != null)
            {
                Gizmos.DrawWireCube(slot.Root.position + (Vector3)_offset, _dropSize);
            }
        }
    }
}
