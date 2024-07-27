using Mirror;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // 移动
    public float speed = 5f;
    private Vector2 direction;
    private GameObject moveJoystickGO;
    private FixedJoystick moveJoystick;
    private GameObject attackJoyStickGO;
    private AttackJoystick attackJoystick;

    // 攻击
    public GameObject bullet;
    public float bulletSpeed = 7f;

    // 组件
    private Animator animator;
    private Rigidbody2D rb;
    private GameObject torch;

    // 同步人物姓名
    public TextMeshProUGUI nameText;
    [SyncVar(hook = nameof(OnNameChange))]
    private string name;

    // 同步人物翻转
    [SyncVar(hook = nameof(OnFlipChange))]
    private float currentX;

    // 同步动画
    [SyncVar(hook = nameof(OnAnimationChange))]
    private bool run;

    // 同步得分
    [SyncVar(hook = nameof(OnScoreChange))]
    private int score;

    // UI组件
    private TextMeshProUGUI scoreText;

    public override void OnStartLocalPlayer()
    {
        // 相机跟随
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);

        CmdSetupPlayer("Player" + Random.Range(100, 999));

        // 绑定UI
        scoreText = GameObject.Find("/Canvas/ScoreText").GetComponent<TextMeshProUGUI>();
        UpdateScoreText();
    }

    private void Start()
    {
        animator = transform.GetChild(1).GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        torch = transform.GetChild(1).GetChild(11).gameObject;

        if (!isLocalPlayer) { return; }

        // 绑定摇杆
        moveJoystickGO = transform.Find("/Canvas/MoveJoystick").gameObject;
        moveJoystick = moveJoystickGO.GetComponent<FixedJoystick>();
        attackJoyStickGO = transform.Find("/Canvas/AttackJoystick").gameObject;
        attackJoystick = attackJoyStickGO.GetComponent<AttackJoystick>();

        // 注册事件
        EventManager.Listen(EEventType.joystick_attack_up.ToString(), OnJoystickAttackUp);
    }

    private void OnDestroy()
    {
        EventManager.Ignore(EEventType.joystick_attack_up.ToString(), OnJoystickAttackUp);
    }

    void Update()
    {
        SetAnimation();

        if (!isLocalPlayer) { return; }

        direction = moveJoystick.Direction;
        SetFlip();

        MoveTorch(attackJoystick.Direction);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }

        // 移动
        Move(moveJoystick.Direction);
    }

    // 切换动画
    void SetAnimation()
    {
        CmdSetAnimation(direction != Vector2.zero);
    }

    // 人物翻转
    private void SetFlip()
    {
        CmdFlip(direction.x);
    }

    private void Move(Vector2 direction)
    {
        rb.MovePosition(rb.position + direction.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnNameChange(string _old, string _new)
    {
        nameText.text = name;
    }

    private void OnFlipChange(float _old, float _new)
    {
        if (currentX < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (currentX > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnAnimationChange(bool _old, bool _new)
    {
        animator.SetBool("run", run);
    }

    [Command]
    private void CmdFlip(float x)
    {
        currentX = x;
    }

    [Command]
    private void CmdSetupPlayer(string _name)
    {
        name = _name;
    }

    [Command]
    private void CmdSetAnimation(bool _run)
    {
        run = _run;
    }

    [Command]
    private void CmdAttack(Vector2 attackDirection)
    {
        RpcAttack(attackDirection);
    }

    [ClientRpc]
    private void RpcAttack(Vector2 attackDirection)
    {
        // 生成子弹
        GameObject bulletClone = Instantiate(bullet, transform.position, Quaternion.identity);

        // 速度
        bulletClone.GetComponent<Rigidbody2D>().velocity = attackDirection.normalized * bulletSpeed;

        // 角度
        bulletClone.transform.rotation = Quaternion.FromToRotation(Vector3.up, attackDirection);

        // 设置攻击者
        bulletClone.GetComponent<Bullet>().owner = gameObject;
    }

    private void OnJoystickAttackUp(object[] arr)
    {
        CmdAttack((Vector2)arr[0]);
    }

    private void MoveTorch(Vector2 torchDirection)
    {
        float angle = 0;
        if (transform.localScale.x == 1)
        {
            if (Mathf.Abs(torchDirection.x - 0) < 0.01f && Mathf.Abs(torchDirection.y - 0) < 0.01f)
            {
                angle = 0.0f;
            }
            else
            {
                angle = Vector2.Angle(torchDirection, new Vector2(1, 0)) - 180;
                if (torchDirection.y < 0)
                {
                    angle = 360 - angle;
                }
            }
        }
        else if (transform.localScale.x == -1)
        {
            if (Mathf.Abs(torchDirection.x - 0) < 0.01f && Mathf.Abs(torchDirection.y - 0) < 0.01f)
            {
                angle = 0.0f;
            }
            else
            {
                angle = Vector2.Angle(torchDirection, new Vector2(1, 0)) - 180;
                if (torchDirection.y < 0)
                {
                    angle = 360 - angle;
                }
                angle = -angle + 180;
            }
        }

        torch.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    private void OnScoreChange(int _old, int _new)
    {
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "跃迁所需资源量 " + score + "/3";
        }
        Debug.Log("跃迁所需资源量 " + score + "/3");
    }

    // 增加得分方法
    [Command]
    public void CmdIncreaseScore(int amount)
    {
        score += amount;
    }
}
