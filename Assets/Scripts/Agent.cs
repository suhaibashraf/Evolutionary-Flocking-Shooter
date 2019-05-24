using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Agent : Entity {
	

	public DNA dna = new DNA ();
	public int points;
	public Gun gun;
	public int speed = 20;

	public Vector3 target = Vector3.zero;

	private bool stop = false;
	private bool hitTower = false;
	private bool missileHit = false;
	private int rotationSpeed = 3;
	private float attackDuration = 6;
	private Quaternion lookRotation;
	private Vector3 direction;

	void Start(){
		target = dna.GetTargetGene (0);
		transform.Rotate (0,Random.Range(-90,90),0);
	}

	void Update(){
		Movement ();
	}

	public void Init() {
		dna = new DNA ();
		missileHit = false;
		hitTower = false;
		stop = false;
	}

	public void SetTarget(Vector3 pos){
		target = pos;
	}

	void faceTarget(){
		if (target != Vector3.zero) {
			direction = (target - transform.position).normalized;
			lookRotation = Quaternion.LookRotation (direction);
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
		}
	}

	void Movement() {
		if (!stop) {
			SideDetection ();
			TowerDetection ();
			faceTarget ();
			transform.Translate (Vector3.forward * speed * Time.deltaTime);
		}
	}

	void TowerDetection (){
		for (int i = 0; i < dna.GetTowerRGeneCount(); i++){
			if (Vector3.Distance (transform.position, dna.GetTowerRGene (i)) < 10.0f) {
				transform.Rotate (0,Random.Range(90,180),0);
				transform.position = new Vector3(transform.position.x-1,0,transform.position.z-1);
			}
		}
		if (dna.GetTowerGene (0) != Vector3.zero) {
			attackDuration += Time.deltaTime;
			if (attackDuration > 5 && hitTower == false) {
				attack ();
				attackDuration = 0;
			}
		}
	}

	void attack(){
		if (dna.GetTowerGene (0) != Vector3.zero) {
			if (Vector3.Distance (transform.position, dna.GetTowerGene (0)) < 30.0f) {
				gun.transform.LookAt (dna.GetTowerGene (0));
				gun.Shoot ();
				hitTower = true;
				dna.Clear ();
				for (int i = 0; i < 4; i++) {
					gameObject.transform.GetChild (1).gameObject.transform.GetChild (0)
						.gameObject.transform.GetChild (i).gameObject.GetComponent<Renderer> ().material.color = Color.red;
				}
			}
		}

	}

	void SideDetection() {
		if (transform.position.z > 40 || transform.position.z < -40 || transform.position.x > 40 || transform.position.x < -40) {
			if (transform.position.z > 40) {
				transform.position = new Vector3(transform.position.x,0,39);
			}
			if (transform.position.z < -40) {
				transform.position = new Vector3(transform.position.x,0,-39);
			}
			if (transform.position.x > 40) {
				transform.position = new Vector3(39,0,transform.position.z);
			}
			if (transform.position.x < -40) {
				transform.position = new Vector3(-39,0,transform.position.z);
			}
			transform.Rotate (0,Random.Range(90,180),0);
			if (target == Vector3.zero)
				faceTarget();
		}
	}
		
	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag.Equals ("Target")) {
			stop = true;
			for (int i = 0; i < 4; i++) {
				gameObject.transform.GetChild (1).gameObject.transform.GetChild (0)
					.gameObject.transform.GetChild (i).gameObject.GetComponent<Renderer> ().material.color = Color.green;
			}
		}
		if (collision.gameObject.tag.Equals ("AttackRange")) {
			missileHit = true;
			Die ();

		}
	}

	void OnCollisionExit(Collision collision){
		if (target == Vector3.zero)
			faceTarget();
	}

	public void AddPoints(int p){
		points += p;
	}

	public Vector3 TowerLocation(){
		return dna.GetTowerGene (0);
	}

	public bool GetStop(){
		return stop;
	}

	public bool GetMissileHit(){
		return missileHit;
	}

	public bool GetTowerHit(){
		return hitTower;
	}

	void OnDestroy(){
		clean ();
	}
}
