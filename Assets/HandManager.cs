using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour, InteractionListenerInterface
{
    
    public InteractionManager interactionManager;

    public GestureController controller;
    // hand interaction variables   手的交互变量
    private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;
    // Start is called before the first frame update
    void Start()
    {
        interactionManager = InteractionManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void HandGripDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
    {
        if (!isHandInteracting || !interactionManager)
            return;
        if (userId != interactionManager.GetUserID())
            return;
        controller.SendMessage("toggle", true);
        lastHandEvent = InteractionManager.HandEventType.Grip;
        //isLeftHandDrag = !isRightHand;
    }

    public void HandReleaseDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
    {
        if (!isHandInteracting || !interactionManager)
            return;
        if (userId != interactionManager.GetUserID())
            return;

        lastHandEvent = InteractionManager.HandEventType.Release;
        controller.SendMessage("toggle", false);
        //isLeftHandDrag = !isRightHand;
    }
    public bool HandClickDetected(long userId, int userIndex, bool isRightHand, Vector3 handScreenPos)
    {
        return true;
    }

}
