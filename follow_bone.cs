using UnityEngine;
using System.Collections;

public class follow_bone : MonoBehaviour
{
    private float moveSpeed = 10.0f;
    private float rotateSpeed = 90.0f;
    private Vector3 cameraRotation;
    private player_controller playercoll;

    public Transform target;
    public int third_pov = 0;
    public float pov_offset = 0;
    public Vector3 third_pov_offset;
    void Start()
    {
        playercoll = GameObject.Find("player").GetComponent<player_controller>();
    }
    void Update()
    {
        if (playercoll.sex_state == player_controller.Sex_State.free_camera)
        {
            float rh = Input.GetAxis("Mouse X");
            float rv = Input.GetAxis("Mouse Y");

            cameraRotation.x -= rv;
            cameraRotation.y += rh;
            transform.eulerAngles = cameraRotation;

            float moveVertical = Input.GetAxis("Vertical");
            float moveHorizontal = Input.GetAxis("Horizontal");
            float jump_axis = Input.GetAxis("Jump");
            if (Input.GetKey(KeyCode.LeftShift))
            {
                jump_axis *= -1;
            }
            third_pov_offset = transform.forward * moveVertical * 0.03f;
            third_pov_offset += transform.right * moveHorizontal * 0.03f;
            third_pov_offset += transform.up * jump_axis * 0.03f;
            transform.position += third_pov_offset;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position + target.forward * pov_offset, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, rotateSpeed * Time.deltaTime);

            float rh = Input.GetAxis("Mouse X");
            float rv = Input.GetAxis("Mouse Y");

            cameraRotation.x -= rv;
            cameraRotation.y += rh;
            transform.eulerAngles = cameraRotation;
        }
    }
}