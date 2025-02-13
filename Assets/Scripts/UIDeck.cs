using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UIDeck : MonoBehaviour
{
    [SerializeField]
    private GameObject uiCardPrefab;
    [SerializeField]
    private GameObject cardVisualPrefab;

    private List<UICardBehaviour> uiCards = new List<UICardBehaviour>();
    private List<UICardVisual> cardsVisuals = new List<UICardVisual>();

    private void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject newUiCard = Instantiate(uiCardPrefab, transform);
            GameObject newCardVisual = Instantiate(cardVisualPrefab, transform);

            newCardVisual.GetComponent<UICardVisual>().uiCard = newUiCard.GetComponent<UICardBehaviour>();
            newCardVisual.transform.SetParent(GameObject.Find("CardsVisualizer").transform);
        } 
    }
}
