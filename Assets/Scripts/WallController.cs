using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour {
	public ParticleSystem particlePrefab;

	// To identify which wall we are.
	public Faces wallPosition;

	// Reference to the game manager;
	public GameManager gameManager;

	// Event handler for a collision with this wall.
	void OnCollisionEnter(Collision collision) {
		// Some nice particle effects for ball collision
		ContactPoint contact = collision.contacts[0];
		Instantiate(particlePrefab, contact.point, Quaternion.FromToRotation(-Vector3.forward, contact.normal));

		// Notify gameManager that there was a collision with this wall.
		gameManager.handleWallHit (wallPosition);
	}
}
