using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CursorControl : MonoBehaviour {

	public GameObject[] cursor;
	public Vector2[] cursormovevector = new Vector2[4];
	public bool[] iscursorconnected = {false, false, false, false};
	public GameObject OptionPrefab;

	public NetworkController NC;

	void OnLevelWasLoaded(){
		cursor [0] = GameObject.Find ("Cursor");
		cursor [1] = GameObject.Find ("Cursor2");
		cursor [2] = GameObject.Find ("Cursor3");
		cursor [3] = GameObject.Find ("Cursor4");
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
	}

	public void move(Vector2 force, int playernumber){
		cursormovevector [playernumber] = force;
	}

	public void click(int playernumber){
		Ray ray = Camera.main.ScreenPointToRay(cursor [2].transform.position);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, Mathf.Infinity);

		if (hit.collider == null)
			return;
		
		if (hit.collider.tag == "BUTTON")
			hit.collider.GetComponent<ButtonAction> ().clicked ();
	}

	public void connected(int playernumber){
		iscursorconnected [playernumber] = true;
    }

	// Update is called once per frame
	public void Update () {
		for (int i = 0; i < 4; ++i) {
			if (iscursorconnected [i]) {
				cursor [i].transform.position += new Vector3 (cursormovevector [i][0], cursormovevector [i][1]) * Time.fixedDeltaTime;
				Mathf.Clamp (cursor [i].transform.position.x, 0, 1920);
				Mathf.Clamp (cursor [i].transform.position.y, 0, 1080);
			}
		}

		for (int i = 0; i < 4; ++i) {
			if (iscursorconnected [i])
				click (i);
		}


	}

}
