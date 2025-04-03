using UnityEngine;

public class ForceLandscape : MonoBehaviour
{
    public static ForceLandscape Instance { get; private set; }

    void Awake(){
        //Preventing duplicate instances
        if (Instance != null && Instance != this){
            Destroy(gameObject);
        }
        else{
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make persistent
        }
        LockLandscape();
    }
    
    //Fuction to lock the game to landscape orientation
    void LockLandscape(){
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Disable all other orientations
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        //Screen.autorotateToLandscapeLeft = false; 
        //Screen.autorotateToLandscapeRight = false;

        #if UNITY_EDITOR
        Screen.SetResolution(2532, 1170, false);
        #endif
    }
}