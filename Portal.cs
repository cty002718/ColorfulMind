using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    [SerializeField]
    protected Transform trDest;                         //目的地

    [SerializeField]
    static protected Image imgBlack;                //黑色畫面
    static private bool isTransporting = false;     //是否正在傳送

    static private float fLerpSpeed = 1.25f;         //畫面變暗的速度
    static private float fActiveDest = 1.5f;        //互動的最小範圍

    private HeroController hc;

    private static CinemachineConfiner vcConfiner;

    private void Awake()
    {
        if (!vcConfiner) { vcConfiner = GameObject.Find("CM vcam1").GetComponent<CinemachineConfiner>(); }
    }

    private void Start()
    {
        imgBlack = GameObject.Find("BlackScreen").GetComponent<Image>();
    }

    #region public function
    public void Transport(Transform _tr)
    {
        if (isTransporting) return;
        if (trDest == null)
        {
            Debug.Log("沒有設定傳送點");
            return;
        }
        if (Vector2.Distance(_tr.position, transform.position) < fActiveDest)
        {
            hc = _tr.GetComponent<HeroController>();
            StartCoroutine(TransportTask(_tr));
        }
    }
    #endregion

    #region transport routine
    private IEnumerator TransportTask(Transform _tr)
    {
        isTransporting = true;
        if (hc) { hc.isTransporting = isTransporting; }
        //畫面變暗
        float Proc = 0;
        while (imgBlack.color != Color.black) 
        {
            imgBlack.color = Color.Lerp(Color.clear, Color.black, Proc +=  fLerpSpeed * Time.deltaTime);
            yield return null;
        }
        //Transport
        _tr.position = trDest.position;
        vcConfiner.m_BoundingShape2D = trDest.parent.Find("CameraConfiner").GetComponent<PolygonCollider2D>();

        yield return new WaitForSeconds(0.2f);
        //畫面恢復
        Proc = 0;
        while (imgBlack.color != Color.clear)
        {
            imgBlack.color = Color.Lerp(Color.black, Color.clear, Proc += fLerpSpeed * Time.deltaTime);
            yield return null;
        }
        isTransporting = false;
        if (hc) { hc.isTransporting = isTransporting; }
        yield return null;
    }
    #endregion
}
