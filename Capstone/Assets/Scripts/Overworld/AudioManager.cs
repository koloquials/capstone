using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will be used for managing all audio related to overworld sound effects that are not related to a
/// character.null i.e. it will handle ambience and puzzle effects. It does NOT handle footsteps

/// This GameObject should have two AudioSources on it: One to manage constant overworld ambience, the other to play
/// shorter sounds at random intervals.
/// Use this script by setting the array size in the inspector and dragging in as many AudioClips desired

/// in the future, this can be used to pass different songs to the RhythmGameController! 
/// 
/// </summary>

public class AudioManager : MonoBehaviour
{
    private AudioSource masterAmbienceSrc;      //this gameObject will have two audio sources: one playing master background ambiencee (always looping)
    private AudioSource individualSoundsSrc;    //the other will be playing various sounds at random intervals 

    public AudioClip masterAmbienceClip;

    private AudioSource[] audioSources; 
    public AudioClip[] audioClips;              //set array size and assign clips via inspector

    // Start is called before the first frame update
    void Start()
    {
        audioSources = gameObject.GetComponents<AudioSource>();

        masterAmbienceSrc = audioSources[0];
        individualSoundsSrc = audioSources[1];

        StartCoroutine(PlayIndividualSounds());
    }  

    // Update is called once per frame
    void Update()
    {
        if (!masterAmbienceSrc.isPlaying) 
            masterAmbienceSrc.Play();
    }
    
    //the individual sounds will be randomly selected and played at random intervals. 
    IEnumerator PlayIndividualSounds() {
        AudioClip randomClip = GetRandomClip();                     //grab a random audioclip
        individualSoundsSrc.pitch = Random.Range(1f, 2f);           //give it a random pitch 
        individualSoundsSrc.volume = Random.Range(0.1f, 0.6f);      //give it a random volume

        individualSoundsSrc.PlayOneShot(randomClip);

        float randomWaitTime = Random.Range(5f, 13f);               //give a random wait time between individual sounds
            
        yield return new WaitForSeconds(randomWaitTime);

        StartCoroutine(PlayIndividualSounds());
    }

    AudioClip GetRandomClip() {
        int randomClip = Random.Range(0, audioClips.Length);

        return audioClips[randomClip];
    }
}
