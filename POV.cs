using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POV : MonoBehaviour
{
    private new Transform transform; // 此處使用new關鍵字隱藏父類的同名成員
    private Transform cameraTransform; // 攝像機的Transform元件
    private Vector3 cameraRotation; // 攝像機旋轉角度
    private GameObject player;
    private player_controller playercontroller;
    private MMD4MecanimModel mmd;
    private MMD4MecanimModelImpl.Morph morph;

    public bool thirdPOV = false;
    public Vector3 cmaeraoffset;
    public Camera main_camera;
    void Start()
    {
        transform = GetComponent<Transform>();
        cameraTransform = main_camera.GetComponent<Transform>();
        cameraRotation = cameraTransform.eulerAngles;

        player = GameObject.Find("player");
        playercontroller = player.GetComponent<player_controller>();
        mmd = player.GetComponent<MMD4MecanimModel>();
    }
    void Update()
    {
        if (morph == null)
        {
            morph = mmd.GetMorph("Ivisible Body");
            if (morph != null)
                morph.weight = 1;
        }
        if (playercontroller.main_mode != player_controller.Main_State.sex)
        {
            Control();
            if (Input.GetKeyDown(KeyCode.Q) && thirdPOV == false)
            {
                //轉第三人稱
                thirdPOV = true;
                morph.weight = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Q) && thirdPOV == true)
            {
                //轉第一人稱
                thirdPOV = false;
                morph.weight = 1;
            }
        }
    }
    private void Control()
    {
        float rh = Input.GetAxis("Mouse X");
        float rv = Input.GetAxis("Mouse Y");

        // 旋轉攝像機
        cameraRotation.x -= rv;
        cameraRotation.y += rh;
        cameraTransform.eulerAngles = cameraRotation;
        // 使主角的面向方向與攝像機一致
        Vector3 rotation = cameraTransform.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        transform.eulerAngles = rotation;

        Vector3 position = transform.position;
        if (thirdPOV == true) //第三人稱
        {
            cameraTransform.position = position + cmaeraoffset + -1 * transform.forward;
        }
        else //第一人稱
        {
            cameraTransform.position = position + cmaeraoffset;
        }
    }
}
