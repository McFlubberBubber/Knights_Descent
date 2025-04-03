using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using TMPro;

public class CardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardDatabase cardDatabase;
    [SerializeField] private Transform handArea;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TMP_Text drawCountText;
    [SerializeField] private TMP_Text discardCountText;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private EnergyManager energyManager; 
    [SerializeField] private EnemyController enemyController; 

    [Header("Visual Feedback")]
    [SerializeField] private Transform drawPileVisual;
    [SerializeField] private Transform discardPileVisual;
    [SerializeField] private float cardMoveDuration = 1.0f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    [Header("Debug View")]
    public List<Card> playerDeck = new List<Card>();
    public List<Card> drawPile = new List<Card>();
    public List<Card> playerHand = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    private const int STARTING_HAND_SIZE = 5;
    private const int CARD_COPIES = 6;
    private bool isInitialized = false;
    private CardDisplay selectedCard;

    // void Start(){
    //     InitializeGame();
    // }
    
    public void SetEnemyController(EnemyController controller){
        enemyController = controller;
    }

    public void InitializeGame(){
        if (cardDatabase == null){
            Debug.LogError("CardDatabase reference not set!");
            return;
        }
        InitializeStarterDeck();
        ShuffleDeck();
        isInitialized = true;
        DrawHand();
    }
    

    void InitializeStarterDeck(){ //Function to add starter cards to the player's deck
        playerDeck.Clear();
        AddCardCopies("Slash");
        AddCardCopies("Shield");
        
        drawPile = new List<Card>(playerDeck);
        Debug.Log($"Initialized deck with {playerDeck.Count} cards");
        UpdateDeckCountText();
    }

    void AddCardCopies(string cardName){ //Adding multiple copies of a card to the deck
        Card card = cardDatabase.GetCardByName(cardName);
        if (card == null){
            Debug.LogError($"{cardName} card not found!");
            return;
        }
        playerDeck.AddRange(Enumerable.Repeat(card, CARD_COPIES));
    }

    void ShuffleDeck(){ //Function to handle shuffling the deck in the draw pile for randomized drawing
        for (int i = drawPile.Count - 1; i > 0; i--){
            int randIndex = Random.Range(0, i + 1);
            Card temp = drawPile[i];
            drawPile[i] = drawPile[randIndex];
            drawPile[randIndex] = temp;
        }
        Debug.Log("Deck shuffled");
    }

    public void DrawHand(){ //Drawing the set amount of cards into the player hand (5)
        if (!isInitialized){
            Debug.LogWarning("Game not initialized! Cannot draw hand");
            return;
        }

        for (int i = 0; i < STARTING_HAND_SIZE; i++){
            if (drawPile.Count == 0){
                Debug.Log("Draw pile empty! Reshuffling...");
                if (discardPile.Count == 0){
                    Debug.LogWarning("No cards left to draw!");
                    return;
                }
                ReshufflePiles(); //Reshuffling the discard into the draw to ensure the player will draw 5 cards no matter what
            }
            DrawSingleCard();
            StartCoroutine(AnimateDrawPile());
        }

        CardHandLayout handLayout = handArea.GetComponent<CardHandLayout>();
        if (handLayout != null){
            handLayout.SetHandUpdating(true); // Set the flag to update the hand layout
        } else {
            Debug.LogError("CardHandLayout component not found on hand area!");
        }
    }

    void DrawSingleCard(){ //Function that will draw a single card from the top of the draw pile
        Card drawnCard = drawPile[0];
        drawPile.RemoveAt(0);

        GameObject newCard = Instantiate(cardPrefab, handArea);
        newCard.SetActive(false);

        CardDisplay display = newCard.GetComponent<CardDisplay>();
        display.card = drawnCard;
        display.UpdateCardDisplay();
        playerHand.Add(drawnCard);

        // Debug.Log($"Drawn card: {drawnCard.cardName}");

        CardLogic cardLogic = newCard.GetComponent<CardLogic>();
        if(cardLogic != null){
            cardLogic.SetPlayerStats(playerStats);
            cardLogic.SetEnergyManager(energyManager);
            cardLogic.SetEnemyController(enemyController);
        }

        StartCoroutine(AnimateCardDraw(newCard));
        UpdateDeckCountText();
    }

    void ReshufflePiles(){ //Function to reshuffle the discard pile into the draw pile
        if (discardPile.Count == 0) return;
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
        Debug.Log("Reshuffled discard pile into draw pile");
        UpdateDeckCountText();
    }

    //Referenced in TurnHandler.cs to end the player's turn
    public void EndTurn()
    {
        // Deselecting whatever card is currently selected
        if (CardDisplay.selectedCard != null)
        {
            CardDisplay.selectedCard.DeselectCard();
            CardDisplay.selectedCard = null;
        }

        // Discarding all cards remaining in hand to the discard pile
        if (playerHand.Count == 0) return;

        foreach (Transform child in handArea)
        {
            // Animate the card moving to the discard pile
            StartCoroutine(AnimateCardDiscard(child.gameObject));
        }

        // Add cards to the discard pile and clear the hand
        discardPile.AddRange(playerHand);
        playerHand.Clear();

        UpdateDeckCountText();
    }

    //Function that will update the UI text for the draw and discard piles
    public void UpdateDeckCountText(){
        drawCountText.text = drawPile.Count.ToString();
        discardCountText.text = discardPile.Count.ToString();
    }

    //Function that handles the animation of the cards being drawn from the draw pile to the hand
    private IEnumerator AnimateCardDraw(GameObject cardObj)
    {
        // Store the hand layout component
        CardHandLayout handLayout = handArea.GetComponent<CardHandLayout>();
        
        // Initial setup - hide card and set at draw pile
        cardObj.transform.position = drawPileVisual.position;
        cardObj.transform.localScale = Vector3.zero;
        cardObj.SetActive(true);
        
        // Calculate final position in hand
        Vector3 endPosition = cardObj.transform.position; // This will be updated by HandLayout
        Quaternion endRotation = cardObj.transform.rotation;
        Vector3 endScale = Vector3.one;
        
        // Temporarily force the card to update its position in the hand layout
        if (handLayout != null){
            handLayout.UpdateHand();
            yield return null; // Wait one frame for layout to update
            endPosition = cardObj.transform.position;
            endRotation = cardObj.transform.rotation;
        }

        // Animation values
        float elapsed = 0f;
        Vector3 startPos = drawPileVisual.position;
        
        while (elapsed < cardMoveDuration){
            float t = moveCurve.Evaluate(elapsed / cardMoveDuration);
            
            // Animate position and scale
            cardObj.transform.position = Vector3.Lerp(startPos, endPosition, t);
            cardObj.transform.localScale = Vector3.Lerp(Vector3.zero, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are exact
        cardObj.transform.position = endPosition;
        cardObj.transform.localScale = endScale;
        if (handLayout != null){
            cardObj.transform.rotation = endRotation;
        }
    }

    //Visual feedback for the draw pile
    private IEnumerator AnimateDrawPile()
    {
        Transform pile = drawPileVisual.transform;
        
        // Simple scale up/down
        float halfDur = 0.2f;
        pile.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(halfDur);
        pile.localScale = Vector3.one;
    }

    //Visual feedback for the discard pile
    private IEnumerator AnimateDiscardPile()
    {
        Transform pile = discardPileVisual.transform;
        
        // Simple scale up/down
        float halfDur = 0.2f;
        pile.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(halfDur);
        pile.localScale = Vector3.one;
    }

    //Function that handles the animation of the cards being discarded from the hand to the discard pile
    private IEnumerator AnimateCardDiscard(GameObject cardObj)
    {
        // Store the hand layout component
        CardHandLayout handLayout = handArea.GetComponent<CardHandLayout>();

        // Initial setup - get the current position of the card in the hand
        Vector3 startPosition = cardObj.transform.position;
        Quaternion startRotation = cardObj.transform.rotation;
        Vector3 startScale = cardObj.transform.localScale;

        // Target position is the discard pile visual
        Vector3 endPosition = discardPileVisual.position;
        Quaternion endRotation = Quaternion.identity; // Reset rotation for discard pile
        Vector3 endScale = Vector3.zero; // Shrink to zero when discarded

        // Animation values
        float elapsed = 0f;
        while (elapsed < cardMoveDuration)
        {
            float t = moveCurve.Evaluate(elapsed / cardMoveDuration);

            // Animate position, rotation, and scale
            cardObj.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            cardObj.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            cardObj.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are exact
        cardObj.transform.position = endPosition;
        cardObj.transform.rotation = endRotation;
        cardObj.transform.localScale = endScale;
        cardObj.SetActive(false);
        Destroy(cardObj);
    }
}   