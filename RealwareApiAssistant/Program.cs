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
        var console = new ConsoleManager(Constants.LogFileName);
        console.WriteLog("Starting Realware API Assistant...");

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
        console.WriteLog("Process is complete. Press any key to close.");
        console.CloseLogFile();
        Console.ReadLine();
    }
}


