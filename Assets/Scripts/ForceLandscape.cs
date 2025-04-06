using UnityEngine;

public class ForceLandscape : MonoBehaviour
{
    public static ForceLandscape Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // Exit early to avoid duplicate logic
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LockLandscape(); // Apply settings
    }

    void LockLandscape()
    {
        // Force landscape orientation (works on mobile)
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        // Set resolution for Windows/Editor (scaled down for practicality)
        #if UNITY_STANDALONE || UNITY_EDITOR
        float scaleFactor = 0.6f; // Adjust to fit your screen
        int targetWidth = Mathf.RoundToInt(2532 * scaleFactor);
        int targetHeight = Mathf.RoundToInt(1170 * scaleFactor);
        Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);
        
        // Force aspect ratio (prevures stretching)
        Camera.main.aspect = 2532f / 1170f;
        #endif
    }
}