using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class ObjectFly : MonoBehaviour
{
    private Transform targetJump;
    private Vector3 offset = new Vector3(0.1f, 2.01f, 0);//determines arch of the curve
    private Vector3 initialPosition;
    private Vector3 newPosition;
    private bool isDoneStep;
    private float duration;
    private float timeJump = 0;
    protected bool isJump;
    private UnityAction jumpCallBack = null;
    private List<Tween> tweens = new List<Tween>();

    // Caclulate new position
    private Vector3 CubicCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        return (((-start + 3 * (control1 - control2) + end) * t + (3 * (start + control2) - 6 * control1)) * t +
                3 * (control1 - start)) * t + start;
    }

    public void StartJump(Transform target, float duration, UnityAction callback = null)
    {
        this.duration = duration;
        initialPosition = transform.position;
        float height = target.position.y + 0.1f;
        offset = new Vector3(offset.x, height, offset.y);
        timeJump = 0;
        targetJump = target;
        isDoneStep = true;
        isJump = true;
        this.jumpCallBack = callback;
    }

    protected void Update()
    {
        if (isJump)
        {
            if (timeJump < duration)
            {
                if (isDoneStep)
                {
                    isDoneStep = false;
                    newPosition = CubicCurve(initialPosition, initialPosition + offset, initialPosition + offset,
                    targetJump.position, timeJump / duration);
                    Tween tween = transform.DOMove(newPosition, Time.deltaTime / 2).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        timeJump += Time.deltaTime / 2;
                        isDoneStep = true;
                    });
                    tweens.Add(tween);
                }
            }
            else
            {
                isJump = false;
                jumpCallBack?.Invoke();
                jumpCallBack = null;
            }
        }
    }
}

