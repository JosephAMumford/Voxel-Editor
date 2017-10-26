using UnityEngine;
using System.Collections;

public class SphericalCamera : MonoBehaviour {

	public float Theta;
	public float Phi;
	public float Radius;
	public float Speed;
	public Vector3 Position = new Vector3();
	public Vector3 Target;

	// Use this for initialization
	void Start () {
		UpdatePosition();
	}

	public void UpdatePosition(){
		if(Phi > 2*Mathf.PI){
			Phi = 0.0f;
		}
		if(Theta > 3.13f){
			Theta = 3.13f;
		}
		if(Theta < 0.01f){
			Theta = 0.01f;
		}
		Position.x = Target.x + Radius * Mathf.Sin(Theta) * Mathf.Cos(Phi);
		Position.y = Target.y + Radius * Mathf.Cos(Theta);
		Position.z = Target.z +  Radius * Mathf.Sin(Theta) * Mathf.Sin(Phi);

		gameObject.transform.position = Position;
		gameObject.transform.LookAt(Target);
	}

	// Update is called once per frame
	void Update () {

		if(Input.anyKey){
			float RotateCam  = Input.GetAxis("RotateCameraPhi") * Speed * Time.deltaTime;
			float MoveCam  = Input.GetAxis("RotateCameraTheta") * Speed * Time.deltaTime;
			Phi += RotateCam;
			Theta += MoveCam;
			UpdatePosition();
		}
		if(Input.GetKeyDown(KeyCode.LeftBracket)){
			Radius--;
			UpdatePosition();
		}
		if(Input.GetKeyDown(KeyCode.RightBracket)){
			Radius++;
			UpdatePosition();
		}
		if(Input.GetAxis("Mouse ScrollWheel") > 0){
			Radius--;
			UpdatePosition();
		}
		if(Input.GetAxis("Mouse ScrollWheel") < 0){
			Radius++;
			UpdatePosition();
		}
	}
}
