using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour {
	public ParticleSystem particlePrefab;

	void OnCollisionEnter(Collision collision) {
		ContactPoint contact = collision.contacts[0];
		Instantiate(particlePrefab, contact.point, Quaternion.FromToRotation(-Vector3.forward, contact.normal));
	}
}
