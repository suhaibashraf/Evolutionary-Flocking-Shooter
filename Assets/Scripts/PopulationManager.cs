using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;

public class PopulationManager : MonoBehaviour {

	public GameObject TargetPrefab;
	public GameObject TankAIPrefab;
	public int populationSize = 4;
	List<GameObject> population = new List<GameObject>();
	public GameObject TowerPrefab;
	public int TowersNo = 1;
	List<GameObject> Defence = new List<GameObject>();
	public static float elapsed = 0;
	public float maxTime = 10.00f;
	
	int generation = 1;
	Vector3 centre;
	 
	GUIStyle guiStyle = new GUIStyle();
	void OnGUI() {
		guiStyle.fontSize = 25;
		guiStyle.normal.textColor = Color.white;
		GUI.BeginGroup (new Rect (10, 10, 250, 250));
		GUI.Box (new Rect (0, 0, 150, 150), "Stats", guiStyle);
		GUI.Label (new Rect(10, 25, 200, 30), "Generation #: " + generation, guiStyle);
		GUI.Label (new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", System.Math.Round(elapsed, 2) + "s"), guiStyle);
		GUI.EndGroup ();
	}

	// initializating GameObjects
	void Start () {
		Vector3 targetPos = new Vector3 (this.transform.position.x + (Random.Range(-10,10) * 3), this.transform.position.y, 30);
		Instantiate (TargetPrefab, targetPos, this.transform.rotation);
		for (int i = 0; i < populationSize; i++){
			Vector3 tankPos = new Vector3 (this.transform.position.x + (Random.Range(-10,10) * 4),
				this.transform.position.y,
				this.transform.position.z + (Random.Range (-40, -30)));
			GameObject a = Instantiate (TankAIPrefab, tankPos, this.transform.rotation);
			a.GetComponent<Agent> ().Init ();
			population.Add (a);
		}
		Vector3 towerPos = Vector3.zero;
		for (int i = 0; i < TowersNo; i++){
			Vector3 newPos = new Vector3 (this.transform.position.x + (Random.Range(-15,15) * 2),
				this.transform.position.y, 0);
			if (towerPos != newPos) {    // Preventing Towers to spawn at same position
				Defence.Add ((GameObject)Instantiate (TowerPrefab, towerPos, this.transform.rotation));
				towerPos = newPos;
			}
		}
	}

	GameObject Breed(GameObject parent1, GameObject parent2) {
		Vector3 startingPos = new Vector3 (this.transform.position.x + (Random.Range (-10, 10) * 4),
			this.transform.position.y,
			this.transform.position.z + (Random.Range (-40, -30)));
		GameObject offspring = Instantiate (TankAIPrefab, startingPos, this.transform.rotation);
		Agent a = offspring.GetComponent<Agent> ();
		if (Random.Range (0, 1000) == 1) {  //Mutate 1 of 1000s
			a.Init ();
			a.dna.Mutate ();
		} else {
			a.Init ();
			a.dna.Combine (parent1.GetComponent<Agent> ().dna, parent2.GetComponent<Agent> ().dna);
		}
		return offspring;
	}

	void BreedNewPopulation() {
		List<GameObject> sortedList = population.OrderBy (o => o.GetComponent<Agent> ().points).ToList ();
		population.Clear ();
		//breed upper half of sorted list
		for(int i = (int)(sortedList.Count/2.0f) - 1; i < sortedList.Count - 1; i++){
			population.Add (Breed (sortedList [i], sortedList [i + 1]));
			population.Add (Breed (sortedList [i + 1], sortedList [i]));
		}
		//destroy all parents and previous population
		for(int i = 0; i < sortedList.Count; i++){
			Destroy (sortedList [i]);
		}
	}
		
	void reachedTarget(){
		for (int i = 0; i < population.Count; i++) {
			if (population [i].GetComponent<Agent> ().GetStop()) { 
				population [i].GetComponent<Agent> ().dna.SetTargetGene (population [i].transform.position);
				population [i].GetComponent<Agent> ().AddPoints(3);
			}
		}
	}

	void MissileHit(){
		for (int i = 0; i < population.Count; i++) {
			if (population [i].GetComponent<Agent> ().GetMissileHit()) {
				population [i].GetComponent<Agent> ().dna.SetTowerRGene (population [i].transform.position);
				population [i].GetComponent<Agent> ().AddPoints(1);
			}
		}
	}
		
	//Calculating the Position of Tower 
	void TowerOrigin(){ 
		List<Vector3> pos = new List<Vector3> ();
		Vector3 centre = Vector3.zero;
		for (int i = 0; i < population.Count; i++) {
			if (population [i].GetComponent<Agent> ().GetMissileHit()) {
				pos.Add (population [i].transform.position); //Geting the positions of dead tanks around the tower
			}
		}
		if (pos.Count > 2) {
			Vector3 deltaA = new Vector3 (pos[1].x -  pos[0].x, 0, pos[1].z -  pos[0].z);
			Vector3 deltaB = new Vector3 (pos[2].x -  pos[1].x, 0, pos[2].z -  pos[1].z);

			//Calculating the centre of circle
			float aSlope = deltaA.z / deltaA.x;
			float bSlope = deltaB.z / deltaB.x;
			float centreX = (aSlope * bSlope * (pos [0].y - pos [2].y) + bSlope * (pos [0].x + pos [1].x) 
				- aSlope * (pos [1].x + pos [2].x)) / (2 * (bSlope - aSlope));
			float centreY = -1 * (centreX - (pos[0].x + pos[1].x)/2) / aSlope + (pos[0].y + pos[1].y) / 2;

			centre = new Vector3 (centreX,0 , centreY);
		}
		if (centre != Vector3.zero) {
			for (int i = 0; i < population.Count; i++) {
				if (population [i].GetComponent<Agent> ().GetMissileHit()) {
					population [i].GetComponent<Agent> ().dna.SetTowerGene (centre);
					population [i].GetComponent<Agent> ().AddPoints(2);
				}
			}
		}
	}

	bool Check(){ //Detect if all tanks are stop or dead
		for (int i = 0; i < population.Count; i++) {
			if (population[i].activeInHierarchy && !population[i].GetComponent<Agent>().GetStop())
				return false;
		}
		return true;
	}

	void DestroyTower (){  
		for (int i = 0; i < population.Count; i++) {
			if (population [i].GetComponent<Agent> ().GetTowerHit() && Defence.Count != 0) {
				for (int j = 0; j < Defence.Count; j++) {
					if (Defence[j] != null && 
					Vector3.Distance (Defence [j].transform.position, population [i].GetComponent<Agent> ().TowerLocation ()) < 14.0f) {
						Destroy (Defence [j]);
						Defence.Remove (Defence[j]);
						break;
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		elapsed += Time.deltaTime;
		DestroyTower ();
		if(elapsed > maxTime || Check()){
			MissileHit ();
			reachedTarget ();
			TowerOrigin ();
			BreedNewPopulation ();
			elapsed = 0;
			generation++;
		}
	}
}