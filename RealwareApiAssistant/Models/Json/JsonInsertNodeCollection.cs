using Newtonsoft.Json.Linq;
using RealwareApiAssistant.Models.Api.Request;

namespace RealwareApiAssistant.Models.Json
{
    public class JsonInsertNodeCollection
    {
        public string FullPath { get; set; }
        public string FullPathFormatted { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int? ArrayIndex { get; set; }
        public JsonInsertNodeCollection ParentNode { get; set; }
        public JsonInsertNodeCollection ChildNode { get; set; }
        public List<ApiValue> Values { get; set; }
        public bool HasParent => ParentNode != null;
        public bool HasChild => ChildNode != null;
        public bool IsArray => ArrayIndex.HasValue;
        public bool HasValues => Values?.Count > 0;



        public static JsonInsertNodeCollection BuildNodeFromPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return new JsonInsertNodeCollection
                {
                    Name = Helpers.Json.GetCleanName(fullPath),
                    Path = fullPath,
                    FullPath = fullPath,
                    FullPathFormatted = Helpers.Json.GetCleanName(fullPath)
                };

            var node = new JsonInsertNodeCollection()
            {
                FullPath = fullPath,
                FullPathFormatted = Helpers.Json.GetCleanName(fullPath)
            };

            int i = 0;
            var paths = fullPath.Split(".");
            var selectedNode = node;
            string completePath = "";
            while (i < paths.Length)
            {
                var name = Helpers.Json.GetCleanName(paths[i]);
                var path = paths[i].ToString();

                if (Helpers.Json.IsArrayPath(path))
                    selectedNode.ArrayIndex = Helpers.Json.GetPathArrayIndex(path);


                selectedNode.Name = Helpers.Json.GetCleanName(path);
                completePath += (i > 0 ? "." : "") + selectedNode.Name;
                selectedNode.Path = completePath;

                if (i + 1 < paths.Length)
                {
                    var newNode = new JsonInsertNodeCollection
                    {
                        Name = name,
                        FullPath = fullPath,
                        ParentNode = selectedNode
                    };
                    selectedNode.ChildNode = newNode;
                    selectedNode = newNode;
                }

                i++;
            }

            

            return node;
        }

        
    }
}
