// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using System.Xml;
using System.Xml.Xsl;


namespace cCoder.Workflow.Activities.Activities.Transformation;

public class XslActivity : TransformationActivity<string, string>
{

    public string Xslt { get; set; }

    public override async Task ExecuteAsync()
    {
        // build the transform
        XslCompiledTransform t = new();
        t.Load(stylesheet:new XmlTextReader(new StringReader(Xslt)));

        using XmlTextReader input = new(new MemoryStream(Encoding.UTF8.GetBytes(s:Source)));
        using XmlTextWriter output = new(new MemoryStream(), Encoding.UTF8);
        t.Transform(input:input, results:output);
        _ = output.BaseStream.Seek(offset:0, origin:SeekOrigin.Begin);
        using StreamReader reader = new(output.BaseStream);
        Result = await reader.ReadToEndAsync();
    }
}