using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    private Image fadeImage;

    void Awake(){
        // Auto-assign if not set in inspector
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();
    }

    //Function to call in the fade to black transition
    public void beginFadeTransition(string sceneName){
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(fadeToBlack(sceneName));
    }

    //Function that consists of the while loop to fade the alpha value of the image
    public IEnumerator fadeToBlack(string sceneName)
    {
        float fadeElapsedTime = 0.0f;
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;

        while (fadeElapsedTime < fadeDuration)
        {
            fadeElapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, fadeElapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }

    //Function that will fade out the image and hide it after the transition
    public IEnumerator FadeOutAndHide(Image fadeImage, float fadeDuration) {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 1; // Start fully visible
        fadeImage.color = color;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Ensure it's fully transparent at the end
        color.a = 0;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}
