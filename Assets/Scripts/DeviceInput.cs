using UnityEngine;
using System.Collections;
using System;
using Valve.VR;
public class DeviceInput : MonoBehaviour
{
    //Swipe directions
    public enum SwipeDirection
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    };
    public event Action OnPressTrigger;    //按住扳机键                           
    public event Action OnPressDownTrigger;  // 按下扳机键                            
    public event Action OnPressUpTrigger;    //抬起扳机键                              
    public event Action OnTouchPad;      //按住触摸板                       

    public event Action OnPressDownGripButton;   //按下侧键                          
    public event Action OnPressDownMenuButton;   //按下菜单键                         
    public event Action<Vector2> OnBeginTouch;  //触摸触摸板的位置                                
    public event Action<Vector2> OnEndTouch;//抬起触摸板的位置

    public event Action OnTouchPadDown;  //按下触摸板                              
    public event Action OnTouchPadUp;//抬起触摸板
    public event Action<SwipeDirection> OnPadSwipe;

    [SerializeField]
    private float m_SwipeWidth = 0.6f;         //The width of a swipe


    private Vector2 m_PadDownPosition;
    private float m_LastHorizontalValue;
    private float m_LastVerticalValue;

    private SteamVR_TrackedObject Hand;


    private SteamVR_Controller.Device device;
    private Vector2 m_PadPosition;

    public void Start()
    {
        Hand = GetComponent<SteamVR_TrackedObject>();
    }

    public void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        // Set the default swipe to be none.
        SwipeDirection swipe = SwipeDirection.NONE;
        if (device == null)
        {
            device = SteamVR_Controller.Input((int)Hand.index);
            print("NUll Device");
            return;
        }
        if (device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            print("OnPressTrigger: " + OnPressTrigger);
            if (OnPressTrigger != null) OnPressTrigger();
        }
        if (device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            if (OnPressDownTrigger != null) OnPressDownTrigger();

        }
        if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            if (OnPressUpTrigger != null) OnPressUpTrigger();
        }

        if (device.GetTouch(EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            if (OnTouchPad != null) OnTouchPad();
            m_PadPosition = device.GetAxis();
        }
        if (device.GetTouchUp(EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            if (OnTouchPadUp != null) OnTouchPadUp();
            if (OnEndTouch != null) OnEndTouch(m_PadPosition);
            swipe = DetectSwipe();
            m_PadDownPosition = Vector2.zero;
        }
        if (device.GetTouchDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            if (OnTouchPadDown != null) OnTouchPadDown();
            if (OnBeginTouch != null) OnBeginTouch(m_PadDownPosition = device.GetAxis());
        }
        if (device.GetPressDown(EVRButtonId.k_EButton_Grip))
        {
            if (OnPressDownGripButton != null) { OnPressDownGripButton(); }
        }
        if (device.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu))
        {
            if (OnPressDownMenuButton != null) OnPressDownMenuButton();
        }
        if (swipe != SwipeDirection.NONE)
        {
            
            if (OnPadSwipe != null)
            {

                OnPadSwipe(swipe);
                if (device.GetTouch(EVRButtonId.k_EButton_SteamVR_Touchpad))
                {
                    m_PadPosition = device.GetAxis();
                }
                else
                {
                    m_PadDownPosition = Vector2.zero;
                }
            }
        }
    }

    private SwipeDirection DetectSwipe()
    {
        Vector2 swipeData = (m_PadPosition - m_PadDownPosition).normalized;

        bool swipeIsVertical = Mathf.Abs(swipeData.y) > m_SwipeWidth;

        bool swipeIsHorizontal = Mathf.Abs(swipeData.x) > m_SwipeWidth;
        if (swipeData.y > 0f && swipeIsVertical)
            return SwipeDirection.UP;

        if (swipeData.y < 0f && swipeIsVertical)
            return SwipeDirection.DOWN;

        if (swipeData.x > 0f && swipeIsHorizontal)
            return SwipeDirection.RIGHT;

        if (swipeData.x < 0f && swipeIsHorizontal)
            return SwipeDirection.LEFT;

        return SwipeDirection.NONE;
    }

    public void OnDisEnable()
    {
        OnPressTrigger = null;    //按住扳机键                           
        OnPressDownTrigger = null;  // 按下扳机键                            
        OnPressUpTrigger = null;    //抬起扳机键                              
        OnTouchPad = null;      //按住触摸板                       
        OnPressDownGripButton = null;   //按下侧键                          
        OnPressDownMenuButton = null;   //按下菜单键                         
        OnBeginTouch = null;  //触摸触摸板的位置                                
        OnEndTouch = null;//抬起触摸板的位置
        OnTouchPadDown = null;  //按下触摸板                              
        OnTouchPadUp = null;//抬起触摸板
        OnPadSwipe = null;
    }

}