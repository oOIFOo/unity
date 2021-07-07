using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Obi;
using System.IO;

public class dialog_control : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    private player_controller palyercontroll;
    public NPC_controller npc_control;
    private Animator NPC_animator;
    private AnimatorOverrideController NPC_AOC;
    private Animator player_animator;
    private AnimationClip s_man;
    private AnimationClip s_girl;
    private DialogueSystemTrigger conversion;

    public Canvas s_select;
    public ObiParticleRenderer obi;
    public MMD4MecanimModel womb;
    public AnimatorOverrideController player_overrideController;
    public AnimatorOverrideController npc_overrideController;
    public string state = "WAIT01";
    public string dir;
    void Start()
    {
        player = GameObject.Find("player");
        palyercontroll = player.GetComponent<player_controller>();
        player_animator = player.GetComponent<Animator>();

        obi.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Sequencer.Message("next");
        }

        if (Input.GetMouseButtonDown(1))
        {
            Sequencer.Message("event_next");
        }

        if (Input.GetKey(KeyCode.B))
        {
            npc_control.head_bone.userRotation = Quaternion.identity;
            npc_control.neck_bone.userRotation = Quaternion.identity;
            npc_control.look_camera();
        }
    }
    public void s_bottom()
    {
        s_select.gameObject.GetComponent<s_select>().load_pathlist();
        palyercontroll.npc_control.main_mode = player_controller.Main_State.s;
    }
    public void follow_bottom()
    {
        palyercontroll.main_mode = player_controller.Main_State.followed;
        palyercontroll.npc_control.main_mode = player_controller.Main_State.followed;
    }
    public void exit_bottom()
    {
        palyercontroll.main_mode = player_controller.Main_State.normal;
    }
    public void end_bottom()
    {
        palyercontroll.main_mode = player_controller.Main_State.normal;
        palyercontroll.npc_control.main_mode = player_controller.Main_State.normal;
    }
    public void change_spd_buttom()
    {
        if (DialogueLua.GetVariable("loop_phase").AsInt == 0)
        {
            DialogueLua.SetVariable("loop_phase", 1);
        }
        else if (DialogueLua.GetVariable("loop_phase").AsInt == 1)
        {
            DialogueLua.SetVariable("loop_phase", 0);
        }
        Sequencer.Message("event_next");
    }
    public void c_buttom()
    {
        npc_control = palyercontroll.NPC.GetComponent<NPC_controller>();
        DialogueLua.SetVariable("loop_phase", 2);
        Sequencer.Message("event_next");
    }
    public void p_bottom()
    {
        DialogueLua.SetVariable("loop_phase", 3);
        Sequencer.Message("event_next");
    }
    public void start_p()
    {
        obi.enabled = true;
        npc_control = palyercontroll.NPC.GetComponent<NPC_controller>();
    }
    public void stop_p()
    {
        obi.enabled = false;

        DialogueLua.SetVariable("loop_phase", 0);
    }
    public void emotion_message(string message)
    {
        string tmp = "基本表情/" + message + ".anim";
        npc_control = palyercontroll.NPC.GetComponent<NPC_controller>();

        NPC_animator = palyercontroll.NPC.GetComponent<Animator>();

        int hash = NPC_animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        if (hash == Animator.StringToHash("Base Layer." + "WAIT01"))
        {
            npc_control.load_morph(tmp, "WAIT01");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "event_wait"))
        {
            npc_control.load_morph(tmp, "event_wait");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "start"))
        {
            npc_control.load_morph(tmp, "start");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "start_wait"))
        {
            npc_control.load_morph(tmp, "start_wait");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "loop_slow"))
        {
            npc_control.load_morph(tmp, "loop_slow");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "loop_fast"))
        {
            npc_control.load_morph(tmp, "loop_fast");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "c"))
        {
            npc_control.load_morph(tmp, "c");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "finish"))
        {
            npc_control.load_morph(tmp, "finish");
        }
        else if (hash == Animator.StringToHash("Base Layer." + "talk"))
        {
            npc_control.load_morph(tmp, "talk");
        }
    }
    public void sset_dir(string dir_)
    {
        dir = dir_;
    }
    public void sset_state(string state_)
    {
        state = state_;
    }
    public void load_animator(string motion)
    {
        string path = Path.Combine("anime_controller", motion);
        NPC_AOC = Resources.Load<AnimatorOverrideController>(path);

        NPC_animator = palyercontroll.NPC.GetComponent<Animator>();
        NPC_animator.runtimeAnimatorController = NPC_AOC;
    }
    public void load_girl_motion(string motion)
    {
        string path = Path.Combine("使用動作", dir, motion + "_vmd");
        s_girl = Resources.Load<AnimationClip>(path);

        NPC_AOC[state] = s_girl;
    }
    public void load_man_motion(string motion)
    {
        string path = Path.Combine("使用動作", dir, motion + "_vmd");
        s_man = Resources.Load<AnimationClip>(path);
        player_overrideController[state] = s_man;
    }
    public void load_conversion(string name)
    {
        conversion = palyercontroll.NPC.GetComponent<DialogueSystemTrigger>();
        conversion.conversation = name;
    }
    public void NPC_animator_true(string name)
    {
        NPC_animator = palyercontroll.NPC.GetComponent<Animator>();
        NPC_animator.SetBool(name, true);
    }
    public void NPC_animator_false(string name)
    {
        NPC_animator = palyercontroll.NPC.GetComponent<Animator>();
        NPC_animator.SetBool(name, false);
    }
    public void player_animator_false(string name)
    {
        player_animator.SetBool(name, false);
    }
    public void player_animator_true(string name)
    {
        player_animator.SetBool(name, true);
    }
}
