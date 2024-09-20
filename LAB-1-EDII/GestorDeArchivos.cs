namespace LAB_1_EDII;

using System.Text;
using Newtonsoft.Json;

public class GestorDeArchivos
{
    private ArbolB tree;
    public GestorDeArchivos(ArbolB tree)
    {
        this.tree = tree;
    }
    
    //////////////// ARCHIVO PARA INSERTAR ////////////////////

    public void ProcesarArchivoInsertar(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            ProcesarLineaInsertar(line);
        }
    }
    
    private void ProcesarLineaInsertar(string linea)
    {
        if (linea.StartsWith("INSERT;"))
        {
            var part = linea.Replace("INSERT;", "").Trim();
            var book = JsonConvert.DeserializeObject<Book>(part);

            if (book == null || string.IsNullOrEmpty(book.Name) || string.IsNullOrEmpty(book.Isbn))
            {
                //Console.WriteLine("Error: Libro deserializado es nulo o tiene datos incompletos.");
                return;
            }

            // Insert into dictionaries
            tree.Insert(book);
        }
        else if (linea.StartsWith("PATCH;"))
        {
            var part = linea.Replace("PATCH;", "").Trim();
            var patchData = JsonConvert.DeserializeObject<Dictionary<string, object>>(part);

            tree.UpdateBookFromPatchData(patchData);
            
        }
        else if (linea.StartsWith("DELETE;"))
        {
            var part = linea.Replace("DELETE;", "").Trim();
            var deleteData = JsonConvert.DeserializeObject<Dictionary<string, string>>(part);

            
            var isbn = deleteData["isbn"];
            tree.Delete(isbn);
            
        }
    }
    
    //////////////// ARCHIVO PARA BUSCAR ////////////////////
    public void ProcesarArchivoBusqueda(string filePath, string outputFilePath)
    {
        var lines = File.ReadAllLines(filePath);

        // Ensure the output TXT file is created and ready for writing
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var line in lines)
            {
                ProcesarLineaBusqueda(line, writer);
            }
        }
    }

    private void ProcesarLineaBusqueda(string linea, StreamWriter writer)
    {
        if (linea.StartsWith("SEARCH;"))
        {
            var part = linea.Replace("SEARCH;", "").Trim();
            var searchData = JsonConvert.DeserializeObject<Dictionary<string, string>>(part);

            if (searchData.ContainsKey("name"))
            {
                var name = searchData["name"];
                tree.BuscarPorNombre(name, writer);
            }
        }
    }

    
    
    
}
