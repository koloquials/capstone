using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yarn.Unity.Example
{
    public class npcMotion : MonoBehaviour
    {
        public GameObject[] pointArray; //Invisible gameobjects that the npc moves to. For consistency's sake, waypoint 0 should be their starting position.
        Dictionary<string, GameObject> waypoints; //New format. String corresponds to waypoint's name.

        //int point = 0; //Which waypoint the npc is at.
        string point;

        bool moving = false; //Whether the npc is moving to a waypoint.
        int motionType = 0;
        float constantZ = 0; //The starting value of the NPC's z coordinate. Used to keep it the same throughout movements, since some would cause it to reset to 0.

        NPC npc; //The NPC script for this npc

        public float movementTime = 5f; //How long it takes the NPC to complete a given motion, in seconds. Might be an overly complicated way of doing this.

        public bool flippy = true; //Whether or not the sprite should flip when moving. For example, Piper would want this true, Micequinns would want it false.
        public bool reverseFlip = false; //Whether or not the sprite is already flipped, and therefore should have it's flips reversed. For example, Fidel's sprites are like this.

        float motionTimer = 0; //Used for moving

        float pushtimer = 0;
        float pushcool = 0;

        Vector3 startPos;

        SpriteRenderer sr;

        public Sprite standingSprite;
        public Sprite motionSprite;

        public Sprite[] spriteArray; //List and dictionary of all the sprites used. Dictionary enables easier lookup, while the public array of sprites is necessary to assign things to the dictionary in the inspector
        public Dictionary<string, Sprite> spriteList;

        // Start is called before the first frame update
        void Start()
        {
            spriteList = new Dictionary<string, Sprite>();
            foreach(Sprite s in spriteArray)
            {
                spriteList.Add(s.name, s);
            }

            waypoints = new Dictionary<string, GameObject>();
            foreach(GameObject g in pointArray)
            {
                waypoints.Add(g.name, g);

            }

            sr = this.GetComponent<SpriteRenderer>();
            npc = this.GetComponent<NPC>();
            constantZ = transform.position.z;
        }

        // Update is called once per frame
        void Update()
        {
            if(moving)
            {
                if (motionType == 0) //Normal movement
                {
                    if (Mathf.Abs(transform.position.x - waypoints[point].transform.position.x) > 0.1)
                    {
                        transform.position = new Vector3(Mathf.Lerp(startPos.x, waypoints[point].transform.position.x, motionTimer / movementTime), transform.position.y, transform.position.z); //Moves the sprite. When motionTimer = movementTime, the sprite has arrived at the destination.
                        motionTimer += Time.deltaTime;
                    }
                    else
                    {
                        moving = false;
                        transform.position = new Vector3(transform.position.x, transform.position.y, constantZ); //Set the z coordinate to the constant value, fixes some issues that were starting up.
                        sr.flipX = false;
                        //npc.setMotion(false);
                        if (standingSprite != null)
                        {
                            sr.sprite = standingSprite;
                        }
                    }
                }
                else if (motionType == 1) //Pushing movement
                {
                    if (Mathf.Abs(transform.position.x - waypoints[point].transform.position.x) > 0.1)
                    {
                        if (pushtimer < 1)
                        {
                            transform.position = new Vector2(Mathf.Lerp(startPos.x, waypoints[point].transform.position.x, motionTimer / movementTime), transform.position.y);
                            motionTimer += Time.deltaTime;
                            pushtimer += Time.deltaTime;
                        }
                        else
                        {
                            pushcool += Time.deltaTime;
                            if(pushcool > 0.5f)
                            {
                                pushtimer = 0;
                                pushcool = 0;
                            }
                        }
                    }
                    else
                    {
                        moving = false;
                        transform.position = new Vector3(transform.position.x, transform.position.y, constantZ);
                        sr.flipX = false;
                        //npc.setMotion(false);
                        if (standingSprite != null)
                        {
                            sr.sprite = standingSprite;
                        }
                    }
                }
            }
        }

        [YarnCommand("moveNPC")]
        public void nextPoint(string waypoint) //Set the waypoint that the npc is going to. Set the key/value combinations in the inspector.
        {
            if (waypoints.ContainsKey(waypoint))
            {
                if(motionSprite != null)
                {
                    sr.sprite = motionSprite;
                }
                point = waypoint;
                moving = true;
                motionTimer = 0;
                startPos = transform.position;
                if (flippy && startPos.x < waypoints[point].transform.position.x)
                {
                    if (reverseFlip)
                        sr.flipX = true;
                    else
                        sr.flipX = false;
                }
                else if (flippy)
                {
                    if (reverseFlip)
                        sr.flipX = false;
                    else
                        sr.flipX = true;
                }
                //npc.setMotion(true);
            }
            else
            {
                Debug.Log("Invalid waypoint");
            }
        }

        [YarnCommand("showNPC")]
        public void appear() //Used for npcs or other overworld elements that are not present at start. These objects should have their sprite color set to transparent
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        }

        [YarnCommand("changeNPC")]
        public void spriteChange(string spriteName) //Sets the overworld sprite of the npc
        {
            if(spriteList.ContainsKey(spriteName))
            {
                if(!moving) //If the sprite isn't moving, set the new sprite right away
                {
                    sr.sprite = spriteList[spriteName];
                }
                standingSprite = spriteList[spriteName]; //In case the sprite is moving, set it so that it reverts to this sprite when done moving
            }
        }

        [YarnCommand("setNPCMove")]
        public void setMovement(string type) //Potentially sets npc movement to different types. Right now 0 is default, 1 is fidel pushing things
        {
            if (type.Equals("normal"))
            {
                motionType = 0;
            }
            else if (type.Equals("push"))
            {
                motionType = 1;
            }
        }

        [YarnCommand("warpNPC")]
        public void warp() //Instantly finishes npc movement. Used for dialogue skipping.
        {
            if(moving)
            {
                transform.position = waypoints[point].transform.position;
                moving = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, constantZ);
                sr.flipX = false;
                //npc.setMotion(false);
                if (standingSprite != null)
                {
                    sr.sprite = standingSprite;
                }
            }
        }
    }
}
