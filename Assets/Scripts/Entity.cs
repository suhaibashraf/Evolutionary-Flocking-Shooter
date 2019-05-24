using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	public Vector3 position;
	public Vector3 velocity;
	public Vector3 acceleration;
	public GameObject deadPrefab;

	GameObject deadTank;
	Vector3 wanderTarget;

	public static float yGlobalAxis = 1f;

	public virtual void Die(){
		deadTank = Instantiate (deadPrefab, transform.position, Quaternion.identity);
		gameObject.SetActive(false);
	}

	public virtual void clean(){
		Destroy (deadTank);
	}
		
	protected void WrapAround(ref Vector3 vector, float min, float max){
		vector.x = WrapAroundFloat (vector.x, min, max);
		vector.y = WrapAroundFloat (vector.y, min, max);
		vector.z = WrapAroundFloat (vector.z, min, max);
	}


	protected float WrapAroundFloat(float value, float min, float max){
		if (value > max)
			value = min;
		else if (value < min)
			value = max;
		return value;
	}
}
