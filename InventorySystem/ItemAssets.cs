using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public Transform pfItemWorld;

    public Sprite bookSprite;
    public Sprite heartSprite;

    public string bookDialoguePath;

}
