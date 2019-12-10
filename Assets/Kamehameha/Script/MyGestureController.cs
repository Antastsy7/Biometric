using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;


public class MyGestureController : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    public GameObject ChargingEffect, BurstEffect;//预制体
    public Vector3 addjust_Charge;
    public int playerIndex = 0;
    public float Charging_time = 3.0f;
    public float Holding_time = 7.0f;
    public Slider slider;
    public Text text;
    public GameObject enemymanager, kame, ha;
    public Image back, front;
    public AudioClip release, charging;
    public AudioSource bgm, audio;
    //动作取消时调用
    private KinectManager manager = null;
    public int score = 0;
    private GameObject charge;
    private GameObject burst;
    
    private enum States
    {
        Default, //啥都没干
        Start_Charging, //摆了一个psi的pose
        Charg_Complete, //charge完毕可以push了，一定时间不push会取消
        Releasing, //正在释放
    }

    private States state;

    private float countdown = 0.0f; //蓄力时间剩余

    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
    {
        if (state == States.Start_Charging && gesture == KinectGestures.Gestures.Psi)
        {
            state = States.Default;
            Destroy(charge);
            charge = null;
            back.color = Color.clear;
            front.color = Color.clear;
            audio.Stop();
            print("Cancel Charging, Set to default");
        }
        return true;
    }
    //动作完成时调用
    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
        if (userIndex != playerIndex)
            return false;
        if (state == States.Default && gesture == KinectGestures.Gestures.Psi)//判断如果完成的姿势是双手向上的姿势
        {
            countdown = Charging_time;
            charge = Instantiate(ChargingEffect);
            state = States.Start_Charging;
            back.color = Color.blue;
            front.color = Color.cyan;
            audio.clip = charging;
            audio.Play();
            kame.SetActive(true);
        }
        else if (state == States.Charg_Complete && gesture == KinectGestures.Gestures.Push)
        {
            Destroy(charge);
            charge = null;
            countdown = Holding_time;
            manager.DeleteGesture(userId, KinectGestures.Gestures.Push);
            manager.DetectGesture(userId, KinectGestures.Gestures.Pull);
            burst = Instantiate(BurstEffect);
            state = States.Releasing;
            back.color = Color.clear;
            front.color = Color.white;
            audio.clip = release;
            audio.Play();
            kame.SetActive(false);
            ha.SetActive(true);
        }
        else if (state == States.Releasing && gesture == KinectGestures.Gestures.Pull)
        {
            Destroy(burst);
            burst = null;
            manager.DeleteGesture(userId, KinectGestures.Gestures.Pull);
            manager.DetectGesture(userId, KinectGestures.Gestures.Psi);
            state = States.Default;
            back.color = Color.clear;
            front.color = Color.clear;
            audio.Stop();
            ha.SetActive(false);
            
        }
        return true;
    }
    //动作进行时调用
    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
        
    }
    //检测到用户时调用
    public void UserDetected(long userId, int userIndex)
    {
        if (userIndex != playerIndex)
            return;
        print("检测到用户");
        manager.DetectGesture(userId, KinectGestures.Gestures.Psi);//添加双手向上保持1秒的姿势检测
        enemymanager.SetActive(true);
        bgm.Play();

    }
    //丢失用户时调用
    public void UserLost(long userId, int userIndex)
    {
        print("丢失用户");
    }
    // Use this for initialization
    void Start () {
        manager = KinectManager.Instance;//初始化KinectManager对象
    }
    void Update ()
    {
        long userid = manager.GetPrimaryUserID();
        switch (state)
        {
            case States.Default:
            {
                break;
            }

            case States.Start_Charging:
            {
                addjust_Charge = (manager.GetJointKinectPosition(userid, 7) +
                                  manager.GetJointKinectPosition(userid, 11)) / 2;
                charge.transform.position =
                    (manager.GetJointKinectPosition(userid, 7) + manager.GetJointKinectPosition(userid, 11)) / 2 - addjust_Charge;
                countdown -= Time.deltaTime;
                slider.value = 1 - countdown / Charging_time;
                if (countdown <= 0.0f)
                {
                    countdown = 10 - Charging_time;
                    manager.DeleteGesture(userid, KinectGestures.Gestures.Psi);
                    manager.DetectGesture(userid, KinectGestures.Gestures.Push);
                    state = States.Charg_Complete;
                    back.color = Color.cyan;
                    front.color = Color.white;
                    Debug.Log("Charging Complete");
                }
                break;
            }
            
            
            case States.Charg_Complete:
            {
                countdown -= Time.deltaTime;
                slider.value = countdown / (10 - Charging_time);
                if (countdown <= 0.0f)
                {
                    Destroy(charge);
                    charge = null;
                    manager.DeleteGesture(userid, KinectGestures.Gestures.Push);
                    manager.DetectGesture(userid, KinectGestures.Gestures.Psi);
                    state = States.Default;
                    back.color = Color.clear;
                    front.color = Color.clear;
                    kame.SetActive(false);
                }
                break;
            }

            case States.Releasing:
            {
                Vector3 pos = (manager.GetJointKinectPosition(userid, 7) +
                               manager.GetJointKinectPosition(userid, 11)) / 2;
                burst.transform.rotation = Quaternion.LookRotation(pos);
                countdown -= Time.deltaTime;
                slider.value = countdown / Holding_time;
                if (countdown <= 0.0f)
                {
                    Destroy(burst);
                    burst = null;
                    manager.DeleteGesture(userid, KinectGestures.Gestures.Pull);
                    manager.DetectGesture(userid, KinectGestures.Gestures.Psi);
                    state = States.Default;
                    back.color = Color.clear;
                    front.color = Color.clear;
                    ha.SetActive(false);
                }

                break;
            }
        }

        text.text = score.ToString();
    }

    public void Gethit()
    {
        score -= 50;
    }


    
}

