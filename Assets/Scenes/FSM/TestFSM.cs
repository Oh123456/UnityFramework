using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityFramework.FSM;

public enum TestStateID
{
    Idle,
    Jump,
    Run,
    Move,
    Attack,
}

public class TestFSM : StateMachine
{
    public TestFSM(object owner) : base(owner)
    {
    }

    protected override void SetDefulatID(out int defaultID)
    {
        defaultID = (int)TestStateID.Idle;
    }

    protected override void SetStates()
    {
        AddState(new TestIdleState());
        AddState(new TestMoveState());
        AddState(new TestJumpState());
        AddState(new TestRunState());
        AddState(new TestAttackState());
    }
}

public class TestIdleState : State
{
    protected override void SetChangeAble(HashSet<int> changeAble)
    {
        changeAble.Add((int)TestStateID.Move);
        changeAble.Add((int)TestStateID.Jump);
        changeAble.Add((int)TestStateID.Run);
        changeAble.Add((int)TestStateID.Attack);
    }

    protected override void SetID(out int id)
    {
        id = (int)TestStateID.Idle;
    }

    public override void Enter()
    {
        FSMTester.Instance.SetText("Idle");
    }
}


public class TestMoveState : State
{
    protected override void SetChangeAble(HashSet<int> changeAble)
    {
        changeAble.Add((int)TestStateID.Idle);
        changeAble.Add((int)TestStateID.Jump);
        changeAble.Add((int)TestStateID.Run);
        changeAble.Add((int)TestStateID.Attack);
    }

    protected override void SetID(out int id)
    {
        id = (int)TestStateID.Move;
    }

    public override void Enter()
    {
        FSMTester.Instance.SetText("Move");
    }
}


public class TestJumpState : State
{
    protected override void SetChangeAble(HashSet<int> changeAble)
    {
        changeAble.Add((int)TestStateID.Idle);
    }

    protected override void SetID(out int id)
    {
        id = (int)TestStateID.Jump;
    }
    public override void Enter()
    {
        FSMTester.Instance.SetText("Jump");
    }
}


public class TestRunState : State
{
    protected override void SetChangeAble(HashSet<int> changeAble)
    {
        changeAble.Add((int)TestStateID.Idle);
        changeAble.Add((int)TestStateID.Move);
        changeAble.Add((int)TestStateID.Jump);
        changeAble.Add((int)TestStateID.Attack);
    }

    protected override void SetID(out int id)
    {
        id = (int)TestStateID.Run;
    }

    public override void Enter()
    {
        FSMTester.Instance.SetText("Run");
    }
}


public class TestAttackState : State
{
    protected override void SetChangeAble(HashSet<int> changeAble)
    {
        changeAble.Add((int)TestStateID.Idle);
    }

    protected override void SetID(out int id)
    {
        id = (int)TestStateID.Attack;
    }

    public override void Enter()
    {
        FSMTester.Instance.SetText("Attack");
    }
}
