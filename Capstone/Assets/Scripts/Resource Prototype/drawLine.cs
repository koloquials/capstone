using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawLine : MonoBehaviour
{

    public GameObject linePrefab;
    public GameObject currentLine;
    public GameObject ogLine;

    public LineRenderer lineRender;
    public EdgeCollider2D edgeCol;
    public List<Vector2> linePoints;
    LineCollider lineCol;

    public int pointMax;
    public bool notHit = true;

    void Start()
    {
        lineCol = currentLine.GetComponent<LineCollider>();
        lineCol.lineObj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //linePoints.Clear();
            DestroyLine();
            CreateLine();
            notHit = true;
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 tempMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((Vector2.Distance(tempMousePos, linePoints[linePoints.Count - 1]) > 0.1f) && lineRender.positionCount <= pointMax && notHit == true)
            {
                UpdateLine(tempMousePos);
            }

            if (lineRender.positionCount == pointMax)
            {
                Debug.Log("line max reached!");
            }
        }
    }

    void CreateLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRender = currentLine.GetComponent<LineRenderer>();
        edgeCol = currentLine.GetComponent<EdgeCollider2D>();
        lineCol = currentLine.GetComponent<LineCollider>();
        lineCol.lineObj = this.gameObject;
        linePoints.Clear();
        linePoints.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        linePoints.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRender.SetPosition(0, linePoints[0]);
        lineRender.SetPosition(1, linePoints[1]);
        edgeCol.points = linePoints.ToArray();
    }

    void UpdateLine(Vector2 newMousePos)
    {
        linePoints.Add(newMousePos);
        lineRender.positionCount++;
        lineRender.SetPosition(lineRender.positionCount -1, newMousePos);
        edgeCol.points = linePoints.ToArray();
    }

    public void DestroyLine()
    {
        Destroy(currentLine);
        Destroy(ogLine);
    }
}
