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
            verbose = args.Contains("--verbose") || args.Contains("-v");
            superVerbose = args.Contains("-vv");
            if (superVerbose) verbose = true;

            if (verbose) WriteLine("Started watching node process under Visual Studio (devenv)");
            while (true)
            {
                var nodeProcesses = Process.GetProcessesByName("node");
                foreach (var nodeProcess in nodeProcesses)
                {
                    if (superVerbose) WriteLine($"Found process id {nodeProcess.Id}");
                    if (IsChildOfDevenv(nodeProcess))
                    {
                        if (superVerbose) WriteLine($"Found node process id {nodeProcess.Id} which is a child of devenv that is going to die");
                        try
                        {
                            nodeProcess.Kill();
                            if (verbose)
                                WriteLine($"Killed node process id {nodeProcess.Id}");
                        }
                        catch (Exception ex)
                        {
                            WriteLine($"Could not kill node process id {nodeProcess.Id}");
                            if (verbose)
                                WriteLine($"Reason: {ex.Message}");
                            if (superVerbose)
                                WriteLine($"Stack trace: {ex.StackTrace}");
                        }
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
        private bool verbose;
        private bool superVerbose;

        private bool IsChildOfDevenv(Process process)
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
            uint parentId;
            try
            {
                var search = new ManagementObjectSearcher("root\\CIMV2", $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {processId}");
                var processResults = search.Get().GetEnumerator();
                if (!processResults.MoveNext()) return null;
                var parentObject = processResults.Current;
                parentId = (uint)parentObject["ParentProcessId"];
            }
            catch (Exception ex)
            {
                WriteLine($"Could not get process info for process id {processId}. VS might be running as admin, in this case you might want to run this command as admin.");
                if (verbose)
                    WriteLine($"Reason: {ex.Message}");
                if (superVerbose)
                    WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
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