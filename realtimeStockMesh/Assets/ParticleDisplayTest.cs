using UnityEngine;
using System.Collections;

public class ParticleDisplayTest : MonoBehaviour {
    private const float Period = 1.0f;
	public CharacterController controller;
	
	private ParticleDisplay pd;
	private ParticleDisplay.Particle[] pdp;
    bool started = false;
    float startTime=0;
    int which = 0;
	void Start () {
		controller = GetComponent<CharacterController>();
		pd = GetComponent<ParticleDisplay>();
		pdp = pd.particles;
        
		for (int i = 0; i < pdp.Length; i++) {
			pdp[i].velocity = Random.insideUnitSphere;
			pdp[i].angularVelocity = Random.Range (0.0f, 180.0f);
			pdp[i].lifetime = Mathf.Infinity;
			//pdp[i].color = new Color(Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
		}
		pd.Play();
	}
	
	void Update () {
		float moveHorizontal = -10 * Input.GetAxis ("Horizontal");
		float moveVertical = -10 * Input.GetAxis ("Vertical");
		float move3rdAxis = -10 * Input.GetAxis("Mouse ScrollWheel");
		Vector3 movement = new Vector3 (moveHorizontal, 10 * move3rdAxis, moveVertical);
		controller.Move( movement );
        

        if (which < 1000) //pdp.Length)
        {
            if (!started)
            {
                started = true;
                startTime = Time.realtimeSinceStartup;
            }

            float diff = Time.realtimeSinceStartup - startTime;
            which =(int)( diff  / Period);
            if (which < 1000) //pdp.Length)
                pd.ModifyMeshes(which);
        }

	}
}
