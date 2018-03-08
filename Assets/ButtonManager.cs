using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;
public class ButtonManager : MonoBehaviour {

	void Start(){
		VuforiaBehaviour.Instance.enabled = false;
	}
    public void newGameBtn(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }

}
