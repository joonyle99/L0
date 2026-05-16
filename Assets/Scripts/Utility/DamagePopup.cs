using TMPro;
using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TextMeshPro))]
public class DamagePopup : MonoBehaviour
{
    private static readonly float DURATION = 0.75f;
    private static readonly float MOVE_X_MIN = 0.25f;
    private static readonly float MOVE_X_MAX = 0.35f;
    private static readonly float MOVE_Y_MIN = 0.7f;
    private static readonly float MOVE_Y_MAX = 0.8f;
    private static readonly float SCALE_PUNCH = 1.15f;
    private static readonly float SCALE_UP_DURATION = 0.08f;
    private static readonly float SCALE_DOWN_DURATION = 0.08f;
    private static readonly float FADE_DELAY = 0.3f;

    [SerializeField] private Color _colorNormal;
    [SerializeField] private Color _colorCritical;

    private TextMeshPro _text;
    private Sequence _sequence;
    private Action _onRelease;

    private Transform _target;
    private Vector3 _offset;

    public void Initialize()
    {
        _text = GetComponent<TextMeshPro>();
        _text.sortingLayerID = SortingLayer.NameToID("VFX");
    }

    public void SetReleaseAction(Action onRelease) => _onRelease = onRelease;

    public void Play(int amount, Transform target, bool isCritical)
    {
        _target = target;
        _offset = Vector3.zero;

        transform.localScale = Vector3.one;
        transform.position = _target.position;

        _text.text = NumberFormatter.FormatDamage(amount);
        var baseColor = isCritical ? _colorCritical : _colorNormal;
        _text.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        var dirX = Random.value < 0.5f ? -1f : 1f;
        var endOffset = new Vector3(
            dirX * Random.Range(MOVE_X_MIN, MOVE_X_MAX),
            Random.Range(MOVE_Y_MIN, MOVE_Y_MAX),
            0f
        );

        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Insert(0f, DOTween.To(OffsetGetter, OffsetSetter, endOffset, DURATION).SetEase(Ease.OutQuad));
        _sequence.Insert(0f, transform.DOScale(Vector3.one * SCALE_PUNCH, SCALE_UP_DURATION).SetEase(Ease.OutBack));
        _sequence.Insert(SCALE_UP_DURATION, transform.DOScale(Vector3.one, SCALE_DOWN_DURATION).SetEase(Ease.InSine));
        _sequence.Insert(FADE_DELAY, _text.DOFade(0f, DURATION - FADE_DELAY).SetEase(Ease.InQuad));
        _sequence.OnComplete(() =>
        {
            _target = null;
            _sequence = null;
            _onRelease?.Invoke();
        });
    }

    private void Update()
    {
        if (_target != null)
        {
            transform.position = _target.position + _offset;
        }
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
        _target = null;
    }

    private Vector3 OffsetGetter()
    {
        return _offset;
    }
    private void OffsetSetter(Vector3 offset)
    {
        _offset = offset;
    }
}
