using Microsoft.Xna.Framework.Input;
using System;
using System.Numerics;

namespace MonoGame_Base.Project.Utility.Basics
{
    public delegate void InteractEventHandler(object sender, EventArgs e);
    public static class InputManager
    {
        public static InteractEventHandler OnInteract;
        public static InteractEventHandler OnStopInteract;
        private static bool interacted;

        private static Vector2 _direction;
        public static Vector2 Direction => _direction;
        public static bool Moving => _direction != Vector2.Zero;
        public static bool Sprint;

        private static KeyboardState _lastKeyboard;
        private static KeyboardState _currentKeyboard;

        public static void Update()
        {
            _direction = Vector2.Zero;

            _lastKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();

            if (_currentKeyboard.GetPressedKeyCount() > 0)
            {
                if (_currentKeyboard.IsKeyDown(Keys.A) || _currentKeyboard.IsKeyDown(Keys.Left)) { _direction.X--; }
                if (_currentKeyboard.IsKeyDown(Keys.D) || _currentKeyboard.IsKeyDown(Keys.Right)) { _direction.X++; }
                if (_currentKeyboard.IsKeyDown(Keys.W) || _currentKeyboard.IsKeyDown(Keys.Up)) { _direction.Y--; }
                if (_currentKeyboard.IsKeyDown(Keys.S) || _currentKeyboard.IsKeyDown(Keys.Down)) { _direction.Y++; }

                if (_currentKeyboard.IsKeyDown(Keys.LeftShift)) { Sprint = true; }
                else if (Sprint) { Sprint = false; }
            }

            // Send interacting only once...
            if (_currentKeyboard.IsKeyDown(Keys.E) && !interacted)
            {
                interacted = true;
                OnInteract?.Invoke(null, new EventArgs());
            }
            else if (_currentKeyboard.IsKeyUp(Keys.E) && interacted)
            {
                interacted = false;
                OnStopInteract?.Invoke(null, new EventArgs());
            }
        }

        public static bool KeyPressed(Keys key)
        {
            return _currentKeyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return _currentKeyboard.IsKeyDown(key);
        }
    }
}
