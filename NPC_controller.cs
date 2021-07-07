using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using Main_State = player_controller.Main_State;
using s_State = player_controller.s_State;
using PixelCrushers.DialogueSystem;
using UnityEditor;

public class NPC_controller : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    private GameObject player;
    private player_controller palyercontroll;
    private string json;
    private NavMeshAgent agent;
    private MMD4MecanimModel mmd;
    private MMD4MecanimModelImpl.Anim tmp = new MMD4MecanimModelImpl.Anim();
    private List<MMD4MecanimModelImpl.Anim> terms = new List<MMD4MecanimModelImpl.Anim>();
    private Camera main_camera;
    private Vector3 s_offset = new Vector3(0.0f, 0.0f, 0.0f);
    private GameObject head;
    private GameObject neck;
    private MMD4MecanimModelImpl.Morph morph;
    private float timer;
    private Collider coll;

    public bool init;
    public MMD4MecanimBone head_bone;
    public MMD4MecanimBone neck_bone;
    public string s_path = "";
    public bool look_at_camera = false;
    public Main_State main_mode;
    public s_State s_state;
    public Vector3 init_position;
    public string prefeb_path;
    public float wanderRadius;
    public float wanderTimer;
    // Start is called before the first frame update
    void Start()
    {
        //if (init == false)
            init_npc();

        anim = gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("player");
        palyercontroll = player.GetComponent<player_controller>();
        mmd = GetComponent<MMD4MecanimModel>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        main_camera = Camera.main;

        //gameObject.transform.position = init_position;
        if(gameObject.transform.parent == null)
            gameObject.transform.parent = GameObject.Find("NPC_p").transform;
        main_mode = Main_State.normal;
        s_state = s_State.initial;
        agent.speed = 2.0f;
        wanderRadius = 200.0f;
        wanderTimer = 10.0f;

        //coll = gameObject.GetComponent<Collider>();
        //coll.enabled = false;
        //rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (main_mode)
        {
            case Main_State.normal:
                agent.isStopped = false;
                checke_move();
                if (look_at_camera == true)
                {
                    agent.isStopped = true;
                    look_camera();
                }

                if(agent.remainingDistance < 3)
                    timer += Time.deltaTime;
                if (timer >= wanderTimer)
                {
                    Vector3 newPos = RandomNavSphere(player.transform.position, wanderRadius, 17);
                    agent.SetDestination(newPos);
                    timer = 0;
                }

                break;

            case Main_State.dialog:
                agent.isStopped = true;
                anim.SetBool("walk_f", false);
                anim.SetBool("walk_r", false);
                anim.SetBool("walk_l", false);
                if (look_at_camera == true && anim.GetCurrentAnimatorStateInfo(0).IsName("WAIT01"))
                {
                    look_camera();
                }
                break;

            case Main_State.s:
                s();
                break;

            case Main_State.followed:
                agent.isStopped = false;
                Vector3 v = new Vector3(player.gameObject.transform.position.x, transform.position.y, player.gameObject.transform.position.z);
                agent.SetDestination(v);
                checke_move();
                agent.speed = 5.0f;
                if (look_at_camera == true && anim.GetCurrentAnimatorStateInfo(0).IsName("WAIT01"))
                {
                    look_camera();
                }
                break;
        }
    }
    void FixedUpdate()
    {
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == player)
        {
            rb.freezeRotation = true;
        }
    }
    private void OnCollisionStay(Collision other)
    {

    }
    void OnTriggerStay(Collider other)
    {

    }
    public void initial_dialog()
    {
        Vector3 lookAt = main_camera.transform.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);
    }
    public void switch_follow()
    {
        agent.isStopped = !agent.isStopped;
    }
    public void look_camera()
    {
        if (head_bone == null)
        {
            if (head == null)
            {
                GameObject tmp;
                tmp = get_head(gameObject, "Root");
                tmp = get_head(tmp, "HipMaster");
                if (tmp == null) return;
                tmp = get_head(tmp, "Torso");
                tmp = get_head(tmp, "Torso2");
                neck = get_head(tmp, "Neck");
                head = get_head(neck, "Head");
            }
            if (head != null)
            {
                head_bone = head.GetComponent<MMD4MecanimBone>();
                neck_bone = neck.GetComponent<MMD4MecanimBone>();
            }
        }

        if (head != null)
        {
            Vector3 targetDir = main_camera.transform.position - head.transform.position;
            Quaternion angle = Quaternion.FromToRotation(transform.forward, targetDir);
            if (angle.eulerAngles.y < 45 || 315 < angle.eulerAngles.y)
            {
                head_bone.userRotation = angle;
            }

            if (targetDir.magnitude > 3)
            {
                look_at_camera = false;
                head_bone.userRotation = Quaternion.identity;
            }
        }
        
    }
    public void load_morph(string file_path, string layer_name) 
    {
        if (mmd != null)
        {
            Debug.Log(file_path);
            tmp.animFile = Resources.Load<TextAsset>(file_path);
            tmp.animatorStateName = "Base Layer." + layer_name;
            tmp.animatorStateNameHash = Animator.StringToHash(tmp.animatorStateName);
            tmp.audioClip = Resources.Load<AudioClip>(s_path + "/s");
            tmp.animData = MMD4MecanimData.BuildAnimData(tmp.animFile);

            terms.Add(tmp);
            mmd.animList = terms.ToArray();
            terms.Clear();

            MMD4MecanimAnim.InitializeAnimModel(mmd);
        }
    }
    void s()
    {
        switch (s_state)
        {
            case s_State.initial:
                //load offset
                if (File.Exists("Assets\\Resources\\" + s_path + "\\offset.txt"))
                {
                    json = File.ReadAllText("Assets\\Resources\\" + s_path + "\\offset.txt");
                    s_offset = JsonUtility.FromJson<Vector3>(json);
                }

                //load anim and audio
                if (anim.GetBool("used") == false)
                {
                    load_morph(s_path + "/girl.anim", "girl_vmd");    
                }

                if (mmd != null) morph = mmd.GetMorph("n");
                if (morph != null) morph.weight = 1;

                s_state = palyercontroll.s_state;
                break;

            case s_State.main_loop:
                checke_move();

                this.transform.rotation = player.transform.rotation;
                this.transform.localPosition = s_offset;

                s_state = palyercontroll.s_state;
                break;

            case s_State.change_position:
                float moveVertical = Input.GetAxis("Vertical");
                float moveHorizontal = Input.GetAxis("Horizontal");
                float jump_axis = Input.GetAxis("Jump");
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    jump_axis *= -1;
                }
                s_offset += new Vector3(moveHorizontal * 0.01f, jump_axis * 0.003f, moveVertical * 0.01f);
                this.transform.localPosition = s_offset;

                s_state = palyercontroll.s_state;
                break;

            case s_State.free_camera:
                this.transform.rotation = player.transform.rotation;
                this.transform.localPosition = s_offset;

                s_state = palyercontroll.s_state;
                break;

            case s_State.finish:
                s_finish();
                s_state = s_State.initial;
                break;

            default:
                break;
        }
    }
    void s_finish()
    {
        //finish s
        this.transform.parent = GameObject.Find("NPC_p").transform;
        transform.position += new Vector3(0.0f, 0.0f, 0.3f);
        anim.SetBool("s1", false);

        if (anim.GetBool("used") == false)
        {
            load_morph("事後/s_after.anim", "s_after");
            if (mmd != null) morph = mmd.GetMorph("n");
            if (morph != null) morph.weight = 1;
        }

        //save offset file
        var sr = File.CreateText("Assets\\Resources\\" + s_path + "\\offset.txt");
        json = JsonUtility.ToJson(s_offset);
        sr.WriteLine(json);
        sr.Close();
    }
    void checke_move()
    {
        float angle = Vector3.SignedAngle(rb.velocity, -transform.right, Vector3.up);
        angle = angle + 180;
        if (System.Math.Abs(rb.velocity.x) < 0.1f && System.Math.Abs(rb.velocity.y) < 0.1f && System.Math.Abs(rb.velocity.z) < 0.1f)
        {
            anim.SetBool("walk_f", false);
            anim.SetBool("walk_r", false);
            anim.SetBool("walk_l", false);
        }
        if (!(System.Math.Abs(rb.velocity.x) < 0.1f && System.Math.Abs(rb.velocity.y) < 0.1f && System.Math.Abs(rb.velocity.z) < 0.1f))
        {
            if (45 <= angle && angle < 135)
            {
                anim.SetBool("walk_f", true);
            }
            else if (225 <= angle && angle < 315)
            {
                anim.SetBool("walk_f", true);
            }
            else if (315 <= angle || angle < 45)
            {
                anim.SetBool("walk_r", true);
            }
            else if (135 <= angle && angle < 225)
            {
                anim.SetBool("walk_l", true);
            }
        }

        if (System.Math.Abs(agent.velocity.x) < 0.1f && System.Math.Abs(agent.velocity.y) < 0.1f && System.Math.Abs(agent.velocity.z) < 0.1f)
        {
            anim.SetBool("walk_f", false);
        }
        else
        {
            anim.SetBool("walk_f", true);
        }
    }
    void OnApplicationQuit()
    {
        /*using (var editScope = new PrefabUtility.EditPrefabContentsScope("Assets/Resources/" + prefeb_path))
        {
            editScope.prefabContentsRoot.GetComponent<NPC_controller>().init_position = gameObject.transform.position;
        }*/
    }
    void init_npc()
    {
        using (var editScope = new PrefabUtility.EditPrefabContentsScope(prefeb_path))
        {
            GameObject sample = GameObject.Find("sample_npc");
            Component[] components = sample.gameObject.GetComponents(typeof(Component));
            foreach (Component component in components)
            {
                if (gameObject.GetComponent(component.GetType()) == null)
                {
                    editScope.prefabContentsRoot.AddComponent(component.GetType());
                    gameObject.AddComponent(component.GetType());
                }
            }

            gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("anime_controller/npc") as RuntimeAnimatorController;

            gameObject.layer = 15;

            gameObject.GetComponent<DialogueSystemTrigger>().conversation = "normal";

            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            gameObject.GetComponent<NavMeshAgent>().baseOffset = 0.0f;
            gameObject.GetComponent<NavMeshAgent>().stoppingDistance = 2.0f;

            gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.77f, 0);
            gameObject.GetComponent<CapsuleCollider>().radius = 0.15f;
            gameObject.GetComponent<CapsuleCollider>().height = 1.6f;

            gameObject.GetComponent<NPC_controller>().prefeb_path = prefeb_path;
            gameObject.GetComponent<NPC_controller>().init_position = gameObject.transform.position;
            gameObject.GetComponent<NPC_controller>().init = true;

            editScope.prefabContentsRoot.GetComponent<Animator>().runtimeAnimatorController = gameObject.GetComponent<Animator>().runtimeAnimatorController;

            editScope.prefabContentsRoot.layer = 15;

            editScope.prefabContentsRoot.GetComponent<DialogueSystemTrigger>().conversation = "normal";

            editScope.prefabContentsRoot.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            editScope.prefabContentsRoot.GetComponent<NavMeshAgent>().baseOffset = 0.0f;
            editScope.prefabContentsRoot.GetComponent<NavMeshAgent>().stoppingDistance = 2.0f;

            editScope.prefabContentsRoot.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.77f, 0.05f);
            editScope.prefabContentsRoot.GetComponent<CapsuleCollider>().radius = 0.1f;
            editScope.prefabContentsRoot.GetComponent<CapsuleCollider>().height = 1.6f;

            editScope.prefabContentsRoot.GetComponent<NPC_controller>().prefeb_path = prefeb_path;
            editScope.prefabContentsRoot.GetComponent<NPC_controller>().init_position = gameObject.transform.position;
            editScope.prefabContentsRoot.GetComponent<NPC_controller>().init = true;
        }
    }
    GameObject get_head(GameObject parent, string name)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if(parent.transform.GetChild(i).gameObject.name.Contains(name))
            {
                return parent.transform.GetChild(i).gameObject;
            }
        }
        return null;
    }
}
