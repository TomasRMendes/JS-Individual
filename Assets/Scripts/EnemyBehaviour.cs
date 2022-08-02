using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    public float HP = 100;
    public float damage = 30;
    public GameObject[] Rails;
    public float ErrorMargin = 5;
    public float AggroRange = 100;
    public float AttackRange = 5;
    public float AttackDuration = 2;

    public float Speed = 1;
    public float RotationSpeed = 1;
    public int start = 0;

    private int next;
    private Vector3 Hook;
    private float swapDelay = 0;
    private float setMovement = 0;

    public Animator animator;
    private GameObject PlayerRef;

    private int BackForwardHash;
    private int LeftRightHash;
    private int IsIdleHash;
    private int OnHitHash;
    private int OnAttackHash;
    private int OnDeathHash;
    public bool Aggro = false;
    public bool idle = false;
    private bool attacking = false;

    // Start is called before the first frame update
    void Start()
    {

        PlayerRef = GameObject.FindGameObjectWithTag("Player");

        BackForwardHash = Animator.StringToHash("BackForward");
        LeftRightHash = Animator.StringToHash("LeftRight");
        IsIdleHash = Animator.StringToHash("IsIdle");
        OnHitHash = Animator.StringToHash("OnHit");
        OnAttackHash = Animator.StringToHash("OnAttack");
        OnDeathHash = Animator.StringToHash("OnDeath");

        next = (start + 1) % Rails.Length;
        Hook = Rails[next].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Animate();

    }


    public bool OnHit(float damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            animator.SetTrigger(OnDeathHash);
            Invoke(nameof(Death), 0.15f);
            return true;
        }

        animator.SetTrigger(OnHitHash);
        return false;
    }

    public void Death()
    {
        Destroy(this.transform.parent.gameObject);
    }


    public void Animate()
    {

    }


    public void Move()
    {
        if (attacking) return;

        Quaternion targetRotation;

        if (!Aggro || (PlayerRef.transform.position - transform.position).magnitude > Mathf.Pow(AggroRange, 2))
        {
            if (idle) return;
            
            
            targetRotation = Quaternion.LookRotation(Hook - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            transform.position += Speed * Time.deltaTime * transform.forward;


            if ((Hook - transform.position).magnitude <= ErrorMargin)
            {
                if(Rails.Length <= 1)
                {
                    idle = true;
                    animator.SetBool(IsIdleHash, true);
                    return;
                }

                if(swapDelay <= 0)
                {
                    swapDelay = 1;
                    next++;
                    next %= Rails.Length;
                    Hook = Rails[next].transform.position;
                }
            }

            if (swapDelay > 0) swapDelay -= Time.deltaTime;


        }
        else
        {
            if (idle)
            {
                idle = false;
                animator.SetBool(IsIdleHash, false);
            }

            Vector3 playerPos = PlayerRef.transform.position;

            if((PlayerRef.transform.position - transform.position).magnitude <= AttackRange)
            {
                Attack();
                return;
            }


            Vector3 removeHeight = Vector3.forward + Vector3.right;
            Vector3 myPos = transform.position;
            playerPos.Scale(removeHeight);
            myPos.Scale(removeHeight);
            targetRotation = Quaternion.LookRotation(playerPos - myPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            transform.position += Speed * Time.deltaTime * transform.forward;

        }

        if (setMovement <= 0)
        {
            setMovement = 0.5f;
            Vector3 direction = targetRotation.eulerAngles - transform.rotation.eulerAngles;
            direction = direction.normalized;

            animator.SetFloat(LeftRightHash, direction.x);
            animator.SetFloat(BackForwardHash, direction.y);

        }
        else setMovement -= Time.deltaTime;


    }

    public void Attack()
    {
        attacking = true;
        animator.SetTrigger(OnAttackHash);
        PlayerRef.GetComponent<StarterAssets.FirstPersonController>().Damage(damage);

        Invoke(nameof(FinishAttack), AttackDuration);
    }

    private void FinishAttack()
    {
        attacking = false;
    }








}
