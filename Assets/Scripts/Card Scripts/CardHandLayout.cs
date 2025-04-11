using UnityEngine;
using System.Collections.Generic;

public class CardHandLayout : MonoBehaviour
{
    [Header("Fan Settings")]
    [SerializeField] private float maxAngle = 40f;         
    [SerializeField] private float horizontalSpread = 1000f; 
    [SerializeField] private float verticalRise = -25f;     
    [SerializeField] private float rotationPivot = 0.8f;    

    private bool needsUpdate = true; // Flag to check if hand needs updating

    private void Start(){
        UpdateHand();
    }

    public void SetHandUpdating(bool shouldUpdate){ // Set the flag to update or not
        needsUpdate = shouldUpdate;
    }

    private void Update(){
        if (needsUpdate){ // Check if the hand needs updating, if so, update the hand and set the flag to false
            UpdateHand();
            needsUpdate = false; 
        }
    }

    //Function that will update the hand layout
    public void UpdateHand(){
        List<Transform> cardsInHand = new List<Transform>(); //Creating a new list of cards in hand

        //Looping through all the children of the transform and adding them to the list if they are a card display
        foreach (Transform card in transform){ 
            if (card.GetComponent<CardDisplay>()?.transform.parent == transform) { // Check if the card is a child of this transform
                cardsInHand.Add(card);
            }
        }
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        //Calculate the angle step and starting angle based on the number of cards for the fanning effect
        // float angleStep = maxAngle / Mathf.Max(1, cardCount - 1); 
        // float startAngle = -maxAngle / 2f;

        //Testing display stuff
        // Adjust horizontal spread based on number of cards
        float spreadFactor = Mathf.Clamp01((cardCount - 1) / 4f); // 0 when 1 card, approaches 1 as count -> 5
        float adjustedHorizontalSpread = Mathf.Lerp(200f, horizontalSpread, spreadFactor); // Tighter at low card count

        // Reduce angle at low counts for tighter clustering
        float adjustedMaxAngle = Mathf.Lerp(10f, maxAngle, spreadFactor);
        float angleStep = adjustedMaxAngle / Mathf.Max(1, cardCount - 1);
        float startAngle = -adjustedMaxAngle / 2f;


        //For loop to position the cards in a fan layout based on how many cards are in hand
        for (int i = 0; i < cardCount; i++){
            Transform card = cardsInHand[i];
            if (CardDisplay.selectedCard == card.GetComponent<CardDisplay>()){
                continue;
            }
            float angle = startAngle + (angleStep * i); 
            float xPos = Mathf.Sin(angle * Mathf.Deg2Rad) * horizontalSpread; //Using sine to calculate the x position of the card
            float yPos = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)) * verticalRise; //Using cosine to calculate the y position of the card

            card.localPosition = new Vector3(xPos, yPos, 0);
            card.localRotation = Quaternion.Euler(0, 0, angle * rotationPivot); // Rotating the card based on the angle

            Canvas canvas = card.GetComponent<Canvas>();
            if (canvas != null) canvas.sortingOrder = i; // Stack order so that cards overlap correctly
        }
    }
}
