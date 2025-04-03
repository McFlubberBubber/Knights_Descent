using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Image))] // Ensure the GameObject has an Image component
public class CardDisplay : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Card Data")] //Storing the actual card data
    public Card card; 

    [Header("UI References")] //Storing all the card UI references
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText; 
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;

    [Header("Visual Settings")]
    public Color attackColor = new Color(0.8f, 0.2f, 0.2f);
    public Color skillColor = new Color(0.2f, 0.4f, 0.8f);

    //Private variables
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform parentBeforeDrag; // Store the original parent before dragging to restore positions later
    private CanvasGroup canvasGroup;
    private int originalIndex;
    public static CardDisplay selectedCard = null; // Static reference to the currently selected card
    private float selectedOffset = -200;
    private CardHandLayout handLayout; 
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>(); // Ensure CanvasGroup exists
        parentBeforeDrag = transform.parent;
        handLayout = parentBeforeDrag.GetComponent<CardHandLayout>();
    }

    //Function that will update the card display with the card data
    public void UpdateCardDisplay(){
        if (card == null){
            Debug.LogWarning("CardDisplay: No Card assigned!", gameObject); //Error handling 
            return;
        }
        nameText.text = card.cardName;
        typeText.text = card.type.ToString();
        descriptionText.text = card.GetDescription();
        costText.text = card.cost.ToString();
        nameText.color = card.type == Card.cardType.Attack ? attackColor : skillColor; //Changing the color of the name text based on the card type
    }

    // Function that handles the card selection and deselection
    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectedCard != null && selectedCard != this){
            selectedCard.DeselectCard(); // Deselect the previously selected card so there can only be one selected card at a time
        }
        if (selectedCard == this){ // If the card is already selected, deselect it
            DeselectCard();
        } else {
            SelectCard(); // Select the clicked card
        }
    }

    //Function that handles the selection of a card
    private void SelectCard(){
        originalIndex = transform.GetSiblingIndex(); 
        originalPosition = transform.localPosition;  
        parentBeforeDrag = transform.parent;        

        transform.SetParent(transform.root);    

        Vector3 targetPosition = originalPosition + new Vector3(0, selectedOffset, 0);
        
        handLayout.SetHandUpdating(false); // Stop hand layout updates
        StartCoroutine(SmoothMove(transform.localPosition, targetPosition, 0.2f));

        selectedCard = this;
    }

    //Function that handles the deselection of a card
    public void DeselectCard(bool playAnimation = true){
        transform.SetParent(parentBeforeDrag);
        transform.SetSiblingIndex(originalIndex);

        handLayout.SetHandUpdating(false); // Stop hand layout updates

        if(playAnimation){
            StartCoroutine(SmoothMove(transform.localPosition, originalPosition, 0.2f));
        } else {
            transform.localPosition = originalPosition; // Set to original position without animation
        }
        selectedCard = null;
    }

    //Function that handles the start of the drag event
    public void OnBeginDrag(PointerEventData eventData){
        if (selectedCard != null) {
            if (selectedCard == this) {
                selectedCard.DeselectCard(false);
            } else {
                selectedCard.DeselectCard(true);
            }
        }

        originalIndex = transform.GetSiblingIndex();
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        parentBeforeDrag = transform.parent;

        transform.SetParent(transform.root);
        transform.localRotation = Quaternion.identity;
        canvasGroup.blocksRaycasts = false;
        handLayout.SetHandUpdating(false);
    }

    //Making the card follow the mouse/finger while dragging
    public void OnDrag(PointerEventData eventData){
        if (selectedCard == this) return;
        transform.position = eventData.position;
    }

    //Function that handles the end of the drag event
    public void OnEndDrag(PointerEventData eventData)
    {
        if (selectedCard == this) return;

        transform.SetParent(parentBeforeDrag);
        transform.SetSiblingIndex(originalIndex);
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        canvasGroup.blocksRaycasts = true;
        handLayout.SetHandUpdating(true);
    }

    //Function that will move the card to a new position smoothly for visual clarit
    private IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smoothstep formula
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPos;
    }

    //Functions for card dropping
}
