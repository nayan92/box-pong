using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	void Awake() {
		rigidbody.velocity = new Vector3(5, 4, 3);
	}
}
