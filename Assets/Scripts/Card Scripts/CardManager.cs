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
    [SerializeField] private TMP_Text deckCountText;
    [SerializeField] private TMP_Text gameOverDeckCount;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] public EnergyManager energyManager; 
    [SerializeField] private EnemyController enemyController; 
    [SerializeField] private Transform selectedLayer; 

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

    private int cardsToDraw = 5;
    private bool isInitialized = false;
    private CardDisplay selectedCard;

    public void SetEnemyController(EnemyController controller){
        enemyController = controller;
    }

    //Initializing the game and checking if the card database is set
    //If the flag is set to true, it will reset the deck and shuffle it, but in continuation of the game, it will not reset the deck
    public void InitializeGame(bool shouldResetDeck = true){
        if (cardDatabase == null){
            Debug.LogError("CardDatabase reference not set!");
            return;
        }

        if (shouldResetDeck){
            InitializeStarterDeck();
            ShuffleDeck();        
        }

        isInitialized = true;
        DrawHand();
        energyManager.RestoreEnergy(); // Reset energy at the start of the game
    }
    
    //Adding specified amount of cards based on name to the player's deck
    void InitializeStarterDeck()
    { 
        playerDeck.Clear();
        AddCard("Slash", 3);
        AddCard("Shield", 3);
        AddCard("Heavy Swing", 1);
        AddCard("Guard", 1);
        
        drawPile = new List<Card>(playerDeck);
        Debug.Log($"Initialized deck with {playerDeck.Count} cards");
        UpdateDeckCountText();
    }

    void AddCard(string cardName, int copies)
    { 
        Card card = cardDatabase.GetCardByName(cardName);
        if (card == null)
        {
            Debug.LogError($"{cardName} card not found!");
            return;
        }
        playerDeck.AddRange(Enumerable.Repeat(card, copies));
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

    //OG functions
    public void DrawHand(){ //Drawing the set amount of cards into the player hand (5)
        if (!isInitialized){
            Debug.LogWarning("Game not initialized! Cannot draw hand");
            return;
        }

        for (int i = 0; i < cardsToDraw; i++){
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
            handLayout.UpdateHand(); // Update the hand layout immediately
        } else {
            Debug.LogError("CardHandLayout component not found on hand area!");
        }
    }

    //Using this function for the card draw function within certain cards
    public void DrawCards(int count){
        for (int i = 0; i < count; i++){
            if (drawPile.Count == 0)
            {
                Debug.Log("Draw pile empty! Reshuffling...");
                if (discardPile.Count == 0){
                    Debug.LogWarning("No cards left to draw!");
                    return;
                }
                ReshufflePiles(); // Pull from discard pile
            }

            DrawSingleCard();
            StartCoroutine(AnimateDrawPile()); // Optional per card animation
        }

        // Update hand layout after drawing cards
        CardHandLayout handLayout = handArea.GetComponent<CardHandLayout>();
        if (handLayout != null){
            handLayout.SetHandUpdating(true);
            handLayout.UpdateHand();
        } else{
            Debug.LogError("CardHandLayout component not found on hand area!");
        }
    }


    public void DrawSingleCard(){ //Function that will draw a single card from the top of the draw pile
        Card drawnCard = drawPile[0];
        drawPile.RemoveAt(0);

        GameObject newCard = Instantiate(cardPrefab, handArea);
        // newCard.SetActive(false);
        newCard.transform.SetParent(handArea, false); // Set the parent to hand area

        CardDisplay display = newCard.GetComponent<CardDisplay>();
        display.selectedLayer = selectedLayer;
        display.card = drawnCard;
        display.UpdateCardDisplay();
        playerHand.Add(drawnCard);

        // Debug.Log($"Drawn card: {drawnCard.cardName}");

        //Assigning the setters within card logic
        CardLogic cardLogic = newCard.GetComponent<CardLogic>();
        if(cardLogic != null){
            cardLogic.SetPlayerStats(playerStats);
            cardLogic.SetEnergyManager(energyManager);
            cardLogic.SetEnemyController(enemyController);
            cardLogic.SetCardManager(this); 
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
        deckCountText.text = $"Deck Size: {playerDeck.Count}";
        gameOverDeckCount.text = $"- Deck count: {playerDeck.Count}";
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

    public void DiscardPlayedCard(GameObject cardObj, Card card){
        if (playerHand.Contains(card)){
            playerHand.Remove(card);
            discardPile.Add(card);
        }

        //Animating the movement of the card to the discard pile for visual clarity
        StartCoroutine(AnimateCardDiscard(cardObj));
        UpdateDeckCountText();
    }

    public void ReturnCardToHand(GameObject cardObj)
    {
        if (cardObj == null) return;

        CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.card == null) return;

        if (!playerHand.Contains(cardDisplay.card))
        {
            playerHand.Add(cardDisplay.card); // Ensure the card is tracked in the hand list
        }

        cardObj.transform.SetParent(handArea, false);
        cardObj.transform.localPosition = Vector3.zero; // Reset position
        cardObj.transform.rotation = Quaternion.identity; // Reset rotation if needed

        // Ensure the card appears in the right place visually
        CardHandLayout handLayout = handArea.GetComponent<CardHandLayout>();
        if (handLayout != null)
        {
            handLayout.SetHandUpdating(true); // Ensure layout updates
        }
    }

    public void ResetDeck(){
        //Moving all the cards from the discard pile and hand back to the draw pile
        drawPile.AddRange(playerHand);
        drawPile.AddRange(discardPile);

        //Clearing the hand and discard pile
        playerHand.Clear();
        discardPile.Clear();

        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject); // Destroy the card objects in the hand area
        }
        UpdateDeckCountText(); // Update the UI text for the deck counts
    }

    //Function that will add a card to the player's deck from the rewards, and shuffle it into the draw pile if specified
    public void AddCardToDeck(Card newCard, bool shuffleIntoDrawPile = true)
    {
        playerDeck.Add(newCard);

        if (shuffleIntoDrawPile)
        {
            drawPile.Add(newCard);
            ShuffleDeck();
        }
        UpdateDeckCountText();
    }
}   