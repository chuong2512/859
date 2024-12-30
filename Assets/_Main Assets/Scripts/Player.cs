using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using Cinemachine;
using DG.Tweening;
using GPHive.Core;
using MoreMountains.NiceVibrations;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Wallet wallet;
    [SerializeField] private SwerveController swerveController;
    [SerializeField] private SwerveControllerConfig swerveControllerConfig;
    private float horizontalSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    public GameObject Bag, bagParent;
    [SerializeField] private SkinnedMeshRenderer bagBlendShape;
    private Rigidbody bagRb;
    [SerializeField] private int sendelemdeObsPrice, jumpObsPrice;
    private Rigidbody rb;
    [SerializeField] private GameObject bigMoneyPrefab;
    [SerializeField] private int poolCount, expandPoolCount;
    [SerializeField] private Transform bigMoneyReferance;
    public List<BigMoney> bigMonies = new();
    [SerializeField] private ParticleSystem jumpMoney;
    [SerializeField] private ParticleSystem moneyLose;

    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int CatchMoneyHash = Animator.StringToHash("CatchMoney");
    private static readonly int Wasted = Animator.StringToHash("Wasted");
    private static readonly int LevelEnd = Animator.StringToHash("LevelEnd");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Sendeleme = Animator.StringToHash("Sendeleme");
    private static readonly int Throwe = Animator.StringToHash("Throwe");

    private float dotOne = .1f,
        dotFifteen = .15f,
        dotZeroFive = .06f,
        dotTwo = .2f,
        dotFive = .5f,
        floatOne = 1,
        floatTreeDotFive = 3.5f;

    private Collider tempOther;

    private bool dontStartFromBegin = false;

    [SerializeField] private GameObject moverElmasPrefebs, moverMoneyPrefebs, moverCoinPrefebs;


    private void OnEnable()
    {
        EventManager.LevelStarted += GameStart;
        EventManager.NextLevelCreated += ResetPlayer;
    }

    private void OnDisable()
    {
        EventManager.LevelStarted -= GameStart;
        EventManager.NextLevelCreated -= ResetPlayer;
    }


    private void GameStart()
    {
        animator.SetTrigger(RunHash);
    }

    private void Start()
    {
        bagRb = Bag.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        Poolable.CreatePool<BigMoney>(bigMoneyPrefab, poolCount, expandPoolCount);
        if (!dontStartFromBegin)
        {
            ResetPlayer();
            dontStartFromBegin = true;
        }
    }

    public void ResetPlayer()
    {
        if (dontStartFromBegin)
        {
            cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;

            transform.position = Vector3.zero;
            animator.ResetAll();

            bagRb.isKinematic = true;
            Bag.transform.parent = bagParent.transform;
            Bag.transform.localPosition = Vector3.zero;
            Bag.transform.localRotation = quaternion.identity;

            swerveController.enabled = true;

            animator.SetTrigger(Idle);

            CameraManager.Instance.SwitchCamera("NormalCam");
            swerveControllerConfig.DefoultSpeed();
            swerveController.enabled = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            foreach (var bigMoney in bigMonies)
                bigMoney.Depossit();

            wallet.RemoveAllMoney();
            wallet.TextShow();
        }
    }

    public void Jump(JumpTrigger jumpTrigger)
    {
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        animator.SetTrigger(JumpHash);
        swerveController.enabled = false;
        CameraManager.Instance.SwitchCamera("JumpCam");
        transform.DOJump(jumpTrigger.endPos.position, jumpTrigger.jumpPower, 1, jumpTrigger.jumpTime)
            .OnComplete(() =>
            {
                cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
                swerveController.enabled = true;
                animator.SetTrigger(RunHash);
                CameraManager.Instance.SwitchCamera("NormalCam");
            }).SetEase(jumpTrigger.ease);
    }

    private void DropBag()
    {
        bagRb.isKinematic = false;
        bagRb.AddForce(Vector3.back * 350f);
        Bag.transform.parent = null;
    }

    private void Die()
    {
        DropBag();
        wallet.RemoveAllMoney();
        wallet.TextHide();
        GameManager.Instance.LoseLevel();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ComonnerMoveTrigger"))
        {
            var commonner = other.transform.parent.GetComponent<Commonner>();
            commonner.StartMove();
        }

        if (other.CompareTag("ComonnerAnimTrigger"))
        {
            var commonner = other.transform.parent.GetComponent<Commonner>();
            commonner.StartAnim();
        }

        if (other.CompareTag("JumpTrigger"))
        {
            Jump(other.GetComponent<JumpTrigger>());
            jumpMoney.Play();
            other.enabled = false;
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }

        if (other.CompareTag("Commonner"))
        {
            Wallet _wallet;
            var commonner = other.GetComponent<Commonner>();

            switch (commonner.commonerType)
            {
                case Commonner.CommonerType.Npc:

                    animator.SetTrigger(CatchMoneyHash);

                    _wallet = other.GetComponent<Wallet>();
                    _wallet.GiveAllMoney(wallet);
                    _wallet.TextHide();


                    commonner.Stole(transform);

                    break;
                case Commonner.CommonerType.PoliceMan:


                    _wallet = other.GetComponent<Wallet>();
                    _wallet.TextHide();

                    if (wallet.MoeyAmount > 0)
                    {
                        animator.SetTrigger(CatchMoneyHash);

                        wallet.AddMoney(_wallet.moeyAmount);
                        moneyLose.Play();
                        commonner.Stole(transform);
                    }
                    else
                    {
                        commonner.Kill(transform);
                        animator.SetTrigger(DieHash);
                        Die();
                    }

                    break;
                case Commonner.CommonerType.Drone:

                    animator.SetTrigger(Wasted);
                    commonner.Kill(transform);
                    Die();

                    break;
            }

            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            other.enabled = false;
        }


        if (other.CompareTag("Collectiable"))
        {
            var collectiable = other.GetComponent<Collectiable>();
            wallet.AddMoney(collectiable.moneyAmount, true);
            other.gameObject.SetActive(false);
            other.enabled = false;
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            PuncBag();
        }

        if (other.CompareTag("CollectiableProp"))
        {
            other.GetComponent<PropCollectiable>().Action(transform);

            var _wallet = other.GetComponent<Wallet>();
            _wallet.GiveAllMoney(wallet);


            other.enabled = false;
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }

        if (other.CompareTag("Conveyor"))
        {
            var conveyor = other.GetComponent<Conveyor>();

            conveyor.Action();
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }

        if (other.CompareTag("SendelemeObs"))
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            if (wallet.MoeyAmount > 0)
            {
                wallet.RemoveMoney(sendelemdeObsPrice);
                animator.SetTrigger(Sendeleme);
                moneyLose.Play();
            }
            else
            {
                animator.SetTrigger(DieHash);
                Die();
            }
        }

        if (other.CompareTag("JumpObs"))
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            if (wallet.MoeyAmount > 0)
            {
                wallet.RemoveMoney(jumpObsPrice);
                swerveController.enabled = false;
                other.enabled = false;
                tempOther = other;
                rb.velocity = Vector3.zero;
                rb.AddForce(Vector3.forward * -800);
                Invoke(nameof(JumpObsCanMove), floatOne);
                moneyLose.Play();
            }
            else
            {
                animator.SetTrigger(DieHash);
                Die();
            }
        }

        if (other.CompareTag("Finish"))
        {
            other.enabled = false;
            swerveController.enabled = false;

            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            wallet.TextHide();
            animator.SetTrigger(Throwe);

            levelEndSc = other.GetComponent<LevelEnd>();
        }
    }

    private LevelEnd levelEndSc;

    public void ThroweBag()
    {
        Bag.transform.parent = null;
        Bag.transform.DOJump(levelEndSc.moneyBagJumpPoint.position, 4, 1, dotFive);
        Bag.transform.DORotateQuaternion(levelEndSc.moneyBagJumpPoint.rotation, dotFive).OnComplete(() =>
        {
            StartCoroutine(ObjectMover());
        });
    }

    private List<GameObject> moverObjects = new();

    private IEnumerator ObjectMover()
    {
        moverObjects = new List<GameObject>();

        var moneyAmount = wallet.moeyAmount;

        var elmasCount = moneyAmount / 100;
        moneyAmount %= 100;

        var moneyCount = moneyAmount / 10;
        moneyAmount %= 10;

        var coinCount = moneyAmount;

        SpawnObjects(moverElmasPrefebs, elmasCount);
        SpawnObjects(moverMoneyPrefebs, moneyCount);
        SpawnObjects(moverCoinPrefebs, coinCount);

        ShuffleList(moverObjects);

        foreach (var obj in moverObjects)
        {
            obj.SetActive(true);
            yield return BetterWaitForSeconds.Wait(dotTwo);
        }

        bagRb.isKinematic = false;


        animator.SetTrigger(RunHash);
        var pos = levelEndSc.LasMovmentPos.position;
        transform.DOMove(new Vector3(pos.x, transform.position.y, pos.z), 0)
            .OnComplete(() => { LevelEndStack(); }).SetEase(Ease.Linear);
    }

    private void SpawnObjects(GameObject prefab, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var go = Instantiate(prefab, Bag.transform.position, Quaternion.identity);
            go.SetActive(false);
            go.transform.parent = LevelManager.Instance.ActiveLevel.transform;
            moverObjects.Add(go);

            var endValue = 40f / count * i;


            if (bagBlendShape)
            {
                bagBlendShape.DOKill();
                DOTween.To(() => bagBlendShape.GetBlendShapeWeight(0), x => bagBlendShape.SetBlendShapeWeight(0, x),
                    endValue,
                    dotFive);
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        var rng = new System.Random();
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }


    public void JumpObsCanMove()
    {
        swerveController.enabled = true;
        tempOther.enabled = true;
    }


    private void LevelEndStack()
    {
        CameraManager.Instance.SwitchCamera("FinishCam");

        animator.SetTrigger(LevelEnd);
        LevelManager.Instance.ActiveLevel.GetComponent<LevelSettings>().yourBest.enabled = true;

        bigMonies.Clear();

        var oneBigMoneyBoundsSize = bigMoneyPrefab.GetComponent<MeshRenderer>().bounds.size;

        var turningMoneyAmount = Mathf.RoundToInt(wallet.MoeyAmount / 6);
        var time = turningMoneyAmount * dotZeroFive;
        var playerMovePos = new Vector3(transform.position.x, oneBigMoneyBoundsSize.y * (turningMoneyAmount - 1),
            transform.position.z);

        var playerStartPos = transform.position;
        var firstTime = true;
        var counter = 0;

        transform.DOMove(playerMovePos, time).SetEase(Ease.Linear)
            .OnComplete(() => { GameManager.Instance.WinLevel(); }).SetEase(Ease.Linear).OnUpdate(() =>
            {
                if (transform.position.y - playerStartPos.y >=
                    (counter + 1) * oneBigMoneyBoundsSize.y - oneBigMoneyBoundsSize.y - .2f || firstTime)
                {
                    CreatBigMoney(oneBigMoneyBoundsSize);
                    counter++;
                    firstTime = false;
                }
            });
    }

    private void CreatBigMoney(Vector3 oneBigMoneyBoundsSize)
    {
        var bigMoney = Poolable.Get<BigMoney>();
        if (bigMonies.Count <= 0)
            bigMoney.GetThis(Vector3.zero, bigMoneyReferance, .05f, true);
        else
            bigMoney.GetThis(bigMonies[^1].transform.position + new Vector3(0, oneBigMoneyBoundsSize.y, 0),
                bigMoneyReferance,
                .05f, false);

        bigMonies.Add(bigMoney);


        wallet.EarnRealMoney(6);
    }

    public void PuncBag()
    {
        Bag.transform.DOComplete();
        Bag.transform.DOPunchScale(Vector3.one * .8f, .5f, 5);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Conveyor"))
        {
            var conveyor = other.GetComponent<Conveyor>();
            conveyor.StopAction();
        }

        if (other.CompareTag("ComonnerMoveTrigger"))
        {
            var commonner = other.transform.parent.GetComponent<Commonner>();
            commonner.StopMove();
        }

        if (other.CompareTag("ComonnerAnimTrigger"))
        {
            var commonner = other.transform.parent.GetComponent<Commonner>();
            commonner.StopAnim();
        }
    }
}