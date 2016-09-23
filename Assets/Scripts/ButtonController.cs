using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RockVR.SDK;

public class ButtonController : MonoBehaviour
{
    private static ButtonController instance;
    public GameObject ButtonSure;
    public GameObject ButtonCancel;
    public GameObject ChangePoint01;
    public GameObject ChangePoint02;
    public GameObject ChangePoint03;
    public GameObject ChangePoint04;
    public GameObject CameraPanel;
    public GameObject ChangePosPanel;

    public static ButtonController Instance
    {
        get
        {
            if (instance==null)
            {
                instance = GameObject.FindObjectOfType<ButtonController>();
            }      
            return instance;
        }

    }

    void Awake()
    {
        OnGameEventClick();
    }
    public void OnGameEventClick()
    {
        if (CameraPanel != null && ChangePosPanel != null)
        {
            CameraPanel.gameObject.SetActive(false);
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

    public void OnButtonChangePosCilck(RV2DCamera Camera,Vector3 Pos)
    {

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
