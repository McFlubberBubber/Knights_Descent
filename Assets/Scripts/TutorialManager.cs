using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Page References")]
    [SerializeField] private GameObject firstPage;
    [SerializeField] private GameObject secondPage;
    [SerializeField] private GameObject thirdPage;
    [SerializeField] private GameObject fourthPage;
    [SerializeField] private GameObject fifthPage;

    [Header("UI Elements")]
    [SerializeField] private Button prevPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private TMP_Text pageTracker;

    //Private variables
    private int totalPages = 5;
    private int pageCounter;

    //When the scene starts, set the starting window of the 'how to play' to the first page contents
    void Start(){
        ResetWindow();
    }

    //Function that will reset the page counter to 0 and then call the display page function
    public void ResetWindow(){
        pageCounter = 0;
        DisplayPage(pageCounter);
    }

    //Function to handle the next button press that would increment the counter 
    public void OnNext(){
        if (pageCounter < totalPages - 1){ //Doing -1 here because the counter starts a 0, but we have 5 pages total
            pageCounter++;
            DisplayPage(pageCounter);
        } 
    }

    //Function to handle the prev button press the would decrease the counter
    public void OnPrevious(){
        if (pageCounter > 0){
            pageCounter--;
            DisplayPage(pageCounter);
        } 
    }

    //Function to display the pages
    private void DisplayPage(int pageIndex){
        //Ensuring the pages are inactive at first
        firstPage.SetActive(false);
        secondPage.SetActive(false);
        thirdPage.SetActive(false);
        fourthPage.SetActive(false);
        fifthPage.SetActive(false);

        //Creating a switch case statement that would handle the different cases of the counter to handle the display
        switch (pageIndex){
            case 0: firstPage.SetActive(true); break;
            case 1: secondPage.SetActive(true); break;
            case 2: thirdPage.SetActive(true); break;
            case 3: fourthPage.SetActive(true); break;
            case 4: fifthPage.SetActive(true); break;
            default: Debug.Log("PageIndex broken"); break;
        }

        //Updating the page tracker at the bottom of the how to play window
        pageTracker.text = $"{pageIndex + 1}/{totalPages}";

        //Making the buttons non interactable when the player hits the limit of pages to scroll through
        prevPageButton.interactable = (pageIndex > 0);
        nextPageButton.interactable = (pageIndex < totalPages - 1);
    }
}
