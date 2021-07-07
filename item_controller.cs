using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.IO;
using PixelCrushers.DialogueSystem;
using Main_State = player_controller.Main_State;
using Sex_State = player_controller.Sex_State;

public class item_controller : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    private GameObject NPC;
    public GameObject NPC1;
    public GameObject NPC2;
    public GameObject NPC3;
    public GameObject NPC4;
    private List<GameObject> NPC_list = new List<GameObject>();
    private player_controller palyercontroll;
    private NPC_controller npc_control;
    private AnimatorOverrideController NPC_AOC;
    private AnimationClip sex_girl;
    private string dir;
    private string state;
    private Rigidbody rb;
    private Collider coll;
    private follow_bone bone;
    private MMD4MecanimModelImpl.Morph morph;
    private MMD4MecanimModel mmd;

    public string conversion_name;
    void Start()
    {
        player = GameObject.Find("player");
        palyercontroll = player.GetComponent<player_controller>();
        rb = player.GetComponent<Rigidbody>();
        coll = player.GetComponent<Collider>();
        bone = Camera.main.gameObject.GetComponent<follow_bone>();
        mmd = player.GetComponent<MMD4MecanimModel>();

        int j = 0;
        NPC_list.Add(NPC1);
        NPC_list.Add(NPC2);
        NPC_list.Add(NPC3);
        NPC_list.Add(NPC4);
        foreach (GameObject prefab_ in NPC_list)
        {
            if (prefab_ != null)
            {
                get_aoc(conversion_name + j.ToString());
                NPC_start(prefab_);
                j++;
            }
        }
        play_anim("event_wait");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void get_aoc(string name)
    {
        string path = Path.Combine("anime_controller", name);
        NPC_AOC = Resources.Load<AnimatorOverrideController>(path);
    }
    void sset_dir(string dir_)
    {
        dir = dir_;
    }
    public void sset_state(string state_)
    {
        state = state_;
    }
    void load_girl_motion(string motion)
    {
        string path = Path.Combine("Ê¹ÓÃ„Ó×÷", dir, motion + "_vmd");
        Debug.Log(path);
        sex_girl = Resources.Load<AnimationClip>(path);

        NPC_AOC[state] = sex_girl;
    }
    void start_use(string motion)
    {
        foreach (GameObject prefab_ in NPC_list)
        {
            if(prefab_ != null)
            {
                npc_control = prefab_.GetComponent<NPC_controller>();
                npc_control.main_mode = Main_State.sex;
            }
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        coll.enabled = false;
        bone.enabled = true;
        morph = mmd.GetMorph("Ivisible Body");
        morph.weight = 0;
        morph = mmd.GetMorph("Ivisible Head");
        morph.weight = 1;

        player.transform.parent = gameObject.transform;
        player.transform.rotation = transform.rotation;
        player.transform.localPosition = Vector3.zero;
        palyercontroll.main_mode = Main_State.sex;
    }
    void end_use(string motion)
    {
        foreach (GameObject prefab_ in NPC_list)
        {
            if (prefab_ != null)
            {
                npc_control = prefab_.GetComponent<NPC_controller>();
                npc_control.main_mode = Main_State.dialog;
            }
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = true;
        coll.enabled = true;
        bone.enabled = false;
        morph = mmd.GetMorph("Ivisible Body");
        morph.weight = 1;
        morph = mmd.GetMorph("Ivisible Head");
        morph.weight = 0;

        player.transform.parent = null;
        palyercontroll.main_mode = Main_State.normal;
    }
    void release_NPC(GameObject NPC_)
    {
        NPC_.transform.parent = GameObject.Find("±ãÆ÷").transform;

        NPC_.GetComponent<NavMeshAgent>().enabled = true;
        NPC_.GetComponent<Collider>().enabled = true;
        NPC_.GetComponent<Usable>().enabled = true;
        NPC_.GetComponent<Rigidbody>().isKinematic = false;
        Physics.IgnoreCollision(coll, NPC_.gameObject.GetComponent<Collider>(), false);

        NPC_.GetComponent<NPC_controller>().sex_state = Sex_State.finish;

        Animator anim = NPC_.GetComponent<Animator>();
        anim.SetBool("used", false);
        anim.Play("WAIT01");
    }
    void NPC_start(GameObject NPC_)
    {
        NPC_.transform.parent = gameObject.transform;
        NPC_.transform.localPosition = Vector3.zero;
        NPC_.transform.rotation = NPC_.transform.parent.rotation;

        NPC_.GetComponent<NavMeshAgent>().enabled = false;
        NPC_.GetComponent<Collider>().enabled = false;
        NPC_.GetComponent<Usable>().enabled = false;
        NPC_.GetComponent<Rigidbody>().isKinematic = true;
        Physics.IgnoreCollision(coll, NPC_.gameObject.GetComponent<Collider>(), true);

        Animator NPC_animator = NPC_.GetComponent<Animator>();
        NPC_animator.runtimeAnimatorController = NPC_AOC;
    }
    public void get(string idx)
    {
        NPC = palyercontroll.NPC;
        NPC_start(NPC);
        switch (idx)
        {
            case "1":
                if(NPC1 != null)
                    release_NPC(NPC1);
                NPC1 = NPC;
                NPC_list[0] = NPC1;
                break;
            case "2":
                if (NPC2 != null)
                    release_NPC(NPC2);
                NPC2 = NPC;
                NPC_list[1] = NPC2;
                break;
            case "3":
                if (NPC3 != null)
                    release_NPC(NPC3);
                NPC3 = NPC;
                NPC_list[2] = NPC3;
                break;
            case "4":
                if (NPC4 != null)
                    release_NPC(NPC4);
                NPC4 = NPC;
                NPC_list[3] = NPC4;
                break;
        }
    }
    public void play_anim(string state)
    {
        foreach (GameObject prefab_ in NPC_list)
        {
            if (prefab_ != null)
            {
                Animator anim = prefab_.GetComponent<Animator>();
                anim.Play(state);
            }
        }
    }
}
