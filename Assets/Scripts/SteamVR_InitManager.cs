using UnityEngine;
using System.Collections;
using Valve.VR;
using System;
public class SteamVR_InitManager : MonoBehaviour
{
    public event Action<SteamVR_TrackedObject> OnLeftDeviceActive;//左手柄激活事件
    public event Action<SteamVR_TrackedObject> OnRightDeviceActive;//右手柄激活事件

    public SteamVR_TrackedObject LeftObject;
    public SteamVR_TrackedObject RightObject;

    private bool[] isAllConnect=new bool[2] {false,false };//0代表右手状态，1代表左手状态
    private uint leftIndex = 100;//左手柄对应的设备ID
    private uint rightIndex = 100;//右手柄对应的设备ID


    private static SteamVR_InitManager instance;

    public DeviceInput LeftHandInput;
    public DeviceInput RightHandInput;
    public static SteamVR_InitManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SteamVR_InitManager>();
            }
            return instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        LeftObject = transform.FindChild("LeftHand").GetComponent<SteamVR_TrackedObject>();
        RightObject = transform.FindChild("RightHand").GetComponent<SteamVR_TrackedObject>();
        LeftObject.gameObject.SetActive(false);
        RightObject.gameObject.SetActive(false);
    }
    void Start()
    {
        StartCoroutine(CheckDeviceActive());
    }
    void OnEnable()
    {
        OnLeftDeviceActive += LeftDeviceActive;
        OnRightDeviceActive += RightDeviceActive;
    }
    void OnDisable()
    {
        OnLeftDeviceActive -= LeftDeviceActive;
        OnRightDeviceActive -= RightDeviceActive;
    }

    /// <summary>
    /// 检测手柄设备是否激活
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckDeviceActive()
    {
        yield return new WaitForSeconds(1);
        
        while (true)
        {
            for (uint i = 1; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                if (i == leftIndex || i == rightIndex) continue;//已经初始化的不再进入判断
                if (OpenVR.System != null && OpenVR.System.IsTrackedDeviceConnected(i))
                {
                    OnDeviceConnected(new object[] { i, true });
                }
            }

            yield return new WaitForFixedUpdate();
        }

    }

    /// <summary>
    /// 检测激活的设备是否是手柄
    /// </summary>
    /// <param name="args"></param>
    private void OnDeviceConnected(object[] args)
    {
        if (args != null && args.Length > 1)
        {
            uint index = (uint)args[0];
            bool isConnect = (bool)args[1];
            var system = OpenVR.System;
            if (isConnect && system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
            {
                uint tmpleftIndex = (uint)system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
                uint tmprightIndex = (uint)system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
                if (index == tmprightIndex)
                {
                    isAllConnect[0] = true;
                    rightIndex = index;
                    OnRightDeviceActive(RightObject);
                }
                else if (index == tmpleftIndex)
                {
                    isAllConnect[1] = true;
                    leftIndex = index;
                    OnLeftDeviceActive(LeftObject);
                }
            }
        }
    }

    private void RightDeviceActive(SteamVR_TrackedObject obj)
    {
        DeviceActive(obj, rightIndex);
        RightHandInput = obj.GetComponent<DeviceInput>();
    }

    private void LeftDeviceActive(SteamVR_TrackedObject obj)
    {
        DeviceActive(obj, leftIndex);
        LeftHandInput = obj.GetComponent<DeviceInput>();
    }
    /// <summary>
    /// 匹配对应的设备号，完成手柄模型的设置
    /// </summary>
    /// <param name="device"></param>
    /// <param name="index"></param>
    void DeviceActive(SteamVR_TrackedObject device, uint index)
    {
        SteamVR_TrackedObject.EIndex eIndex = (SteamVR_TrackedObject.EIndex)Enum.Parse(typeof(SteamVR_TrackedObject.EIndex), "Device" + index);
        device.index = eIndex;
        device.GetComponentInChildren<SteamVR_RenderModel>().index = eIndex;
        device.gameObject.SetActive(true);
    }

}
