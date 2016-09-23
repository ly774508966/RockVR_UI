using UnityEngine;
using System.Collections;
using System;
using RockVR.SDK;
using System.Collections.Generic;
using UnityEngine.Audio;
/// <summary>
/// 这是一个测试类，简单的测试了手柄的激活以及部分手柄的操作事件
/// 两个手柄分开注册，这样扩展性非常好，相同的按键可以做不同的处理
/// 大家可以补充测试
/// </summary>
public class TestEvent : MonoBehaviour
{
    public GameObject ButtonSure;
    public GameObject ButtonCancel;
    public GameObject ChangePoint01;
    public GameObject ChangePoint02;
    public GameObject ChangePoint03;
    public GameObject ChangePoint04;
    public GameObject CameraPanel;
    public GameObject ChangePosPanel;
    public RV2DCamera rvck2Dcamera;
    public List<GameObject> pointList;
    public Transform parent;
    void OnEnable()
    {
        print("OnRightDeviceActive");

        SteamVR_InitManager.Instance.OnLeftDeviceActive += OnLeftDeviceActive;//左手柄激活
        SteamVR_InitManager.Instance.OnRightDeviceActive += OnRightDeviceActive;//右手柄激活
    }

    private void OnRightDeviceActive(SteamVR_TrackedObject obj)
    {
        print("OnRightDeviceActive" + obj);
        SteamVR_InitManager.Instance.RightHandInput.OnPressDownTrigger += OnPressDownTrigger;
        SteamVR_InitManager.Instance.RightHandInput.OnPressDownMenuButton += OnPressDownMenuButton;
    }

    private void OnPressDownTrigger()
    {
        if (ChangRayScale.instance.OBJ != null)
        {
            switch (ChangRayScale.instance.OBJ.tag)
            {
                case "GameEvent":
                    //print(OnGameEventClick.ToString());
                    OnGameEventClick();
                    break;
                case "ButtonSure":
                    OnButtonSureClick();
                    break;
                case "ButtonCancel":
                    OnButtonCancelClick();
                    break;
                case "ButtonChangePos1":
                    OnButtonChangePosCilck(rvck2Dcamera, pointList[0].transform.position);
                    break;
                case "ButtonChangePos2":
                    OnButtonChangePosCilck(rvck2Dcamera, pointList[1].transform.position);
                    break;
                case "ButtonChangePos3":
                    OnButtonChangePosCilck(rvck2Dcamera, pointList[2].transform.position);
                    break;
                case "ButtonChangePos4":
                    OnButtonChangePosCilck(rvck2Dcamera, pointList[3].transform.position);
                    break;
                default:
                    print("aaaa");
                    break;
            }
        }
        print("OnPressDownTrigger++++RightHandInput");
    }

    private void OnPressDownMenuButton()
    {
        if (rvck2Dcamera.Capturing())
        {
            rvck2Dcamera.FinishCapture();
        }
        else
        {
            rvck2Dcamera.StartCapture();
        }
        print("OnTouchPadDown");
    }

    private void OnLeftDeviceActive(SteamVR_TrackedObject obj)
    {
        print("OnLeftDeviceActive" + obj);
        SteamVR_InitManager.Instance.LeftHandInput.OnPressDownTrigger += OnPressDownTrigger;
        SteamVR_InitManager.Instance.LeftHandInput.OnPressDownMenuButton += OnPressDownMenuButton;
    }


    void OnDisable()
    {
        SteamVR_InitManager.Instance.OnLeftDeviceActive -= OnLeftDeviceActive;//左手柄激活
        SteamVR_InitManager.Instance.OnRightDeviceActive -= OnRightDeviceActive;//右手柄激活
    }
    void Awake()
    {
        OnButtonCancelClick();
    }
    public void OnGameEventClick()
    {
        if (CameraPanel != null && ChangePosPanel != null)
        {
            CameraPanel.gameObject.SetActive(true);
            ChangePosPanel.gameObject.SetActive(false);
        }
    }

    public void OnButtonSureClick()
    {
        if (CameraPanel != null && ChangePosPanel != null)
        {
            CameraPanel.gameObject.SetActive(false);
            ChangePosPanel.gameObject.SetActive(true);
        }

    }

    public void OnButtonCancelClick()
    {
        if (CameraPanel != null && ChangePosPanel != null)
        {
            CameraPanel.gameObject.SetActive(false);
            ChangePosPanel.gameObject.SetActive(false);
        }
    }
    public void OnButtonChangePosCilck(RV2DCamera camera, Vector3 Pos)
    {

        if (!camera.Enabled())
        {
            camera.EnableCamera();
        }
        print(camera.m_CaptureCamera.transform.position);
        camera.gameObject.transform.SetParent(parent, false);
        camera.gameObject.transform.localPosition = new Vector3(0.0f, 0.11f, 0.11f);
        camera.gameObject.transform.localRotation = Quaternion.identity;
        camera.m_CaptureCamera.transform.position = Pos;
    }
}
