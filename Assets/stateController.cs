using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class stateController : MonoBehaviour {

	public Button next;
	public Button prev;
    public GameObject[] images;

	private Animator animator;
    private Image currentStepImg;
    private int indexOfActiveStep = 0;

	public string successStory;

	// Use this for initialization
	void Start () {
		Button Next = next.GetComponent<Button>();
		Button Prev = prev.GetComponent<Button>();
		animator = GetComponent<Animator>();
		Next.onClick.AddListener(nextStep);
		Prev.onClick.AddListener(prevStep);
        Debug.Log("Images");
        //images = GameObject.FindGameObjectsWithTag("steps");
        /*foreach (GameObject img in images)
        {
            img.SetActive(false);
        }*/
        images[0].SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void nextStep()
	{
        if (indexOfActiveStep == -1) {
            indexOfActiveStep = 0;
            images[indexOfActiveStep].SetActive(true);
        } else if(indexOfActiveStep == images.Length-1){
			SceneManager.LoadScene (successStory);
        }else{
            images[indexOfActiveStep].SetActive(false);
            indexOfActiveStep++;
            images[indexOfActiveStep].SetActive(true);
        }
		Debug.Log ("Next");
		animator.SetTrigger("Next");
	}
	void prevStep(){
        if(indexOfActiveStep > 0){
            images[indexOfActiveStep].SetActive(false);
            indexOfActiveStep--;
            images[indexOfActiveStep].SetActive(true);
        }
		Debug.Log ("Prev");
		animator.SetTrigger ("Prev");
	}
}