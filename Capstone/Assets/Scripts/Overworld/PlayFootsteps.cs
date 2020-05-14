using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Small script that goes onto every object that needs to play a footstep sound. 
/// </summary>

//every object that has this script needs an AudioSource
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class PlayFootsteps : MonoBehaviour
{
    public AudioSource footstepsSrc;
    public AudioClip footstepsSound;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        footstepsSrc = gameObject.GetComponent<AudioSource>();
        footstepsSrc.clip = footstepsSound;

        animator = this.gameObject.GetComponent<Animator>();

        footstepsSrc.playOnAwake = false;
    }

    //call this coroutine to play the footstep sound
    //Note: the way that this is set up is it assumes that the clip is just a single footstep, not a track of x number of footsteps. 
    //waitTime is the time needed to make sure the footstep aligns with the animation and when the character's foot hits the ground
    public IEnumerator Footsteps(float waitTime) {
        footstepsSrc.PlayOneShot(footstepsSrc.clip);

        yield return new WaitForSeconds(waitTime);

        StartCoroutine(Footsteps(waitTime));
    }

    //overloading time!! 
    public void PlayAnimation(string boolToSet, bool status) {
        animator.SetBool(boolToSet, status);
    }
}
