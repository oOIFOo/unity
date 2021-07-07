using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ground_check : MonoBehaviour
{
    // Start is called before the first frame update
    public player_controller player;
    private Collider coll;
    void Start()
    {
        coll = gameObject.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            player.ground = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            player.ground = false;
        }
    }
}
