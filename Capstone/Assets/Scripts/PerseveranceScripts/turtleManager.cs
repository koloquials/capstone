using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turtleManager : MonoBehaviour
{
    public GameObject turtlePrefab;

    public Text loseText;
    public Text winText;
    public Text startText;

    bool active = false;
    bool start = false;

    GameObject[] turtles; //Player is turtle at index 16 (x = 15)
    int survivor;

    // Start is called before the first frame update
    void Start()
    {
        turtles = new GameObject[30];
        survivor = (int)(Random.Range(0, 29)); //Which turtle lives
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && !start) //Before it starts
        {
            if (Input.GetKey(KeyCode.Z)) //Secret developer key, makes the player win.
            {
                survivor = 15;
            }
            if (Input.GetKey(KeyCode.Space)) //If it hasn't started, space will start it
            {
                for (int x = 0; x < 30; x++)
                {
                    bool d = true;
                    bool p = false;
                    if (x == survivor)
                        d = false;
                    if (x == 15)
                        p = true;
                    turtles[x] = GameObject.Instantiate(turtlePrefab, new Vector2(-9 + (x * 0.6f), -4.5f), Quaternion.identity);
                    turtles[x].GetComponent<turtle>().initialize(d, p);
                }
                active = true;
                start = true;
                startText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (turtles[15].GetComponent<turtle>().isDead())
            {
                loseText.gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.Space))
                    restart();
            }
            else if (turtles[15].GetComponent<turtle>().isSafe())
            {
                winText.gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.Space))
                    restart();
            }
        }
    }

    private void restart()
    {
        loseText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        survivor = (int)(Random.Range(0, 29));
        for (int x = 0; x < 30; x++)
        {
            bool d = true;
            bool p = false;
            if (x == survivor)
                d = false;
            if (x == 15)
                p = true;
            turtles[x].GetComponent<turtle>().initialize(d, p);
        }
    }
}
