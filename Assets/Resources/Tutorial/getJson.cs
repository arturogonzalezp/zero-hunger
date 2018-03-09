using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using Vuforia;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*public class LoadWWW : MonoBehaviour
{

	// Use this for initialization
	IEnumerator Start()
	{
		WWW www = new WWW("http://www.dailytrend.mx/media/bi/styles/gallerie/public/images/2017/03/alvcover.jpg");
		while (!www.isDone)
			yield return null;
		Debug.Log(www.texture.name);
		GameObject rawImage = GameObject.Find("cacaca");
		rawImage.GetComponent<RawImage>().texture = www.texture;
	}
}*/




public class getJson : MonoBehaviour {
	private string url = "http://zero-hunger-api.herokuapp.com/get/tutorials";
	public GameObject tutorialIconPrefab;
	public Transform canvas;
	private string urlIcon = "http://www.dailytrend.mx/media/bi/styles/gallerie/public/images/2017/03/alvcover.jpg";

	// Use this for initialization
	void Start () {
		VuforiaBehaviour.Instance.enabled = false;
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
			int x = -95,y = 1200, j = 0;
			position = new Vector3[players.Length];
			for (int i = 0; i < players.Length; i++,j++) {
				if (j == 3) {
					y -= 330;
					x = -95;
					j = 0;
				}
				Debug.Log(players[i].imageUrl);
				urlIcon = players [i].imageUrl;
				string scene = players [i].id;
				WWW wwwIcon = new WWW (urlIcon);
				while (!wwwIcon.isDone){
					yield return wwwIcon;
					Debug.Log(wwwIcon.texture.name);
					tutorialIconPrefab.GetComponentInChildren<RawImage>().texture = wwwIcon.texture;
				}
				position [i] = new Vector3 (x+=330,  y, 0);
				//Transform player = (Transform) Instantiate (tutorialIconPrefab, position[i], transform.rotation);
				GameObject player = (GameObject)Instantiate (tutorialIconPrefab, position [i], Quaternion.identity);
				player.GetComponent<Button>().onClick.AddListener (() => changeScene(scene));
				player.transform.SetParent (canvas, true);
				player.transform.position = position [i];
				Debug.Log("X height: "+this.canvas.localScale.x);

			}
		} else {
			Debug.Log("WWW Error: ");
		}    
	}
	void changeScene(String scene){
		SceneManager.LoadScene (scene);
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
	public string imageUrl;
}
