using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Canvas mainMenuCanvas;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 메인 메뉴 열고 닫기
        GameEvent.Instance.EventSetActiveMainMenu += SetActiveMouse;
        GameEvent.Instance.EventSetActiveMainMenu += SetActiveMainMenu;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isOpen = mainMenuCanvas.gameObject.activeSelf;
            if (isOpen)
            {
                GameEvent.Instance.OnEventSetActiveMainMenu(isOpen);
                return;
            }

            GameEvent.Instance.OnEventSetActiveMainMenu(isOpen);
        }
    }

    private void SetActiveMouse(bool isOpen)
    {
        Cursor.visible = isOpen ? false : true;

        if(isOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        Cursor.lockState = CursorLockMode.None;
    }

    private void SetActiveMainMenu(bool isOpen)
    {
        mainMenuCanvas.gameObject.SetActive(!isOpen);
    }
}
