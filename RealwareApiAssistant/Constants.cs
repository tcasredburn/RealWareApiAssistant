namespace RealwareApiAssistant
{
    public class Constants
    {
        public const string LogFileName = "RealwareApiAssistant_{0}.log";
        public const string LogFileDateFormat = "yyyyMMdd-hhmmss";
        public const string ApiBearerConnection = "Bearer {0}";
        public const string DefaultErrorsAboveMessage = "Errors were found in the validation script so the procedure was not started. See logs above.";
        public const int ExcelColumnWarningLimit = 50;
        public const int RetryRequestWaitTimeInMs = 1000;
        public const string ApplicationJsonParameter = "application/json";

        public const string ApplicationName = "Realware Api Assistant";
        public const string ApplicationVersion = "1.5";
    }
}
