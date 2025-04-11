using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject fadeTransition;
    private SceneTransition sceneTransition;

    void Start(){
        //Setting certain elements to be hidden / seen
        tutorialPanel.SetActive(false);
        fadeTransition.SetActive(true);

        sceneTransition = fadeTransition.GetComponent<SceneTransition>();
        Image fadeImage = fadeTransition.GetComponent<Image>();

        // Set the fade image alpha to 1 (fully visible)
        Color fadeColor = fadeImage.color;
        fadeColor.a = 1;  // Set to fully visible (alpha = 1)
        fadeImage.color = fadeColor;

        StartCoroutine(sceneTransition.FadeOutAndHide(fadeImage, 1.0f)); // Fade out the transition image
    }

    //Function that will handle the scene transition from the main menu to the battle scene
    public void StartGame(){
        fadeTransition.SetActive(true);
        sceneTransition.beginFadeTransition("BattleScreen");
        Debug.Log("Button presed cuh");
    }

    public void DisplayTutorial(){ //Function to display the tutorial
        tutorialPanel.SetActive(true);
    } 

    public void CloseTutorial(){ //Function to close/hide the tutorial panel
         tutorialPanel.SetActive(false);
    }

    public void QuitApplication(){ //Function that will close the application when the button is pressed
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
