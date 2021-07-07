using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.IO;

public class NPC_loader : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] prefab;
    public int npc_num;
    public int mob_num;
    public float spondRadius;

    private GameObject player;
    private List<string> prefab_list = new List<string>();
    private List<string> mob_list = new List<string>();
    void Start()
    {
        player = GameObject.Find("player");
        string[] prefabs;
        string[] dirs = Directory.GetDirectories("Assets/Resources/npc_prefeb");
        foreach (string dir in dirs)
        {
            prefabs = Directory.GetFiles(dir, "*.prefab");
            foreach (string name in prefabs)
            {
                prefab_list.Add(name);
            }
        }

        for (int i = 0; i < npc_num; i++)
        {
            int rnd = Random.Range(0, prefab_list.Count-1);
            using (var editScope = new PrefabUtility.EditPrefabContentsScope(prefab_list[rnd]))
            {
                if (editScope.prefabContentsRoot.GetComponent<NPC_controller>() == null)
                    editScope.prefabContentsRoot.AddComponent<NPC_controller>();

                editScope.prefabContentsRoot.GetComponent<NPC_controller>().prefeb_path = prefab_list[rnd];
            }

            string name = prefab_list[rnd].Replace(".prefab", "");
            name = name.Replace("Assets/Resources/", "");
            GameObject npc_prefeb = (GameObject)Resources.Load(name);
            Vector3 newPos = NPC_controller.RandomNavSphere(player.transform.position, spondRadius, 1 << 4);
            Instantiate(npc_prefeb, newPos, Quaternion.identity);
        }

        prefabs = Directory.GetFiles("Assets/Resources/npc_prefeb/mob", "*.prefab");
        foreach (string name in prefabs)
        {
            mob_list.Add(name);
        }

        for (int i = 0; i < mob_num; i++)
        {
            int rnd = Random.Range(0, mob_list.Count);
            using (var editScope = new PrefabUtility.EditPrefabContentsScope(mob_list[rnd]))
            {
                if (editScope.prefabContentsRoot.GetComponent<NPC_controller>() == null)
                    editScope.prefabContentsRoot.AddComponent<NPC_controller>();

                editScope.prefabContentsRoot.GetComponent<NPC_controller>().prefeb_path = mob_list[rnd];
            }

            string name = mob_list[rnd].Replace(".prefab", "");
            name = name.Replace("Assets/Resources/", "");
            GameObject npc_prefeb = (GameObject)Resources.Load(name);
            Vector3 newPos = NPC_controller.RandomNavSphere(player.transform.position, spondRadius, 1 << 4);
            Instantiate(npc_prefeb, newPos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
