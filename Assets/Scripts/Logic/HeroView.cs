using TMPro;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Transform _bodyRoot;
    [SerializeField] private TextMeshPro _attackText; // stats
    [SerializeField] private TextMeshPro _healthText;

    private HeroInstance _heroInstance;
    public HeroInstance HeroInstance => _heroInstance;
    private int _slotIdx;
    private Action<Vector2> _onDrag;
    private Func<int, Vector3, bool> _onDropped;

    private bool _isDragging;
    private Vector3 _dragOffset;
    private Vector3 _originalWorldPos;
    private GameObject _bodyInstance;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void Setup(HeroInstance heroInstance, int slotIdx, Action<Vector2> onDrag, Func<int, Vector3, bool> onDropped)
    {
        _heroInstance = heroInstance;
        _slotIdx = slotIdx;
        _onDrag = onDrag;
        _onDropped = onDropped;

        ApplyBody();
        Refresh();
    }

    private void ApplyBody()
    {
        if (_bodyInstance != null)
        {
            Destroy(_bodyInstance);
            _bodyInstance = null;
        }

        var bodyPrefab = _heroInstance.Data.BodyPrefab;
        if (bodyPrefab == null) return;

        _bodyInstance = Instantiate(bodyPrefab, _bodyRoot);
        _bodyInstance.transform.localPosition = Vector3.zero;
        _bodyInstance.transform.localRotation = Quaternion.identity;
    }

    public void Refresh()
    {
        _attackText.text = _heroInstance.CurrAttack.ToString();
        _healthText.text = _heroInstance.CurrHealth.ToString();
    }

    public void Clear()
    {
        if (_bodyInstance != null)
        {
            Destroy(_bodyInstance);
            _bodyInstance = null;
        }

        _heroInstance = null;
        _slotIdx = -1;
        _onDrag = null;
        _onDropped = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
        _originalWorldPos = transform.position;
        var mouseWorldPos = _camera.ScreenToWorldPoint(eventData.position); mouseWorldPos.z = 0f;
        _dragOffset = _originalWorldPos - mouseWorldPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
        var mouseWorldPos = _camera.ScreenToWorldPoint(eventData.position); mouseWorldPos.z = 0f;
        var transferred = _onDropped?.Invoke(_slotIdx, mouseWorldPos) ?? false;
        if (!transferred) transform.position = _originalWorldPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mouseWorldPos = _camera.ScreenToWorldPoint(eventData.position); mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos + _dragOffset;
        _onDrag?.Invoke(eventData.position);
    }
}
