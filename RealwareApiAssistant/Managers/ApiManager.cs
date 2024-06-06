using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
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
            var changes = getChangeListPerformant();

            if (changes == null)
                return;

            if (confirmUpdateChanges(changes))
                processChangesToApi(changes);
        }

        private List<ApiRequest> getChangeListPerformant()
        {
            console.WriteLog($"Beginning read from excel file at '{script.ExcelFile}' using fast data import method...");

            var requests = new List<ApiRequest>();

            var fileData = readDataPerformant(script.ExcelFile);

            List<string> columns = new List<string>();
            List<List<string>> rows = new List<List<string>>();

            // Read in the column listing row
            // Note: this error can occur reading the data since this is the first instance it is being referenced.
            List<string> columnRow;
            try
            {
                console.WriteLog($"Processing column header row...");
                columnRow = fileData.First();
            }
            catch (IOException exFileInUse)
            {
                console.WriteErrorWithDetails("Excel file is open", exFileInUse.Message);
                return null;
            }

            //Column only row
            foreach (var cell in columnRow.ToList())
            {
                columns.Add(cell);
            }
            console.WriteLog($"Processed {columns.Count} columns.");

            if (promptColumnsUsed(columns.Count))
                return null;

            //Row data
            int row_index = 0;
            int error_rows = 0;
            foreach (var row in fileData.ToList().Skip(1))
            {
                var currentRow = new List<string>();

                foreach (var cell in row)
                    currentRow.Add(cell);

                if ((row_index % script.ExcelFileRowUpdateCount) == 0)
                    console.WriteLog($"Processed {row_index} rows.");

                // Skip if fields do not match
                if (columns.Count > currentRow.Count)
                {
                    console.WriteWarning($"Row {row_index+1} has blank fields so columns cannot match. Skipping.");
                    error_rows++;
                }
                // Build change data
                else
                {
                    var change = getChange(columns, currentRow);

                    requests.Add(change);
                }

                row_index++;
            }
            console.WriteLog($"Processed {row_index-error_rows}/{row_index} rows.");

            return requests;
        }

        private ApiRequest getChange(List<string> columns, List<string> currentRow)
        {
            var request = new ApiRequest();

            addStaticIdsToRequest(request);
            addStaticValuesToRequest(request);
            addExcelIdsAndValues(request, columns, currentRow);

            return request;
        }

        private void addExcelIdsAndValues(ApiRequest request, List<string> columns, List<string> currentRow)
        {
            for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                string columnName = columns[columnIndex];
                string value = currentRow[columnIndex];

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
                    var changes = script.GetRealWareApiValueChanges(columnName);

                    foreach (var change in changes)
                    {
                        if (change.ExcelFromColumn != null && change.ExcelFromColumn == columnName)
                            change.FromValue = getToValue(value, change.ValueIsNumeric);

                        if (change.ExcelToColumn != null && change.ExcelToColumn == columnName)
                            change.ToValue = getToValue(value, change.ValueIsNumeric);

                        request.Values.Add(new ApiValue()
                        {
                            Path = change.Path,
                            RealwarePropertyName = change.RealWareColumn,
                            FromValue = change.FromValue,
                            ToValue = getToValue(change.ToValue, change.ValueIsNumeric),
                            ValueIsJson = change.ValueIsJson,
                            IsNew = false
                        });
                    }
                }

                // Add Value Inserts
                if (script.IsRealWareApiValueInsert(columnName))
                {
                    var inserts = script.GetRealWareApiValueInserts(columnName);

                    foreach(var insert in inserts)
                    {
                        if (insert.ExcelFromColumn != null && insert.ExcelFromColumn == columnName)
                            insert.FromValue = getToValue(value, insert.ValueIsNumeric);

                        if (insert.ExcelToColumn != null && insert.ExcelToColumn == columnName)
                            insert.ToValue = getToValue(value, insert.ValueIsNumeric);

                        request.Values.Add(new ApiValue()
                        {
                            Path = insert.Path,
                            RealwarePropertyName = insert.RealWareColumn,
                            FromValue = insert.FromValue,
                            ToValue = getToValue(insert.ToValue, insert.ValueIsNumeric),
                            ValueIsJson = insert.ValueIsJson,
                            IsNew = true
                        });
                    }
                }
            }
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
                        ToValue = valueChange.ToValue,
                        ValueIsJson = valueChange.ValueIsJson
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
                        IsNew = true,
                        ValueIsJson = valueInsert.ValueIsJson
                    });
                }
            }
        }

        private object getToValue(object toValue, bool isNumeric)
        {
            if (toValue == null)
                return null;

            if(script.ForceExcelNULLValues)
            {
                if (toValue.ToString() == "NULL")
                    return null;
            }

            if(isNumeric)
            {
                if (double.TryParse(toValue.ToString(), System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out double numVal))
                    return numVal;
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
            object lockObject = new object();

            Parallel.ForEach(changes, new ParallelOptions { MaxDegreeOfParallelism = script.Threads }, change =>
            {
                int currentIndex;
                lock (lockObject)
                {
                    currentIndex = index++;
                }


                int retries = 1 + script.RetryCount; // number of retries
                bool success = false;
                while (!success && retries > 0)
                {
                    try
                    {
                        var result = executeCommandToApi(change, currentIndex);

                        string idField = string.Join("|", change.Ids.Select(x => x.Value).ToArray());

                        string message = $"{DateTime.Now} - ThreadId:{Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(2, '0')} - {idField} - {result.Message} - ";
                        string suffix = $" - ({currentIndex}/{changes.Count})";
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
                        success = true; // if operation is successful, exit loop
                    }
                    catch (Exception ex)
                    {
                        if (retries <= 0) // if no retries left, rethrow the exception
                        {
                            throw;
                        }
                        else
                        {
                            Thread.Sleep(script.RetryWaitTimeInMs); // wait for x seconds before the next retry
                            retries--;
                        }
                    }
                }
            });
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
                if (getResult.Response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    getResult.MessageDetail = "Are you sure an object with that identifer exists in RealWare?";
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
                            if(column.ValueIsJson)
                            {
                                var cleanJsonObject = JsonConvert.DeserializeObject<dynamic>(column.ToValue?.ToString());
                                jToken[column.RealwarePropertyName] = cleanJsonObject;
                            }
                            else if(column.ToValue?.ToString() == "[]")
                                jToken[column.RealwarePropertyName] = new JArray(); 
                            else
                                jToken[column.RealwarePropertyName] = column.ToValue?.ToString();
                        }
                    }
                }
            }

            //Inserts
            var insertValueList = change.Values.FindAll(x => x.IsNew);

            if (insertValueList.Count > 0)
            {
                var builder = new JsonNewObjectBuilder((JObject?)jsonData, insertValueList);
                totalInsertCount = builder.InsertCount;
                try
                {
                    builder.BuildJsonResult();
                    jsonData = builder.GetResult();
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                }
                valueInserts = builder.InsertCountSuccessful;
            }

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

        private IEnumerable<List<string>> readDataPerformant(string fileName)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                if (workbookPart != null)
                {
                    using (OpenXmlReader oxr = OpenXmlReader.Create(worksheetPart))
                    {
                        IEnumerable<SharedStringItem> sharedStrings = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();
                        while (oxr.Read())
                        {
                            if (oxr.ElementType == typeof(Row))
                            {
                                oxr.ReadFirstChild();
                                List<string> rowData = new List<string>();
                                do
                                {
                                    if (oxr.ElementType == typeof(Cell))
                                    {
                                        Cell c = (Cell)oxr.LoadCurrentElement();
                                        string cellValue;
                                        if (c.DataType != null && c.DataType == CellValues.SharedString)
                                        {
                                            SharedStringItem ssi = sharedStrings.ElementAt(int.Parse(c.CellValue.InnerText));
                                            cellValue = ssi.Text.Text;
                                        }
                                        else
                                        {
                                            // Fixes weird number issues with data being formatted in an odd way from Excel
                                            if (c.CellValue != null)
                                            {
                                                string innerText = c.CellValue.InnerText;
                                                if (double.TryParse(innerText, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double numVal))
                                                {
                                                    cellValue = numVal.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
                                                }
                                                else
                                                {
                                                    cellValue = innerText;
                                                }
                                            }
                                            else
                                            {
                                                cellValue = "";
                                            }

                                        }
                                        rowData.Add(cellValue);
                                    }
                                }
                                while (oxr.ReadNextSibling());
                                yield return rowData;
                            }
                        }
                    }
                }
            }
        }
    }
}
