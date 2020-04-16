using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on an empty GameObject. The object will start inactive and is set to active when the player enters the rhythm game state 
/// 
/// </summary>
public class RhythmGameController : MonoBehaviour {
    //the rhythmGame script goes on an empty gameObject that also has the SimpleClock on it.
    //SimpleClock is a SINGLETON.
    //this script monitors events (keys pressed), generating all of the notes

    FiniteStateMachine<RhythmGameController> rhythmGameStateMachine;

    public Yarn.Unity.Example.CameraFollow cam;

    public GameObject notePrefab;

    //the first x# of notes for any song will be scripted
    List<string> notesCombinations = new List<string>() { "UU", "DD", "LL", "RR", "UD", "LR", "UU", "DD", "LL", "RR" };

    //note combinations will be generated at random, but they will not appear with equal likelihood! 
    string[] mostLikelyCombos = { "UU", "DD", "LL", "RR" }; //these have a 40% chance of appearing
    string[] secondLikelyCombos = { "UL", "UR", "DL", "DR", "LU", "LD", "RU", "RD" }; //these have a 40% chance of appearing
    string[] leastLikelyCombos = { "UD", "DU", "LR", "RL" }; //these have a 20% chance of appearing

    //the song will be represented in a 2D array. The outer array representing the measure and the inner array 
    //representing the four beats. 
    GameObject[,] thisSong = new GameObject[77, 4];
    string[] thisSongSequence; //array to hold the entire song's string combinations ONLY. This has nothing to do with the actual internal workings of the rhythm game, but just makes setting the fret easier

    //booleans to control moving between states of the rhythm game
    bool inPhase1 = false; //this gets set to true when the IntroAnimation state has finished
    public int phase1Threshold;

    //Simpleclock starts 
    public int currMeasure;
    public int currBeat;

    //UI element scripts and objects.
    public NewFretFeedback fretFeedbackScript; //reference to the fret (important for swapping the sprite on the fret out)
    public Orbitter orbitterScript;
    public SpriteRenderer background;
    public Transform noteObjectsParent;
    public bool gameEnded = false;

    public ScaleObject backgroundScaler;
    public GameObject lifeSprite1; //Sprites for your five lives. Should start disabled.
    public GameObject lifeSprite2;
    public GameObject lifeSprite3;
    public GameObject lifeSprite4;
    public GameObject lifeSprite5;

    public GameObject[] lifeSprites;

    void Start() {
        Debug.Log("This should only be called once!");

        lifeSprites = new GameObject[] {lifeSprite1, lifeSprite2, lifeSprite3, lifeSprite4, lifeSprite5};

        //make and begin running the state machine
        rhythmGameStateMachine = new FiniteStateMachine<RhythmGameController>(this);
        rhythmGameStateMachine.TransitionTo<IntroAnimation>();

        noteObjectsParent = transform.GetChild(3).gameObject.GetComponent<Transform>();

        //visual utility and feedback scripts
        fretFeedbackScript = transform.GetChild(0).gameObject.GetComponent<NewFretFeedback>();
        orbitterScript = transform.GetChild(1).GetChild(0).gameObject.GetComponent<Orbitter>();
        // backgroundScript = transform.GetChild(2).gameObject.GetComponent<Background>();
        background = transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
        backgroundScaler = background.gameObject.GetComponent<ScaleObject>();

        //generate the random combination for the second phase of the song and make the song into one string
        GenerateCombinations();

        // SetThisSongSequence(phase1NotesCombinations, phase2NotesCombinations);

        this.thisSongSequence = notesCombinations.ToArray();
    
        GenerateNotes();

        Debug.Log("This song is this many notes long: " + thisSongSequence.Length);
    }

    void OnEnable() {
        if (rhythmGameStateMachine != null) {
            Debug.Log("Enabling");
            rhythmGameStateMachine.TransitionTo<IntroAnimation>();
        }
    }

    void OnDisable() {
        gameEnded = false;
    }

    // Update is called once per frame
    void Update() {
        //have to constantly know what measure, beat and tick we're on
        //SimpleClock starts counting from measure 2 and beat 1. currMeasure and currBeat always account for this offset and give us the values that we will need in order to index correctly from
        //thisSong[,]
        currMeasure = SimpleClock.Instance.Measures - 2;
        currBeat = SimpleClock.Instance.Beats - 1;

        rhythmGameStateMachine.Update();

        Test();
    }

    //generate the entire list of combinations first
    //so much SPAGHETT
    private void GenerateCombinations() {
        //oof, hard coded values
        int combosToGenerate = 154 - 10;
        string thisNotesCombo = "";

        for (int i = 0; i < combosToGenerate; i++) {
            int comboBias = Random.Range(0, 5);
            int getComboIndex = 0;

            if (comboBias == 0 || comboBias == 1) {
                getComboIndex = Random.Range (0, mostLikelyCombos.Length);
                thisNotesCombo = mostLikelyCombos[getComboIndex];
            } else if (comboBias == 2) {
                getComboIndex = Random.Range (0, secondLikelyCombos.Length);
                thisNotesCombo = secondLikelyCombos[getComboIndex];
            } else {
                getComboIndex = Random.Range (0, leastLikelyCombos.Length);
                thisNotesCombo = leastLikelyCombos[getComboIndex];
            }

            notesCombinations.Add(thisNotesCombo);

            thisNotesCombo = "";
        }
    }

    //initialisation function--all notes for a song are generated at the beginning as to improve
    //performance of the rhythm game as it runs
    private void GenerateNotes() {
        string thisNotesCombo = "";
        int combinationStepper = 0;

        for (int i = 0; i < thisSong.GetLength (0); i++) {
            for (int j = 0; j < thisSong.GetLength (1); j++) {
                //Starting index 0, we will not be pressing anything anything on the second and fourth beats
                //of a measure, so just put null at that space
                if (j == 1 || j == 3) {
                    thisSong[i, j] = null;
                } else {
                    GameObject newNote = Instantiate(notePrefab, noteObjectsParent);
                    NewNote newNoteScript = newNote.gameObject.GetComponent<NewNote>();

                    //set the properties of each note 
                    newNoteScript.SetBeat (j + 1); //must be +1 because the beats are counted from 1~4
                    newNoteScript.SetMeasure (i + 2); //must be +2 because SimpleClock is set up so that the measures start counting at 2
                    //as soon as the song starts
                    //Debug.Log ("trying to get combination: " + thisSongSequence[combinationStepper]);
                    newNoteScript.SetSprite(thisSongSequence[combinationStepper], noteObjectsParent.transform.position, fretFeedbackScript.gameObject.GetComponent<Transform>().transform.position);

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

    public string GetArrowKeys() {
        if (Input.GetKeyDown (KeyCode.UpArrow))
            return "U";
        else if (Input.GetKeyDown (KeyCode.LeftArrow))
            return "L";
        else if (Input.GetKeyDown (KeyCode.DownArrow))
            return "D";
        else if (Input.GetKeyDown (KeyCode.RightArrow))
            return "R";

        return "";
    }

    public string GetWASD() {
        if (Input.GetKeyDown (KeyCode.W))
            return "U";
        else if (Input.GetKeyDown (KeyCode.A))
            return "L";
        else if (Input.GetKeyDown (KeyCode.S))
            return "D";
        else if (Input.GetKeyDown (KeyCode.D))
            return "R";

        return "";
    }

    //have a function that looks at current beat and measure and returns the expected key combination
    //then have another function that checks if it was correct or not 
    private string GetExpectedCombination() {
        string expectedCombo = "";
        Debug.Log("grabbing combination at: " + currMeasure + " and beat: " + currBeat);

        //bounds check--occasionally SimpleClock will return that beat is 5, which is impossible. Also, measures start counting at 2. 0 Measures is the song has not started playing yet. 
        if (currBeat < 5) {
            Debug.Log("Checking a combination");
            int expectedNoteBeat = currBeat;
            int expectedNoteMeasure = currMeasure;
            //if we're at the second beat in a measure, want to get the third beat, which is index 2
            if (currBeat == 1) {
                expectedNoteMeasure = currMeasure;
                expectedNoteBeat = 2;
            }
            //if we're at the fourth beat in a measure, then to get the first beat of the next one (which is the one
            //expecting to be pressed), get index 0 for the NEXT measure)
            else if (currBeat == 3) {
                expectedNoteMeasure = currMeasure + 1;
                expectedNoteBeat = 0;
            }

            Debug.Log("Checking measure: " + expectedNoteMeasure + " and beat: " + expectedNoteBeat);

            //this will result in index out of bounds trying to check the last beat in the song. Have to make sure the measure doesn't go out. 
            if (expectedNoteMeasure < 78) {
                GameObject posInSong = thisSong[expectedNoteMeasure, expectedNoteBeat];
            // Debug.Log ("What were are trying to extract from 2D array: " + posInSong);

                if (posInSong != null) {
                    expectedCombo = posInSong.gameObject.GetComponent<NewNote>().GetCombination();
                    Debug.Log ("Current note combination: " + expectedCombo);
                }
            }
        }

        return expectedCombo;
    }

    //check if the keys the player pressed match what the expected combination at the given moment is
    private bool CombinationCheck(string pressedKeys, string expectedCombo) {
        if (pressedKeys.Equals(expectedCombo))
            return true;
        else
            return false;
    }

    public void CallCoroutine(string coroutineToCall) {
        if (coroutineToCall.Equals("StopAll")) {
            StopAllCoroutines();
        }

        if (coroutineToCall.Equals("ScaleAllNotesUp")) {
            ScaleNotes();
        }

        if (coroutineToCall.Equals("IntroAnimation")) {
            StartCoroutine(IntroAnim());
        }

        if (coroutineToCall.Equals("FretPulse")) {
            StartCoroutine(fretFeedbackScript.FretHit(true));
            fretFeedbackScript.rippleParticleSystem.Play();
        }

        //this conditional calls the coroutine that is on NewNote for the note to move itself dependent on where we are in the song right now
        if (coroutineToCall.Equals("StartMovement")) {
            //always looking to move the note that is 4 measures ahead. Which means that this always has to be performing an out of bounds check, because when the song is on bar 73, beat 4, it will
            //be attempting to move bar 77 beat 4 (which is the end of the song). Do NOT allow this to be called if we are past bar 73 beat 4. 
            if (currMeasure < 73)
                MoveNote(currMeasure + 4, currBeat);
        }

        if (coroutineToCall.Equals("ClosingAnimation")) {
            StartCoroutine(ClosingAnim());
        }
    }

    public void MoveNote(int currMeasure, int currBeat) {
        Debug.Log("Moving: " + currMeasure + " and " + currBeat);
        StartCoroutine(thisSong[currMeasure, currBeat].gameObject.GetComponent<NewNote>().WaitAndMove(0f));
    }

    //All notes and lifeSprites start at scale (0, 0, 0). This scales them all up at the moment we enter phase 2
    public void ScaleNotes() {
        for (int i = 0; i < thisSong.GetLength(0); i++) {
            for (int j = 0; j < thisSong.GetLength(1); j++) {
                GameObject currNote = thisSong[i, j];

                if (thisSong[i, j] != null && !currNote.gameObject.GetComponent<NewNote>().finishedMoving) {
                    StartCoroutine(currNote.gameObject.GetComponent<ScaleObject>().Scale(0.75f, new Vector3(0.5f, 0.5f, 1f)));
                }
            }
        }

        //all lifeSprites start disabled as well
        foreach(GameObject lifeSprite in lifeSprites) {
            lifeSprite.SetActive(true);
            StartCoroutine(lifeSprite.gameObject.GetComponent<ScaleObject>().Scale(0.75f, new Vector3 (0.4f, 0.4f, 1f)));
        }
    }

    //Intro Animation. Scale all Rhythm Game UI components up. Orbitter scales to full size before the fret starts
    public IEnumerator IntroAnim() {
        orbitterScript.ScaleOrbitter(2f, new Vector3(0.5f, 0.5f, 3f));
        StartCoroutine(backgroundScaler.Scale(4.25f, new Vector3(24f, 12f, 1f)));

        yield return new WaitForSeconds(2.25f);

        fretFeedbackScript.ScaleFret(2f, new Vector3(2.0f, 2.0f, 2.0f));
    }

    //Closing Animation. Scale all Rhythm Game UI components down. Reverse order of Intro Animation.
    public IEnumerator ClosingAnim() {
        fretFeedbackScript.ScaleFret(2f, new Vector3(0f, 0f, 2f));
        StartCoroutine(backgroundScaler.Scale(4.25f, new Vector3(0f, 0f, 1f)));

        yield return new WaitForSeconds(2.25f);

        orbitterScript.ScaleOrbitter(2f, new Vector3(0f, 0f, 3f));

        yield return new WaitForSeconds(2f);

        gameEnded = true;       //let the GameManager know that the rhythm game has been closed
    }

    public bool WindowCheck() {
        //hitting the third beat of a measure
        if (SimpleClock.Instance.Beats == 0) {
            return true;
        }

        if ( (SimpleClock.Instance.Beats == 2 && (SimpleClock.Instance.Ticks >= 48)) || (SimpleClock.Instance.Beats == 3 && (SimpleClock.Instance.Ticks <= 48)) ) {
            return true;
        }
        
        //hitting the first beat of a measure
        //occasionally SimpleClock will return that it was Beat 5, even though that's incorrect (musically). Idk why, coding with time be messy like that lol
        else if ((SimpleClock.Instance.Beats == 4 && (SimpleClock.Instance.Ticks >= 48)) || (SimpleClock.Instance.Beats == 1 && (SimpleClock.Instance.Ticks <= 48)) || SimpleClock.Instance.Beats == 5) {
            return true;
        }

        //every other instance is not in window, so return false
        return false;
    }



    //miscellaneous debugging functions! 

    //Debug function to see what is populating the 2D array
    void Print2DArray() {
        for  (int i = 0; i < thisSong.GetLength (0); i++) {
            for  (int j = 0; j < thisSong.GetLength (1); j++) {
                Debug.Log ("measure: " + i + " beat: " + j + " holds " + thisSong[i, j]);
            }
        }
    }

    //debugging function to test starting note movement
    public void Test() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("Entering phase one, calling coroutine");
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            ScaleNotes();
        }
    }

    //debugging function to see when we are in and out of the window. blue background means in window, black means out of window
    private void ChangeBackground(bool inWindow, bool restarted) {
        if (restarted) {
            Color inWindowColour = Color.red;
            inWindowColour.a = 0.65f;
            background.color = inWindowColour;
        }
        
        else if (inWindow) {
            Color inWindowColour = Color.blue;
            inWindowColour.a = 0.65f;
            background.color = inWindowColour;
        } else {
            Color outOfWindowColour = Color.black;
            outOfWindowColour.a = 0.65f;
            background.color = outOfWindowColour;
        }
    }

    
    /// <summary>
    /// State machine that handles the three states that the rhythm game goes through: intro animation, rhythm game and closing animation.
    /// </summary>
    //intro animation for the rhythm game. Player input cannot be taken.
    private class IntroAnimation : FiniteStateMachine<RhythmGameController>.State {
        public override void OnEnter() {
            Debug.Log("Calling the intro animation");
            Context.CallCoroutine("IntroAnimation");
        }

        public override void Update() {
            //GetScaleStatus will report when the fret has finished scaling to its final size. Once there, move on 
            //to phase 1 of the game.
            Context.inPhase1 = Context.fretFeedbackScript.GetScaleStatus();
            if (Context.inPhase1) {
                TransitionTo<Phase1>();
            }
        }

        public override void OnExit() {
            Debug.Log("Animation finish, transitioning");
        }
    }

    //phase 1 of rhythm game--press an x# of notes 
    private class Phase1 : FiniteStateMachine<RhythmGameController>.State {
        FiniteStateMachine<Phase1> phaseWindowStateMachine;
        private int noteCounter = 0;
        private bool startedNoteMovement = false;           //manage making sure the coroutine is only called once
        private string pressedArrow;
        private string pressedWASD;

        string pressedCombo = "";
        string expectedCombo = "";
        private bool windowExited = false;                   //boolean that represents if we have entered the window, to make sure we only enter that state once
            
        private bool started = false;

        private bool phase1 = true;
        private bool phase2 = false;

        private int strikes = 0;

        public override void OnEnter() {
            Debug.Log("starting phase 1");
            phaseWindowStateMachine = new FiniteStateMachine<Phase1>(this);
            phaseWindowStateMachine.TransitionTo<Resting>();
        }

        public override void Update() {
            phaseWindowStateMachine.Update();

            if (Input.GetKeyDown(KeyCode.Y)) {
                SimpleClock.Instance.FirstBeat();
            }

            //exit out of the rhythm game 
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Debug.Log("Exiting via escape");
                TransitionTo<ClosingAnimation>();
            } 

            //if the noteCounter is past the first phase, transition into phase 2
            if (noteCounter > Context.phase1Threshold && phase1) {
                Debug.Log("phase1 ended, entering phase2");
                Context.CallCoroutine("ScaleAllNotesUp");
                phase1 = false;
                phase2 = true;
            }

            //if in the window and state machine already is not in InWindow state, transition to the state
            //makes sure that we do not repeatedly enter the window
            if (Context.WindowCheck() && (phaseWindowStateMachine.CurrentState.GetType() != typeof(InWindow)) && started)  {
                Debug.Log("parent transitioning the child state to InWindow");
                phaseWindowStateMachine.TransitionTo<InWindow>();
            }
            else if (!Context.WindowCheck() && (phaseWindowStateMachine.CurrentState.GetType() == typeof(InWindow)) && started) {
                Debug.Log("parent transitioning the child state to OutOfWindow");
                phaseWindowStateMachine.TransitionTo<OutOfWindow>();
            }

            //criteria to end the song
            //the song is 77 bars long (inclusive), when the clock hits 80 bars, end of game
            if (SimpleClock.Instance.Measures > 78)  {
                Debug.Log("Forcing to closing animation");
                TransitionTo<ClosingAnimation>();
            }
        }

        //function to handle strikes
        public void StrikeCheck() {
            Context.lifeSprites[strikes].SetActive(false);
        }

        //function that resets all variables and restarts the rhythm game
        public void RestartRhythmGame() {

            Context.CallCoroutine("StopAll");
        
            SimpleClock.Instance.songSource.Stop();
            SimpleClock.Instance.SetBPM(138);

            phase1 = true;
            phase2 = false;

            noteCounter = 0;
            strikes = 0; 

            Context.fretFeedbackScript.SetFret(Context.thisSongSequence[noteCounter]);
            Context.fretFeedbackScript.ResetFret();

            //reset all notes. 
            //notes are not destroyed when they reach the goal, they just turn invisible and teleport somewhere irrelevant, so restarting the rhythm game is just resetting their position
            for (int i = 0; i < Context.thisSong.GetLength(0); i++) {
                for (int j = 0; j < Context.thisSong.GetLength(1); j++) {
                //index out of bounds exception somewhere :' ( 
                    if (Context.thisSong[i, j] != null) {
                    NewNote thisNoteScript = Context.thisSong[i, j].gameObject.GetComponent<NewNote>();
                    thisNoteScript.ResetNote(false, false);
                    }
                }
            }

            foreach(GameObject lifeSprite in Context.lifeSprites) {
                lifeSprite.transform.localScale = new Vector3(0f, 0f, 0f);
                lifeSprite.SetActive(false);
            }

            phaseWindowStateMachine.TransitionTo<Resting>();

            Context.ChangeBackground(false, true);

            started = false;

            // Context.orbitterScript.StopRotation();
            // Context.orbitterScript.ResetPosition();
        }

        public override void OnExit() {

        }


        /// <summary>
        /// nested state machine which operates whilst the rhythm game itself is running.
        /// Handles three cases: resting, within a time window and outside of a time window
        /// This state machine is NOT self sufficient, the parent state controls the window. The only state that should be calling TransitionTo<>() is resting
        /// </summary>

        /// Resting: a special case that only occurs at the very beginning of the rhythm game. 
        /// starting the rhythm game is a special case because it is dependent on input, not the window system like the rest 
        /// of the game. After receiving first correct input, the clock begins and it will start at beat 1 and the 48 ticks following it do not constitute a valid window
        /// since the player already entered input.
        private class Resting : FiniteStateMachine<Phase1>.State {

            string pressedCombo = "";
            string expectedCombo = "";
            string pressedArrow;
            string pressedWASD;

            bool firstComboPressed;

            public override void OnEnter() {
                Debug.Log("Entering resting phase");
                firstComboPressed = false;
                Context.Context.ChangeBackground(false, true);
            }
            public override void Update() {
                pressedArrow = Context.Context.GetArrowKeys();
                pressedWASD = Context.Context.GetWASD();
                expectedCombo = Context.Context.thisSongSequence[0];
                pressedCombo = pressedArrow + pressedWASD;

                //first combination was pressed correctly, game will otherwise stay resting the entire time.
                if (expectedCombo.Equals(pressedCombo) && !firstComboPressed) {
                    StartRhythmGame();
                }

                if (firstComboPressed && !Context.Context.WindowCheck()) {
                    Debug.Log("First case handled and transitioning to Out of Window at : " + SimpleClock.Instance.Beats + " and this tick: " + SimpleClock.Instance.Ticks);
                    TransitionTo<OutOfWindow>();
                }
            }

            //utility function for starting the rhythm game
            private void StartRhythmGame() {
                SimpleClock.Instance.FirstBeat();
                // Context.Context.orbitterScript.StartRotation();
                Context.noteCounter += 1;
                Context.Context.fretFeedbackScript.SetFret(Context.Context.thisSongSequence[Context.noteCounter]);
                firstComboPressed = true;
                Context.Context.CallCoroutine("FretPulse");
            }

            public override void OnExit() { 
                Debug.Log("Starting notes moving in");
                Context.startedNoteMovement = true;
                Context.started = true;
            }
        }

        //InWindow: the 96 ticks that a player is allowed to hit keys and make a combination 
        private class InWindow : FiniteStateMachine<Phase1>.State { 
            string pressedCombo = "";
            string expectedCombo = "";
            string pressedArrow;
            string pressedWASD;
            float windowLength;

            public override void OnEnter() {
                Debug.Log("Entering window at measure " + SimpleClock.Instance.Measures +  " and beat " + SimpleClock.Instance.Beats + " and tick " + SimpleClock.Instance.Ticks);
                pressedCombo = "";
                expectedCombo = Context.Context.GetExpectedCombination();

                pressedArrow = "";
                pressedWASD = "";
                //each window is one beat long
                windowLength = SimpleClock.BeatLength();

                Context.Context.ChangeBackground(true, false);
            }

            public override void Update() {
                // Debug.Log("Time left in the window: " + windowLength);

                if (pressedArrow.Equals("")) //if a pressed key has not yet been registered, 
                    pressedArrow = Context.Context.GetArrowKeys(); //then check for a pressed key
                if (pressedWASD.Equals(""))
                    pressedWASD = Context.Context.GetWASD();
            }

            public override void OnExit() {
                pressedCombo = pressedArrow + pressedWASD;

                Debug.Log ("this is what was expected: " + expectedCombo + "this was what was pressed: " + pressedCombo);
                Debug.Log("exiting window at measure " + SimpleClock.Instance.Measures +  " and beat " + SimpleClock.Instance.Beats + " and tick " + SimpleClock.Instance.Ticks);
                
                //phase 1 handling: if an incorrect combination was pressed, restart the rhythm game
                if (!Context.Context.CombinationCheck(pressedCombo, expectedCombo) && Context.phase1) {
                    Context.RestartRhythmGame(); 
                }

                //phase 2 check: if an incorrect combination was pressed, grant a strike
                if (!Context.Context.CombinationCheck(pressedCombo, expectedCombo) && Context.phase2) {
                    Context.StrikeCheck();

                    //increment strike counter after doing the StrikeCheck to prevent an index-out-of-bounds error

                    Context.strikes++;
                    
                    //restart the game if more than 5 strikes
                    if (Context.strikes >= 5) {
                        Context.RestartRhythmGame();
                    }
                }

                //fret visual feedback 
                Context.Context.CallCoroutine("FretPulse");
                
                Context.noteCounter += 1;

                if (Context.noteCounter < Context.Context.thisSongSequence.Length)
                    Context.Context.fretFeedbackScript.SetFret(Context.Context.thisSongSequence[Context.noteCounter]);

                //move the next note
                Context.Context.CallCoroutine("StartMovement");
            }
        }

        private class OutOfWindow : FiniteStateMachine<Phase1>.State {
            float outOfWindowLength;
            public override void OnEnter() {
                Debug.Log("Out of Window OnEnter()");
                Context.Context.ChangeBackground(false, false);
            }
            public override void Update() {

            }

            public override void OnExit() {
                Debug.Log("Exiting out of window");
            }
        }
    }
    
    private class ClosingAnimation : FiniteStateMachine<RhythmGameController>.State {
        public override void OnEnter() {
            Debug.Log("Closing the rhythm game");
            Context.CallCoroutine("ClosingAnimation");
        }
    }
}