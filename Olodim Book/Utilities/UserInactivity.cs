using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PdfFlipBook.Utilities
{
    public static class UserInactivity
    {
        private struct Lastinputinfo
        {
            public uint CbSize;
            public uint DwTime;
        }

        private static Lastinputinfo _lastInPutNfo;

        static UserInactivity()
        {
            _lastInPutNfo = new Lastinputinfo();
            _lastInPutNfo.CbSize = (uint)Marshal.SizeOf(_lastInPutNfo);
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref Lastinputinfo plii);

        /// <summary>
        /// Idle time in ticks
        /// </summary>
        /// <returns></returns>
        private static uint GetIdleTickCount()
        {
            return ((uint)Environment.TickCount - GetLastInputTime());
        }

        /// <summary>
        /// Last input time in ticks
        /// </summary>
        /// <returns></returns>
        private static uint GetLastInputTime()
        {
            if (!GetLastInputInfo(ref _lastInPutNfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return _lastInPutNfo.DwTime;
        }

        public static int GetSeconds()
        {
            return (int)(GetIdleTickCount() / 1000);
        }
    }
}
