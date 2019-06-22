using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TrajectoryDrawer : MonoBehaviour
{
    public float initialVelocity = 10.0f;
    public float timeResolution = 0.5f;   // points in trajectory curve spacing 
    public float maxTime = 10.0f;          // lenght of trajectory
    private LineRenderer lineRenderer;
    public List<Vector3> trajectoryPoints;
    private Slider PowerSlider;
    // Use this for initialization
    void Start()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        PowerSlider = GameObject.Find("Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        initialVelocity = PowerSlider.value;
        Vector3 velocityVector = -transform.forward * initialVelocity;
        Vector3 currentPosition = transform.position;
        lineRenderer.positionCount = ((int)(maxTime / timeResolution));
        trajectoryPoints.Clear();
        int index = 0;
        for (float t = 0; t < maxTime; t += timeResolution)
        {
            if (index < lineRenderer.positionCount)
            {
                lineRenderer.SetPosition(index, currentPosition);
                trajectoryPoints.Insert(index, currentPosition);
                // raycast between each point in the line

                // velocity and gravity calculations for dy 
                currentPosition += velocityVector * timeResolution;
                velocityVector += Physics.gravity * timeResolution;

                index++;
            }
        }
        
    }
}
