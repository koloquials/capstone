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

    //length of the song in beats (wii shop has 97*3)
    public int songLenghtinBeats;

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

    bool playing = false; //Whether the song is playing

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();

        //calculate number of seconds per beat
        secondsPerBeat = 60f / sBPM;

        //startMusic();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //track seconds since song began
        //songPos = (float)(AudioSettings.dspTime - dspSongTime);
        if(playing)
            songPos = (float)(Time.time - dspSongTime);

        //track seconds since song began + OFFSET
        //songPos = (float)(AudioSettings.dspTime - dspSongTime - offset);

        //track beats
        //note !! beat starts at 0, so beats of songs will be one beat behind where they ought to
        songPosinBeats = songPos / secondsPerBeat;
        //Debug.Log (songPosinBeats);
    }

    public void startMusic() //Start the song
    {

        //record time @ start
        //dspSongTime = (float)AudioSettings.dspTime;
        dspSongTime = Time.time; //AudioSettings.dspTime did not work well with restarting the song, trying this instead

        //music start!!
        music.Play();
        playing = true;
    }

    public void stopMusic() //Stop and reset the song
    {
        music.Stop();
        songPosinBeats = 0;
        songPos = 0;
        playing = false;
    }
}
