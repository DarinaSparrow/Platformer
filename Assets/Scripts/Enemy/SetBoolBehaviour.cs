using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoolBehaviour : StateMachineBehaviour
{
    public string boolName; // Имя булевой переменной в аниматоре
    public bool updateOnState, updateOnStateMachine; // Флаги для обновления переменной при входе и выходе из состояния и стейт-машины
    public bool valueOnEnter, valueOnExit; // Значения переменной при входе и выходе в состояние или стейт-машину

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnState) animator.SetBool(boolName, valueOnEnter); // Если требуется обновление переменной при входе в состояние, устанавливаем значение переменной
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnState) animator.SetBool(boolName, valueOnExit); // Если требуется обновление переменной при выходе из состояния, устанавливаем значение переменной
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine) animator.SetBool(boolName, valueOnEnter);  // Если требуется обновление переменной при входе в стейт-машину, устанавливаем значение переменной
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine) animator.SetBool(boolName, valueOnExit);  // Если требуется обновление переменной при выходе из стейт-машины, устанавливаем значение переменной
    }
}
