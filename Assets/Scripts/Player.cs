using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
// Command：客户端调用，服务器执行
// ClientRpc: 服务器调用，客户端执行
// SyncVar与ClientRpc类似，用于修饰变量
public class Player : NetworkBehaviour
{
    // 移动
    public float speed = 8f;
    private Vector2 direction;
    private GameObject moveJoystickGO;
    private FixedJoystick moveJoystick;
    private GameObject attackJoyStickGO;
    public AttackJoystick attackJoystick;

    // 攻击
    public GameObject bullet;
    public float bulletSpeed = 7f;
    public bool canMove = true;
    public float canMoveTime = 2.0f;
    public float invincibleTime = 1.0f;
    public bool invincible = false;
    public GameObject resource;

    // 手电筒
    private GameObject torchBtn;
    public struct FlashLight
    {
        public FlashLight(float angle_, float radius_, float intensity_)
        {
            angle = angle_;
            radius = radius_;
            intensity = intensity_;
        }

        public float angle;
        public float radius;
        public float intensity;
    }

    [SyncVar(hook = nameof(OnFlashLightChange))]
    private FlashLight lightInfo;

    public struct LightCollider
    {
        public LightCollider(Vector2 p0_, Vector2 p1_, Vector2 p2_, Vector2 p3_)
        {
            p0 = p0_;
            p1 = p1_;
            p2 = p2_;
            p3 = p3_;
        }

        public Vector2 p0;
        public Vector2 p1;
        public Vector2 p2;
        public Vector2 p3;
    }

    private Light2D flashLight;
    private PolygonCollider2D lightCollider;

    [SyncVar(hook = nameof(OnLightColliderChange))]
    private LightCollider lightColliderInfo;

    private bool clicked = false;   // 是否处于强光状态
    private bool cooled = false;    // 是否处于冷却状态
    public float duration = 3.0f;  // 强光持续时间
    public float coolTime = 5.0f;  // 冷却时间

    // 组件
    private Animator animator;
    private Rigidbody2D rb;
    public GameObject arm;

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
    //[SyncVar(hook = nameof(OnScoreChange))]
    public int score;

    // UI组件
    private TextMeshProUGUI scoreText;
    private Slider slider;
    private GameObject canvas;

    private PositionManager positionManager;
    private CameraMove cameraMove;

    public override void OnStartLocalPlayer()
    {
        // 相机跟随
        //Camera.main.transform.SetParent(transform);
        //Camera.main.transform.localPosition = new Vector3(0, 0, -10);
        cameraMove = transform.Find("/MainCamera").GetComponent<CameraMove>();
        cameraMove.targetPlayer = transform;
        cameraMove.box = transform.Find("/Layer1/CameraBorder").gameObject;

        CmdSetupPlayer("Player" + Random.Range(100, 999));
    }

    private void Start()
    {
        animator = transform.GetChild(1).GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        arm = transform.GetChild(1).GetChild(11).gameObject;
        flashLight = arm.transform.GetChild(0).GetComponent<Light2D>();
        arm.transform.GetChild(0).GetComponent<Torch>().owner = gameObject;
        lightCollider = arm.transform.GetChild(0).GetComponent<PolygonCollider2D>();
        slider = transform.GetChild(0).GetChild(1).GetComponent<Slider>();
        positionManager = transform.Find("/PositionManager").GetComponent<PositionManager>();
        canvas = transform.GetChild(0).gameObject;

        if (!isLocalPlayer) { return; }

        // 绑定摇杆
        moveJoystickGO = transform.Find("/Canvas/MoveJoystick").gameObject;
        moveJoystick = moveJoystickGO.GetComponent<FixedJoystick>();
        attackJoyStickGO = transform.Find("/Canvas/AttackJoystick").gameObject;
        attackJoystick = attackJoyStickGO.GetComponent<AttackJoystick>();
        torchBtn = transform.Find("/Canvas/torchBtn").gameObject;
        torchBtn.GetComponent<Button>().onClick.AddListener(TorchBtnOnClick);

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
        UpdateInvincible();

        if (!isLocalPlayer) { return; }

        direction = moveJoystick.Direction;
        SetFlip();
        UpdateTorchStatus();
        UpdateMove();

        MoveTorch(attackJoystick.Direction);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }

        // 移动
        if (canMove)
        {
            Move(moveJoystick.Direction);
        }
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
            canvas.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (currentX > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            canvas.transform.localScale = new Vector3(-1, 1, 1);
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

        arm.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

	private void TorchBtnOnClick()
    {
        CmdSetTorch(30, 9, 6);
        Vector2 p0 = new Vector2(0, 9);
        Vector2 p1 = new Vector2(-2.3f, 8.7f);
        Vector2 p2 = new Vector2(0, 0.1f);
        Vector2 p3 = new Vector2(2.3f, 8.7f);
        CmdSetLightCollider(p0, p1, p2, p3);

        if (isLocalPlayer)
        {
            torchBtn.GetComponent<Button>().interactable = false;
            clicked = true;
        }
    }

    private void TorchBtnCoolDown()
    {
        CmdSetTorch(120, 4.5f, 3);
        Vector2 p0 = new Vector2(0, 4.5f);
        Vector2 p1 = new Vector2(-3.8f, 2.3f);
        Vector2 p2 = new Vector2(0, 0.1f);
        Vector2 p3 = new Vector2(3.8f, 2.3f);
        CmdSetLightCollider(p0, p1, p2, p3);
    }

    private void OnFlashLightChange(FlashLight _old, FlashLight _new)
    {
        flashLight.pointLightOuterAngle = lightInfo.angle;
        flashLight.pointLightOuterRadius = lightInfo.radius;
        flashLight.intensity = lightInfo.intensity;
    }

    private void OnLightColliderChange(LightCollider _old, LightCollider _new)
    {
        Vector2[] currPoints = new Vector2[4];
        currPoints[0] = lightColliderInfo.p0;
        currPoints[1] = lightColliderInfo.p1;
        currPoints[2] = lightColliderInfo.p2;
        currPoints[3] = lightColliderInfo.p3;
        lightCollider.points = currPoints;
    }

    [Command]
    private void CmdSetTorch(float angle, float radius, float intensity)
    {
        lightInfo = new FlashLight(angle, radius, intensity);
    }

    [Command]
    private void CmdSetLightCollider(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        lightColliderInfo = new LightCollider(p0, p1, p2, p3);
    }

    private void UpdateTorchStatus()
    {
        if (clicked == true)
        {
            duration -= Time.deltaTime;
            if (duration <= 0.0f)
            {
                cooled = true;
                clicked = false;
                duration = 3.0f;
                TorchBtnCoolDown();
            }
        }
        if (cooled == true)
        {
            coolTime -= Time.deltaTime;
            if (coolTime <= 0.0f)
            {
                cooled = false;
                coolTime = 5.0f;
                torchBtn.GetComponent<Button>().interactable = true;
            }
        }
    }

    private void UpdateMove()
    {
        if (!canMove)
        {
            canMoveTime -= Time.deltaTime;
            if (canMoveTime <= 0.0f)
            {
                canMove = true;
                canMoveTime = 2.0f;
            }
        }
    }

    private void UpdateInvincible()
    {
        if (invincible)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0.0f)
            {
                invincible = false;
                invincibleTime = 1.0f;
            }
        }
    }

    public void Attack(GameObject other)
    {
        CmdAttack(other, moveJoystick.Direction.normalized);
    }

    [Command]
    private void CmdAttack(GameObject other, Vector2 dir)
    {
        RpcAttack(other, dir);
    }

    [ClientRpc]
    private void RpcAttack(GameObject other, Vector2 dir)
    {
        Player player = other.transform.GetComponent<Player>();
        player.canMove = false;
        other.transform.GetComponent<Rigidbody2D>().AddForce(dir * 5.0f, ForceMode2D.Impulse);

        if (!player.invincible)
        {
            if (player.score > 0)
            {
                Instantiate(resource, other.transform.position, Quaternion.identity);
            }
            player.score = Mathf.Max(player.score - 1, 0);

            player.ChangeSpeed();

            player.slider.value = player.score;
            player.invincible = true;
        }
    }

    public void UsePotral(int index)
    {
        if (score == 3)
        {
            if (index == 2)
            {
                transform.position = positionManager.layer2StartPosition.position;
                cameraMove.box = transform.Find("/Layer2/CameraBorder").gameObject;
            }
            else if (index == 3)
            {
                //transform.position = positionManager.layer3StartPosition.position;
                //cameraMove.box = transform.Find("/Layer3/CameraBorder").gameObject;
            }
            
            CmdClearScore();
        }
    }

    public void GetResource(int amount)
    {
        // 如果是别的玩家捡到资源，直接跳过
        if (!isLocalPlayer) return;

        CmdGetResource(amount);
    }

    [Command]
    private void CmdGetResource(int amount)
    {
        RpcGetResource(amount);
    }

    [ClientRpc]
    private void RpcGetResource(int amount)
    {
        score = Mathf.Min(score + amount, 3);
        slider.value = score;
        ChangeSpeed();
    }

    [Command]
    private void CmdClearScore()
    {
        RpcClearScore();
    }

    [ClientRpc]
    private void RpcClearScore()
    {
        score = 0;
        slider.value = score;
        ChangeSpeed();
    }

    private void ChangeSpeed()
    {
        switch (score)
        {
            case 0:
                speed = 8.0f;
                break;
            case 1:
                speed = 6.0f;
                break;
            case 2:
                speed = 4.0f;
                break;
            case 3:
                speed = 2.0f;
                break;
        }
    }
}
