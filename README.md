# RealWareApiAssistant
Utility for helping make calls to the RealWare Api using scripts. The scripting language is a unique and simple way to make updates to accounts, permits, sales, etc. in RealWare.

## Creating a Script

### Example Script
```json
{
  "ApiSettings": {
    "ApiPath": "http://asrwiis/Test/EncompassAPI",
    "Token": "<insert api token here>"
  },
  "ApiOperation": "/api/improvements/{accountNo}/{impNo}/{taxYear}",
  "Method": "PUT",
  
  "ExcelFile": "script_1_data.xlsx",
  "IdColumns": [
    {
      "IdName": "{accountNo}",
      "ColumnValue": "ACCOUNTNO"
    },
	{
      "IdName": "{impNo}",
      "ColumnValue": "IMPNO"
    },
    {
      "IdName": "{taxYear}",
      "StaticValue": "2022"
    }
  ],
  "ValueChanges": [
    {
      "Path": "",
      "RealWareColumn": "ApproachType",
      "ToValue": "Cost"
    },
	{
      "Path": "",
      "RealWareColumn": "CostMethod",
      "ToValue": "RCNLD"
    },
	{
      "Path": "",
      "RealWareColumn": "Appraiser",
      "ToValue": "553"
    },
    {
	  "Path": "",
	  "RealWareColumn": "AppraisalDate",
	  "ToValue": "6/7/2022"
    }
  ],

  "CustomLogFileLocation": null,
  "SkipConfirmations": false,
  "SkipWarningPrompts": false,
  "RetryImmediatelyAfterBadRequest": true,
  "ForceExcelNULLValues": true
}
```
### Understanding the Parts
```json
"ApiSettings": {
    "ApiPath": "http://asrwiis/Test/EncompassAPI",
    "Token": "<insert api token here>"
  }
```
Connection settings for talking to the api. The ApiPath refers to the url of the API. The token is different per user and can be found using the API or in Realware system maintenance.

```json
"ApiOperation": "/api/improvements/{accountNo}/{impNo}/{taxYear}",
  "Method": "PUT",
```

The ApiOperation is the url of the specified api call you are trying to make. This can be copy/pasted directly from the swagger page.

The Method can be GET, POST, PUT, or DELETE. Currently, only PUT and DELETE are supported.

```json
"ExcelFile": "script_1_data.xlsx",
```

The ExcelFile can be a local file or direct path. This refers to the input file to use for collecting the script paramter data.

```json
"IdColumns": [
    {
      "IdName": "{accountNo}",
      "ColumnValue": "ACCOUNTNO"
    },
	{
      "IdName": "{impNo}",
      "ColumnValue": "IMPNO"
    },
    {
      "IdName": "{taxYear}",
      "StaticValue": "2022"
    }
  ],
```
The IdColumns define the variables in the ApiOperation. Each variable will need an associated IdColumn value to define what it is. Two types of values can be defined here, Column and Static values.
* Static values (StaticValue) should be used when the value will always be the same
* Column values (ColumnValue) should be used when the excel row defines the value. The header row in excel should match the ColumnValue string.

```json
  "ValueChanges": [
    {
      "Path": "",
      "RealWareColumn": "ApproachType",
      "ToValue": "Cost"
    },
	{
      "Path": "",
      "RealWareColumn": "CostMethod",
      "ToValue": "RCNLD"
    },
	{
      "Path": "",
      "RealWareColumn": "Appraiser",
      "ToValue": "553"
    },
    {
	  "Path": "",
	  "RealWareColumn": "AppraisalDate",
	  "ToValue": "6/7/2022"
    }
  ],
```

ValueChanges define what values should be updated in a PUT.

Path refers to the node path of the property. For example, an empty path is signified by "". It will only change properties on the basic level. When changing children objects, use the name of that object followed by the array (if applicable). As an example, making changes to all occupancies would be written as "Occupancies[\*]" in the path. For only the first item in the list, use Occupancies[0].

RealwareColumn refers to the property name of the realware value. Note: custom realware variables will be their original name (ex: AcctOD0).

FromValue/ExcelFromColumn
* (Optional) Use FromValue to specify a static value to change from. Only properties with the value specified here will change on script execution.
* (Optional) Use ExcelFromColumn to specify a column name in the excel file to restrict changes that only match the specified row/column value.

ToValue/ExcelToColumn
* Use ToValue to specify a static value to change the realware property value to.
* Use ExcelToColumn to specify a column name in the excel file to change too the specified row/column value.

```json
"CustomLogFileLocation": null,
  "SkipConfirmations": false,
  "SkipWarningPrompts": false,
  "RetryImmediatelyAfterBadRequest": true,
  "ForceExcelNULLValues": true
```

CustomLogFileLocation - Set a custom location to save the log file to.

SkipConfirmations - Skips any "Press Y to continue" prompts

SkipWarningPrompts - "Skips any warning prompts that would halt the script to warn the user"

RetryImmediatelyAfterBadRequest - If a request fails, retries for one time after 1000 milliseconds.

ForceExcelNULLValues - Allows the use of NULL. It will convert the value ```NULL``` into a ```null``` database value.
