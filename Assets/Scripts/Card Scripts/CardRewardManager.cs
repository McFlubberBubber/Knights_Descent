using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardRewardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cardRewardPanel;
    public GameObject victoryScreen;
    public Transform cardOptionArea;
    public Button confirmButton;
    public Button skipButton;
    public Button rewardButton1;
    public Button rewardButton2;
    public Button rewardButton3;

    [Header("Card Settings")]
    public int numberOfOptions = 3;
    public CardDatabase cardDatabase; 
    public GameObject cardPrefab; 

    [Header("Button Fill Images")]
    public Image rewardButton1Fill; // Reference to the fill image of RewardButton1
    public Image rewardButton2Fill; // Reference to the fill image of RewardButton2
    public Image rewardButton3Fill; // Reference to the fill image of RewardButton2


    private Card selectedCard;
    private List<GameObject> spawnedCards = new List<GameObject>();
    [SerializeField] private CardManager cardManager;
    private Color rewardButton1OriginalColor;
    private Color rewardButton2OriginalColor;
    private Color rewardButton3OriginalColor;

    void Start()
    {
        confirmButton.interactable = false;
        confirmButton.onClick.AddListener(ConfirmCard);
        skipButton.onClick.AddListener(SkipReward);

        rewardButton1OriginalColor = rewardButton1Fill.color;
        rewardButton2OriginalColor = rewardButton2Fill.color;
        rewardButton3OriginalColor = rewardButton3Fill.color;
    }

    public void ShowCardRewards()
    {
        victoryScreen.SetActive(false);
        cardRewardPanel.SetActive(true);
        confirmButton.interactable = false;
        selectedCard = null;

        SpawnCardOptions();
    }

    void SpawnCardOptions()
    {
        ClearPreviousCards();

        List<Card> randomCards = cardDatabase.GetRandomCards(numberOfOptions); // Get 3 random cards
        foreach (Card card in randomCards)
        {
            // Creating a new card object and setting its parent to the card option area
            GameObject cardGO = Instantiate(cardPrefab, cardOptionArea);
            cardGO.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            CardDisplay display = cardGO.GetComponent<CardDisplay>();

            // Setting the card data to the card display
            display.DisplayCard(card);
            display.SetRewardSelectionMode(OnCardSelected);

            // Access the CardLogic component of the instantiated card
            CardLogic cardLogic = cardGO.GetComponent<CardLogic>();
            if (cardLogic != null)
            {
                cardLogic.SetCardManager(cardManager); // Set the CardManager for this card
            }
            else
            {
                Debug.LogWarning("CardLogic component not found on card prefab!");
            }

            // Adding the card to the spawned cards list
            spawnedCards.Add(cardGO);
        }
    }

    void OnCardSelected(Card card)
    {
        selectedCard = card;
        confirmButton.interactable = true; // Enable confirm button when a card is selected

        // Highlight selected card
        foreach (GameObject go in spawnedCards)
        {
            var display = go.GetComponent<CardDisplay>();
            bool isSelected = display.card == card;
            display.Highlight(isSelected); // Highlight only the selected card
        }
    }

    void ConfirmCard(){
        if (selectedCard != null && cardManager != null){
            cardManager.AddCardToDeck(selectedCard);
        }
        ReturnToVictoryScreen();
    }

    void SkipReward(){
        ReturnToVictoryScreen();
    }

    void ReturnToVictoryScreen(){
        ClearPreviousCards();
        cardRewardPanel.SetActive(false);
        victoryScreen.SetActive(true);
    }

    void ClearPreviousCards(){
        foreach (var card in spawnedCards){
            Destroy(card);
        }
        spawnedCards.Clear();
    }

    void DisableRewardButtons()
    {
        // Initially disable both buttons and grey them out
        SetButtonDisabled(rewardButton1, rewardButton1Fill);
        SetButtonDisabled(rewardButton2, rewardButton2Fill);
        SetButtonDisabled(rewardButton3, rewardButton3Fill);
    }

    void SetButtonDisabled(Button button, Image buttonFill)
    {
        if (button != null)
        {
            button.interactable = false;
            if (buttonFill != null)
                buttonFill.color = Color.grey; // Grey out the fill when disabled
        }
    }

    // Call this function when the player clicks on either of the reward buttons
    public void OnRewardButtonClicked(Button clickedButton)
    {
        // Disable the clicked reward button only
        if (clickedButton == rewardButton1)
        {
            SetButtonDisabled(rewardButton1, rewardButton1Fill);
        }
        else if (clickedButton == rewardButton2)
        {
            SetButtonDisabled(rewardButton2, rewardButton2Fill);
        }
        else if (clickedButton == rewardButton3)
        {
            SetButtonDisabled(rewardButton3, rewardButton3Fill);
        }
    }

    //Function to reset the reward button states
    public void ResetRewardButtons(){
        ResetRewardButton(rewardButton1);
        ResetRewardButton(rewardButton2);
        ResetRewardButton(rewardButton3);
    }
    public void ResetRewardButton(Button clickedButton){
        if (clickedButton == rewardButton1)
        {
            rewardButton1.interactable = true;
            if (rewardButton1Fill != null)
                rewardButton1Fill.color = rewardButton1OriginalColor;
        }
        else if (clickedButton == rewardButton2)
        {
            rewardButton2.interactable = true;
            if (rewardButton2Fill != null)
                rewardButton2Fill.color = rewardButton2OriginalColor;
        }
        else if (clickedButton == rewardButton3)
        {
            rewardButton3.interactable = true;
            if (rewardButton3Fill != null)
                rewardButton3Fill.color = rewardButton3OriginalColor;
        }
    }
}
