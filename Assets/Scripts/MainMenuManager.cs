using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject fadeTransition;
    // public AudioSource menuAudio;
    private SceneTransition sceneTransition;

    void Start()
    {
        settingsPanel.SetActive(false);
        fadeTransition.SetActive(true);

        // volumeSlider.value = menuAudio.volume;
        // volumeSlider.onValueChanged.AddListener(SetVolume);

        sceneTransition = fadeTransition.GetComponent<SceneTransition>();
        Image fadeImage = fadeTransition.GetComponent<Image>();

        // Set the fade image alpha to 1 (fully visible)
        Color fadeColor = fadeImage.color;
        fadeColor.a = 1;  // Set to fully visible (alpha = 1)
        fadeImage.color = fadeColor;

        StartCoroutine(sceneTransition.FadeOutAndHide(fadeImage, 1.0f)); // Fade out the transition image
    }

    public void StartGame()
    {
        fadeTransition.SetActive(true);
        sceneTransition.beginFadeTransition("BattleScreen");
        Debug.Log("Button presed cuh");
    }

    public void DisplaySettings(){
        settingsPanel.SetActive(true);
    } 

    public void CloseSettings(){
         settingsPanel.SetActive(false);
    }

    // public void SetVolume(float volume){
    //     menuAudio.volume = volume;
    // }

    public void QuitApplication(){
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
