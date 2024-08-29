using FlyffUAutoFSPro._Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace FlyffUAutoFSPro.AppWindows
{
    /// <summary>
    /// Interaction logic for CheckKeyPressedWindow.xaml
    /// </summary>
    public partial class CheckKeyPressedWindow : Window
    {
        private bool _inputFinish = false;
        public List<ActionKey> PressedActionKeys = new List<ActionKey>();

        private int _minKeys;
        private int _maxKeys;
        private int _minNormalKeys;
        private int _minModifierKeys;
        private int? _maxNormalKeys;
        public CheckKeyPressedWindow(int minKeys, int maxKeys , int minNormalKeys = 0, int minModifierKeys = 0, int? maxNormalKeys = null)
        {
            _minKeys = minKeys;
            _maxKeys = maxKeys;
            _minNormalKeys = minNormalKeys;
            _minModifierKeys = minModifierKeys;
            _maxNormalKeys = maxNormalKeys;

            InitializeComponent();
            this.Title = Properties.Resources.presskey;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var pressedKeys = GetDownKeys();

            if (!_inputFinish)
            {
                if (pressedKeys.Contains(Key.Enter))
                {
                    PressedActionKeys = new List<ActionKey>();
                    _inputFinish = true;
                    StopKeyDedection();
                }

                var pressedKeysOk = pressedKeys.Where(x => GlobalValues.AvailableKeys.Select(x => x.Value).Any(t => t.KeybordKeys.Contains(x))).ToList();

                PressedActionKeys = pressedKeysOk.KeysToActionkeys();
                PressedButtonLabel.Content = PressedActionKeys.ActionKeysToString();

                if (pressedKeysOk.Count() >= _minKeys && pressedKeysOk.Count() == _maxKeys)
                {
                    _inputFinish = true;
                }
            }
           
            e.Handled = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            _inputFinish = true;
            StopKeyDedection();
            e.Handled = true;
        }


        private void StopKeyDedection()
        {
            int modifiers = 0;
            int normalKeys = 0;

            foreach (var k in PressedActionKeys)
            {
                if (k.KeybordKeys.First().IsModifierKey())
                {
                    modifiers++;
                }
                else
                {
                    normalKeys++;
                }
            }

            if(_minNormalKeys > normalKeys || _minModifierKeys > modifiers || _maxNormalKeys.HasValue && _maxNormalKeys.Value > normalKeys)
            {
                PressedActionKeys = new List<ActionKey>();
                PressedButtonLabel.Content = Properties.Resources.waiting;
                _inputFinish = false;
                MessageBox.Show(this,string.Format(Properties.Resources.wrongkeycombination_message, _minNormalKeys == 0 ? "-" : _minNormalKeys.ToString(), _minModifierKeys == 0 ? "-" : _minModifierKeys.ToString()),Properties.Resources.wrongkeycombination);
            }
            else
            {
                Window.GetWindow(this).DialogResult = true;
                Window.GetWindow(this).Close();
            }

        }

        private static readonly byte[] DistinctVirtualKeys = Enumerable
        .Range(0, 256)
        .Select(KeyInterop.KeyFromVirtualKey)
        .Where(item => item != Key.None)
        .Distinct()
        .Select(item => (byte)KeyInterop.VirtualKeyFromKey(item))
        .ToArray();

        /// <summary>
        /// Gets all keys that are currently in the down state.
        /// </summary>
        /// <returns>
        /// A collection of all keys that are currently in the down state.
        /// </returns>
        public IEnumerable<Key> GetDownKeys()
        {
            var downKeys = new List<Key>();

            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (key != Key.None && Keyboard.IsKeyDown(key))
                {
                    downKeys.Add(key);
                }
            }
            return downKeys;

        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] keyState);
    }





    
}
