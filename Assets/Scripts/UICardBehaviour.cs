using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class UICardBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Canvas canvas; // Use it to enable and disabel RayCast for all obj -> canvas.GetComponent<GraphicRaycaster>().enabled = false;
    [HideInInspector] public Image imageComponent; // Enabel disable Raycast for specific img -> imageComponent.raycastTarget = false;
    [HideInInspector]public RectTransform rectTransform;
    public RectTransform cardSlot;
    private RectTransform cardsContainer;
    [HideInInspector] public bool freeCard;

    //States
    public bool isDraging;
    public bool isHovered;

    //Movement
    [Header("Movement")]
    Vector2 targetPosition;
    public float followSpeed;
    public float maxSpeed;
    private Vector2 dragOffset;
    public float maxRotationAngle;
    public float rotationSpeed;

    //Events
    [Header("Events")]
    [HideInInspector] public UnityEvent<UICardBehaviour> PointerEnterEvent;
    [HideInInspector] public UnityEvent<UICardBehaviour> PointerExitEvent;
    [HideInInspector] public UnityEvent<UICardBehaviour> BeginDragEvent;
    [HideInInspector] public UnityEvent<UICardBehaviour> EndDragEvent;

    void Start()
    {
        // Set values for diferent private variables
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        cardsContainer = GameObject.Find("CardsContainer").GetComponent<RectTransform>();
        targetPosition = new Vector2(0, 0);
    }

    void Update()
    {
        // Update target position based on if the card is being draged and move card to its target position
        if (isDraging)
        {
            Vector2 mousePosition = Input.mousePosition / canvas.scaleFactor;
            targetPosition = mousePosition - dragOffset;

            if (freeCard)
            {
                targetPosition = mousePosition - dragOffset;
            }
        }
        else
        {
            if(cardSlot != null)
                targetPosition = cardSlot.anchoredPosition;
        }

        if (freeCard)
        {
            Vector2 mousePosition = Input.mousePosition / canvas.scaleFactor;
            targetPosition = mousePosition - dragOffset;
        }

        // Move a card twords target position
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, followSpeed * Time.deltaTime);

        // Clamp movement speed
        Vector2 direction = targetPosition - rectTransform.anchoredPosition;
        if (direction.magnitude > maxSpeed * Time.deltaTime)
        {
            rectTransform.anchoredPosition += direction.normalized * (maxSpeed * Time.deltaTime);
        }

        // Apply Correct Rotation
        if (isDraging)
        {
            float rotationAmount = Mathf.Clamp(-direction.x * 0.1f, -maxRotationAngle, maxRotationAngle); // Invert X-axis effect
            Quaternion targetRotation = Quaternion.Euler(0, 0, rotationAmount); // Only rotate around Z-axis
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Reset rotation when not dragging
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }

        // Handle being dragged in/out of cards container
        if (!IsOverlappingCardsContainer() && !freeCard)
        {
            if (!isDraging) return;

            cardsContainer.GetComponent<CardsContainer>().RemoveCardFromContainer(this);
        }
        if(IsOverlappingCardsContainer() && freeCard)
        {
            //if (!isDraging) return;

            Debug.Log("AddMe");
            cardsContainer.GetComponent<CardsContainer>().AddCard(this);
        }
    }

    private bool IsOverlappingCardsContainer()
    {
        Rect containerRect = GetScreenRect(cardsContainer);
        Rect cardRect = GetScreenRect(this.rectTransform);

        return containerRect.Overlaps(cardRect);
    }

    Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = corners[0].y;
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        return new Rect(x, y, width, height);
    }

    // Events
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraging = true;
        BeginDragEvent.Invoke(this);

        dragOffset = eventData.position / canvas.scaleFactor - rectTransform.anchoredPosition;
    }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
        EndDragEvent.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        PointerEnterEvent.Invoke(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        PointerExitEvent.Invoke(this);
    }

    public int GetCardSlotsSiblingIndex()
    {
        return cardSlot.transform.GetSiblingIndex();
    }
}
