using UnityEngine;

public abstract class Slot
{
    public Transform Root;
    public SpriteRenderer Frame;

    public void SetActiveFrame(bool active)
    {
        Frame?.gameObject.SetActive(active);
    }
}
