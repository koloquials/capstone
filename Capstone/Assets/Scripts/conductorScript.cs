using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class conductorScript : MonoBehaviour
{
    //add feedback; circle as initial loop, break through & add score threshhold (in form of hp bar)



    public static conductorScript instance;

    //beats per minute
    public float sBPM;
   
    //number of seconds per beat!!
    public float secondsPerBeat;

    //track song pos in seconds
    public float songPos;

    //track song pos in beats
    public float songPosinBeats;

    //seconds passed since song began
    public float dspSongTime;

    //musique,,,,,,
    public AudioSource music;

    //NOTE-- bc wii shop theme has no beat offset, this is currently hidden
    //enter offset manually & enter so beat tracking runs properly
    //public float offset;

    //all pos in beats of song
    public float[] notes;

    //next spawned note
    int nextIndex = 0;

    public GameObject notePrefab;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();

        //calculate number of seconds per beat
        secondsPerBeat = 60f / sBPM;

        //record time @ start
        dspSongTime = (float)AudioSettings.dspTime;

        //music start!!
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //track seconds since song began
        songPos = (float)(AudioSettings.dspTime - dspSongTime);

        //track seconds since song began + OFFSET
        //songPos = (float)(AudioSettings.dspTime - dspSongTime - offset);

        //track beats
        //note !! beat starts at 0, so beats of songs will be one beat behind where they ought to
        songPosinBeats = songPos / secondsPerBeat;
        //Debug.Log (songPosinBeats);
    }
}
