using UnityEngine;
using System.Collections;

public class ParticleDisplayTest : MonoBehaviour {
	public CharacterController controller;
	
	private ParticleDisplay pd;
	private ParticleDisplay.Particle[] pdp;
    bool started = false;
    float startTime=0;
    int nth_snapshot = 0;
	void Start () {
		controller = GetComponent<CharacterController>();
		pd = GetComponent<ParticleDisplay>();
		pd.Play();
        print("Total elements in runningList = " + MarketDataParser.runningList.Count);
    }

    void Update () {
		float moveHorizontal = -5 * Input.GetAxis ("Horizontal");
		float moveVertical = -5 * Input.GetAxis ("Vertical");
		float move3rdAxis = -5 * Input.GetAxis("Mouse ScrollWheel");
		Vector3 movement = new Vector3 (moveHorizontal, 10 * move3rdAxis, moveVertical);
		controller.Move( movement );
        
        {
            if (!started)
            {
                started = true;
                startTime = Time.realtimeSinceStartup;
            }

            float diff = Time.realtimeSinceStartup - startTime-1;
            if (diff < 0)
                diff = 0; 
            float framerate = 10.0F;
            nth_snapshot = (int)( diff * framerate) % 240;
            pd.UpdateMeshes(nth_snapshot);
        }

	}
}
