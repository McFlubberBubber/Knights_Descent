using UnityEngine;

public class PlayerDropZone : MonoBehaviour
{
    //Function that checks if a card has entered the player drop zone
    private void OnTriggerEnter2D(Collider2D other){
        CardDisplay cardDisplay = other.GetComponent<CardDisplay>();

        if (cardDisplay != null){
            Debug.Log("Card entered player area: " + cardDisplay.card.cardName);
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        CardDisplay cardDisplay = other.GetComponent<CardDisplay>();

        if (cardDisplay != null){
            Debug.Log("Card left player area: " + cardDisplay.card.cardName);
        }
    }
}
