using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityFramework.UI;

public class TestUITest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            TestUI testUI = UIManager.Instance.Show<TestUI>("Canvas");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("TestUI");
        }
    }
}
