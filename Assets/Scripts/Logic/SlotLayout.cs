using UnityEngine;

public class SlotLayout : MonoBehaviour
{
    [SerializeField] private float _spacing = 1.5f;

#if UNITY_EDITOR
    [ContextMenu("Arrange")]
#endif

    public void Arrange()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        float totalWidth = (childCount - 1) * _spacing;
        float startX = (-1) * totalWidth / 2f;

        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            child.localPosition = new Vector3(startX + i * _spacing, 0f, 0f);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Arrange();
    }
#endif
}