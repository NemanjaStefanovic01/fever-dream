using UnityEngine;

public class UICardVisual : MonoBehaviour
{
    [HideInInspector]private RectTransform rectTransform;
    [HideInInspector] public UICardBehaviour uiCard;

    private Vector3 originalScale;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        originalScale = rectTransform.localScale;
    }

    private void Update()
    {
        rectTransform.position = uiCard.GetComponent<RectTransform>().position;
        rectTransform.rotation = uiCard.GetComponent<RectTransform>().rotation;

        if (uiCard.transform.parent.name == "CardsContainer")
        {
            if (uiCard.isHovered || uiCard.isDraging)
            {
                rectTransform.localScale = originalScale * 1.1f;
            }
            else
            {
                rectTransform.localScale = originalScale;
            }
        }
    }
}
