using Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace AobScanExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public struct ProcessEntry32
        {
            public uint dwSize;

            public uint cntUsage;

            public uint th32ProcessID;

            public IntPtr th32DefaultHeapID;

            public uint th32ModuleID;

            public uint cntThreads;

            public uint th32ParentProcessID;

            public int pcPriClassBase;

            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
        private string sr;

        private int x;

        public Mem MemLib = new Mem();

        private static string string_0;
        private IContainer icontainer_0;
        [DllImport("KERNEL32.DLL")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);

        [DllImport("KERNEL32.DLL")]
        public static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);

        [DllImport("KERNEL32.DLL")]
        public static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);
        private async Task PutTaskDelay(int Time)
        {
            await Task.Delay(Time);
        }

        public async void bimsocool(string type)
        {
            x = 0;
            await DzVl(type);
        }
        public async void memory(string search, string replace)
        {

            if (Convert.ToInt32(PID.Text) == 0)
            {
                MessageBox.Show("Error");
            }
            MemLib.OpenProcess(Convert.ToInt32(PID.Text));

            IEnumerable<long> source = await MemLib.AoBScan(0L, 140737488355327L, (search), writable: true, executable: true);
            if (source.Count() != 0)
            {
                for (int i = 0; i < source.Count(); i++)
                {
                    MemLib.WriteMemory(source.ElementAt(i).ToString("X"), "bytes", (replace));
                }

                MessageBox.Show("Success");
            }
            else
            {
                MessageBox.Show("Error");
            }
            MemLib.CloseProcess();

        }

        private async Task<IntPtr> DzVl(string type)
        {
            string ProcScan = procname.Text;
            IntPtr intPtr = IntPtr.Zero;
            uint num = 0u;
            IntPtr intPtr2 = CreateToolhelp32Snapshot(2u, 0u);
            if ((int)intPtr2 > 0)
            {
                ProcessEntry32 processEntry = default(ProcessEntry32);
                processEntry.dwSize = (uint)Marshal.SizeOf(processEntry);
                for (int num2 = Process32First(intPtr2, ref processEntry); num2 == 1; num2 = Process32Next(intPtr2, ref processEntry))
                {
                    IntPtr intPtr3 = Marshal.AllocHGlobal((int)processEntry.dwSize);
                    Marshal.StructureToPtr(processEntry, intPtr3, fDeleteOld: true);
                    ProcessEntry32 processEntry2 = (ProcessEntry32)Marshal.PtrToStructure(intPtr3, typeof(ProcessEntry32));
                    Marshal.FreeHGlobal(intPtr3);
                    if (processEntry2.szExeFile.Contains(ProcScan) && processEntry2.cntThreads > num)
                    {
                        num = processEntry2.cntThreads;
                        intPtr = (IntPtr)processEntry2.th32ProcessID;
                    }
                    
                    
                }
                PID.Text = Convert.ToString(intPtr);
                await PutTaskDelay(1000);              

                if(type == "Start")
                {
                    memory(hexscan.Text, hexrep.Text);
                }

            }
            return intPtr;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            bimsocool("Start");
        }
    }
}
