using UnityEngine;
using System.Collections;

public class ChangRayScale : MonoBehaviour
{
    public static ChangRayScale instance;
    private Transform ControllerPoint;
    private Transform ControllerSign;
    public GameObject OBJ=null;
    // Use this for initialization

    void Awake()
    {
        instance = this;

    }
    void Start()
    {
        ControllerPoint = transform.Find("ray");
        ControllerSign = transform.Find("Sign");
        ControllerPoint.gameObject.SetActive(true);
        ControllerSign.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(ControllerPoint.transform.position, ControllerPoint.transform.forward * 100);
        Debug.DrawLine(ControllerPoint.transform.position, ControllerPoint.transform.position + ControllerPoint.transform.forward * 100, Color.red);  //绘制射线，包含发射位置，发射距离和射线的颜色；  
        RaycastHit hitInfo;                                 //定义一个RaycastHit变量用来保存被撞物体的信息；  
        if (Physics.Raycast(ray, out hitInfo, 100))         //如果碰撞到了物体，hitInfo里面就包含该物体的相关信息；  
        {
            
            float Temp = Vector3.Distance(ControllerPoint.transform.position, hitInfo.point);
            ControllerPoint.transform.localScale = new Vector3(1, 1, Temp);
            ControllerSign.gameObject.SetActive(true);
            ControllerSign.transform.position = hitInfo.point;
            OBJ = hitInfo.collider.gameObject;
            print(OBJ.tag);
        }
        else
        {
            ControllerPoint.transform.localScale = new Vector3(1, 1, 100);
            ControllerSign.gameObject.SetActive(false);
            OBJ = null;
        }
    }
}
