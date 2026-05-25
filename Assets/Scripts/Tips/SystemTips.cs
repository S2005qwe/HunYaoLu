using DG.Tweening;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/**
 * Title:系统提示框
 * Description:
 */


public class SystemTips : MonoBehaviour
{

    [SerializeField, Header("提示文本")] private Text _texMsg;
    [SerializeField, Header("颜色曲线")] private AnimationCurve _colorCurve;
    [SerializeField, Header("移动曲线")] private AnimationCurve _moveCurve;



    private void FixedUpdate()
    {

    }

    private void OnDestroy()
    {

    }

    public void RefreshUI(string msg)
    {


        _texMsg.text=msg;
        _texMsg.DOColor(Color.red, 2).SetEase(_colorCurve);


        RectTransform rectTrans = transform as RectTransform;
        rectTrans.DOAnchorPosY(rectTrans.anchoredPosition.y + UnityEngine.Random.Range(200, 260), 2)
            .SetEase(_moveCurve);

        //定时销毁当前对象

        Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(v => {
            Destroy(gameObject);
        });



    }




}
