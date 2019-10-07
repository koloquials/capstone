using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ropeSpawner : MonoBehaviour
{
    public GameObject gameManager;
    ResourceProtoManager managerScript;
    public Transform ropeLink;

    void Start()
    {
        managerScript = gameManager.GetComponent<ResourceProtoManager>();

        //for (int i = 0; i < linkNum; i++)
        if (managerScript.linkCount < managerScript.linkLength)
        {
            Transform nextLink;
            nextLink = Instantiate(ropeLink, new Vector3(1 * 0, 0, 0), Quaternion.identity);
            //-0.525f
            Joint2D nextHJoint = nextLink.gameObject.GetComponent<Joint2D>();
            nextHJoint.connectedBody = this.gameObject.GetComponent<Rigidbody2D>();
            Joint2D nextDJoint = nextLink.gameObject.GetComponent<DistanceJoint2D>();
            nextDJoint.connectedBody = this.gameObject.GetComponent<Rigidbody2D>();
            managerScript.linkCount++;
            //if (managerScript.linkCount == managerScript.linkLength)
            if (managerScript.linkCount == 1)
            {
                //MouseFollow mouseScript = nextLink.gameObject.GetComponent<MouseFollow>();
                MouseFollow mouseScript = this.gameObject.GetComponent<MouseFollow>();
                mouseScript.enabled = true;
                this.gameObject.GetComponent<Joint2D>().enabled = false;
                this.gameObject.GetComponent<DistanceJoint2D>().enabled = false;
                //nextDJoint.enabled = false;
            }
        }
    }

}
