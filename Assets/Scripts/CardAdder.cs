using UnityEngine;
using UnityEngine.EventSystems;

public class CardAdder : MonoBehaviour, IPointerClickHandler
{
    private CardBehaviour cardBehaviour;
    private CardsContainer cardContainer;

    private bool added = false;

    private void Start()
    {
        cardBehaviour = GetComponent<CardBehaviour>();
        cardContainer = GameObject.Find("CardsContainer").GetComponent<CardsContainer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (added) return;

        if(cardBehaviour != null && cardContainer != null)
        {
            cardContainer.AddCard(cardBehaviour);
            added = true;
        }
    }
}
