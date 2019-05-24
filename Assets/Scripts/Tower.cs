using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Tower : MonoBehaviour {
	public float timer = 0;
	public Rigidbody range;

	public Gun gun;
	// Use this for initialization
	void Start () {

	}
	// Update is called once per frame
	void Update(){
	}

	void FixedUpdate () {
		timer += Time.deltaTime;
		if (timer >= 1) {
			timer = 0;
		}

	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag.Equals ("Agent")) {
			gun.transform.LookAt (collision.transform.position);
			gun.Shoot();
		}
	}

	public virtual void Die(){
		Destroy(gameObject);
	}
}