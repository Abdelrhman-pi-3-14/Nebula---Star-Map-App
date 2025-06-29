using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIBlocker : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetBlocking(false);
    }

    public void SetBlocking(bool shouldBlock)
    {
        canvasGroup.blocksRaycasts = shouldBlock;
        canvasGroup.alpha = shouldBlock ? 0.5f : 0f; // Optional visual feedback
    }
}
