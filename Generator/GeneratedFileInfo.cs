namespace Generator
{
    public class GeneratedFileInfo
    {
        public string FullFileName { set; get; }

        public string SourceCode { get; set; }

        public GeneratedFileInfo(string fileName, string code)
        {
            FullFileName = fileName;
            SourceCode = code;
        }
    }
}
