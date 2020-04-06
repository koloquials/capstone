using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public Yarn.Unity.Example.CameraFollow cam;
    public Yarn.Unity.Example.PlayerCharacter player;   //The script for moving the player. Used to stop being able to move and interact during the rhythm game.

    public GameObject rhythmGameController;

    FiniteStateMachine<GameManager> gameManagerStateMachine;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerStateMachine = new FiniteStateMachine<GameManager>(this);
        gameManagerStateMachine.TransitionTo<Overworld>();
    }

    // Update is called once per frame
    void Update()
    {
        gameManagerStateMachine.Update();

        //remove these later. Here for debugging purposes. Start 
        if (Input.GetKeyDown(KeyCode.C) && (gameManagerStateMachine.CurrentState.GetType() != typeof(RhythmGame))) {
            gameManagerStateMachine.TransitionTo<RhythmGame>();
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            gameManagerStateMachine.TransitionTo<Overworld>();
        }
        //End
    }

    public void RhythmGameState() {
        gameManagerStateMachine.TransitionTo<RhythmGame>();
    }

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
            Context.cam.setGame(true);
            Context.rhythmGameController.gameObject.SetActive(true);
        }

        //rhythm game is self sufficient. It will handle entering and exiting on its own.
        public override void Update() {
            if (Context.rhythmGameController.gameObject.GetComponent<RhythmGameController>().gameEnded) {
                TransitionTo<Overworld>();
            }
        }

        public override void OnExit() {
            Debug.Log("Closing rhythm game state. Reactivating player movement");
            Context.player.motionControl(true);
            Context.rhythmGameController.gameObject.SetActive(false);
        }
    }
}
