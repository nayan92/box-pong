using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {
	public float speed = 5f;
	
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	private 
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = rigidbody.position;
			stream.Serialize(ref syncPosition);
			
			syncVelocity = rigidbody.velocity;
			stream.Serialize(ref syncVelocity);
		} else {
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = rigidbody.position;
		}
	}
	
	void Awake() {
		lastSynchronizationTime = Time.time;
	}
	
	void Update() {
		if (networkView.isMine) {
			InputMovement();
		} else {
			SyncedMovement();
		}
	}
	
	
	private void InputMovement() {
		if (Input.GetKey(KeyCode.W)) {
			rigidbody.MovePosition(rigidbody.position + transform.up * speed * Time.deltaTime);
//			transform.Translate(Vector3.up * speed * Time.deltaTime);
		}
		
		if (Input.GetKey(KeyCode.S)) {
			rigidbody.MovePosition(rigidbody.position - transform.up * speed * Time.deltaTime);
		}
		
		if (Input.GetKey(KeyCode.D)) {
			rigidbody.MovePosition(rigidbody.position + transform.right * speed * Time.deltaTime);
		}
		
		if (Input.GetKey(KeyCode.A)) {
			rigidbody.MovePosition(rigidbody.position - transform.right * speed * Time.deltaTime);
		}
	}
	
	private void SyncedMovement() {
		syncTime += Time.deltaTime;
		rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}
}
