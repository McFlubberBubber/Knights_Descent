using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    //Public variables
    public float fadeDuration = 1.0f;
    public Image fadeImage;

    // void start()
    // {
    //     StartCoroutine(fadeIn());
    // }

    //Function that will begin the fade transition from MainMenu scene to battlescreen
    public void beginFadeTransition(string sceneName){
        StartCoroutine(fadeOut(sceneName));
    }

    // //Function that will handle the fade in effect on the panel
    // IEnumerator fadeIn(){
    //     float fadeElapsedTime = 0.0f;
    //     Color color = fadeImage.color;
    //     color.a = 1; //setting alpha to 1
    //     fadeImage.color = color;

    //     while (fadeElapsedTime < fadeDuration){
    //         fadeElapsedTime += Time.deltaTime; //incrementing to the elapsed time based on the public var
    //         color.a = Mathf.Lerp(1, 0, fadeElapsedTime / fadeDuration); //transitions alpha value from 1 to 0, third arg = interpolation factor
    //         fadeImage.color = color;
    //         yield return null; 
    //     }

    // }

    //Function that will handle the fade out effect on the panel
    IEnumerator fadeOut(string sceneName){
        float fadeElapsedTime = 0.0f;
        Color color = fadeImage.color;
        color.a = 0; //setting alpha to 0
        fadeImage.color = color;

        while (fadeElapsedTime < fadeDuration){
            fadeElapsedTime += Time.deltaTime; //incrementing to the elapsed time based on the public var
            color.a = Mathf.Lerp(0, 1, fadeElapsedTime / fadeDuration); //transitions alpha value from 0 to 1, third arg = interpolation factor
            fadeImage.color = color;
            yield return null; 
        }
        SceneManager.LoadScene(sceneName); //Loading the scene based on the argument
    }
}

