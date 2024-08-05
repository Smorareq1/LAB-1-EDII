namespace LAB_1_EDII;

using Newtonsoft.Json;

public class GestorDeArchivos
{
    private BPlusTree tree;
    private List<Book> searchResults = new List<Book>();

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
        
        Console.WriteLine("Resultados de busquedas: ");
        if (searchResults.Count != 0)
        {
            tree.PrintSearchResults(searchResults);
        }
        else
        {
            Console.WriteLine("No se encontraron resultados.");
        }
    }

    private void ProcessLine(string line)
    {
        var parts = line.Split(new[] { "INSERT;", "PATCH;", "DELETE;", "SEARCH;" }, StringSplitOptions.RemoveEmptyEntries);
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

            else if (line.Contains("SEARCH;"))
            {
                var searchData = JsonConvert.DeserializeObject<Dictionary<string, string>>(part);

                if (searchData.ContainsKey("isbn"))
                {
                    searchResults.Add(tree.SearchByIsbn(searchData["isbn"]));
                }
                else if(searchData.ContainsKey("name"))
                {
                    searchResults.AddRange(tree.SearchByName(searchData["name"]));
                }
                
            }
        }
    }
}