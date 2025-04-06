using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

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
    public List<Card> GetRandomCards(int count)
    {
        List<Card> nonStarterCards = allCards.Where(card => !card.isStarterCard).ToList();
        List<Card> randomCards = new List<Card>();

        for (int i = 0; i < count && nonStarterCards.Count > 0; i++)
        {
            int index = Random.Range(0, nonStarterCards.Count);
            randomCards.Add(nonStarterCards[index]);
            nonStarterCards.RemoveAt(index);
        }

        return randomCards;
    }

}