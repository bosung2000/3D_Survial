using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footStepClips;
    private AudioSource audilSource;
    private Rigidbody rb;
    public float footstepThreshold;
    public float footstepRate;
    private float footStepTime;

    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody>();
        audilSource= GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rb.velocity.y ) < 0.1f)
        {
            if (rb.velocity.magnitude >footstepThreshold)
            {
                if (Time.time- footStepTime > footstepRate)
                {
                    footStepTime= Time.time;
                    audilSource.PlayOneShot(footStepClips[Random.Range(0,footStepClips.Length)]);

                }
            }
        }
    }
}
