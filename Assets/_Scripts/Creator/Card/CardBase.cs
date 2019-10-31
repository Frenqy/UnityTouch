using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardBase : MonoBehaviour
{
    protected Media media;
    public virtual Media mediaSetting
    {
        get
        {
            media.pos = transform.position;
            media.rotate = transform.rotation;
            media.scale = transform.localScale;
            return media;
        }
    }


    protected GameObject IntroImg;
    protected GameObject AddBtn;
    protected GameObject CloseBtn;

    protected virtual void Start()
    {
        IntroImg = transform.Find("IntroImg").gameObject;
        AddBtn = transform.Find("AddBtn").gameObject;
        CloseBtn = transform.Find("CloseBtn").gameObject;
    }

    public virtual void AddMedia()
    {
        IntroImg.SetActive(false);
        AddBtn.SetActive(true);
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
