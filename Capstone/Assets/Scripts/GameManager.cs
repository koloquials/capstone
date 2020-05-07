using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;

namespace Yarn.Unity.Example {
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }        //GameManager can be accessed by any script in the scene via GameManager.Instance.(...)

        public Yarn.Unity.Example.CameraFollow cam;
        public Yarn.Unity.Example.PlayerCharacter player;   //The script for moving the player. Used to stop being able to move and interact during the rhythm game.

        public GameObject rhythmGameController;

        public AudioManager audioManager;

        FiniteStateMachine<GameManager> gameManagerStateMachine;

        public GameObject overworldRhythmController;

        void Awake() {
            //singleton pattern
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            }
            else {
                _instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            overworldRhythmController = GameObject.Find("OverworldRhythm");

            gameManagerStateMachine = new FiniteStateMachine<GameManager>(this);
            gameManagerStateMachine.TransitionTo<Overworld>();

            audioManager = transform.GetChild(0).GetComponent<AudioManager>();
        }

        // Update is called once per frame
        void Update()
        {  
            gameManagerStateMachine.Update();

            //remove these later. Here for debugging purposes. Start 
            if (Input.GetKeyDown(KeyCode.C) && (gameManagerStateMachine.CurrentState.GetType() != typeof(RhythmGame))) {
                Debug.Log("Entering the rhythm game");
                gameManagerStateMachine.TransitionTo<RhythmGame>();
            }
        }

        [YarnCommand("rhythmGame")]                 //enter the rythm game state
        public void RhythmGameState() {
            gameManagerStateMachine.TransitionTo<RhythmGame>();
        }

        [YarnCommand("overworldPuzzle")]            //enter overworld puzzle state 
        public void OverworldPuzzleState() {
            gameManagerStateMachine.TransitionTo<OverworldPuzzle>();
        }

        //the machine's resting state, nothing much special handled here. 
        public void OverworldState() {
            gameManagerStateMachine.TransitionTo<Overworld>();
        }

        private class Overworld : FiniteStateMachine<GameManager>.State {
            public override void OnEnter() {
            }   

            public override void Update() {
            }

            public override void OnExit() {
            }
        }

        private class RhythmGame : FiniteStateMachine<GameManager>.State {
            public override void OnEnter() {
                Debug.Log("Entering rhythm game state. Deactivating player movement");
                Context.player.motionControl(false); 
                CameraFollow.Instance.setGame(true);
                // Context.cam.setGame(true);
                Context.rhythmGameController.gameObject.SetActive(true);

                Context.audioManager.ControlAmbience(false);            //turn overworld ambience off
            }

            //rhythm game is self sufficient. It will handle entering and exiting on its own.
            public override void Update() {
                if(Context.rhythmGameController.gameObject.GetComponent<RhythmGameController>().gameEnded) {
                    TransitionTo<Overworld>();
                }
            }

            public override void OnExit() {
                Debug.Log("Closing rhythm game state. Reactivating player movement");
                Context.player.motionControl(true);
                Context.rhythmGameController.gameObject.SetActive(false);
                
                Context.audioManager.ControlAmbience(true);             //turn overworld ambience back on
            }
        }

        private class OverworldPuzzle : FiniteStateMachine<GameManager>.State {
            public GameObject environmentSensorObj;

            public override void OnEnter() {
                Debug.Log("Entering puzzle state");
                Context.overworldRhythmController.active = true;
                environmentSensorObj = GameObject.Find("BallSensor");
            }

            public override void Update() {
                if (!environmentSensorObj.gameObject.GetComponent<environmentSensor>().GetStatus()) {    //exit back out to overworld state when the gate has been unlocked
                    Debug.Log("environment senor unlocked. Exiting");
                    TransitionTo<Overworld>();
                }
            }

            public override void OnExit() {
                Debug.Log("exiting puzzle state");
                Context.overworldRhythmController.active = false;
            }
        }
    }
}
