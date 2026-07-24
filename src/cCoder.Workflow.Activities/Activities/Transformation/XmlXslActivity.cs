// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using System.Xml;
using System.Xml.Xsl;
using cCoder.Workflow.Activities.Models;
using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Activities.Transformation;

public class XmlXslActivity<TResult> : TransformationActivity<string, TResult>
{

    public string Xslt { get; set; }

    [JsonIgnore]
    public dynamic[] FlattenedResult => cCoder.Workflow.Activities.Support.Data.Flatten(source:Result);

    public override async Task ExecuteAsync()
    {
        if (Source == null || Source.Length < 2)
        {
            Log(level:WorkflowLogLevel.Warning, message:"   Data source appears to be empty, nothing to transform.");
            return;
        }

        if (Xslt != null && Xslt.Length > 2)
        {
            XslCompiledTransform t = new();
            t.Load(stylesheet:new XmlTextReader(new StringReader(Xslt)));

            using XmlTextReader input = new(new MemoryStream(Encoding.UTF8.GetBytes(s:Source)));
            using XmlTextWriter output = new(new MemoryStream(), Encoding.UTF8);
            t.Transform(input:input, results:output);
            _ = output.BaseStream.Seek(offset:0, origin:SeekOrigin.Begin);
            using StreamReader reader = new(output.BaseStream);
            Result = cCoder.Workflow.Activities.Support.Data.ParseXml<TResult>(data:await reader.ReadToEndAsync());
        }
        else
        {
            Log(level:WorkflowLogLevel.Warning, message:"   No Xsl tranform has been provided, to run on the data.");
        }
    }
}