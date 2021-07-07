using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.IO;
using PixelCrushers.DialogueSystem;

public class player_controller : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private Collider coll;
    private POV my_pov;
    private follow_bone bone;
    private Animator npc_animator;
    private float speed = 5.0f;
    private MMD4MecanimModel mmd;
    private MMD4MecanimModelImpl.Morph morph;
    private AnimationClip s_man;
    private AnimationClip s_girl;

    public bool ground = true;
    public GameObject NPC;
    public NPC_controller npc_control;
    public Camera main_camera;
    public AnimatorOverrideController player_overrideController;
    public AnimatorOverrideController npc_overrideController;
    public string s_path;
    public Canvas X_ray;
    public enum Main_State
    {
        normal = 0,
        dialog = 1,
        s = 2,
        followed = 3
    }
    public Main_State main_mode;
    public enum s_State
    {
        initial = 0,
        main_loop = 1,
        change_position = 2,
        free_camera = 3,
        finish = 4
    }
    public s_State s_state;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = player_overrideController;
        coll = gameObject.GetComponent<Collider>();
        my_pov = GetComponent<POV>();
        bone = main_camera.gameObject.GetComponent<follow_bone>();
        main_mode = Main_State.normal;
        mmd = gameObject.GetComponent<MMD4MecanimModel>();
    }
    void Update()
    {
        switch (main_mode)
        {
            case Main_State.normal:
                detect_NPC();
                CheckKey();
                break;

            case Main_State.dialog:
                break;

            case Main_State.s:
                s_change_mode();
                break;

            case Main_State.followed:
                detect_NPC();
                CheckKey();
                break;
            default:
                break;
        }
    }
    void FixedUpdate()
    {
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");
        if (s_state != s_State.change_position && s_state != s_State.free_camera && main_mode != Main_State.dialog)
        {
            if (ground == true)
            {
                rb.velocity += gameObject.transform.forward * speed * moveVertical;
                rb.velocity += gameObject.transform.right * speed * moveHorizontal;
            }
            else if (ground == false)
            {
                rb.velocity += gameObject.transform.forward * speed * moveVertical;
                rb.velocity += gameObject.transform.right * speed * moveHorizontal;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
    }
    private void OnTriggerStay(Collider other)
    {
    }
    private void OnTriggerExit(Collider other)
    {
    }
    private void OnCollisionEnter(Collision other)
    {

    }
    private void OnCollisionStay(Collision other)
    {
    }
    private void OnCollisionExit(Collision other)
    {
    }
    void select_NPC()
    {
        Ray ray = new Ray(main_camera.transform.position, main_camera.transform.forward * 100);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, 1 << 15))
        {
            NPC = hit.collider.gameObject;
            npc_control = NPC.gameObject.GetComponent<NPC_controller>();
        }
    }
    void detect_NPC()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 2.5f);
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.layer == 15)
                {
                    NPC_controller tmp = cols[i].gameObject.GetComponent<NPC_controller>();
                    tmp.look_at_camera = true;
                }
            }
        }
    }
    public void initial_dialog()
    {
        select_NPC();
        main_mode = Main_State.dialog;
        //get npc component
        npc_control.main_mode = Main_State.dialog;
        npc_animator = NPC.gameObject.GetComponent<Animator>();
        npc_animator.runtimeAnimatorController = npc_overrideController;
        npc_control.initial_dialog();

        //change camera
        Vector3 target = NPC.transform.position;
        target.y = main_camera.transform.position.y - 0.08f;
        main_camera.transform.LookAt(target);

        main_camera.transform.position -= main_camera.transform.forward * 0.15f;
        main_camera.transform.position -= new Vector3(0, 0.12f, 0);

        my_pov.enabled = false;
    }
    public void switch_NavMeshAgent()
    {
        NPC.gameObject.GetComponent<NavMeshAgent>().isStopped = !NPC.gameObject.GetComponent<NavMeshAgent>().isStopped;
    }
    public void initial_use()
    {
        DialogueLua.SetVariable("end_use", false);
        main_mode = Main_State.s;
        npc_control.main_mode = Main_State.s;

        Physics.IgnoreCollision(coll, NPC.gameObject.GetComponent<Collider>(), true);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        bone.enabled = true;
        npc_control.s_path = s_path;
        morph = mmd.GetMorph("Ivisible Body");
        morph.weight = 0;
        morph = mmd.GetMorph("Ivisible Head");
        morph.weight = 1;

        NPC.transform.parent = transform;
        NPC.transform.rotation = transform.rotation;
        NPC.transform.localPosition = new Vector3(0, 0, 0);

        if(npc_control.head_bone != null) npc_control.head_bone.userRotation = Quaternion.identity;

        X_ray.enabled = true;
    }
    public void all_var_false()
    {
        animator.SetBool("loop", false);
        npc_animator.SetBool("loop", false);
        animator.SetBool("c", false);
        npc_animator.SetBool("c", false);
        animator.SetBool("finish", false);
        npc_animator.SetBool("finish", false);
    }
    public void finish_use()
    {
        animator.SetBool("used", false);
        animator.Play("WAIT");
        all_var_false();

        s_state = s_State.finish;
    }
    public void initial_s()
    {
        s_man = Resources.Load<AnimationClip>(s_path + "/man_vmd");
        s_girl = Resources.Load<AnimationClip>(s_path + "/girl_vmd");
        player_overrideController["default_s"] = s_man;
        npc_overrideController["default_s"] = s_girl;
        main_mode = Main_State.s;
        npc_control.main_mode = Main_State.s;

        Physics.IgnoreCollision(coll, NPC.gameObject.GetComponent<Collider>(), true);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        bone.enabled = true;
        npc_control.s_path = s_path;
        morph = mmd.GetMorph("Ivisible Body");
        morph.weight = 0;
        morph = mmd.GetMorph("Ivisible Head");
        morph.weight = 1;

        NPC.transform.parent = transform;
        NPC.transform.rotation = transform.rotation;
        NPC.transform.localPosition = new Vector3(0, 0, 0);

        X_ray.enabled = true;
    }
    public void s_change_mode()
    {
        switch (s_state)
        {
            case s_State.initial:
                s_state = s_State.main_loop;
                break;

            case s_State.main_loop:
                CheckKey();

                if (Input.GetKey(KeyCode.Z))
                {
                    s_state = s_State.change_position;
                }
                if (Input.GetKey(KeyCode.C))
                {
                    morph = mmd.GetMorph("Ivisible Head");
                    morph.weight = 0;
                    s_state = s_State.free_camera;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    s_state = s_State.finish;
                }
                break;

            case s_State.free_camera:
                if (Input.GetKey(KeyCode.V))
                {
                    morph = mmd.GetMorph("Ivisible Head");
                    morph.weight = 1;
                    s_state = s_State.main_loop;
                }
                break;

            case s_State.change_position:
                if (Input.GetKey(KeyCode.X))
                {
                    s_state = s_State.main_loop;
                }
                break;

            case s_State.finish:
                Physics.IgnoreCollision(coll, NPC.gameObject.GetComponent<Collider>(), false);
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                bone.enabled = false;
                animator.SetBool("s", false);
                morph = mmd.GetMorph("Ivisible Body");
                morph.weight = 1;
                morph = mmd.GetMorph("Ivisible Head");
                morph.weight = 0;

                s_state = s_State.initial;
                npc_control.s_state = s_State.finish;
                main_mode = Main_State.normal;
                DialogueLua.SetVariable("end_use", true);

                X_ray.enabled = false;
                break;
            default:
                break;
        }
    }
    void CheckKey()
    {
        float angle = Vector3.SignedAngle(rb.velocity, -transform.right, Vector3.up);
        angle = angle + 180;
        if (System.Math.Abs(rb.velocity.x) < 0.1f && System.Math.Abs(rb.velocity.y) < 0.1f && System.Math.Abs(rb.velocity.z) < 0.1f)
        {
            animator.SetBool("walk_f", false);
            animator.SetBool("walk_r", false);
            animator.SetBool("walk_b", false);
            animator.SetBool("walk_l", false);
        }
        if (!(System.Math.Abs(rb.velocity.x) < 0.1f && System.Math.Abs(rb.velocity.y) < 0.1f && System.Math.Abs(rb.velocity.z) < 0.1f))
        {
            if (45 <= angle && angle < 135)
            {
                animator.SetBool("walk_f", true);
            }
            else if (225 <= angle && angle < 315)
            {
                animator.SetBool("walk_b", true);
            }
            else if (315 <= angle || angle < 45)
            {
                animator.SetBool("walk_r", true);
            }
            else if (135 <= angle && angle < 225)
            {
                animator.SetBool("walk_l", true);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("run", true);
            speed = 10.0f;
        }
        else
        {
            animator.SetBool("run", false);
            speed = 5.0f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (ground == true)
            {
                rb.velocity += new Vector3(0, 1.5f, 0);
                ground = false;
            }
            animator.SetBool("jump", true);
        }
        else
        {
            animator.SetBool("jump", false);
        }

        if (ground == true)
        {
            animator.SetBool("ground", true);
        }
        else
        {
            animator.SetBool("ground", false);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            my_pov.cmaeraoffset.y = 1.0f;
        }
        else
        {
            my_pov.cmaeraoffset.y = 1.5f;
        }
    }
}
