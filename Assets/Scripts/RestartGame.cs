using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RestartGame : MonoBehaviour {

	public void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
	}	
}