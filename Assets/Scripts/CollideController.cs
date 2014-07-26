using UnityEngine;
using System.Collections;

public class CollideController : MonoBehaviour {

	private ParticleSystem ps;

	void Start () {
		ps = GetComponent<ParticleSystem>();
	}
	
	void Update () {
		if(light.intensity > 0) {
			light.intensity -= 0.05f;
		}

		if(ps && !ps.IsAlive()) {
			Destroy(gameObject);
		}
	}
}
