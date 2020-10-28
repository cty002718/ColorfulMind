using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float distance = 1f;
    public LayerMask statueMask;
    GameObject statue;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.right * transform.localScale.x, distance, statueMask);


        if (hit.collider != null && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            statue = hit.collider.gameObject;
            statue.GetComponent<FixedJoint2D>().enabled = true;
            statue.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
        }
        else if(Input.GetKeyUp(KeyCode.Space))
            statue.GetComponent<FixedJoint2D>().enabled = false;
    }
}
