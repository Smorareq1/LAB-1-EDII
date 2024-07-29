namespace LAB_1_EDII;

using Newtonsoft.Json;

public class GestorDeArchivos
{
    private BPlusTree tree;

    public GestorDeArchivos(BPlusTree tree)
    {
        this.tree = tree;
    }

    public void ProcessLogFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            ProcessLine(line);
        }
    }

    private void ProcessLine(string line)
    {
        var parts = line.Split(new[] { "INSERT;", "PATCH;", "DELETE;" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (line.Contains("INSERT;"))
            {
                var book = JsonConvert.DeserializeObject<Book>(part);
                tree.Insert(book);
            }
            else if (line.Contains("PATCH;"))
            {
                var patchData = JsonConvert.DeserializeObject<Dictionary<string, object>>(part);
                tree.UpdateBookFromPatchData(patchData);
            }

            else if (line.Contains("DELETE;"))
            {
                var deleteData = JsonConvert.DeserializeObject<Dictionary<string, string>>(part);
                if (deleteData.ContainsKey("isbn"))
                {
                    tree.Delete(deleteData["isbn"]);
                }
            }
        }
    }
}