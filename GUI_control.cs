using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.IO;
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GUI_control : MonoBehaviour
{
	// Start is called before the first frame update
	public Canvas change_motion_UI;
	public GameObject player;
	public Text bottom1_text;
	public Text bottom2_text;
	public Text bottom3_text;
	public Text bottom4_text;
	public Text bottom5_text;
	public Text bottom6_text;
	public Text bottom7_text;
	public Text bottom8_text;
	public Text bottom9_text;
	public Text bottom0_text;
	public Canvas sex_select_canvas;
	public string sex_path;
	public GameObject npc_prefeb;

	public string prefeb_name;
	private string[] path_list;
	private string[] npc_list;
	void Start()
	{
		player = GameObject.Find("player");

		npc_list = File.ReadAllLines("Assets\\Resources\\npc_list.txt");
		for(int i = 0; i < npc_list.Length; i++)
        {
			npc_prefeb = (GameObject)Resources.Load(npc_list[i].Replace(".prefab", ""));
			Instantiate(npc_prefeb, player.transform.position + new Vector3(0.0f, 0.0f, 0.3f), Quaternion.identity);
		}
	}
	void Update()
	{
		if (Input.GetKey(KeyCode.H))
		{
			path_list = File.ReadAllLines("Assets\\Resources\\sex_path.txt");
			bottom1_text.text = path_list[0];
			bottom2_text.text = path_list[1];
			bottom3_text.text = path_list[2];
			bottom4_text.text = path_list[3];
			bottom5_text.text = path_list[4];
			bottom6_text.text = path_list[5];
			bottom7_text.text = path_list[6];
			bottom8_text.text = path_list[7];
			bottom9_text.text = path_list[8];
			bottom0_text.text = path_list[9];
			change_motion_UI.gameObject.SetActive(true);
		}
		if (Input.GetKeyDown(KeyCode.Y))
		{
			prefeb_name = select_prefeb("prefab");
			npc_prefeb = (GameObject)Resources.Load(prefeb_name.Replace(".prefab", ""));
			if (npc_prefeb != null)
            {
				npc_prefeb = Instantiate(npc_prefeb, player.transform.position + player.transform.forward, Quaternion.identity);
		
				if (npc_prefeb.GetComponent<NPC_controller>() == null)
                {
					npc_prefeb.AddComponent<NPC_controller>();
					using (var editScope = new PrefabUtility.EditPrefabContentsScope("Assets/Resources/" + prefeb_name))
                    {
						editScope.prefabContentsRoot.AddComponent<NPC_controller>();
					}
				}
				NPC_controller controller = npc_prefeb.GetComponent<NPC_controller>();

				controller.prefeb_path = prefeb_name;
				controller.init_position = player.transform.position + new Vector3(0.0f, 0.0f, 0.3f);
			}
		}
		if (Input.GetKeyDown(KeyCode.U))
        {
			var sr = File.CreateText("Assets\\Resources\\npc_list.txt");
			GameObject tmp = GameObject.Find("便器");
			for(int i = 0; i < tmp.transform.childCount; i++)
            {
				GameObject prefab_ = tmp.transform.GetChild(i).gameObject;
				NPC_controller controller = prefab_.GetComponent<NPC_controller>();
				sr.WriteLine(controller.prefeb_path);
			}
			sr.Close();
		}
	}
	void OnGUI()
	{
	}
	public void exit_bottom()
	{
		var sr = File.CreateText("Assets\\Resources\\sex_path.txt");
		sr.WriteLine(bottom1_text.text);
		sr.WriteLine(bottom2_text.text);
		sr.WriteLine(bottom3_text.text);
		sr.WriteLine(bottom4_text.text);
		sr.WriteLine(bottom5_text.text);
		sr.WriteLine(bottom6_text.text);
		sr.WriteLine(bottom7_text.text);
		sr.WriteLine(bottom8_text.text);
		sr.WriteLine(bottom9_text.text);
		sr.WriteLine(bottom0_text.text);
		sr.Close();

		change_motion_UI.gameObject.SetActive(false);
	}
	public void select_file()
	{
		sex_path = select_motion("anim");
	}
	public void sex1_bottom()
	{
		bottom1_text.text = sex_path;
	}
	public void sex2_bottom()
	{
		bottom2_text.text = sex_path;
	}
	public void sex3_bottom()
	{
		bottom3_text.text = sex_path;
	}
	public void sex4_bottom()
	{
		bottom4_text.text = sex_path;
	}
	public void sex5_bottom()
	{
		bottom5_text.text = sex_path;
	}
	public void sex6_bottom()
	{
		bottom6_text.text = sex_path;
	}
	public void sex7_bottom()
	{
		bottom7_text.text = sex_path;
	}
	public void sex8_bottom()
	{
		bottom8_text.text = sex_path;
	}
	public void sex9_bottom()
	{
		bottom9_text.text = sex_path;
	}
	public void sex0_bottom()
	{
		bottom0_text.text = sex_path;
	}
	OpenFileName openFileName;
	public string select_motion(string type)
	{
		openFileName = new OpenFileName();
		openFileName.structSize = Marshal.SizeOf(openFileName);
		openFileName.filter = "文件(*." + type + ")\0*." + type + "";
		openFileName.file = new string(new char[256]);
		openFileName.maxFile = openFileName.file.Length;
		openFileName.fileTitle = new string(new char[64]);
		openFileName.maxFileTitle = openFileName.fileTitle.Length;
		openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
		openFileName.title = "選擇文件";
		openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

		if (LocalDialog.GetSaveFileName(openFileName))//点击系统对话框框保存按钮
		{
			string[] sArray = openFileName.file.Split('\\');
			return sArray[sArray.Length - 2];
		}

		return null;
	}
	public string select_prefeb(string type)
	{
		openFileName = new OpenFileName();
		openFileName.structSize = Marshal.SizeOf(openFileName);
		openFileName.filter = "文件(*." + type + ")\0*." + type + "";
		openFileName.file = new string(new char[256]);
		openFileName.maxFile = openFileName.file.Length;
		openFileName.fileTitle = new string(new char[64]);
		openFileName.maxFileTitle = openFileName.fileTitle.Length;
		openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
		openFileName.title = "選擇文件";
		openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

		if (LocalDialog.GetSaveFileName(openFileName))
		{
			string[] sArray = openFileName.file.Split('\\');
			string name = "";
			for (int i = sArray.Length-1; i >= 0; i--)
            {
				if (sArray[i] == "Resources")
                {
					break;
                }
				if (name == "") name = sArray[i];
				else name = sArray[i] + '/' + name;
			}
			return name;
		}

		return null;
	}
}
