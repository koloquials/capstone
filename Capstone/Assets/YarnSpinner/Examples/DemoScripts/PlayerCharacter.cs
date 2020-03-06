﻿/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Yarn.Unity.Example
{
    public class PlayerCharacter : MonoBehaviour
    {

        //sprites 
        private SpriteRenderer mySpriteRenderer;
        private Animator myAnimator;

        public Sprite p_stand2;
        public Sprite p_walk1;

        public float minPosition = -5.3f;
        public float maxPosition = 5.3f;

        public float moveSpeed = 1.0f;

        public float interactionRadius = 2.0f;

        public float movementFromButtons { get; set; }

        bool motion = true;


        private float speed = 2f;
        private Vector3 mousePosition;
        private Vector3 targetPosition;
        private bool isMoving;
        private bool facingRight = true;

        void Start()
        {
            mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            myAnimator = gameObject.GetComponent<Animator>();

        }
        /// Draw the range at which we'll start talking to people.
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;

            // Flatten the sphere into a disk, which looks nicer in 2D games
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 0));

            // Need to draw at position zero because we set position in the line above
            Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
        }

        /// Update is called once per frame
        void Update()
        {
            // Remove all player control when we're in dialogue
            // Edited to also remove player control when in the rhythm game.
            if (FindObjectOfType<DialogueRunner>().isDialogueRunning == true || !motion)
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    FindObjectOfType<ClassicDialogueUI>().SetOption(0);
                }
                else if (Input.GetKey(KeyCode.Alpha2))
                {
                    FindObjectOfType<ClassicDialogueUI>().SetOption(1);
                }
                else if (Input.GetKey(KeyCode.Alpha3))
                {
                    FindObjectOfType<ClassicDialogueUI>().SetOption(2);
                }
                else if (Input.GetKey(KeyCode.Alpha4))
                {
                    FindObjectOfType<ClassicDialogueUI>().SetOption(3);
                }
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    FindObjectOfType<ClassicDialogueUI>().ScrollOption(-1);
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    FindObjectOfType<ClassicDialogueUI>().ScrollOption(1);
                }
                if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
                {
                    FindObjectOfType<ClassicDialogueUI>().ScrollOption(0);
                }
                return;
            }


            // Move the player, clamping them to within the boundaries 
            // of the level.

            // var movement = Input.GetAxis("Horizontal");
            // movement += movementFromButtons;
            // movement *= (moveSpeed * Time.deltaTime);

            // var newPosition = transform.position;
            // newPosition.x += movement;
            // 

            // transform.position = newPosition;

            // Detect if we want to start a conversation
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Mouse being pressed");
                CheckForNearbyNPC();
            }
            DetectMovement();
        }

        //detect if the player pressed the mouse to move the character and move the character accordingly
        public void DetectMovement()
        {
            if (Input.GetMouseButtonUp(0))
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = transform.position.z;
                targetPosition.y = transform.position.y;
                targetPosition.x = mousePosition.x;

                //check whether the player is moving toward the right or left (on the screen) of where character
                //currently is 
                if (targetPosition.x >= transform.position.x)
                {
                    facingRight = true;
                }
                else
                {
                    facingRight = false;
                }

                if (!isMoving)
                {
                    isMoving = true;
                }
            }

            if (isMoving)
            {
                myAnimator.SetBool("isRunning", true);
                SetDirection();
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition, maxPosition);
            }

            //if player is where they pressed to be
            if (targetPosition == transform.position)
            {
                myAnimator.SetBool("isRunning", false);
                CheckForNearbyNPC();
                isMoving = false;
            }
        }

        //check which side character is walking towards to flip the sprite accordingly 
        public void SetDirection()
        {
            if (!facingRight)
            {
                mySpriteRenderer.flipX = true;
            }
            else
            {
                mySpriteRenderer.flipX = false;
            }
        }

        public void Clicked(Vector3 newPosition)
        {
            Debug.Log("Entering clicked");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                newPosition = hit.point;
                transform.position = newPosition;
            }
        }
        /// Find all DialogueParticipants
        /** Filter them to those that have a Yarn start node and are in range; 
         * then start a conversation with the first one
         */
        public void CheckForNearbyNPC()
        {
            var allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
            var target = allParticipants.Find(delegate (NPC p)
            {
                return string.IsNullOrEmpty(p.talkToNode) == false && // has a conversation node?
                (p.transform.position - this.transform.position)// is in range?
                .magnitude <= interactionRadius
                && !p.moving;
            });
            if (target != null)
            {
                // Kick off the dialogue at this node.
                FindObjectOfType<DialogueRunner>().StartDialogue(target.talkToNode);
            }
        }

        public void motionControl(bool m)
        {
            motion = m;
        }
    }
}
