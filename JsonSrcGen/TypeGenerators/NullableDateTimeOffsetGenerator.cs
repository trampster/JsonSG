using System;
using System.Text;

namespace JsonSrcGen.TypeGenerators
{
    public class NullalbeDateTimeOffsetGenerator : IJsonGenerator
    {
        public string TypeName => "DateTimeOffset?";

        public void GenerateFromJson(CodeBuilder codeBuilder, int indentLevel, JsonType type, Func<string, string> valueSetter, string valueGetter)
        {
            string propertyValueName = $"property{UniqueNumberGenerator.UniqueNumber}Value";
            codeBuilder.AppendLine(indentLevel, $"json = json.ReadNullableDateTimeOffset(out DateTimeOffset? {propertyValueName});");
            codeBuilder.AppendLine(indentLevel, valueSetter(propertyValueName));
        }

        public void GenerateToJson(CodeBuilder codeBuilder, int indentLevel, StringBuilder appendBuilder, JsonType type, string valueGetter)
        {
            codeBuilder.MakeAppend(indentLevel, appendBuilder);
            codeBuilder.AppendLine(indentLevel, $"if({valueGetter} == null)");
            codeBuilder.AppendLine(indentLevel, "{");
            codeBuilder.AppendLine(indentLevel+1, $"builder.Append(\"null\");");
            codeBuilder.AppendLine(indentLevel, "}");
            codeBuilder.AppendLine(indentLevel, "else");
            codeBuilder.AppendLine(indentLevel, "{");
            codeBuilder.AppendLine(indentLevel+1, $"builder.AppendDateTimeOffset({valueGetter}.Value);");
            codeBuilder.AppendLine(indentLevel, "}");
        }
        
        public CodeBuilder ClassLevelBuilder => null;
    }
}