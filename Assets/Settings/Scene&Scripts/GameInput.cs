using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public static class GameInput
{
    public static bool PressedOne()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        return k != null && (k.digit1Key.wasPressedThisFrame || k.numpad1Key.wasPressedThisFrame);
#else
return Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);
#endif
    }
    public static bool PressedTwo()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        return k != null && (k.digit2Key.wasPressedThisFrame || k.numpad2Key.wasPressedThisFrame);
#else
return Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
#endif
    }
}