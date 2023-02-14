using Newtonsoft.Json.Linq;
using RealwareApiAssistant.Models.Api.Request;
using RealwareApiAssistant.Models.Json;

namespace RealwareApiAssistant.Builders
{
    public class JsonNewObjectBuilder
    {
        public JToken? GetResult() => jsonData;
        public string GetResultAsString() => jsonData.ToString();
        public List<JsonInsertNodeCollection> InsertNodes { get; private set; }
        public int InsertCount { get; private set; }
        public int InsertCountSuccessful { get; private set; }
        JToken? jsonData;

        public JsonNewObjectBuilder(JToken? jsonData, List<ApiValue> insertValues)
        {
            this.jsonData = jsonData;

            if (this.jsonData == null)
                this.jsonData = new JObject();

            initializeValues(insertValues);

            InsertCountSuccessful = 0;
        }

        public JsonNewObjectBuilder BuildJsonResult()
        {
            foreach (var node in
                InsertNodes
                .OrderBy(x => x.FullPath)
                .ThenBy(x => x.ArrayIndex)
                )
            {
                // If it's empty, make sure we create the first object as the object
                // with values. OW, addon to the originally created node.
                if (string.IsNullOrEmpty(node.FullPath))
                    jsonData = (JObject)getJsonInsertObjectFromValues(node.Values);
                else
                    jsonData = buildInsertNode(jsonData, node);
            }
            return this;
        }

        private void initializeValues(List<ApiValue> values)
        {
            InsertNodes = new List<JsonInsertNodeCollection>();

            var insertPathList = values
                .FindAll(x => x.IsNew)
                .Select(x => x.Path)
                .Distinct()
                .OrderBy(x => x);

            InsertCount = insertPathList.Count();

            foreach (var path in insertPathList)
            {
                var collection = JsonInsertNodeCollection.BuildNodeFromPath(path);

                var insertValues = values.FindAll(x => x.Path.Equals(path));
                var current = collection;
                while (current != null)
                {
                    current.Values = values.FindAll(x => x.Path.Equals(path));
                    current = current.ChildNode;
                }

                InsertNodes.Add(collection);
            }
        }

        private JToken? buildInsertNode(JToken? json, JsonInsertNodeCollection node)
        {
            JToken? previousToken = json;
            JToken? currentToken = json;
            bool isArray = false;
            var shortPaths = node.FullPath.Split('.');

            foreach (var shortPath in shortPaths)
            {
                var cleanPath = Helpers.Json.GetCleanName(shortPath);
                var pathArrayIndex = Helpers.Json.GetPathArrayIndex(shortPath);
                currentToken = currentToken.SelectToken(cleanPath);
                isArray = Helpers.Json.IsArrayPath(shortPath);

                // Fixes issue with going through an already
                // established array and not creating it.
                //
                // Note: This could definitely be improved
                //
                if(isArray 
                    && pathArrayIndex >= 0
                    && shortPath != shortPaths?.Last())
                {
                    var array = currentToken as JArray;
                    if (array != null)
                        currentToken = array[pathArrayIndex];
                }

                // If current path is not found, create it
                if (currentToken == null)
                {
                    currentToken = previousToken;

                    createPathNode(currentToken, cleanPath, isArray);

                    currentToken = currentToken.SelectToken(cleanPath);
                }
                // If the path exists, make sure we check for values and select the
                // most recently added one
                else
                {
                    //if (currentToken.Type == JTokenType.Array)
                    //{
                    //    if (currentToken.HasValues)
                    //    {
                    //        if(((JArray)currentToken).Count == pathArrayIndex + 2)// 0 should be starting index not 1? Maybe need to check for this better
                    //        {
                    //            currentToken = currentToken.Last();
                    //        }
                    //    }
                    //}
                }

                previousToken = currentToken;
            }

            // Insert the values into the selected node path
            var newJsonObject = getJsonInsertObjectFromValues(node.Values);

            if (isArray)
                ((JArray)currentToken).Add(newJsonObject);
            else
                ((JObject)currentToken).Add(newJsonObject);
            InsertCountSuccessful++;

            return json;
        }

        private JToken createPathNode(JToken? jToken, string path, bool isArray)
        {
            if (isArray)
                ((JObject)jToken).Add(new JProperty(path, new JArray()));
            else
                ((JObject)jToken).Add(new JProperty(path, null));

            return jToken;
        }

        private object getJsonInsertObjectFromValues(List<ApiValue> apiValues)
        {
            var obj = new JObject();
            foreach (var value in apiValues)
                obj.Add(new JProperty(value.RealwarePropertyName, value.ToValue));
            return obj;
        }
    }
}
