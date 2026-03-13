using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _controls;
    public Vector2 MoveInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsLookingRow { get; private set; }



    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    public void Update()
    {
        //Vector2(x,y)‚Æ‚µ‚Ä“ü—Í‚ğæ“¾
        MoveInput = _controls.Player.Move.ReadValue<Vector2>();

        //LeftShift‚ª‰Ÿ‚³‚ê‚Ä‚¢‚éŠÔAtrue‚É‚È‚é
        //ReadValueAsButton()‚Í‚µ‚«‚¢’l(0.5)‚ğ’´‚¦‚Ä‚¢‚ê‚ÎAtrue‚ğ•Ô‚µ‚Ü‚·
        IsSprinting = _controls.Player.Sprint.ReadValue<float>() > 0.5f;

        IsLookingRow = _controls.Player.LookRow.ReadValue<bool>();
    }
}
