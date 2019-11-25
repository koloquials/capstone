using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yarn.Unity.Example
{
    public class npcMotion : MonoBehaviour
    {
        public GameObject[] waypoints; //Invisible gameobjects that the npc moves to. For consistency's sake, waypoint 0 should be their starting position.

        int point = 0; //Which waypoint the npc is at.

        bool moving = false; //Whether the npc is moving to a waypoint.

        NPC npc; //The NPC script for this npc

        float motionTimer = 0;

        Vector2 startPos;

        // Start is called before the first frame update
        void Start()
        {
            npc = this.GetComponent<NPC>();
        }

        // Update is called once per frame
        void Update()
        {
            if(moving)
            {
                if(Vector2.Distance(transform.position, waypoints[point].transform.position) > 0.1)
                {
                    transform.position = new Vector2(Mathf.Lerp(startPos.x, waypoints[point].transform.position.x, motionTimer/5f), transform.position.y);
                    motionTimer += Time.deltaTime;
                }
                else
                {
                    moving = false;
                    npc.setMotion(false);
                }
            }
        }

        [YarnCommand("moveNPC")]
        public void nextPoint()
        {
            if (point < waypoints.Length - 1)
            {
                point += 1;
                moving = true;
                motionTimer = 0;
                startPos = transform.position;
                npc.setMotion(true);
            }
            else
            {
                Debug.Log("Not enough waypoints");
            }
        }
    }
}
