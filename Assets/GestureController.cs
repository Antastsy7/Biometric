using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureController : MonoBehaviour, KinectGestures.GestureListenerInterface
{

    public LineRenderer lineRenderer;
    
    private bool flag, grip;
    private long userId;
    private List<Vector3> movePath;
    private Vector3 jointPos;
    private KinectInterop.JointType joint;
    private KinectManager manager;
    private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;

    // Start is called before the first frame update
    void Start()
    {
        manager = KinectManager.Instance;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (flag && grip)
        {
            userId = manager.GetPrimaryUserID();
            jointPos = manager.GetJointPosition(userId, (int)KinectInterop.JointType.HandRight);
            jointPos.z = 0;
            movePath.Add(jointPos);
            lineRenderer.positionCount = movePath.Count;
            lineRenderer.SetPositions(movePath.ToArray());
        }
    }

    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
        KinectInterop.JointType joint)
    {
        return true;
    }

    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
        KinectInterop.JointType joint, Vector3 screenPos)
    {
        return true;
    }

    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress,
        KinectInterop.JointType joint, Vector3 screenPos)
    {
        
    }

    public void UserDetected(long userId, int userIndex)
    {
        flag = true;
        movePath = new List<Vector3>();
        lineRenderer.positionCount = 0;
        
        
    }

    public void UserLost(long userId, int userIndex)
    {
        flag = false;
        
    }

    public void toggle(bool b)
    {
        if (b)
        {
            grip = true;
        }
        else
        {
            grip = false;
            lineRenderer.positionCount = 0;
            movePath.Clear();
        }
    }
}
