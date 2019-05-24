using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA{

	List<Vector3> targetGenes = new List<Vector3>();
	List<Vector3> towerRangeGenes = new List<Vector3> ();
	List<Vector3> towerGenes = new List<Vector3>();
	const int maxVal = 45;

	public DNA() {
		Init ();
	}

	//Initialing
	public void Init() {
		targetGenes.Clear ();
		towerRangeGenes.Clear ();
		towerGenes.Clear ();
		targetGenes.Add(new Vector3(0,0,0));
		towerGenes.Add(new Vector3(0,0,0));
	}

	//Combing the DNA of Parents
	public void Combine(DNA d1, DNA d2){
		if (Random.Range (0, 2) == 1) {
			targetGenes [0] = d1.GetTargetGene(0);
		} else {
			targetGenes [0] = d2.GetTargetGene(0);
		}
		if (Random.Range (0, 2) == 1) {
			towerRangeGenes = d1.GetTowerRGene ();
		} else {
			towerRangeGenes = d2.GetTowerRGene ();
		}
		if (Random.Range (0, 2) == 1) {
			towerGenes [0] = d1.GetTowerGene(0);
		} else {
			towerGenes [0] = d2.GetTowerGene(0);
		}
	}
		
	public void Mutate(){
		targetGenes [0] = new Vector3(Random.Range(-maxVal, maxVal), 0, Random.Range(-maxVal, maxVal));
		int count = Random.Range (0, 5);
		for (int i = 0; i < count; i++) {
			towerRangeGenes.Add(new Vector3 (Random.Range (-maxVal, maxVal), 0, Random.Range (-maxVal, maxVal)));
		}
		towerGenes [0] = new Vector3(Random.Range(-maxVal, maxVal), 0, Random.Range(-maxVal, maxVal));
	}

	public void SetTargetGene(Vector3 val){

		targetGenes[0] = val;
	}

	public Vector3 GetTargetGene(int pos){
		return targetGenes [pos];
	}

	public void SetTowerRGene(Vector3 val){
		if (val == Vector3.zero && towerGenes != null) {
			towerGenes.Clear ();
		}
		towerRangeGenes.Add (val);
	}

	public Vector3 GetTowerRGene(int pos){
		return towerRangeGenes [pos];
	}

	public List<Vector3> GetTowerRGene(){
		return towerRangeGenes;
	}

	public int GetTowerRGeneCount(){
		return towerRangeGenes.Count;
	}

	public void SetTowerGene(Vector3 val){
		towerGenes[0] = val;
		if (val == Vector3.zero) {
			Debug.Log (towerGenes[0]);
		}
	}

	public Vector3 GetTowerGene(int pos){ 
		return towerGenes [0];
	}

	public void Clear(){
		towerRangeGenes.Clear ();
		towerGenes.Clear ();
		towerGenes.Add(new Vector3(0,0,0));
	}
}
