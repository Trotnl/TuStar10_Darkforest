using Mirror;
using TMPro;
using UnityEngine;


// Command���ͻ��˵��ã�������ִ��
// ClientRpc: ���������ã��ͻ���ִ��
// SyncVar��ClientRpc���ƣ��������α���
public class Player : NetworkBehaviour
{
    // �ƶ�
    public float speed = 5f;
    private Vector2 direction;
    private GameObject moveJoystickGO;
    private FixedJoystick moveJoystick;

    // ����
    public GameObject bullet;
    public float bulletSpeed = 7f;

    // ���
    private Animator animator;
    private Rigidbody2D rb;

    // ͬ����������
    public TextMeshProUGUI nameText;
    [SyncVar(hook = nameof(OnNameChange))]
    private string name;

    // ͬ�����﷭ת
    private SpriteRenderer sprite;
    [SyncVar(hook = nameof(OnFlipChange))]
    private bool flipX;

    // ͬ������
    [SyncVar(hook = nameof(OnAnimationChange))]
    private bool run;

    public override void OnStartLocalPlayer()
    {
        // �������
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);

        CmdSetupPlayer("Player" + Random.Range(100, 999));
    }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        

        if (!isLocalPlayer) { return; }

        // ��ҡ��
        moveJoystickGO = transform.Find("/Canvas/MoveJoystick").gameObject;
        moveJoystick = moveJoystickGO.GetComponent<FixedJoystick>();

        // ע���¼�
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
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }

        // �ƶ�
        Move(moveJoystick.Direction);
    }

    // �л�����
    void SetAnimation()
    {
        CmdSetAnimation(direction != Vector2.zero);
    }

    // ���﷭ת
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
        // �����ӵ�
        GameObject bulletClone = Instantiate(bullet, transform.position, Quaternion.identity);

        // �ٶ�
        bulletClone.GetComponent<Rigidbody2D>().velocity = attackDirection.normalized * bulletSpeed;

        // �Ƕ�
        bulletClone.transform.rotation = Quaternion.FromToRotation(Vector3.up, attackDirection);

        // ���ù�����
        bulletClone.GetComponent<Bullet>().owner = gameObject;
    }

    private void OnJoystickAttackUp(object[] arr)
    {
        CmdAttack((Vector2)arr[0]);
    }
}
