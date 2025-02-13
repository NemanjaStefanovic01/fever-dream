using UnityEngine;
using UnityEngine.EventSystems;

public class CardAdder : MonoBehaviour, IPointerClickHandler
{
    private UICardBehaviour cardBehaviour;
    private CardsContainer cardContainer;

    private bool added = false;

    private void Start()
    {
        cardBehaviour = GetComponent<UICardBehaviour>();
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
