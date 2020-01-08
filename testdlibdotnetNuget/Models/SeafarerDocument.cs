namespace testdlibdotnetNuget
{
    public class SeafarerDocument
    {
        public string documentId { get; set; }
        public byte[] document { get; set; }

        public override string ToString()
        {
            return "Document["+documentId + " " + document+"]";
        }
    }
}