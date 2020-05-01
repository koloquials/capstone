using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will be used for managing all audio related to overworld sound effects that are not related to a
/// character.null i.e. it will handle ambience and puzzle effects. It does NOT handle footsteps

/// in the future, this can be used to pass different songs to the RhythmGameController! 
/// 
/// </summary>

public class AudioManager : MonoBehaviour
{
    public AudioSource masterAmbienceSrc;      //this gameObject will have two audio sources: one playing master background ambiencee (always looping)
    public AudioSource individualSoundsSrc;    //the other will be playing various sounds at random intervals 

    public AudioClip masterAmbienceClip;
    public AudioClip echoDropClip;
    public AudioClip waterDropletsClip;

    public AudioSource[] audioSources;
    public AudioClip[] audioClips;

    public int randomSelection;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = gameObject.GetComponents<AudioSource>();

        masterAmbienceSrc = audioSources[0];
        individualSoundsSrc = audioSources[1];

        audioClips = new AudioClip[] { echoDropClip, waterDropletsClip };
    }  

    // Update is called once per frame
    void Update()
    {
        if (!masterAmbienceSrc.isPlaying) 
            masterAmbienceSrc.Play();

        //play a random individual clip at random intervals 
        if (!individualSoundsSrc.isPlaying)
            randomSelection = Random.Range(0, 100);

        if (randomSelection <= 10 && !individualSoundsSrc.isPlaying) {
            Debug.Log("Playing a random sound");
            AudioClip randomClip = GetRandomClip();
            individualSoundsSrc.clip = randomClip;
            individualSoundsSrc.Play();
        }
    }

    AudioClip GetRandomClip() {
        int randomClip = Random.Range(0, audioClips.Length);

        return audioClips[randomClip];
    }
}
