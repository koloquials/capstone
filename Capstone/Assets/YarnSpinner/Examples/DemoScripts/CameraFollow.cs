/*

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

namespace Yarn.Unity.Example {

    /// Control the position of the camera and its behaviour
    /** Camera should have minPosition and maxPosition of the
     * same because we're dealing with 2D. The movement speed
     * shouldn't be too fast nor too slow
     */
    public class CameraFollow : MonoBehaviour {
        //CameraFollow can be accessed by any script in the screen with CameraFollow.Instance.()
        private static CameraFollow _instance;
        public static CameraFollow Instance { get { return _instance; } }

        /// Target of the camera
        public Transform target;

        /// Minimum position of camera
        private float minPosition;

        /// Maximum position of camera
        public float maxPosition = 5.3f;
        private float minMaxDistance;

        /// Movement speed of camera
        public float moveSpeed = 1.0f;

        public bool inGame = false; //Whether or not the rhythm game is active.

        public GameObject wall1; 
        float vertExtent;
        float horzExtent;

        ShakeBehaviour shakeScript;

        // Update is called once per frame

        void Awake() {
            //singleton pattern
            if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            }
            else {
                _instance = this;
            }

            minPosition = transform.position.x;
            maxPosition = -35f;                         //hard-coded values, oof, but (camera.transform.position.x + Screen.Width/2) should NOT exceed wall1.transform.position.x;

            minMaxDistance = Mathf.Abs(minPosition - maxPosition);

            vertExtent = Camera.main.orthographicSize;   
            horzExtent = vertExtent * Screen.width / Screen.height;

            shakeScript = this.gameObject.GetComponent<ShakeBehaviour>();
        }

        void Update () {
            if (inGame)
            {
                transform.position = new Vector3(0f, 0f, -10f); //NOTE! At current, the entire scene is structured around the rhythm game being at the origin.
            }
            else
            {
                if (target == null)
                {
                    return;
                }
                var newPosition = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);

                newPosition.x = Mathf.Clamp(newPosition.x, minPosition, maxPosition);
                newPosition.y = transform.position.y;
                newPosition.z = transform.position.z;

                transform.position = newPosition;
            }

            if (Input.GetKeyDown(KeyCode.T)) {
                ScreenShake();
            }
        }

        public void setGame(bool g)
        {
            inGame = g;
        }

        //Each room will have its own unique min and max camera position. Everytime a room is entered (aka, a door is activated), call this.
        //the camera teleports itself with the player, 
        public void SetNewRoom() {
            minPosition = transform.position.x;
            maxPosition = minPosition + minMaxDistance;
        }

        public void ScreenShake() {
            shakeScript.TriggerShake(0.1f, 0.5f);
        }
    }
}

