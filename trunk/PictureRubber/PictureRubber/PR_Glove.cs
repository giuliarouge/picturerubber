using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace PictureRubber
{
    internal class PR_Glove
    {
        #region Constants


        private const UInt32 MouseEventfLeftDown = 0x0002;
        private const UInt32 MouseEventfLeftUp = 0x0004;

        #endregion


        #region DllImports

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, long dx, long dy, uint dwData, IntPtr dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        #endregion

        /// <summary> 
        /// Sends the mouse left click. 
        /// </summary> 
        /// <param name="location">The location.</param> 
        public static void SendMouseLeftClick(float _x, float _y)
        {
            SetCursorPos((int)_x, (int)_y);
            mouse_event(MouseEventfLeftDown, 0, 0, 0, new IntPtr());
            mouse_event(MouseEventfLeftUp, 0, 0, 0, new IntPtr());
        }
    }
}
