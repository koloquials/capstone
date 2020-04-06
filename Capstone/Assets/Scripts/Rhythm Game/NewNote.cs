using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public class NewNote : MonoBehaviour
{
    SpriteRenderer mySpriteRenderer;

    [HideInInspector] public int measure;
    [HideInInspector] public int beat;

    private bool moving = true;

    private int BPM = 138;

    Vector3 destinationPos;
    Vector3 startPos;

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

    public float delayTime = 3f;
   // private float SimpleClock.Instance.quarter = 5f;
    private float currTime = 0f;

    private SpriteGlowEffect glowScript;

    [HideInInspector] public string myCombination;

    void Awake()
    {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        destinationPos = new Vector3(0f, transform.position.y, 0f);
        glowScript = gameObject.GetComponent<SpriteGlowEffect>();
    }

    void Start() {
        SetVisibility(false);
    }

    public IEnumerator WaitAndMove(float delayTime)
    {
        currTime = 0f;

        // SetMaterial(true);
        SetVisibility(true);

        // Debug.Log("starting from: " + startPos + " and moving towards: " + destinationPos);
        // Debug.Log("will take this long to move across the screen: " + SimpleClock.MeasureLength() * 4);
        yield return new WaitForSeconds(delayTime); // start at time X
        while (currTime < (SimpleClock.MeasureLength() * 4))
        { // until one second passed
            currTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, destinationPos, currTime/(SimpleClock.MeasureLength() * 4)); // lerp from A to B in one second
            // Debug.Log("note " + myCombination + " is currently at " + transform.position);
            yield return 1; // wait for next frame
        }
        
        //make a note invisible once it has reached its destination
        SetVisibility(false);
    }

    //function for resetting all properties of the note that are mutable as the game is played. 
    public void ResetNote(bool enabled) {
        transform.position = startPos;
        // SetMaterial(enabled);
        SetVisibility(enabled);
    }

    private void SetMaterial(bool enabled) {
        glowScript.enabled = enabled;
    }

    private void SetVisibility(bool visible) {
        mySpriteRenderer.enabled = visible;
    }

    //SetSprite is a little more loaded than a setter function should be
    //takes three parameters: the combination for this note, the parentTransform (used to set start position), and the fretTransform (used to set finish position)
    public void SetSprite(string thisNotesCombo, Vector3 parentTransform, Vector3 fretTransform)
    {
        switch (thisNotesCombo)
        {//I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
            case "UU":
                mySpriteRenderer.sprite = UU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.32f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.32f));
                SetCombination(thisNotesCombo);
                break;
            case "UR":
                mySpriteRenderer.sprite = UR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.27f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.27f));
                SetCombination(thisNotesCombo);
                break;
            case "UL":
                mySpriteRenderer.sprite = UL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.22f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.22f));
                SetCombination(thisNotesCombo);
                break;
            case "UD":
                mySpriteRenderer.sprite = UD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.17f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.17f));
                SetCombination(thisNotesCombo);
                break;
            case "RU":
                mySpriteRenderer.sprite = RU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.12f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.12f));
                SetCombination(thisNotesCombo);
                break;
            case "RR":
                mySpriteRenderer.sprite = RR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.07f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.07f));
                SetCombination(thisNotesCombo);
                break;
            case "RL":
                mySpriteRenderer.sprite = RL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.02f));
                SetDestinationPos(new Vector2(1f, parentTransform.y + 0.02f));
                SetCombination(thisNotesCombo);
                break;
            case "RD":
                mySpriteRenderer.sprite = RD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.03f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.03f));
                SetCombination(thisNotesCombo);
                break;
            case "LU":
                mySpriteRenderer.sprite = LU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.08f));
                SetDestinationPos(new Vector2(1f, parentTransform.y -0.08f));
                SetCombination(thisNotesCombo);
                break;
            case "LR":
                mySpriteRenderer.sprite = LR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.13f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.13f));
                SetCombination(thisNotesCombo);
                break;
            case "LL":
                mySpriteRenderer.sprite = LL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.18f));
                SetDestinationPos(new Vector2(1f, parentTransform.y -0.18f));
                SetCombination(thisNotesCombo);
                break;
            case "LD":
                mySpriteRenderer.sprite = LD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.23f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.23f));
                SetCombination(thisNotesCombo);
                break;
            case "DU":
                mySpriteRenderer.sprite = DU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.28f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.28f));
                SetCombination(thisNotesCombo);
                break;
            case "DR":
                mySpriteRenderer.sprite = DR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.33f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.33f));
                SetCombination(thisNotesCombo);
                break;
            case "DL":
                mySpriteRenderer.sprite = DL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.38f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.38f));
                SetCombination(thisNotesCombo);
                break;
            case "DD":
                mySpriteRenderer.sprite = DD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.43f));
                SetDestinationPos(new Vector2(1f, parentTransform.y - 0.43f));
                SetCombination(thisNotesCombo);
                break;
        }
    }

    public Sprite GetSprite() {
        return mySpriteRenderer.sprite;
    }

    public void SetStartPos(Vector2 startPos)
    {
        this.startPos = new Vector3(startPos.x, startPos.y, 0f);
        transform.position = startPos;
    }

    public void SetDestinationPos(Vector2 destinationPos) {
        this.destinationPos = new Vector3(destinationPos.x, destinationPos.y, 0f);
    }


    //Setters for IMMUTABLE properties of each note. 
    //set this note's measure in the entire song
    public void SetMeasure(int measure)
    {
        this.measure = measure;
    }

    //set this note's beat in the measure
    public void SetBeat(int beat)
    {
        this.beat = beat;
    }

    public void SetCombination(string myCombination) {
        this.myCombination = myCombination;
    }

    public string GetCombination() {
        return myCombination;
    }

    //Debug function to check all properties of a note
    public void PrintEveryProperty() {
        Debug.Log("My combination is: " + GetCombination() + " my assigned measure is: " + measure + " my assigned beat is: " + beat);
    }
}
