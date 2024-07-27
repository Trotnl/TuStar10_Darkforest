using Mirror;
using TMPro;
using UnityEngine;


// Command：客户端调用，服务器执行
// ClientRpc: 服务器调用，客户端执行
// SyncVar与ClientRpc类似，用于修饰变量
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
    private SpriteRenderer sprite;
    [SyncVar(hook = nameof(OnFlipChange))]
    private bool flipX;

    // 同步动画
    [SyncVar(hook = nameof(OnAnimationChange))]
    private bool run;

    public override void OnStartLocalPlayer()
    {
        // 相机跟随
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);

        CmdSetupPlayer("Player" + Random.Range(100, 999));
    }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        torch = transform.GetChild(0).GetChild(1).gameObject;

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
        SetFlip();

        if (!isLocalPlayer) { return; }

        direction = moveJoystick.Direction; 
        CmdTorch(attackJoystick.Direction);
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
        CmdFlip(direction.x < 0);
    }

    private void Move(Vector2 direction)
    {
        rb.MovePosition(rb.position + direction.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnNameChange(string _old, string _new)
    {
        nameText.text = name;
    }

    private void OnFlipChange(bool _old, bool _new)
    {
        sprite.flipX = flipX;
    }

    private void OnAnimationChange(bool _old, bool _new)
    {
        animator.SetBool("run", run);
    }

    [Command]
    private void CmdFlip(bool _flipX)
    {
        flipX = _flipX;
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

    [Command]
    private void CmdTorch(Vector2 torchDirection)
    {
        RpcTorch(torchDirection);
    }

    [ClientRpc]
    private void RpcTorch(Vector2 torchDirection)
    {
        torch.transform.rotation = Quaternion.FromToRotation(Vector3.up, torchDirection);
        Debug.Log(torchDirection.x + "" + torchDirection.y.ToString());
    }
}
