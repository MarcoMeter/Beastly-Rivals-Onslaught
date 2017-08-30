using UnityEngine;
using System.Collections;

public class RotateParticles : MonoBehaviour
{

    // Member
    public float speed = 100f;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0, 0);
	}
}
