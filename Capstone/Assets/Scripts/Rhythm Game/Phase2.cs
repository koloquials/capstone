using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2 : MonoBehaviour
{
    public GameObject[,] thisSong;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start of Phase2 script, launching the note movements)");
        StartCoroutine(StartNoteMovement());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartNoteMovement()
    {
        for (int i = 0; i < thisSong.GetLength(0); i++)
        {
            for (int j = 0; j < thisSong.GetLength(1); i++)
            {
                StartCoroutine(thisSong[i, j].gameObject.GetComponent<NewNote>().WaitAndMove(5f));

                yield return new WaitForSeconds(3f);
            }
        }
    }
}
