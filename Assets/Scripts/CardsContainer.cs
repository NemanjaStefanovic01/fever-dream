using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CardsContainer : MonoBehaviour
{
    [Header("Container Settings")]
    [SerializeField] private float heightPaddingModifyer = 1.5f;
    RectTransform rectTransform;
    private float containerHeight;
    public int numOfSlots;
    private CardBehaviour hoveredCard;
    private CardBehaviour dragedCard;
    private bool isCrossing;

    [Header("Slot Settings")]
    public GameObject cardSlotPrefab;
    public float slotWidth = 200f;
    public float slotHeight = 280f;
    [SerializeField]private List<RectTransform> cardSlots = new List<RectTransform>();

    [Header("Card Settings")]
    [SerializeField]private List<CardBehaviour> cards = new List<CardBehaviour>();

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
                float pos = -(cardSlots.Count/2 * 200) + 100 + i*200;
                cardSlots[i].anchoredPosition = new Vector2(pos, 0);
            }
        }
        else //Odd num of card slots
        {
            for (int i = 0; i < cardSlots.Count; i++)
            {
                float pos = -((cardSlots.Count - 1) * 100) + i * 200;
                cardSlots[i].anchoredPosition = new Vector2(pos, 0);
            }
        }
    }

    public void AddCard(CardBehaviour card)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);

        cards.Add(card);

        card.transform.SetParent(this.transform);

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
        slotRect.sizeDelta = new Vector2(slotWidth, this.GetComponent<RectTransform>().sizeDelta.y);
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

    // Listener functions
    private void CardPointerEnter(CardBehaviour card)
    {
        hoveredCard = card;
    }
    private void CardPointerExit(CardBehaviour card)
    {
        hoveredCard = null;
    }

    private void BeginDrag(CardBehaviour card)
    {
        dragedCard = card;
    }
    private void EndDrag(CardBehaviour card)
    {
        dragedCard = null;
    }
}
