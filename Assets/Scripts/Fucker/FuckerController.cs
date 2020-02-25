using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuckerController : MonoBehaviour
{
    public Rigidbody2D body;
    public FuckeCollider checker;

    [Space]
    [Min(0)]
    public float 最大速度 = 10;
    [Min(0)]
    public float 速度增量 = 1;

    [Space]
    [Range(0, 1)]
    public float 滑翔减速 = 0.75f;

    [Space]
    [Range(0, 90)]
    public float 可走角度 = 60f;

    bool CheckCanWalk()
    {
        foreach (var (contacts, count) in checker.contacts.Values)
        {
            for (int i = 0; i < count; i++)
            {
                var contact = contacts[i];
                if(Vector2.Angle(Vector2.up, contact.normal) <= 可走角度)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckIsOnWall()
    {
        return CheckIsOnWall(null);
    }
    bool CheckIsOnWall(bool? left)
    {
        foreach (var (contacts, count) in checker.contacts.Values)
        {
            for (int i = 0; i < count; i++)
            {
                var contact = contacts[i];
                if (Vector2.Angle(Vector2.up, contact.normal).Trans(v => v > 可走角度 && v <= 180 - (90 - 可走角度)))
                {
                    if (!left.HasValue)
                    {
                        return true;
                    }
                    else if (left == true)
                    {
                        if (Vector2.SignedAngle(Vector2.up, contact.normal) < 0) return true;
                    }
                    else
                    {
                        if (Vector2.SignedAngle(Vector2.up, contact.normal) > 0) return true;
                    }
                }
            }
        }
        return false;
    }

    void FixedUpdate()
    {
        Debug.Log(CheckIsOnWall());
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (!CheckIsOnWall(true))
            {
                body.velocity = body.velocity.Item(Vector2Item.X).ApproachingLimitCap((最大速度, 速度增量).Map(v => v * Vector2.left));
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!CheckIsOnWall(false))
            {
                body.velocity = body.velocity.Item(Vector2Item.X).ApproachingLimitCap((最大速度, 速度增量).Map(v => v * Vector2.right));
            }
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.F))
        {
            if (body.velocity.y < 0)
            {
                body.velocity = body.velocity.Item(Vector2Item.Y).Map(v => v * 滑翔减速);
            }
        }

    }

}