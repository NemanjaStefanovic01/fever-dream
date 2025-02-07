using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas canvas; // Use it to enable and disabel RayCast for all obj -> canvas.GetComponent<GraphicRaycaster>().enabled = false;
    private Image imageComponent; // Enabel disable Raycast for specific img -> imageComponent.raycastTarget = false;
    private RectTransform rectTransform;

    //States
    public bool isDraging;
    public bool isHovered;

    //Movement
    [Header("Movement")]
    Vector2 targetPosition;
    public float followSpeed;
    public float maxSpeed;
    public float rotationSpeed;
    public float maxRotationAngle;

    void Start()
    {
        // Set values for diferent private variables
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        targetPosition = Vector3.zero;
    }

    void Update()
    {
        // Update target position based on if the card is being draged and move card to its target position
        if (isDraging)
        {
            Vector2 mousePosition = Input.mousePosition / canvas.scaleFactor;
            targetPosition = mousePosition;
        }
        else
        {
            targetPosition = Vector3.zero;
        }

        // Move a card twords target position
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, followSpeed * Time.deltaTime);

        // Clamp movement speed
        Vector2 direction = targetPosition - rectTransform.anchoredPosition;
        if (direction.magnitude > maxSpeed * Time.deltaTime)
        {
            rectTransform.anchoredPosition += direction.normalized * (maxSpeed * Time.deltaTime);
        }
    }

    

    // Events
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraging = true;
    }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
