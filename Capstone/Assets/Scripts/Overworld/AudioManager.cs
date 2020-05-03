using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Usage: AudioManager is a Singleton, so it can be called from any script in the scene with AudioManager.Instance

/// This script will be used for managing all audio related to overworld sound effects that are not related to a
/// character.null i.e. it will handle ambience and puzzle effects. It does NOT handle footsteps

/// This GameObject should have two AudioSources on it: One to manage constant overworld ambience, the other to play
/// shorter sounds at random intervals.
/// Use this script by setting the array size in the inspector and dragging in as many AudioClips desired

/// in the future, this can be used to pass different songs to the RhythmGameController! 
/// 

/// The object on which this script goes will likely have a lot of AudioSources, just make sure that they are 
/// accessed correctly
/// </summary>

public class AudioManager : MonoBehaviour
{
    //AudioManager is a singleton
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    private AudioSource masterAmbienceSrc;      //this gameObject will have two audio sources: one playing master background ambiencee (always looping)
    private AudioSource individualSoundsSrc;    //the other will be playing various sounds at random intervals 

    private AudioSource dialogueRunnerSrc;      //this source is played when dialogue is being displayed. 
    private AudioSource puzzleSoundSrc;         //this source will play sound effects for the puzzles

    private AudioSource[] audioSources; 
    public AudioClip[] audioClips;              //set array size and assign clips via inspector

    private Dictionary<string, AudioClip> puzzleSoundsDict;     //all sound effects for the puzzles will be stored and accessed via a dictionary

    void Awake() {
        //singleton pattern
        if (_instance != null && _instance != this) 
            Destroy(this.gameObject);
        else 
            _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSources = gameObject.GetComponents<AudioSource>();

        masterAmbienceSrc = audioSources[0];
        individualSoundsSrc = audioSources[1];
        dialogueRunnerSrc = audioSources[2];
        puzzleSoundSrc = audioSources[3];

        puzzleSoundsDict = gameObject.GetComponent<SoundFxDictionary>().GetDictionary();        //SoundFxDictionary will generate the dictionary 

        StartCoroutine(PlayIndividualSounds(Random.Range(5f, 8f)));     //first time calling this will have shorter wait time
    }  

    // Update is called once per frame
    void Update()
    {
        if (!masterAmbienceSrc.isPlaying) 
            masterAmbienceSrc.Play();
    }

    //Tell all overworld sound effects to stop playing. Just disables them, does not tell them to stop playing. Call this for the rhythm game.
    public void ControlAmbience(bool enabled) {
        foreach(AudioSource src in audioSources) {
            Debug.Log("Stopping ambience " + enabled);
            src.enabled = enabled;
        }
    }
    
    //the individual sounds will be randomly selected and played at random intervals. 
    IEnumerator PlayIndividualSounds(float waitTime) {
        yield return new WaitForSeconds(waitTime);

        AudioClip randomClip = GetRandomClip();                     //grab a random audioclip
        individualSoundsSrc.pitch = Random.Range(1f, 2f);           //give it a random pitch 
        individualSoundsSrc.volume = Random.Range(0.1f, 0.6f);      //give it a random volume

        individualSoundsSrc.PlayOneShot(randomClip);

        float randomWaitTime = Random.Range(5f, 13f);               //give a random wait time between individual sounds


        StartCoroutine(PlayIndividualSounds(randomWaitTime));
    }

    AudioClip GetRandomClip() {                 //helper function to generate random ambience clip
        int randomClip = Random.Range(0, audioClips.Length);

        return audioClips[randomClip];
    }

    //Dialogue running scripts can call this function 
    //pass it true to tell it to run dialogue sound. pass false to make it stop.
    public void PlayDialogueSound(bool running) {
        if (running) {
            if (!dialogueRunnerSrc.isPlaying)
                dialogueRunnerSrc.Play();
        }
        else 
            dialogueRunnerSrc.Stop();
    }

    //not entirely sure if this is the best way to implement this, lol
    //pass it a string which will let it know which soundeffect to call.
    public void PlayPuzzleSound(string clipName) {
        AudioClip clipToPlay = puzzleSoundsDict[clipName];          //get desired sound from dictionary

        puzzleSoundSrc.PlayOneShot(clipToPlay);                     //play it once
    }
}
