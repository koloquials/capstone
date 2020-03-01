using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewNote : MonoBehaviour
{
    SpriteRenderer mySpriteRenderer;

    [HideInInspector] public int measure;
    [HideInInspector] public int beat;

    private bool moving = false;

    private float motionTimer = 0;

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

    void Awake()
    {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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

        if (moving)
        {
            Move();
        }
    }

    void Move()
    {
        transform.position = new Vector3(Mathf.Lerp(startPos.x, destinationPos.x, motionTimer / ((60 / BPM) * 15)), transform.position.y, transform.position.z);
        motionTimer += Time.deltaTime;
    }

    public void SetSprite(string thisNotesCombo)
    {
        switch (thisNotesCombo)
        {//I think this is the easiest way to assign sprites and positions based on the 16 possible combinations.
            case "UU":
                mySpriteRenderer.sprite = UU;
                SetStart(new Vector2(14f, 0.32f));
                break;
            case "UR":
                mySpriteRenderer.sprite = UR;
                SetStart(new Vector2(14f, 0.27f));
                break;
            case "UL":
                mySpriteRenderer.sprite = UL;
                SetStart(new Vector2(14f, 0.22f));
                break;
            case "UD":
                mySpriteRenderer.sprite = UD;
                SetStart(new Vector2(14f, 0.17f));
                break;
            case "RU":
                mySpriteRenderer.sprite = RU;
                SetStart(new Vector2(14f, 0.12f));
                break;
            case "RR":
                mySpriteRenderer.sprite = RR;
                SetStart(new Vector2(14f, 0.07f));
                break;
            case "RL":
                mySpriteRenderer.sprite = RL;
                SetStart(new Vector2(14f, 0.02f));
                break;
            case "RD":
                mySpriteRenderer.sprite = RD;
                SetStart(new Vector2(14f, -0.03f));
                break;
            case "LU":
                mySpriteRenderer.sprite = LU;
                SetStart(new Vector2(14f, -0.08f));
                break;
            case "LR":
                mySpriteRenderer.sprite = LR;
                SetStart(new Vector2(14f, -0.13f));
                break;
            case "LL":
                mySpriteRenderer.sprite = LL;
                SetStart(new Vector2(14f, -0.18f));
                break;
            case "LD":
                mySpriteRenderer.sprite = LD;
                SetStart(new Vector2(14f, -0.23f));
                break;
            case "DU":
                mySpriteRenderer.sprite = DU;
                SetStart(new Vector2(14f, -0.28f));
                break;
            case "DR":
                mySpriteRenderer.sprite = DR;
                SetStart(new Vector2(14f, -0.33f));
                break;
            case "DL":
                mySpriteRenderer.sprite = DL;
                SetStart(new Vector2(14f, -0.38f));
                break;
            case "DD":
                mySpriteRenderer.sprite = DD;
                SetStart(new Vector2(14f, -0.43f));
                break;
        }
    }

    public void SetStart(Vector2 s)
    {
        startPos = new Vector3(s.x, s.y, transform.position.z);
        transform.position = startPos;
        motionTimer = 0;
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
