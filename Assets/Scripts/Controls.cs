using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls
{
    private static readonly KeyCode[]
        m_up = { KeyCode.UpArrow, KeyCode.W }, 
        m_down = {KeyCode.DownArrow, KeyCode.S}, 
        m_left = {KeyCode.LeftArrow, KeyCode.A}, 
        right = {KeyCode.RightArrow, KeyCode.D},
        m_shoot = {KeyCode.Space, KeyCode.Z, KeyCode.Return};

    public static bool GetUp()
    {
        foreach (KeyCode keyCode in m_up)
        {
            if (Input.GetKey(keyCode))
                return true;
        }
        return false;
    }

    public static bool GetDown()
    {
        foreach (KeyCode keyCode in m_down)
        {
            if (Input.GetKey(keyCode))
                return true;
        }
        return false;
    }

    public static bool GetLeft()
    {
        foreach (KeyCode keyCode in m_left)
        {
            if (Input.GetKey(keyCode))
                return true;
        }
        return false;
    }

    public static bool GetLeftPress()
    {
        foreach (KeyCode keyCode in m_left)
        {
            if (Input.GetKeyDown(keyCode))
                return true;
        }
        return false;
    }

    public static bool GetRight()
    {
        foreach (KeyCode keyCode in right)
        {
            if (Input.GetKey(keyCode))
                return true;
        }
        return false;
    }

    public static bool GetRightPress()
    {
        foreach (KeyCode keyCode in right)
        {
            if (Input.GetKeyDown(keyCode))
                return true;
        }
        return false;
    }

    public static bool GetShootPress()
    {
        foreach (KeyCode keyCode in m_shoot)
        {
            if (Input.GetKeyDown(keyCode))
                return true;
        }
        return false;
    }

    public static int GetVertical()
    {
        if (GetUp()) return 1;
        else if (GetDown()) return -1;
        else return 0;
    }

    public static int GetHorizontal()
    {
        if (GetRight()) return 1;
        else if (GetLeft()) return -1;
        else return 0;
    }
}
