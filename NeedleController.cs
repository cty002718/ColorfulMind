using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        HeroController controller= other.GetComponent<HeroController>();

        if(controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }
}
