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
    private Transform parentBeforeDrag; 
    private CanvasGroup canvasGroup;
    private int originalIndex;
    public static CardDisplay selectedCard = null; 
    private float selectedOffset = 200;
    private CardHandLayout handLayout;
    private CardLogic cardLogic;
    public Transform selectedLayer;

    //Variables for reward selection
    private System.Action<Card> onRewardSelected;
    private bool isRewardMode = false;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>(); // Ensure CanvasGroup exists
        parentBeforeDrag = transform.parent;
        handLayout = parentBeforeDrag.GetComponent<CardHandLayout>();
        cardLogic = GetComponent<CardLogic>();
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
    public void DisplayCard(Card newCard){
        card = newCard;
        UpdateCardDisplay();
    }

    public void SetRewardSelectionMode(System.Action<Card> callback)
    {
        isRewardMode = true;
        onRewardSelected = callback;

        // Disable dragging logic when in reward mode
        canvasGroup.blocksRaycasts = true;
    }

    // Function that handles the card selection and deselection
    public void OnPointerClick(PointerEventData eventData){
        //If the card has been instantiated for a reward selection
        if (isRewardMode){
            onRewardSelected?.Invoke(card);
            Highlight(true);
            return;
        }

        if (selectedCard != null && selectedCard != this){
            selectedCard.DeselectCard();
        }

        if (selectedCard == this){
            DeselectCard();
        } 
        else {
            SelectCard();
        }
    }


    //Function that handles the selection of a card
    private void SelectCard(){
        originalIndex = transform.GetSiblingIndex(); 
        originalPosition = transform.localPosition;  
        parentBeforeDrag = transform.parent;        

        if (selectedLayer != null) {
            transform.SetParent(selectedLayer, true);
        } else {
            transform.SetParent(transform.root, true);
        }

        Vector3 targetPosition = originalPosition + new Vector3(0, selectedOffset, 0);
        
        handLayout.SetHandUpdating(false); // Stop hand layout updates
        StartCoroutine(SmoothMove(transform.localPosition, targetPosition, 0.2f));

        selectedCard = this;
    }

    //Function that handles the deselection of a card
    public void DeselectCard(bool playAnimation = true){
        transform.SetParent(parentBeforeDrag, true);
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
        //if the card has been instantiated for a reward selection
        if (isRewardMode) return;

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
        if (isRewardMode) return;

        if (selectedCard == this) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData){
        if (isRewardMode) return;

        RaycastHit2D hit = Physics2D.Raycast(eventData.position, Vector2.zero);

        if (hit.collider != null) {
            if (hit.collider.CompareTag("Player") && card.type == Card.cardType.Skill) {
                // Debug.Log("Skill card dropped on player: " + card.cardName);
                cardLogic.PlayCard();
            }
            else if (hit.collider.CompareTag("Enemy") && card.type == Card.cardType.Attack) {
                // Debug.Log("Attack card dropped on enemy: " + card.cardName);
                cardLogic.PlayCard();
            }
            else {
                // Debug.Log("Card dropped on invalid target or wrong card type.");
                ReturnCardToOriginalPosition();
            }
        }
        else {
            ReturnCardToOriginalPosition();
        }
        canvasGroup.blocksRaycasts = true;
        handLayout.SetHandUpdating(true);
    }

    private void ReturnCardToOriginalPosition(){
        transform.SetParent(parentBeforeDrag);
        transform.SetSiblingIndex(originalIndex);
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
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

    public void Highlight(bool isActive){
        //If the bool value is active, scale the card up to 1.75f, else scale it down to 1.5f
        if (isActive){
            transform.localScale = Vector3.one * 1.75f; // Scale up on selection
        } else if (transform.localScale != Vector3.one){
            transform.localScale = Vector3.one * 1.5f; // Reset scale on deselection
        }
    }
}
