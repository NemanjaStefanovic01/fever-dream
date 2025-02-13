using UnityEditor.Rendering;
using UnityEngine;

public class CardsVisualiser : MonoBehaviour
{
    private UICardVisual hoveredCard;
    private UICardVisual dragedCard;
    public CardsContainer cardsContainer;

    private int cardsOriginalSiblingIndex;

    private void Update()
    {
        if (hoveredCard == null && dragedCard == null)
        {
            SetDefaultDisplayOrder();
        }

        // Reorder on drag or hover
        if (hoveredCard != null)
        {
            hoveredCard.transform.SetAsLastSibling();
        }
        if(dragedCard != null)
        {
            dragedCard.transform.SetAsLastSibling();
        }
    }

    public void SetHoveredCardsVisual(UICardBehaviour uiCard)
    {
        if (uiCard == null)
        {
            hoveredCard = null;
            return;
        }

        for (int i = 0; i < this.transform.childCount; i++)
        {
            UICardVisual cardVisual = this.transform.GetChild(i).GetComponent<UICardVisual>();

            if (cardVisual.uiCard == uiCard)
            {
                hoveredCard = cardVisual;
            }
        }
    }
    public void SetDragedCardsVisual(UICardBehaviour uiCard)
    {
        if (uiCard == null)
        {
            dragedCard = null;
            return;
        }

        for (int i = 0; i < this.transform.childCount; i++)
        {
            UICardVisual cardVisual = this.transform.GetChild(i).GetComponent<UICardVisual>();

            if (cardVisual.uiCard == uiCard)
            {
                dragedCard = cardVisual;
            }
        }
    }

    public void SetDefaultDisplayOrder()
    {
        Debug.Log("hi");
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform cardVisual = this.transform.GetChild(i);

            UICardBehaviour card = cardVisual.GetComponent<UICardVisual>().uiCard;
            RectTransform cardSlot = null;

            if (card != null)
            {
                cardSlot = card.cardSlot;
            }

            if (cardSlot != null)
            {
                cardVisual.SetSiblingIndex(cardsContainer.cardSlots.IndexOf(cardSlot));
            }
        }
    }

    public void SetCardOriginalSiblingIndex(UICardBehaviour card)
    {
        if (card == null) return;

        RectTransform cardSlot = card.cardSlot;

        cardsOriginalSiblingIndex = cardSlot.GetSiblingIndex();
    }

    public void SetCardToOriginalSiblingIndex(UICardBehaviour card)
    {
        if (card == null) return;

        for(int i = 0; i < this.transform.childCount; i++)
        {
            Transform cardVisual = this.transform.GetChild(i);
            if (cardVisual.GetComponent<UICardVisual>().uiCard == card)
            {
                cardVisual.transform.SetSiblingIndex(cardsOriginalSiblingIndex);
            }
        }
    }
}
