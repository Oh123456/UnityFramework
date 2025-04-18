using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityFramework.FSM;
using UnityFramework.Singleton.UnSafe;

public class FSMTester : MonoSingleton<FSMTester>
{
    TestFSM testFSM ;
    [SerializeField] TMPro.TMP_Text text;

    protected override void Initiation()
    {
        base.Initiation();
        testFSM = new TestFSM(this);
    }

    public void SetText(string s)
    {
        text.text = $"Current State :{s}";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            testFSM.ChangeState((int)TestStateID.Idle);
        if (Input.GetKeyDown(KeyCode.X))
            testFSM.ChangeState((int)TestStateID.Jump);
        if (Input.GetKeyDown(KeyCode.C))
            testFSM.ChangeState((int)TestStateID.Move);
        if (Input.GetKeyDown(KeyCode.V))
            testFSM.ChangeState((int)TestStateID.Run);
        if (Input.GetKeyDown(KeyCode.B))
            testFSM.ChangeState((int)TestStateID.Attack);

    }
}
