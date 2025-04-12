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

    //Function utilised by the card reward manager to get a random list of cards based on the parameter ' int count'
    public List<Card> GetRandomCards(int count)
    {
        List<Card> nonStarterCards = allCards.Where(card => !card.isStarterCard).ToList(); //Finding out which cards don't have the starter flag
        List<Card> randomCards = new List<Card>();

        //For looping based on the parameter and until there are no starter cards left
        for (int i = 0; i < count && nonStarterCards.Count > 0; i++){
            int index = Random.Range(0, nonStarterCards.Count); //Getting a random integer from 0 to the number of cards in the nonStarterCards list
            randomCards.Add(nonStarterCards[index]); //Adding a random card using the index
            nonStarterCards.RemoveAt(index); //Removing the card from the list 
        }
        return randomCards;
    }
}