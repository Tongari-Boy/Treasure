using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls _controls;
    public Vector2 MoveInput { get; private set; }
    public bool IsSprinting { get; private set; }
    
    //Space‚ª‰Ÿ‚³‚ê‚Ä‚¢‚é‚©
    public bool IsRotatingMode { get; private set; }
    //‰ñ“]•ûŒü
    public float RotationDirection { get; private set; }



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
        
        IsRotatingMode = _controls.Player.RotationModifier.ReadValue<float>() > 0.5f;

        if (IsRotatingMode)
        {
            MoveInput = Vector2.zero;

            float right = _controls.Player.LookRight.ReadValue<float>();
            float left = _controls.Player.LookLeft.ReadValue<float>();
            RotationDirection = right - left;
        }
        else
        {
            //Vector2(x,y)‚Æ‚µ‚Ä“ü—Í‚ğæ“¾
            MoveInput = _controls.Player.Move.ReadValue<Vector2>();
            //‰ñ“]•ûŒü‚Í0
            RotationDirection = 0;
        }


        //LeftShift‚ª‰Ÿ‚³‚ê‚Ä‚¢‚éŠÔAtrue‚É‚È‚é
        //ReadValueAsButton()‚Í‚µ‚«‚¢’l(0.5)‚ğ’´‚¦‚Ä‚¢‚ê‚ÎAtrue‚ğ•Ô‚·
        IsSprinting = _controls.Player.Sprint.ReadValue<float>() > 0.5f;
    }
}
