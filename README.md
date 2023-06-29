# RealWare Api Assistant
Utility for helping make calls to the RealWare Api using scripts. The scripting language is a unique and simple way to make updates to accounts, permits, sales, etc. in RealWare. This can be used to manually run or schedule a data import or update from an excel file to RealWare data.

## Creating a Script

### Example Script
```json
{
  "ApiSettings": {
    "ApiPath": "http://<server name>/<server instance>/EncompassAPI",
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
  "ValueInserts":[],

  "CustomLogFileLocation": null,
  "SkipConfirmations": false,
  "SkipWarningPrompts": false,
  "RetryImmediatelyAfterBadRequest": true,
  "ForceExcelNULLValues": true,
  "ExportJsonSettings":{
	  "ExportJsonFiles": true,
	  "FilePath": null
  },
  "Threads": 6,
  "RetryCount": 3
}
```
### Understanding the Parts
```json
"ApiSettings": {
    "ApiPath": "http://<server name>/<server instance>/EncompassAPI",
    "Token": "<insert api token here>"
  }
```
Connection settings for talking to the api. 

ApiPath - The url of the API.
- Example: http://Server1/Test/EncompassAPI

Token - Unique token. This is different per user and can be found using the API or in Realware system maintenance.

```json
"ApiOperation": "/api/improvements/{accountNo}/{impNo}/{taxYear}",
  "Method": "PUT",
```

The ApiOperation is the url of the specified api call you are trying to make. This can be copy/pasted directly from the swagger page.

The Method can be POST, PUT, or DELETE.
- POST: Generally used for Creating a new object. Examples in RealWare would be creating a new permit or Sale.
- PUT: Generally used for Updating an existing object. An example would be changing data fields or adding additional land abstracts, notes, etc.
- DELETE: Used for Deleting an object. This will delete an Account, Sale, Permit, etc.

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
* Static values (StaticValue) should be used when the value will always be the same and is defined in the script.
* Column values (ColumnValue) should be used when the excel row defines the value. The header row in excel should match the ColumnValue string. This field is case-sensitive.

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
* (Optional) Use ExcelFromColumn to specify a column name in the excel file to restrict changes that only match the specified row/column value. This field is case-sensitive.

ToValue/ExcelToColumn
* Use ToValue to specify a static value to change the realware property value to.
* Use ExcelToColumn to specify a column name in the excel file to change too the specified row/column value. This field is case-sensitive.

```json
"CustomLogFileLocation": null,
  "SkipConfirmations": false,
  "SkipWarningPrompts": false,
  "RetryImmediatelyAfterBadRequest": true,
  "ForceExcelNULLValues": true,
```

CustomLogFileLocation - Set a custom location to save the log file to.

SkipConfirmations - Skips any "Press Y to continue" prompts.

SkipWarningPrompts - Skips any warning prompts that would halt the script to warn the user.

RetryImmediatelyAfterBadRequest - If a GET request fails before the actual PUT or POST, the GET request will retry for one additional attempt after 1000 milliseconds.

ForceExcelNULLValues - Allows the use of NULL. It will convert the value ```NULL``` into a ```null``` database value.


```json
  "ExportJsonSettings":{
	  "ExportJsonFiles": true,
	  "FilePath": null
  }
```

ExportJsonFiles - Enables file creation of every Json object to .json files in a local folder.

FilePath - Leave blank to create a Json folder in the script location. Otherwise, specify a directory.

```json
  "Threads": 6,
  "RetryCount": 3
```

Threads - Creates specified number of unique threads to complete the work. If not specified, the default is 1.
- It is recommended to review your CPU logical processors before setting a number.

RetryCount - The number of times the API call will retry if a failure occurs. If not specified, the default is 0.
- Note: It is recommended to set this to 0 if you are doing multiple requests on the same identifier (for example, you have 3 different excel rows for the account 112233)
