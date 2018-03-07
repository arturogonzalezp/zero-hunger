using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stateController : MonoBehaviour {

	public Button next;
	public Button prev;
	private Animator animator;

	// Use this for initialization
	void Start () {
		Button Next = next.GetComponent<Button>();
		Button Prev = prev.GetComponent<Button>();
		animator = GetComponent<Animator>();
		Next.onClick.AddListener(nextStep);
		Prev.onClick.AddListener(prevStep);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void nextStep()
	{
		Debug.Log ("Next");
		animator.SetTrigger("Next");
	}
	void prevStep(){
		Debug.Log ("Prev");
		animator.SetTrigger ("Prev");
	}
}