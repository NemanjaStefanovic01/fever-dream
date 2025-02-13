using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class CardsContainer : MonoBehaviour
{
    [Header("Container Settings")]
    [SerializeField] private float heightPaddingModifyer = 1.5f;
    RectTransform rectTransform;
    private float containerHeight;
    private UICardBehaviour hoveredCard;
    private UICardBehaviour dragedCard;
    private bool isCrossing;
    
    public int numOfSlots;
    public CardsVisualiser cardVisualiser;

    [Header("Slot Settings")]
    public GameObject cardSlotPrefab;
    public float slotWidth = 200f;
    public float slotHeight = 280f;
    [HideInInspector]public List<RectTransform> cardSlots = new List<RectTransform>();

    [Header("Card Settings")]
    [HideInInspector]public List<UICardBehaviour> cards = new List<UICardBehaviour>();

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetContainerDim();        
    }

    
    void Update()
    {
        SetContainerDim(); // Set container dimensions based on cards slots in card slots list
        SetSlotPositions(); // Set position of each slot based on container width

        CheckForCardSlotSwitch();

        // Update the visualiser
        cardVisualiser.SetHoveredCardsVisual(hoveredCard);
        cardVisualiser.SetDragedCardsVisual(dragedCard);
    }

    private void SetContainerDim()
    {
        containerHeight = slotHeight * heightPaddingModifyer;

        float newWidth = cardSlots.Count * slotWidth;
        rectTransform.sizeDelta = new Vector2(newWidth, containerHeight);
    }
    private void SetSlotPositions()
    {
        if(cardSlots.Count % 2 == 0) //Even num of card slots
        {
            for(int i = 0; i <  cardSlots.Count; i++)
            {
                float pos = -(cardSlots.Count/2 * slotWidth) + (slotWidth/2) + i * slotWidth;
                cardSlots[i].anchoredPosition = new Vector2(pos, 0);
            }
        }
        else //Odd num of card slots
        {
            for (int i = 0; i < cardSlots.Count; i++)
            {
                float pos = -((cardSlots.Count - 1) * (slotWidth / 2)) + i * slotWidth;
                cardSlots[i].anchoredPosition = new Vector2(pos, 0);
            }
        }
    }

    public void AddCard(UICardBehaviour card)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);

        card.rectTransform.sizeDelta = new Vector2(slotWidth, slotHeight);

        cards.Add(card);

        card.transform.SetParent(this.transform);
        card.freeCard = false;

        AddCardSlot();
        SetContainerDim();
        SetSlotPositions();

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].cardSlot = cardSlots[i];
        }

        //foreach (CardBehaviour aCard in cards)
        //{
        //    Debug.Log(aCard.name + ": " + aCard.transform.GetSiblingIndex());
        //}
    }
    private void AddCardSlot()
    {
        GameObject slot = Instantiate(cardSlotPrefab, transform);
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(slotWidth, slotHeight);
        cardSlots.Add(slotRect);
    }

    private void CheckForCardSlotSwitch()
    {
        if (dragedCard == null) return;

        if (isCrossing) return;

        for (int i = 0; i < cards.Count; i++)
        {
            if(dragedCard.transform.position.x > cards[i].transform.position.x)
            { 
                if (dragedCard.GetCardSlotsSiblingIndex() < cards[i].GetCardSlotsSiblingIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if(dragedCard.transform.position.x < cards[i].transform.position.x)
            {
                if(dragedCard.GetCardSlotsSiblingIndex() > cards[i].GetCardSlotsSiblingIndex())
                {
                    Swap(i);
                    
                    break;
                }
            }
        }
    }

    private void Swap(int index)
    {
        isCrossing = true;

        // Swap card slots
        RectTransform tempSlot = dragedCard.cardSlot;
        dragedCard.cardSlot = cards[index].cardSlot;
        cards[index].cardSlot = tempSlot;
        
        // Swap positions within cards List
        int draggedIndex = cards.IndexOf(dragedCard);
        int targetIndex = index;

        if (draggedIndex != -1 && targetIndex != -1)
        {
            (cards[draggedIndex], cards[targetIndex]) = (cards[targetIndex], cards[draggedIndex]);
        }

        isCrossing = false;
    }

    public void RemoveCardFromContainer(UICardBehaviour card)
    {
        cards.Remove(card);
        card.freeCard = true;

        cardSlots.Remove(card.cardSlot);
        
        Destroy(card.cardSlot.gameObject);

        dragedCard = null;
    }

    // Listener functions
    private void CardPointerEnter(UICardBehaviour card)
    {
        hoveredCard = card;

        cardVisualiser.SetDefaultDisplayOrder();
        cardVisualiser.SetCardOriginalSiblingIndex(card);
    }
    private void CardPointerExit(UICardBehaviour card)
    {
        hoveredCard = null;

        cardVisualiser.SetCardToOriginalSiblingIndex(card);
    }

    private void BeginDrag(UICardBehaviour card)
    {
        dragedCard = card;

        foreach(UICardBehaviour cardBehaviour in cards)
        {
            if(cardBehaviour != card)
                cardBehaviour.canvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
    }
    private void EndDrag(UICardBehaviour card)
    {
        dragedCard = null;

        foreach (UICardBehaviour cardBehaviour in cards)
        {
            if (cardBehaviour != card)
                cardBehaviour.canvas.GetComponent<GraphicRaycaster>().enabled = true;
        }

        cardVisualiser.SetDefaultDisplayOrder();
    }
}
