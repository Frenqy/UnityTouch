using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardBase : MonoBehaviour
{
    [HideInInspector] public Media mediaSetting;

    protected RawImage raw;
    protected GameObject IntroImg;
    protected GameObject AddBtn;
    protected GameObject CloseBtn;

    private void Start()
    {
        raw = GetComponent<RawImage>();
        IntroImg = transform.Find("IntroImg").gameObject;
        AddBtn = transform.Find("AddBtn").gameObject;
        CloseBtn = transform.Find("CloseBtn").gameObject;
    }

    public virtual void AddMedia()
    {
        raw.color = Color.white;
        IntroImg.SetActive(false);
        AddBtn.SetActive(true);
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
