using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float timeToMove = 5f;
    private float currTime = 0f;

    void Awake()
    {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        destinationPos = new Vector3(0f, transform.position.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // if (moving)
        // {
        //     if (scale < 1)
        //     {
        //         scale = Mathf.Lerp(scale, 1, 0.05f);
        //         if (scale > 0.98f)
        //         {
        //             scale = 1f;
        //         }
        //         transform.localScale = new Vector2(scale, scale);
        //     }
        // }

        // if (moving)
        // {
        //     Move();
        // }
    }

    public IEnumerator WaitAndMove(float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // start at time X
        while (currTime < timeToMove)
        { // until one second passed
        currTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, destinationPos, currTime/timeToMove); // lerp from A to B in one second
            yield return 1; // wait for next frame
        }
    }

    public void SetSprite(string thisNotesCombo, Vector3 parentTransform)
    {
        switch (thisNotesCombo)
        {//I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
            case "UU":
                mySpriteRenderer.sprite = UU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.32f));
                SetDestinationPos(new Vector2(0f, parentTransform.y + 0.32f));
                break;
            case "UR":
                mySpriteRenderer.sprite = UR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.27f));
                SetDestinationPos(new Vector2(0f, parentTransform.y + 0.27f));
                break;
            case "UL":
                mySpriteRenderer.sprite = UL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.22f));
                SetDestinationPos(new Vector2(0f, parentTransform.y + 0.22f));
                break;
            case "UD":
                mySpriteRenderer.sprite = UD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.17f));
                SetDestinationPos(new Vector2(0f, parentTransform.y + 0.17f));
                break;
            case "RU":
                mySpriteRenderer.sprite = RU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.12f));
                SetDestinationPos(new Vector2(0f, parentTransform.y + 0.12f));
                break;
            case "RR":
                mySpriteRenderer.sprite = RR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.07f));
                SetDestinationPos(new Vector2(0f, parentTransform.y + 0.07f));
                break;
            case "RL":
                mySpriteRenderer.sprite = RL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y + 0.02f));
                SetDestinationPos(new Vector2(0f, parentTransform.y + 0.02f));
                break;
            case "RD":
                mySpriteRenderer.sprite = RD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.03f));
                SetDestinationPos(new Vector2(0f, parentTransform.y - 0.03f));
                break;
            case "LU":
                mySpriteRenderer.sprite = LU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.08f));
                SetDestinationPos(new Vector2(0f, parentTransform.y -0.08f));
                break;
            case "LR":
                mySpriteRenderer.sprite = LR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.13f));
                SetDestinationPos(new Vector2(0f, parentTransform.y - 0.13f));
                break;
            case "LL":
                mySpriteRenderer.sprite = LL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.18f));
                SetDestinationPos(new Vector2(0f, parentTransform.y -0.18f));
                break;
            case "LD":
                mySpriteRenderer.sprite = LD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.23f));
                SetDestinationPos(new Vector2(0f, parentTransform.y - 0.23f));
                break;
            case "DU":
                mySpriteRenderer.sprite = DU;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.28f));
                SetDestinationPos(new Vector2(0f, parentTransform.y - 0.28f));
                break;
            case "DR":
                mySpriteRenderer.sprite = DR;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.33f));
                SetDestinationPos(new Vector2(0f, parentTransform.y - 0.33f));
                break;
            case "DL":
                mySpriteRenderer.sprite = DL;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.38f));
                SetDestinationPos(new Vector2(0f, parentTransform.y - 0.38f));
                break;
            case "DD":
                mySpriteRenderer.sprite = DD;
                SetStartPos(new Vector2(parentTransform.x, parentTransform.y - 0.43f));
                SetDestinationPos(new Vector2(0f, parentTransform.y - 0.43f));
                break;
        }
    }

    public void SetStartPos(Vector2 startPos)
    {
        this.startPos = new Vector3(startPos.x, startPos.y, 0f);
        transform.position = startPos;
    }

    public void SetDestinationPos(Vector2 destinationPos) {
        this.destinationPos = new Vector3(destinationPos.x, destinationPos.y, 0f);
    }
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

    public void SetMoving()
    {
        moving = true;
    }

    public void stopMotion() //THE NAMES ARE MISLEADING: The notes work better if they're always moving but invisible when you're not supposed to see them. Currently changes sprite alpha.
    {
        moving = false;
        mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b, 0);
    }

    public void startMotion()
    {
        moving = true;
        transform.localScale = Vector2.zero;
        //scale = 0;
        mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b, 1);
    }
}
