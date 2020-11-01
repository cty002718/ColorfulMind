using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueBehavior : MonoBehaviour
{
    public GameObject hero;

    private void Start()
    {
        if (hero == null) { hero = GameObject.Find("hero"); }
    }

    void Update()
    {
        CheckLayer();
    }


    //functions
    /*
    威廷2020/10/27 
    功能：把update的功能包起來，修正圖層的修改方式，
    */
    private void CheckLayer()
    {
        if (hero.transform.position.y < this.transform.position.y)
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = hero.GetComponent<SpriteRenderer>().sortingOrder - 1;
        else
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = hero.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}
