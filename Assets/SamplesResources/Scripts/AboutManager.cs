/*============================================================================== 
Copyright (c) 2017-2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using UnityEngine;
using UnityEngine.UI;

public class AboutManager : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    public Text aboutText;
    #endregion //PUBLIC_MEMBERS


    #region PRIVATE_METHODS
    void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion //PRIVATE_METHODS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        UpdateAboutText();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            LoadNextScene();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
			// On Android, the Back button is mapped to the Esc key
			// Exit app
            Application.Quit();
#endif
        }
    }
    #endregion // MONOBEHAVIOUR_METHODS

    void UpdateAboutText()
    {
        if (!aboutText) return;

        string vuforiaVersion = Vuforia.VuforiaUnity.GetVuforiaLibraryVersion();
        string unityVersion = Application.unityVersion;

        string vuforia = Vuforia.VuforiaRuntime.Instance.HasInitialized
                                ? "<color=green>Yes</color>"
                                : "<color=red>No (enable Vuforia in XR Settings)</color>";

        //public Image pictureBox1; 

        string about =
            "\n<size=26>This program was made with Unity and Vuforia.</size>" +
            "\n<size=26>This program has limits, so this is a list that you should not do:</size>" +
            "\n1.To use it properly you need to look a ground plane, it must be horizontal, never vertical." +
            "\nExample:" +
            "\n" +
            "\n" +
           // pictureBox1 
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n2. The ground plane must not have any objects, so it could work properly." +
            "\nExample:" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n3. The camera must be at a minimum of 20cm (7.8 inches) from the plane." +
            "\nExample:" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n4. Vuforia has supported devices. The IOS devices are for IOS 9 onwards. The Android devices are with Android 5 onwards." +
            "\nExample:" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\nThe way to use it properly is not doing the things in the list and using a proper device." +
            "\nThe animation should look like this: " +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n";

        aboutText.text = about;

        Debug.Log("Vuforia " + vuforiaVersion + "\nUnity " + unityVersion);

    }

}