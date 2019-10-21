using UnityEngine;
using System.Collections;

public class camScript : MonoBehaviour
{

	public GameObject player;

	Vector3 initialPos;
	Vector3 weightedDirect;

	float shakeTimer = 0;
	float thisMagnitude = 0.5f;

	bool screenShaking = false;

	public void Shake()
	{
		StartCoroutine("Screenshaker");
	}

	public IEnumerator Screenshaker()
	{

		float time = .15f;

		//shake camera
		while (time > 0.0f)
		{
			Debug.Log(time);
			Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -7.2f) + (Vector3)Random.insideUnitCircle + Vector3.back * -2.0f;
			time -= Time.deltaTime;
			yield return 0;
		}

		//return cam to normal pos
		Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -2.0f);
	}
}