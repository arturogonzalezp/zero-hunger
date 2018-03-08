using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class getJson : MonoBehaviour {
	private string url = "http://zero-hunger-api.herokuapp.com/get/jsontest";
	public Transform tutorialIconPrefab;
	public Transform canvas;

	// Use this for initialization
	void Start () {
		WWW www = new WWW(url);
		StartCoroutine(WaitForRequest(www));
		Debug.Log (canvas.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		Vector3[] position;
		// check for errors
		if (www.error == null)
		{
			PlayersContainer playersContainer = JsonUtility.FromJson<PlayersContainer> (www.text);
			Player[] players = playersContainer.result;
			/*for (int y = 0; y < players.Length; y++){ 
				for (int x = 0; x < players.Length; x++){ 
					Transform player = (Transform) Instantiate(tutorialIconPrefab, Vector3 (x, y, 0), Quaternion.identity); 
					player.transform.SetParent(canvas,false);
				} 
			}*/ 
			
			for (int i = 0; i < 5; i++) {
				position = new Vector3[players.Length];
				position [i] = new Vector3 (i * 100, i * 100, 0);
				//Transform player = (Transform) Instantiate (tutorialIconPrefab, position[i], transform.rotation);

				Transform player = (Transform)Instantiate (tutorialIconPrefab, position [i], Quaternion.identity); 

				player.transform.SetParent (canvas, true);
			}
		} else {
			Debug.Log("WWW Error: ");
		}    
	}
}
[Serializable]
public class PlayersContainer{
	public string status;
	public Player[] result;
}
[Serializable]
public class Player{
	public string id;
	public string name;
	public string path;
}
