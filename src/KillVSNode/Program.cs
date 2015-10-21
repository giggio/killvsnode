using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using static System.Console;

namespace KillVSNode
{
    public class Program
    {
        public void Main(string[] args)
        {
            TreatControlCAsInput = false;
            CancelKeyPress += (s, a) => stopped = true;
            var verbose = args.Contains("--verbose") || args.Contains("-v");
            var superVerbose = args.Contains("-vv");
            if (superVerbose) verbose = true;

            if (verbose) WriteLine("Started watching node process under Visual Studio (devenv)");
            while (true)
            {
                var nodeProcesses = Process.GetProcessesByName("node");
                foreach (var nodeProcess in nodeProcesses)
                {
                    if (superVerbose) WriteLine($"Found process id {nodeProcess.Id}");
                    if (IsChildOfDevenv(nodeProcess, superVerbose))
                    {
                        if (superVerbose) WriteLine($"Found node process id {nodeProcess.Id} which is a child of devenv that is going to die");
                        try
                        {
                            nodeProcess.Kill();
                        }
                        catch (Exception ex)
                        {
                            WriteLine($"Could not kill process id {nodeProcess.Id}. Error details: \n{ex.Message}");
                        }
                        if (verbose)
                            WriteLine($"Killed node process id {nodeProcess.Id}");
                    }
                    else if (superVerbose)
                    {
                        WriteLine($"Node process is not a child of devenv");
                    }
                }
                System.Threading.Thread.Sleep(1000);
                if (stopped)
                    return;
            }
        }

        private bool stopped = false;

        private bool IsChildOfDevenv(Process process, bool superVerbose)
        {
            var parentProcess = process;
            do
            {
                parentProcess = GetProcessParent(parentProcess);
                if (superVerbose)
                {
                    if (parentProcess != null)
                        WriteLine($"Found parent process '{parentProcess.ProcessName}' id {parentProcess.Id}");
                    else
                        WriteLine($"Parent process not found");
                }
            }
            while (parentProcess != null && !string.Equals(parentProcess.ProcessName, "devenv", StringComparison.InvariantCultureIgnoreCase));
            return parentProcess != null && string.Equals(parentProcess.ProcessName, "devenv", StringComparison.InvariantCultureIgnoreCase);
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