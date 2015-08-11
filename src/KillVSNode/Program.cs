using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace KillVSNode
{
    public class Program
    {
        public void Main(string[] args)
        {
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += (s, a) => stopped = true;
            var verbose = args.Contains("--verbose") || args.Contains("-v");

            if (verbose) Console.WriteLine("Started watching node process under Visual Studio (devenv)");
            while (true)
            {
                var nodeProcesses = Process.GetProcessesByName("node");
                foreach (var nodeProcess in nodeProcesses)
                {
                    if (IsChildOfDevenv(nodeProcess))
                    {
                        nodeProcess.Kill();
                        if (verbose)
                            Console.WriteLine($"Killed node process id {nodeProcess.Id}");
                    }
                }
                System.Threading.Thread.Sleep(1000);
                if (stopped)
                    return;
            }
        }

        private bool stopped = false;

        private bool IsChildOfDevenv(Process process)
        {
            var parentProcess = process;
            do
            {
                parentProcess = GetProcessParent(parentProcess);
            }
            while (parentProcess != null && string.Compare(parentProcess.ProcessName, "devenv", StringComparison.InvariantCultureIgnoreCase) > 0);
            return parentProcess != null;
        }

        private Process GetProcessParent(Process process)
        {
            var processId = process.Id;
            if (processId == 0) return null;
            var search = new ManagementObjectSearcher("root\\CIMV2", $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {processId}");
            var processResults = search.Get().GetEnumerator();
            if (!processResults.MoveNext()) return null;
            var parentObject = processResults.Current;
            var parentId = (uint)parentObject["ParentProcessId"];
            try
            {
                var parentProcess = Process.GetProcessById((int)parentId);
                return parentProcess;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

    }
}
