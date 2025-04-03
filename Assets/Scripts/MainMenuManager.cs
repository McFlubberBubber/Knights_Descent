using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    //Public variables
    public GameObject settingsPanel;
    public GameObject fadeTransition;
    public Slider volumeSlider;
    public AudioSource menuAudio;
    public SceneTransition transitionScript;

    void Start(){
        settingsPanel.SetActive(false); //Ensure the settings panel is not displayed from the start
        fadeTransition.SetActive(false); //Ensuring the fade panel is not interfering with the buttons
        volumeSlider.value = menuAudio.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    //Function that will transition player from main menu to game scene
    public void StartGame(){
        fadeTransition.SetActive(true);
        transitionScript.beginFadeTransition("BattleScreen");
        Debug.Log("Button presed cuh");
    }

    //Function that will show the settings panel
    public void DisplaySettings(){
        settingsPanel.SetActive(true);
    }

    //Function that will close the settings panel
    public void CloseSettings(){
        settingsPanel.SetActive(false);
    }

    //Function that will set the volume of the game
    public void SetVolume(float volume){
        menuAudio.volume = volume;
    }

    //Function to close the application
    public void QuitApplication(){
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
