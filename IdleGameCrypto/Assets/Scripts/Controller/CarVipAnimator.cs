using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVipAnimator : MonoBehaviour
{
    public Animator _animator;

    public Action OnClicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if(OnClicked != null)
        {
            OnClicked();
        }
    }
}
