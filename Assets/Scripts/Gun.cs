using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class Gun : MonoBehaviour {

	public LayerMask collisionMask;
	public float damage = 5;
	public Transform spawn;
	public Transform shellEjectionPoint;
	public Rigidbody shell;
	public float shotDistance = 15;
	public Light faceLight;	

	private LineRenderer tracer;

	ParticleSystem gunParticles;  
	Light gunLight;  

	void Start(){
		if (GetComponent<LineRenderer> ()) {
			tracer = GetComponent<LineRenderer>();
		}
		if (GetComponent<ParticleSystem> ()) {
			gunParticles = GetComponent<ParticleSystem> ();
		}
		if (GetComponent<Light> ()) {
			gunLight = GetComponent<Light> ();
		}
	}
		
	public void Shoot(){
		gunLight.enabled = true;
		faceLight.enabled = true;
		gunParticles.Stop ();
		gunParticles.Play ();
		Ray ray = new Ray (spawn.position, spawn.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, shotDistance, collisionMask)) {
			shotDistance = hit.distance;
			if(hit.collider.GetComponent<Entity>()){
			}
		}
		Debug.DrawRay(ray.origin,ray.direction * shotDistance,Color.red,1);
		AudioSource audio = GetComponent<AudioSource>();
		audio.Play ();
		if (tracer) {
			StartCoroutine("RenderTracer",ray.direction*shotDistance);
		}
		Rigidbody newShell = Instantiate (shell, shellEjectionPoint.position, Quaternion.identity) as Rigidbody;
		newShell.AddForce (shellEjectionPoint.right * Random.Range (150f, 200f) + spawn.forward * Random.Range (-10f, 10f));
	}

	IEnumerator RenderTracer(Vector3 hitPoint){
		tracer.enabled = true;
		tracer.SetPosition(0,spawn.position);
		tracer.SetPosition (1, spawn.position + hitPoint);
		yield return null;
		tracer.enabled = false;
		gunParticles.Stop ();
		gunLight.enabled = false;
		faceLight.enabled = false;
	}
}
