using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GPHive.Core;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;


public class Commonner : MonoBehaviour
{
    public enum CommonerType
    {
        PoliceMan,
        Npc,
        Drone
    }

    [SerializeField] private float forwardSpeed;
    private Rigidbody rb;
    private bool move = false, levelStarted = false, stoled = false;
    [SerializeField] private List<GameObject> commonnerTypes = new();

    [SerializeField] private List<SkinnedMeshRenderer> skinnedMeshRenderers = new();
    [SerializeField] private List<MeshRenderer> meshRenderers = new();
    [SerializeField] private List<GameObject> bags = new();

    [SerializeField] private Animator animator;

    [SerializeField] private ParticleSystem stunParticle;
    [SerializeField] private ParticleSystem shootParticle;

    [SerializeField] private bool mysterious;

    [ShowIf("mysterious")] [SerializeField]
    private GameObject mysteriousPoliceModel;

    [ShowIf("mysterious")] [SerializeField]
    private Animator mysteriousPoliceAnimator;

    private MoneyParticle moneyParticle;

    private static readonly int Open = Animator.StringToHash("Open");
    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int StoleHash = Animator.StringToHash("Stole");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private bool moveTriggred = false;
    private bool animTriggred = false;
    [SerializeField] private Material grayMatarial;

    private float one = 1, dotFive = .5f, dotTree = .4f;

    public CommonerType commonerType;


    private void Start()
    {
        moneyParticle = GetComponent<MoneyParticle>();
        rb = GetComponent<Rigidbody>();
        if (commonerType == CommonerType.Npc || (mysterious && commonerType == CommonerType.PoliceMan))
        {
            foreach (var commonner in commonnerTypes) commonner.SetActive(false);
            var go = commonnerTypes[Random.Range(0, commonnerTypes.Count)];

            go.SetActive(true);

            animator = go.GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        EventManager.LevelStarted += GameStart;
        EventManager.LevelFailed += StopAniamtion;
        EventManager.LevelSuccessed += StopAniamtion;
    }

    private void OnDisable()
    {
        EventManager.LevelStarted -= GameStart;
        EventManager.LevelFailed -= StopAniamtion;
        EventManager.LevelSuccessed -= StopAniamtion;
    }

    private void ReturnToGray()
    {
        foreach (var mesh in meshRenderers) mesh.material = grayMatarial;
        /* mesh.material.SetTexture("_BaseMap", null);
             var hexColor = "636363";
             Color color;
             ColorUtility.TryParseHtmlString("#" + hexColor, out color);
             mesh.material.SetColor("_BaseColor", color);*/
        foreach (var mesh in skinnedMeshRenderers) mesh.material = grayMatarial;
        /* mesh.material.SetTexture("_BaseMap", null);
            var hexColor = "636363";
            Color color;
            ColorUtility.TryParseHtmlString("#" + hexColor, out color);
            mesh.material.SetColor("_BaseColor", color);*/
    }

    private void StopAniamtion()
    {
        if (!stoled) animator.SetTrigger(Idle);
    }


    private void GameStart()
    {
        levelStarted = true;
    }


    public void StartMove()
    {
        if (!moveTriggred)
        {
            animator.SetTrigger(RunHash);


            move = true;
            moveTriggred = true;
        }
    }


    public void StartAnim()
    {
        if (!animTriggred)
        {
            animator.SetTrigger(RunHash);
            animTriggred = true;
        }
    }

    public void StopMove()
    {
        if (moveTriggred && !stoled)
        {
            animator.SetTrigger(Idle);
            move = false;
            moveTriggred = false;
        }
    }

    public void StopAnim()
    {
        if (moveTriggred && !stoled)
        {
            animator.SetTrigger(Idle);
            animTriggred = false;
        }
    }

    public void Stole(Transform player)
    {
        GameObject bag = null;
        foreach (var obj in bags)
            if (obj.activeInHierarchy)
                bag = obj;

        if (mysterious && commonerType == CommonerType.PoliceMan)
        {
            animator.gameObject.SetActive(false);
            mysteriousPoliceModel.SetActive(true);
            animator = mysteriousPoliceAnimator;
        }

        if (bag && !mysterious)
        {
            BagDrop(bag, player);
        }
        else
        {
            if (commonerType == CommonerType.Npc)
            {
                moneyParticle.PlayerTransform = player;
                moneyParticle.Action();
            }
        }


        move = false;
        if (commonerType != CommonerType.Npc)
        {
            shootParticle.Play();
            animator.SetTrigger(Shoot);
            targetPlayer = player;
            lookPlayerBool = true;
        }
        else
        {
            stunParticle.Play();
            animator.SetTrigger(StoleHash);
        }


        ReturnToGray();

        stoled = true;
        StartCoroutine(MoveRigidbody(rb, transform.position + new Vector3(0, 0, 15), 0.5f));
        transform.DOPunchScale(Vector3.one * .3f, dotFive, 1);
    }

    private void BagDrop(GameObject bag, Transform player)
    {
        bag.transform.parent = LevelManager.Instance.ActiveLevel.transform;

        bag.transform.DOJump(new Vector3(bag.transform.position.x, 0, bag.transform.position.z + 15), 5, 1, dotFive)
            .OnComplete(
                () =>
                {
                    bag.GetComponent<Animator>().SetTrigger(Open);

                    moneyParticle.PlayerTransform = player;
                    moneyParticle.Action(bag.transform);
                });
    }

    public void Kill(Transform target)
    {
        if (mysterious && commonerType == CommonerType.PoliceMan)
        {
            animator.gameObject.SetActive(false);
            mysteriousPoliceModel.SetActive(true);
            animator = mysteriousPoliceAnimator;
        }

        if (commonerType == CommonerType.PoliceMan)
        {
            shootParticle.Play();
            animator.SetTrigger(Shoot);
            targetPlayer = target;
            lookPlayerBool = true;
        }
        else if (commonerType == CommonerType.Drone)
        {
            animator.enabled = false;
        }

        stoled = true;
    }

    private Transform targetPlayer;
    private bool lookPlayerBool;

    private void LookPlayer()
    {
        if (targetPlayer && lookPlayerBool)
        {
            transform.LookAt(targetPlayer.position, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    private IEnumerator MoveRigidbody(Rigidbody rb, Vector3 targetPosition, float duration)
    {
        var startPosition = rb.position;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, t));
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        Movment();
    }

    private void Update()
    {
        LookPlayer();
    }

    private void Movment()
    {
        if (GameManager.Instance.GameState != GameState.Playing || !move || !levelStarted)
            return;

        var forwardMovement = forwardSpeed * Time.deltaTime * transform.forward;

        rb.MovePosition(rb.position + forwardMovement);
    }
}