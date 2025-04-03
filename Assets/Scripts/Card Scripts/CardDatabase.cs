using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Scriptable Objects/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public List<Card> allCards;  // List of all available cards

    // New method to safely retrieve cards by name
    public Card GetCardByName(string cardName){
        if (string.IsNullOrEmpty(cardName)){
            Debug.LogWarning("Card name is null or empty!");
            return null;
        }

        Card foundCard = allCards.Find(c => c.cardName == cardName);
        
        if (foundCard == null){
            Debug.LogWarning($"Card '{cardName}' not found in database!");
        }
        return foundCard;
    }

    // Optional: Alternative version that creates a new instance
    public Card GetCardCopyByName(string cardName){
        Card original = GetCardByName(cardName);
        if (original == null) return null;

        // Create a new instance to avoid modifying the original
        Card copy = Instantiate(original);
        copy.name = original.name; // Remove "(Clone)" from name
        return copy;
    }
}