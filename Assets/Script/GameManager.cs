using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MouseSetting
{
    void SetMouse();
    MouseSetting ChangeMouseMode();
}

public class OffMouse : MonoBehaviour, MouseSetting
{
    public void SetMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public MouseSetting ChangeMouseMode()
    {
        return GetComponent<OnMouse>();
    }
}

public class OnMouse : MonoBehaviour, MouseSetting
{
    public void SetMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public MouseSetting ChangeMouseMode()
    {
        return GetComponent<OffMouse>();
    }
}

public class GameManager : MonoBehaviour
{
    MouseSetting mouse;

    void Start()
    {
        gameObject.AddComponent<OffMouse>();
        gameObject.AddComponent<OnMouse>();

        mouse = GetComponent<OffMouse>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mouse = mouse.ChangeMouseMode();
            mouse.SetMouse();
        }
    }
}
