using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealwareApiAssistant.Builders;
using RealwareApiAssistant.Models.Api.Request;
using RealwareApiAssistant.Models.Api.Result;
using RealwareApiAssistant.Models.IO;
using RestSharp;

namespace RealwareApiAssistant.Managers
{
    public class ApiManager
    {
        ConsoleManager console;
        ApiScript script;
        RestClient client;

        public ApiManager(ConsoleManager console, ApiScript script)
        {
            this.console = console;
            this.script = script;

            client = new RestClient(script.ApiSettings.ApiPath);

            executeProcedure();
        }

        private void executeProcedure()
        {
            var changes = getChangeList();

            if (changes == null)
                return;

            if (confirmUpdateChanges(changes))
                processChangesToApi(changes);
        }

        private bool confirmUpdateChanges(List<ApiRequest> changes)
        {
            bool result = true;

            if(!script.SkipConfirmations)
            {
                console.WriteInput($"{changes.Count} values were found. Press Y to continue.");

                if (console.ReadLine()?.ToUpper() != "Y")
                {
                    console.WriteLog("Cancelled by user.");
                    result = false;
                }
            }

            return result;
        }

        private List<ApiRequest> getChangeList()
        {
            console.WriteLog($"Beginning read from excel file at '{script.ExcelFile}'...");

            int rowCount = 1;
            var requests = new List<ApiRequest>();

            using (var workbook = new XLWorkbook(script.ExcelFile))
            {
                var sheet = workbook.Worksheets.First();

                console.WriteLog($"Selected sheet \"{sheet.Name}\".");

                var columnUsedCount = sheet.ColumnsUsed().Count();
                if (promptColumnsUsed(columnUsedCount))
                    return null;

                // Note: starts at row 2, skips header column by default
                for (int row = 2; row <= sheet.RowsUsed().Count(); row++)
                {
                    if ((rowCount % script.ExcelFileRowUpdateCount) == 0)
                        console.WriteLog($"Processed {rowCount} rows.");

                    var change = getChange(sheet, row);

                    requests.Add(change);
                    rowCount++;
                }
            }

            return requests;
        }

        private ApiRequest getChange(IXLWorksheet sheet, int row)
        {
            var request = new ApiRequest();

            addStaticIdsToRequest(request);
            addStaticValuesToRequest(request);
            addExcelIdsAndValues(request, sheet, row);

            return request;
        }

        private void addStaticIdsToRequest(ApiRequest request)
        {
            foreach (var id in script.GetStaticIdColumns())
            {
                request.Ids.Add(new ApiColumn()
                {
                    ColumnId = id.IdName,
                    Value = id.StaticValue
                });
            }
        }

        private void addStaticValuesToRequest(ApiRequest request)
        {
            foreach (var valueChange in script.ValueChanges)
            {
                if (valueChange.ExcelFromColumn == null && valueChange.ExcelToColumn == null)
                {
                    request.Values.Add(new ApiValue
                    {
                        Path = valueChange.Path,
                        RealwarePropertyName = valueChange.RealWareColumn,
                        FromValue = valueChange.FromValue,
                        ToValue = valueChange.ToValue
                    });
                }
            }

            foreach (var valueInsert in script.ValueInserts)
            {
                if (valueInsert.ExcelFromColumn == null && valueInsert.ExcelToColumn == null)
                {
                    request.Values.Add(new ApiValue
                    {
                        Path = valueInsert.Path,
                        RealwarePropertyName = valueInsert.RealWareColumn,
                        FromValue = valueInsert.FromValue,
                        ToValue = valueInsert.ToValue,
                        IsNew = true
                    });
                }
            }
        }

        private void addExcelIdsAndValues(ApiRequest request, IXLWorksheet sheet, int row)
        {
            for (int column = 1; column <= sheet.ColumnsUsed().Count(); column++)
            {
                string columnName = sheet.Cell(1, column).Value.ToString();
                string value = sheet.Cell(row, column).Value.ToString();

                // Add Ids
                if (script.IsRealWareApiId(columnName))
                {
                    var id = script.GetRealWareApiId(columnName);

                    request.Ids.Add(new ApiColumn()
                    {
                        ColumnId = id.IdName,
                        Value = value
                    });
                }

                // Add Value Changes
                if (script.IsRealWareApiValueChange(columnName))
                {
                    var change = script.GetRealWareApiValueChange(columnName);

                    if (change.ExcelFromColumn != null && change.ExcelFromColumn == columnName)
                        change.FromValue = getToValue(sheet.Cell(row, column).Value);

                    if (change.ExcelToColumn != null && change.ExcelToColumn == columnName)
                        change.ToValue = getToValue(sheet.Cell(row, column).Value);

                    request.Values.Add(new ApiValue()
                    {
                        Path = change.Path,
                        RealwarePropertyName = change.RealWareColumn,
                        FromValue = change.FromValue,
                        ToValue = getToValue(change.ToValue),
                        IsNew = false
                    });
                }

                // Add Value Inserts
                if (script.IsRealWareApiValueInsert(columnName))
                {
                    var insert = script.GetRealWareApiValueInsert(columnName);

                    if (insert.ExcelFromColumn != null && insert.ExcelFromColumn == columnName)
                        insert.FromValue = getToValue(sheet.Cell(row, column).Value);

                    if (insert.ExcelToColumn != null && insert.ExcelToColumn == columnName)
                        insert.ToValue = getToValue(sheet.Cell(row, column).Value);

                    request.Values.Add(new ApiValue()
                    {
                        Path = insert.Path,
                        RealwarePropertyName = insert.RealWareColumn,
                        FromValue = insert.FromValue,
                        ToValue = getToValue(insert.ToValue),
                        IsNew = true
                    });
                }
            }
        }

        private object getToValue(object toValue)
        {
            if (toValue == null)
                return null;

            if(script.ForceExcelNULLValues)
            {
                if (toValue.ToString() == "NULL")
                    return null;
            }

            return toValue;
        }

        private bool promptColumnsUsed(int columnUsedCount)
        {
            if(script.SkipWarningPrompts)
                return false;

            if (columnUsedCount <= Constants.ExcelColumnWarningLimit)
                return false;

            console.WriteInput($"Column used count is extremely high ({columnUsedCount})! Are you sure you want to process all of this data? Y/N");
            if (Console.ReadKey().Key != ConsoleKey.Y)
                return true;

            return false;
        }

        private void processChangesToApi(List<ApiRequest> changes)
        {
            int index = 1;
            foreach(var change in changes)
            {
                var result = executeCommandToApi(change, index);

                string idField = string.Join("|", change.Ids.Select(x => x.Value).ToArray());

                string message = $"{DateTime.Now} - {idField} - {result.Message} - ";
                string suffix = $" - ({index}/{changes.Count})";
                if (result.Response.StatusDescription == "OK")
                {
                    message += $"Total_Changes:{result.ChangeCount} Total_Inserts:{result.InsertCount}" + suffix;
                    console.WriteSuccess(message);
                }
                else if (result.Response.StatusDescription == "Created")
                {
                    message += $"Total_Inserts:{result.InsertCount}" + suffix;
                    console.WriteSuccess(message);
                }
                else
                {
                    message += suffix;
                    console.WriteErrorWithDetails(message, result.MessageDetail);
                }

                index++;
            }
        }

        private ApiResult executeCommandToApi(ApiRequest change, int index)
        {
            switch(script.Method)
            {
                case Method.DELETE:
                    return executeDeleteCommandToApi(change);
                case Method.PUT:
                    return executePutCommandToApi(change, index);
                case Method.POST:
                    return executePostCommandToApi(change, index);
                default:
                    console.WriteError($"'{script.Method}' methods have not been implemented!");
                    return new ApiResult();
            }
        }

        private ApiResult executeDeleteCommandToApi(ApiRequest change)
        {
            var request = buildRequest(Method.DELETE, change.Ids);

            return executeApiRequest(request);
        }

        private ApiResult executePutCommandToApi(ApiRequest change, int index)
        {
            var getRequest = buildRequest(Method.GET, change.Ids);

            var getResult = executeApiRequest(getRequest);

            // Retry once if enabled
            if (!getResult.Response.IsSuccessful && script.RetryImmediatelyAfterBadRequest)
            {
                Thread.Sleep(Constants.RetryRequestWaitTimeInMs);
                getResult = executeApiRequest(getRequest);
            }

            // Cancel here if failed
            if (!getResult.Response.IsSuccessful || getResult.Response.Content == null)
                return getResult;

            dynamic json = JsonConvert.DeserializeObject(getResult.Response.Content);

            if (json == null)
            {
                getResult.Message = "Failed - Did not receive JSON object.";
                return getResult;
            }

            // Update json values from get (includes insert)
            ApiJsonResult jsonResult = updateJsonValues(json, change);

            // Update the values in RealWare via the API
            var putRequest = buildRequest(Method.PUT, change.Ids);
            putRequest.AddJsonBody(jsonResult.Json);

            // Save to json file if saved
            if (script.ExportJsonSettings?.ExportJsonFiles == true)
                Helpers.Json.ExportJsonToFile(script.ExportJsonSettings, change.Ids,
                    Method.PUT, jsonResult.Json, index);

            var result = executeApiRequest(putRequest);
            result.ChangeCount = jsonResult.ChangeCount;
            result.InsertCount = jsonResult.InsertCount;

            return result;
        }

        private ApiResult executePostCommandToApi(ApiRequest change, int index)
        {
            // Update json values from get (includes insert)
            JObject baseJson = new JObject();
            ApiJsonResult jsonResult = updateJsonValues(null, change);

            // Update the values in RealWare via the API
            var postRequest = buildRequest(Method.POST, change.Ids);
            postRequest.AddJsonBody(jsonResult.Json);

            // Save to json file if saved
            if(script.ExportJsonSettings?.ExportJsonFiles == true)
                Helpers.Json.ExportJsonToFile(script.ExportJsonSettings, change.Ids, 
                    Method.POST, jsonResult.Json, index);

            var result = executeApiRequest(postRequest);
            result.InsertCount = jsonResult.InsertCount;

            return result;
        }

        private ApiJsonResult updateJsonValues(dynamic json, ApiRequest change)
        {
            int valueChanges = 0;
            int valueInserts = 0;
            int totalInsertCount = 0;
            JToken? jsonData = null;
            if (json != null)
                jsonData = JToken.Parse(json.ToString());

            // Changes
            int totalChangeCount = change.Values.Where(x => !x.IsNew).Count();
            if (jsonData != null)
            {
                foreach (var column in change.Values.FindAll(x => !x.IsNew))
                {
                    string path = column.Path ?? "";
                    var jTokens = jsonData.SelectTokens(path);

                    foreach (var jToken in jTokens)
                    {
                        if ((column.FromValue != null && column.FromValue.ToString() == jToken[column.RealwarePropertyName].ToString())
                                    || column.FromValue == null)
                        {
                            valueChanges++;
                            if(column.ToValue?.ToString() == "[]")
                                jToken[column.RealwarePropertyName] = new JArray(); 
                            else
                                jToken[column.RealwarePropertyName] = column.ToValue?.ToString();
                        }
                    }
                }
            }

            //Inserts
            var builder = new JsonNewObjectBuilder((JObject?)jsonData, change.Values.FindAll(x => x.IsNew));
            totalInsertCount = builder.InsertCount;
            try
            {
                builder.BuildJsonResult();
                jsonData = builder.GetResult();
            }
            catch{ }
            valueInserts = builder.InsertCountSuccessful;

            //Return the result
            return new ApiJsonResult()
            {
                Json = jsonData.ToString(),
                ChangeCount = valueChanges,
                TotalChangeCount = totalChangeCount,
                InsertCount = valueInserts,
                TotalInsertCount = totalInsertCount
            };
        }

        private RestRequest buildRequest(Method method, List<ApiColumn> ids)
        {
            var url = Helpers.Api.GetUrlPath(script.ApiOperation, ids);

            var request = new RestRequest(url, method);

            request.AddHeader("Authorization", string.Format(Constants.ApiBearerConnection, script.ApiSettings.Token));

            return request;
        }

        private ApiResult executeApiRequest(RestRequest request)
        {
            var result = client.ExecuteAsync(request).Result;

            return new ApiResult
            {
                Message = result.StatusDescription,
                MessageDetail = result.Content,
                Response = result
            };
        }
    }
}
