using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Basic : EnemyAI_Base
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("Enemy Base xuất hiện và chuẩn bị tấn công!");
    }

    protected override void Update()
    {
        base.Update();
    }

    void Shout()
    {
        Debug.Log("Enemy hét lên: Tao thấy mày rồi!!!");
    }
}
