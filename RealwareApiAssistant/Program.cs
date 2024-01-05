using RealwareApiAssistant;
using RealwareApiAssistant.Helpers;
using RealwareApiAssistant.Managers;

class Program
{
    static void Main(string[] args)
    {
        string scriptPath = string.Join(" ", args);
        Console.Title = $"{Constants.ApplicationName} - V{Constants.ApplicationVersion} - {scriptPath}";

        //1 - Start
        var console = new ConsoleManager();
        console.WriteLog(DateTime.Now.ToString());
        console.WriteLog($"Starting Realware API Assistant V{Constants.ApplicationVersion}...");

        //2 - Load Script
        var script = Script.ReadScriptFromFile(console, scriptPath);
        Script.InitializeScript(console, script);

        //3 - Execution
        if(Script.ValidateScript(console, script))
            new ApiManager(console, script);
        else
            console.WriteWarning(Constants.DefaultErrorsAboveMessage);

        //4 - Close
        console.WriteLog(Environment.NewLine);
        if (!script.SkipConfirmations)
            console.WriteLog("Process is complete. Press any key to close.");
        else
            console.WriteLog("Process is complete. Closing.");
        console.CloseLogFile();
        if (!script.SkipConfirmations)
            Console.ReadLine();
        Environment.Exit(0);
    }
}


