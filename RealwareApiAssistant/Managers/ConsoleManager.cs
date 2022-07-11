namespace RealwareApiAssistant.Managers
{
    /// <summary>
    /// Manages the console messages and log.
    /// </summary>
    public class ConsoleManager
    {
        private StreamWriter log;
        private string logFilePath;

        public ConsoleManager(string logFilePath)
        {
            this.logFilePath = logFilePath;
            SetLogFileLocation(logFilePath);
        }

        public void SetLogFileLocation(string logFilePath)
        {
            var logFile = File.CreateText(string.Format(logFilePath, DateTime.Now.ToString(Constants.LogFileDateFormat)));
            log = logFile;

            if (log == null)
            {
                WriteWarning($"Could not create log file {logFilePath}. A log file will not be saved for this script.");
                return;
            }
        }

        internal void CloseLogFile()
        {
            log?.Close();
        }

        public string? ReadLine() => Console.ReadLine();
        public bool WriteLog(string message) => WriteLog(message, ConsoleColor.White);
        public bool WriteInput(string message) => WriteLog(message, ConsoleColor.Magenta);
        public bool WriteSuccess(string message) => WriteLog(message, ConsoleColor.Green);
        public bool WriteWarning(string message) => WriteLog("[Warning] " + message, ConsoleColor.Yellow);
        public bool WriteError(string message) => WriteLog("[Error] " + message, ConsoleColor.Red);
        public bool WriteErrorWithDetails(string message, string details)
        {
            WriteError(message);
            WriteLog("          Details: " + details, ConsoleColor.Red);
            return false;
        }

        public bool WriteMissingSettingText(string name)
            => WriteError($"Parameter '{name}' is required. Please specify in settings file.");
        public bool WriteMissingSettingValueChangeText(int position, string name)
            => WriteWarning($"Value change at position {position} is missing column '{name}'.");

        private bool WriteLog(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            log?.WriteLineAsync(message);
            return false;
        }
    }
}
