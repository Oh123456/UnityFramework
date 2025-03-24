using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityFramework;


public class TestSetting : GameSettings
{
    public int test = 0;
    public string rotprtm = "rotprtm";

    public override void Initialize()
    {
        test = 0;
        rotprtm = "rotlqkf";
    }
}

public class SettingTest : MonoBehaviour
{
    TestSetting testSetting;
    // Start is called before the first frame update
    void Start()
    {
        testSetting = GameSettingsUtil.LoadGetSetting<TestSetting>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            testSetting.test = 1000;
            testSetting.rotprtm = "askjfdhgkjdfsghdfskjghfdksjghjk";

            GameSettingsUtil.SaveSettings(testSetting); 

        }
    }
}
