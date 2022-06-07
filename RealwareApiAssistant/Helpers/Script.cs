using RealwareApiAssistant.Managers;
using RealwareApiAssistant.Models.IO;

namespace RealwareApiAssistant.Helpers
{
    public static class Script
    {
        internal static ApiScript ReadScriptFromFile(ConsoleManager console, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                console.WriteError("No script file was specified! Please add path of settings file as an argument.");
                return null;
            }

            if (!File.Exists(filePath))
            {
                console.WriteError($"No script file was found at location \"{filePath}\".");
                return null;
            }

            // Actually load the file
            try
            {
                string script = File.ReadAllText(filePath);

                if(string.IsNullOrWhiteSpace(script))
                {
                    console.WriteError($"The script appears to be empty: \"{filePath}\".");
                    return null;
                }

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiScript>(script);
                result.scriptFilePath = filePath;
                return result;
            }
            catch (Exception ex)
            {
                console.WriteError($"Error reading script file: {ex.Message}");
            }
            return null;
        }

        internal static void InitializeScript(ConsoleManager console, ApiScript script)
        {
            if (script == null)
                return;

            initializeCustomLogFile(console, script);
        }

        private static void initializeCustomLogFile(ConsoleManager console, ApiScript script)
        {
            if (string.IsNullOrWhiteSpace(script.CustomLogFileLocation))
                return;

            if (!Directory.Exists(script.CustomLogFileLocation))
            {
                console.WriteWarning($"Custom log file location does not exist: \"{script.CustomLogFileLocation}\".");
                script.CustomLogFileLocation = string.Empty;
                return;
            }

            console.SetLogFileLocation(script.CustomLogFileLocation);
        }

        internal static bool ValidateScript(ConsoleManager console, ApiScript script)
        {
            bool result = true;

            // Script is empty - EXIT
            if (script == null)
                return false;

            if (string.IsNullOrWhiteSpace(script.ApiOperation))
                result = console.WriteMissingSettingText(nameof(script.ApiOperation));

            if (script.Method == null)
                result = console.WriteMissingSettingText(nameof(script.Method));

            if (script.IdColumns == null)
                result = console.WriteMissingSettingText(nameof(script.IdColumns));
            else if (script.IdColumns.Count == 0)
                result = console.WriteWarning("You need atleast one id column specified or nothing will happen.");

            if (script.ValueChanges == null)
                result = console.WriteMissingSettingText(nameof(script.ValueChanges));
            else if (script.ValueChanges.Count == 0)
                result = console.WriteWarning("You need atleast one value change specified or nothing will happen.");
            else
            {
                int index = 0;
                foreach (var valueChange in script.ValueChanges)
                {
                    if (valueChange.ExcelFromColumn != null && valueChange.ExcelFromColumn == valueChange.ExcelToColumn)
                        result = console.WriteError($"Value change at position {index} CANNOT have matching excel column names for {nameof(valueChange.ExcelFromColumn)} and {nameof(valueChange.ExcelToColumn)} ({valueChange.ExcelFromColumn}).");
                    if (string.IsNullOrWhiteSpace(valueChange.RealWareColumn))
                        console.WriteMissingSettingValueChangeText(index, nameof(valueChange.RealWareColumn));
                    if (valueChange.ToValue == null && string.IsNullOrWhiteSpace(valueChange.ExcelToColumn))
                        console.WriteWarning($"Value change at position {index} is missing columns '{nameof(valueChange.ToValue)}' and '{nameof(valueChange.ExcelToColumn)}'.");
                    index++;
                }
            }

            // API Settings - EXIT
            if(script.ApiSettings == null)
                return console.WriteMissingSettingText(nameof(ApiScript.ApiSettings));

            if (string.IsNullOrWhiteSpace(script.ApiSettings.ApiPath))
                result = console.WriteMissingSettingText(nameof(ApiScript.ApiSettings.ApiPath));
            else
                console.WriteLog($"You are connecting to {script.ApiSettings.ApiPath}...");

            if (string.IsNullOrWhiteSpace(script.ApiSettings.Token))
                result = console.WriteMissingSettingText(nameof(ApiScript.ApiSettings.Token));

            // Excel File - EXIT
            if (string.IsNullOrWhiteSpace(script.ExcelFile))
                return console.WriteMissingSettingText(nameof(script.ExcelFile));

            if (!File.Exists(script.ExcelFile))
            {
                var newFilePath = Path.Join(Path.GetDirectoryName(script.scriptFilePath), script.ExcelFile);
                if (!File.Exists(newFilePath))
                    result = console.WriteError($"Excel file does not exist at \"{script.ExcelFile}\".");
                else
                    script.ExcelFile = newFilePath;
            }

            if (script.Method != RestSharp.Method.PUT
               && script.Method != RestSharp.Method.POST
               && script.Method != RestSharp.Method.DELETE)
                console.WriteError("Method must be either PUT, POST, or DELETE.");

            return result;
        }
        
    }
}
