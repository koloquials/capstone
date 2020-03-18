using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script will control the entire rhythm game: it takes care of key input, checking if a combination was correct, 
/// 
/// </summary>
public class RhythmGameController : MonoBehaviour
{
    //the rhythmGame script goes on an empty gameObject that also has the SimpleClock on it.
    //SimpleClock is a SINGLETON.
    //this script monitors events (keys pressed), generating all of the notes

    FiniteStateMachine<RhythmGameController> rhythmGameStateMachine;
    
    public GameObject notePrefab;

    //the first x# of notes for any song will be scripted
    List<string> phase1NotesCombinations = new List<string>() { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD", "LL", "RR" };

    //note combinations will be generated at random, but they will not appear with equal likelihood! 
    string[] mostLikelyCombos = { "UU", "DD", "LL", "RR" };                                 //these have a 40% chance of appearing
    string[] secondLikelyCombos = { "UL", "UR", "DL", "DR", "LU", "LD", "RU", "RD" };       //these have a 40% chance of appearing
    string[] leastLikelyCombos = { "UD", "DU", "LR", "RL" };                                //these have a 20% chance of appearing

    //store all the combinations for the 
    List<string> phase2NotesCombinations = new List<string>();

    //the song will be represented in a 2D array. The outer array representing the measure and the inner array 
    //representing the four beats. 
    GameObject[,] thisSong = new GameObject[77, 4];
    string[] thisSongSequence;          //array to hold the entire song's combinations ONLY. This has nothing to do with the
                                        //actual internal workings of the rhythm game, but just makes setting the fret easier

    public Sprite UU;
    public Sprite UD;
    public Sprite UL;
    public Sprite UR;
    public Sprite DU;
    public Sprite DD;
    public Sprite DL;
    public Sprite DR;
    public Sprite LU;
    public Sprite LD;
    public Sprite LL;
    public Sprite LR;
    public Sprite RU;
    public Sprite RD;
    public Sprite RL;
    public Sprite RR;

    //booleans to control moving between states of the rhythm game
    bool inPhase1 = false;      //this gets set to true when the IntroAnimation state has finished
    bool inPhase2 = false;      //this gets set to true when the player has hit the first 10 key combinations
                                //correctly and will move into Phase2 state

    public SimpleClock simpleClockScript;   //another script that is attached to the same object (not a child)
                                            //SimpleClock manages the song itself (beats, measures, playing it, etc...)


    public int currMeasure;
    public int currBeat;
    public int currTick;

    //UI element scripts and objects.
    public NewFretFeedback fretFeedbackScript;          //reference to the fret (important for swapping the sprite on the fret out)
    public Orbitter orbitterScript;
    public SpriteRenderer background;

    void Start()
    {
        //make and begin running the state machine
        rhythmGameStateMachine = new FiniteStateMachine<RhythmGameController>(this);
        rhythmGameStateMachine.TransitionTo<LoadRhythmGame>();

        //grab a reference to the clock
        simpleClockScript = gameObject.GetComponent<SimpleClock>();

        //generate the random combination for the second phase of the song and make the song into one string
        GenerateCombinations();
        SetThisSongSequence(phase1NotesCombinations, phase2NotesCombinations);
        GenerateNotes();
        

        //visual utility and feedback scripts
        fretFeedbackScript = transform.GetChild(0).gameObject.GetComponent<NewFretFeedback>();
        orbitterScript = transform.GetChild(1).gameObject.GetComponent<Orbitter>();
        background = transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();

        //fret will be self-sufficient
        fretFeedbackScript.SetPhase1Sequence(phase1NotesCombinations);
        fretFeedbackScript.SetPhase2Sequence(phase2NotesCombinations);

        fretFeedbackScript.SetSong(phase1NotesCombinations, phase2NotesCombinations);
    }

    // Update is called once per frame
    void Update()
    {
        //have to constantly know what measure, beat and tick we're on
        currMeasure = simpleClockScript.Measures;
        currBeat = simpleClockScript.Beats;
        // Debug.Log("currMeasure is: " + currMeasure + "currBeat is: " + currBeat);
        currTick = simpleClockScript.Ticks;

        //this will call the Update function of whatever state it is in. 
        rhythmGameStateMachine.Update();

        Test();
    }

    //generate the entire list of combinations first
    private void GenerateCombinations()
    {
        //oof, hard coded values
        int combosToGenerate = 154 - 10;
        string thisNotesCombo = "";

        for (int i = 0; i < combosToGenerate; i++)
        {
            int comboBias = Random.Range(0, 5);
            int getComboIndex = 0;

            if (comboBias == 0 || comboBias == 1)
            {
                getComboIndex = Random.Range(0, mostLikelyCombos.Length);
                thisNotesCombo = mostLikelyCombos[getComboIndex];
            }
            else if (comboBias == 2)
            {
                getComboIndex = Random.Range(0, secondLikelyCombos.Length);
                thisNotesCombo = secondLikelyCombos[getComboIndex];
            }
            else
            {
                getComboIndex = Random.Range(0, leastLikelyCombos.Length);
                thisNotesCombo = leastLikelyCombos[getComboIndex];
            }

            phase2NotesCombinations.Add(thisNotesCombo);

            thisNotesCombo = "";
        }
    }

    //initialisation function--all notes for a song are generated at the beginning as to improve
    //performance of the rhythm game as it runs
    private void GenerateNotes()
    {
        string thisNotesCombo = "";
        int combinationStepper = 0;

        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); j++)
            {
                //Starting index 0, we will not be pressing anything anything on the second and fourth beats
                //of a measure, so just put null at that space
                if (j == 1 || j == 3)
                {
                    thisSong[i, j] = null;
                }
                else
                {
                    GameObject newNote = Instantiate(notePrefab);
                    NewNote newNoteScript = newNote.gameObject.GetComponent<NewNote>();


                    //set the properties of each note 
                    newNoteScript.SetBeat(j + 1);               //must be +1 because the beats are counted from 1~4
                    newNoteScript.SetMeasure(i + 2);            //must be +2 because SimpleClock is set up so that the measures start counting at 2
                                                                //as soon as the song starts
                    //Debug.Log("trying to get combination: " + thisSongSequence[combinationStepper]);
                    newNoteScript.SetSprite(thisSongSequence[combinationStepper], transform.position);

                    //add the note to the 2D array, the combination to the correct combination list
                    thisSong[i, j] = newNote;
                    //thisSongsKeyCombos[i] = thisNotesCombo;

                    //newNoteScript.PrintEveryProperty();

                    thisNotesCombo = "";
                    combinationStepper++;
                }
            }
        }
    }

    //utility function: the song is one sequence, but it has two phases with notes that are handled differently. 
    //(scripted in phase 1, generated in phase 2). Function to put both lists together to make the entire song.
    void SetThisSongSequence(List<string> phase1NotesCombinations, List<string> phase2NotesCombinations)
    {
        phase1NotesCombinations.AddRange(phase2NotesCombinations);
        this.thisSongSequence = phase1NotesCombinations.ToArray();

        // Debug.Log("the song, in combinations, is this long: " + thisSongSequence.Length);
        // foreach (string combination in thisSongSequence)
            // Debug.Log("Setting the song sequence: " + combination);
    }

    public string GetArrowKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            return "U";
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            return "L";
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            return "D";
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            return "R";

        return "";
    }

    public string GetWASD()
    {
        if (Input.GetKeyDown(KeyCode.W))
            return "U";
        else if (Input.GetKeyDown(KeyCode.A))
            return "L";
        else if (Input.GetKeyDown(KeyCode.S))
            return "D";
        else if (Input.GetKeyDown(KeyCode.D))
            return "R";

        return "";
    }

    //have a function that looks at current beat and measure and returns the expected key combination
    //then have another function that checks if it was correct or not 
    private string GetExpectedCombination()
    {
        string expectedCombo = "";

        //this if statement is because sometimes SimpleClock returns a fifth beat and that was breaking everything
        //also, measures start counting at 2. 0 Measures is the song has not started playing yet. 
        if (currBeat < 5 && currMeasure > 0)
        {
            int expectedNoteBeat = currBeat;
            int expectedNoteMeasure = currMeasure;
            //if we're at the second beat in a measure, want to get the third beat, which is index 2
            if (currBeat == 2) {
                // Debug.Log("Tracking values from beat two: " + expectedNoteMeasure + " and " + expectedNoteBeat);
                expectedNoteMeasure = currMeasure;
                expectedNoteBeat = 2;
                
            }
            //if we're at the fourth beat in a measure, then to get the first beat of the next one (which is the one
            //expecting to be pressed), get index 0 for the NEXT measure)
            else if (currBeat == 4) {
                // Debug.Log("Tracking values from beat four: " + expectedNoteMeasure + " and " + expectedNoteBeat);
                expectedNoteMeasure = currMeasure + 1;
                expectedNoteBeat = 0;
            }

            expectedNoteMeasure -= 2;

            //must be -2, because the first measure of the song returned by SimpleClock is actually 2. 
            Debug.Log("this is what we are trying to extract from the song: " + expectedNoteMeasure + " and " + expectedNoteBeat);
            Debug.Log("Values according to the SimpleClock: " + currMeasure + " and " + currBeat);
            GameObject posInSong = thisSong[expectedNoteMeasure, expectedNoteBeat];
            
            if (posInSong != null)
            {
                expectedCombo = posInSong.gameObject.GetComponent<NewNote>().GetCombination();
                Debug.Log("Current note combination: " + expectedCombo);
            }
        }

        return expectedCombo;
    }

    //check if the keys the player pressed match what the expected combination at the given moment is
    private bool CombinationCheck(string pressedKeys, string expectedCombo)
    {
        if (pressedKeys.Equals(expectedCombo))
            return true;
        else
            return false;
    }


    //utility functions for handling the present state of the rhythm game 
    public void RestartRhythmGame()
    {
        //notes are not destroyed when they reach the goal, they just turn invisible and
        // teleport somewhere irrelevant, so restarting the rhythm game is just resetting their start position

        //this code is specifically for resetting phase1 of the game
        StopAllCoroutines();

        //Reset the clock 
        SimpleClock.Instance.ResetSong();

        //reset all notes.
        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); j++)
            {
                //index out of bounds exception somewhere :'( 
                if (thisSong[i, j] != null)
                {
                    NewNote thisNoteScript = thisSong[i, j].gameObject.GetComponent<NewNote>();
                    thisNoteScript.ResetNote(thisNoteScript.GetCombination(), transform.position, false);
                }
            }
        }
    }

    //used for starting the notes moving in phase 2
    //the entire song is stored in thisSong[,], INCLUDING the 10 scripted, static notes in phase1
    public IEnumerator StartNoteMovement()
    {
        //notes don't start moving until phase 2. Setting i to start at currMeasure and j at currBeat ensures that
        //the note objects for the first, scripted sequence don't start moving too quickly.
        for (int i = currMeasure; i < thisSong.GetLength(0); i++)
        {
            for (int j = currBeat; j < thisSong.GetLength(1); i++)
            {
                StartCoroutine(thisSong[i, j].gameObject.GetComponent<NewNote>().WaitAndMove(0f));
                Debug.Log("i value is: " + i + "j value is: " + j);

                //this will have to be changed to get the notes to launch at the right moment
                yield return new WaitForSeconds(SimpleClock.QuarterLength());
            }
        }
    }

    //miscellaneous debugging functions! 

    //Debug function to see what is populating the 2D array
    void Print2DArray()
    {
        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); j++)
            {
                Debug.Log("measure: " + i + " beat: " + thisSong[i, j]);
            }
        }
    }

    //debugging function to test starting note movement
    public void Test()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Entering phase one, calling coroutine");

            StartCoroutine(StartNoteMovement());
        }
    }

    //debugging function to see when we are in and out of the window. blue background means in window, black means out of window
    private void ChangeBackground(bool inWindow)
    {
        if (inWindow)
        {
            Color inWindowColour = Color.blue;
            inWindowColour.a = 0.65f;
            background.color = inWindowColour;
        }
        else
        {
            Color outOfWindowColour = Color.black;
            outOfWindowColour.a = 0.65f;
            background.color = outOfWindowColour;
        }
    }



    //State machine
    //before the rhythm game. This is preparation for loading up the rhythm game and leading into the animation      
    private class LoadRhythmGame : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            //accessing variables that are part of rhythm game controller
            //Context.thisSong;
            Debug.Log("Entering LoadRhythmGame state");
        }

        public override void Update()
        {
            //call transitionto, then onExit will run, then onenter for whatever transitionto will be run
            if (Input.GetKeyDown(KeyCode.P))
            {
                TransitionTo<IntroAnimation>();
            }
        }

        public override void OnExit()
        {

        }
    }

    //intro animation for the rhythm game. Player input cannot be taken.
    private class IntroAnimation : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            Debug.Log("Scaling the fret and orbitter");
            Context.StartCoroutine(Context.fretFeedbackScript.ScaleFret(2f));
            Context.StartCoroutine(Context.orbitterScript.ScaleOrbitter(2f));
        }

        public override void Update()
        {
            //GetScaleStatus will report when the fret has finished scaling to its final size. Once there, move on 
            //to phase 1 of the game.
            Context.inPhase1 = Context.fretFeedbackScript.GetScaleStatus();
            if (Context.inPhase1)
            {
                TransitionTo<Phase1>();
            }
        }

        public override void OnExit()
        {

        }
    }

    //phase 1 of rhythm game--press an x# of notes 
    private class Phase1 : FiniteStateMachine<RhythmGameController>.State
    {
        FiniteStateMachine<Phase1> phaseWindowStateMachine;
        private int noteCounter = 0;

        public override void OnEnter()
        {
            Debug.Log("starting phase 1");
            phaseWindowStateMachine = new FiniteStateMachine<Phase1>(this);
            phaseWindowStateMachine.TransitionTo<Resting>();
        }

        public override void Update()
        {
            phaseWindowStateMachine.Update();
            //if the noteCounter is past the first phase, transition into phase 2
            if (noteCounter >= Context.phase1NotesCombinations.Count) {
                TransitionTo<Phase2>();
            }
            // Debug.Log("This is the current expected combination: " + Context.GetExpectedCombination());
        }

        public override void OnExit()
        {

        }

        //state for handling starting the rhythm game. This is a special case.
        private class Resting : FiniteStateMachine<Phase1>.State
        {
            int startingPosition;               //start at the beginning of the 2D array which represents the song

            string pressedCombo = "";
            string expectedCombo = "";
            string pressedArrow;
            string pressedWASD;

            bool started; 
            float bufferTime;

            public override void OnEnter()
            {
                started = false;
                bufferTime = SimpleClock.BeatLength() / 2;
                Context.noteCounter = 0;                        //every time we enter the Resting phase, we want to be at the very beginning of the song
                Context.Context.fretFeedbackScript.SetFret(Context.Context.thisSongSequence[Context.noteCounter]);               
                // Debug.Log("Buffer time is: " + bufferTime);
            }
            public override void Update()
            {
                pressedArrow = Context.Context.GetArrowKeys();
                pressedWASD = Context.Context.GetWASD();
                expectedCombo = Context.Context.thisSongSequence[0];
                pressedCombo = pressedArrow + pressedWASD;

                //first combination was pressed correctly, game will otherwise stay resting the entire time.
                if (expectedCombo.Equals(pressedCombo))
                {
                    StartRhythmGame();
                }

                if (started) {
                    bufferTime -= Time.deltaTime;
                    // Debug.Log(bufferTime);
                }
                if (bufferTime <= 0) {
                    // Debug.Log("Buffer for offset special first case over and starting rhythm game");
                    TransitionTo<OutOfWindow>();
                }
            }

            //utility function for starting the rhythm game
            private void StartRhythmGame()
            {
                // Debug.Log(Context.Context.currMeasure + " and " + Context.Context.currBeat);
                Context.Context.simpleClockScript.FirstBeat();
                Context.noteCounter += 1;
                Context.Context.fretFeedbackScript.SetFret(Context.Context.thisSongSequence[Context.noteCounter]);
                started = true;
            }

            public override void OnExit()
            {
            }
        }

        //internal states of phase 1 
        private class InWindow : FiniteStateMachine<Phase1>.State
        {
            string pressedCombo = "";
            string expectedCombo = "";
            string pressedArrow;
            string pressedWASD;
            float windowLength;
            bool canPress;

            public override void OnEnter()
            {
                Debug.Log("Entering window");
                pressedCombo = "";
                expectedCombo = Context.Context.GetExpectedCombination();

                pressedArrow = "";
                pressedWASD = "";
                //each window is one beat long
                windowLength = SimpleClock.BeatLength();
                canPress = true;

                Context.Context.ChangeBackground(true);
            }

            public override void Update()
            {
                windowLength -= Time.deltaTime;         //decrement the time left 

                if (pressedArrow.Equals(""))           //if a pressed key has not yet been registered, 
                    pressedArrow = Context.Context.GetArrowKeys();          //then check for a pressed key
                if (pressedWASD.Equals("")) 
                    pressedWASD = Context.Context.GetWASD();
                //transition out of the window if it is over
                if (windowLength <= 0f)
                {
                    TransitionTo<OutOfWindow>();
                }
                // Debug.Log("We are currently at measure: " + Context.Context.currMeasure + " and at beat: " + Context.Context.currBeat);
            }

            public override void OnExit()
            {
                // Context.Context.fretFeedbackScript.RippleEffect();
                Debug.Log("Exiting window");
                pressedCombo = pressedArrow + pressedWASD;

                Context.noteCounter += 1;

                //set the next expected note
                Context.Context.fretFeedbackScript.SetFret(Context.Context.thisSongSequence[Context.noteCounter]);

                Debug.Log("this is what was expected: " + expectedCombo + "this was what was pressed: " + pressedCombo);

                if (Context.Context.CombinationCheck(pressedCombo, expectedCombo))
                {
                    //correct combination was pressed
                    Debug.Log("No strike");
                }
                else
                {
                    //restart the rhythm game
                    Debug.Log("Incorrect combination, restarting the game");
                    Context.Context.RestartRhythmGame();
                    TransitionTo<Resting>();
                }
            }
        }

        private class OutOfWindow : FiniteStateMachine<Phase1>.State
        {
            float outOfWindowLength;
            public override void OnEnter()
            {
                Debug.Log("outside of window, cannot press");
                outOfWindowLength = SimpleClock.BeatLength();
                Context.Context.ChangeBackground(false);
            }
            public override void Update()
            {
                outOfWindowLength -= Time.deltaTime;
                // Debug.Log("Time remaining until next window: " + outOfWindowLength);

                if (outOfWindowLength <= 0f)
                {
                    TransitionTo<InWindow>();
                }

            }

            public override void OnExit()
            {

            }
        }
    }

    //phase 2--notes will move across the screen and player has to hit them at the right time.
    private class Phase2 : FiniteStateMachine<RhythmGameController>.State
    {
        //phase 2 has strikes. The player gets to mess up 4 times and on the 5th, the game restarts
        private int strikes = 0;
        public GameObject lifeSprite1;
        public GameObject lifeSprite2;
        public GameObject lifeSprite3;
        public GameObject lifeSprite4;
        public GameObject lifeSprite5;
        GameObject[] lifeSprites;


        public override void OnEnter()
        {
            Debug.Log("entering phase 2");
            // lifeSprites = new GameObject[] {lifeSprite1, lifeSprite2, lifeSprite3, lifeSprite4, lifeSprite5};
            // foreach (GameObject lifeSprite in lifeSprites) {
            //     lifeSprite.SetActive(true);
            // }

            Debug.Log("Entering phase2 state");
            Context.StartCoroutine(Context.StartNoteMovement());
        }

        public override void Update()
        {
        }

        private void StrikeCheck()
        {
            if (strikes > 0)
            {
                lifeSprite5.SetActive(false);
            }
            if (strikes > 1)
            {
                lifeSprite4.SetActive(false);
            }
            if (strikes > 2)
            {
                lifeSprite3.SetActive(false);
            }
            if (strikes > 3)
            {
                lifeSprite2.SetActive(false);
            }
            if (strikes > 4)
            {
                lifeSprite1.SetActive(false);
            }
        }

        public override void OnExit()
        {

        }
    }

    private class ClosingAnimation : FiniteStateMachine<RhythmGameController>.State
    {
        public override void OnEnter()
        {
            Debug.Log("Start of Phase2 script, launching the note movements)");
        }
        public override void Update()
        {

        }

        public override void OnExit()
        {

        }
    }
}


