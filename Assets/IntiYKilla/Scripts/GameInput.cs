using UnityEngine;
#if !ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace IntiYKilla
{
    // Lectura de controles. Funciona tanto con el Input Manager clásico
    // como con el paquete Input System (el que traen por defecto los proyectos de Unity 6).
    public static class GameInput
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        public static float Horizontal()
        {
            float x = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
            float axis = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(axis) > Mathf.Abs(x)) x = axis;
            return Mathf.Clamp(x, -1f, 1f);
        }

        public static bool JumpPressed() =>
            Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton0);

        public static bool JumpHeld() =>
            Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.JoystickButton0);

        public static bool SwitchPressed() =>
            Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3) ||
            Input.GetKeyDown(KeyCode.JoystickButton5);

        public static bool InteractPressed() =>
            Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2);

        public static bool StartPressed() =>
            Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton7);

        public static bool RestartPressed() => Input.GetKeyDown(KeyCode.R);

        public static float Vertical()
        {
            float y = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y -= 1f;
            float axis = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(axis) > Mathf.Abs(y)) y = axis;
            return Mathf.Clamp(y, -1f, 1f);
        }

        // Solo Espacio / botón A (sirve para soltarse de una enredadera).
        public static bool JumpButtonPressed() =>
            Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);

        // Lanzar una piedra con la huaraca (honda andina).
        public static bool AttackPressed() =>
            Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.JoystickButton1);

        public static bool Level2Pressed() => Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
        public static bool Level3Pressed() => Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3);
#elif ENABLE_INPUT_SYSTEM
        static Keyboard K => Keyboard.current;
        static Gamepad G => Gamepad.current;

        public static float Horizontal()
        {
            float x = 0f;
            if (K != null)
            {
                if (K.aKey.isPressed || K.leftArrowKey.isPressed) x -= 1f;
                if (K.dKey.isPressed || K.rightArrowKey.isPressed) x += 1f;
            }
            if (G != null)
            {
                float s = G.leftStick.x.ReadValue();
                if (Mathf.Abs(s) > 0.3f && Mathf.Abs(s) > Mathf.Abs(x)) x = s;
                if (G.dpad.left.isPressed) x = -1f;
                if (G.dpad.right.isPressed) x = 1f;
            }
            return Mathf.Clamp(x, -1f, 1f);
        }

        public static bool JumpPressed() =>
            (K != null && (K.spaceKey.wasPressedThisFrame || K.wKey.wasPressedThisFrame || K.upArrowKey.wasPressedThisFrame)) ||
            (G != null && G.buttonSouth.wasPressedThisFrame);

        public static bool JumpHeld() =>
            (K != null && (K.spaceKey.isPressed || K.wKey.isPressed || K.upArrowKey.isPressed)) ||
            (G != null && G.buttonSouth.isPressed);

        public static bool SwitchPressed() =>
            (K != null && K.qKey.wasPressedThisFrame) ||
            (G != null && (G.buttonNorth.wasPressedThisFrame || G.rightShoulder.wasPressedThisFrame));

        public static bool InteractPressed() =>
            (K != null && K.eKey.wasPressedThisFrame) ||
            (G != null && G.buttonWest.wasPressedThisFrame);

        public static bool StartPressed() =>
            (K != null && (K.enterKey.wasPressedThisFrame || K.numpadEnterKey.wasPressedThisFrame || K.spaceKey.wasPressedThisFrame)) ||
            (G != null && (G.startButton.wasPressedThisFrame || G.buttonSouth.wasPressedThisFrame));

        public static bool RestartPressed() => K != null && K.rKey.wasPressedThisFrame;

        public static float Vertical()
        {
            float y = 0f;
            if (K != null)
            {
                if (K.wKey.isPressed || K.upArrowKey.isPressed) y += 1f;
                if (K.sKey.isPressed || K.downArrowKey.isPressed) y -= 1f;
            }
            if (G != null)
            {
                float s = G.leftStick.y.ReadValue();
                if (Mathf.Abs(s) > 0.3f && Mathf.Abs(s) > Mathf.Abs(y)) y = s;
                if (G.dpad.up.isPressed) y = 1f;
                if (G.dpad.down.isPressed) y = -1f;
            }
            return Mathf.Clamp(y, -1f, 1f);
        }

        public static bool JumpButtonPressed() =>
            (K != null && K.spaceKey.wasPressedThisFrame) || (G != null && G.buttonSouth.wasPressedThisFrame);

        public static bool AttackPressed() =>
            (K != null && (K.fKey.wasPressedThisFrame || K.jKey.wasPressedThisFrame)) ||
            (G != null && G.buttonEast.wasPressedThisFrame);

        public static bool Level2Pressed() =>
            K != null && (K.digit2Key.wasPressedThisFrame || K.numpad2Key.wasPressedThisFrame);
        public static bool Level3Pressed() =>
            K != null && (K.digit3Key.wasPressedThisFrame || K.numpad3Key.wasPressedThisFrame);
#else
        public static float Horizontal() => 0f;
        public static bool JumpPressed() => false;
        public static bool JumpHeld() => false;
        public static bool SwitchPressed() => false;
        public static bool InteractPressed() => false;
        public static bool StartPressed() => false;
        public static bool RestartPressed() => false;
        public static float Vertical() => 0f;
        public static bool JumpButtonPressed() => false;
        public static bool AttackPressed() => false;
        public static bool Level2Pressed() => false;
        public static bool Level3Pressed() => false;
#endif
    }
}
