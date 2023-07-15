using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakAnimal : Animal
{
    public void Run(Vector3 _targetPos) // 반대 방향으로 뛸 것
    {
        // direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles; // 내 위치 - 플레이어 위치. 쿼터니언을 오일러로 변경
        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.z).normalized;

        currentTime = runTime;
        nav.speed = runSpeed;
        isWalking = false;
        isRunning = true;
        anim.SetBool("Running", isRunning);
    }

    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);

        if(isDead == false)
            Run(_targetPos);
    }
}
