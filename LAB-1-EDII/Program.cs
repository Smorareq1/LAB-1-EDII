
namespace LAB_1_EDII;
class Program
{
    static void Main(string[] args)
    {
        pruebasJson();
    }

    private static void pruebasJson()
    {
        try
        {
            BPlusTree tree = new BPlusTree(5);
            GestorDeArchivos gestorPrincipal = new GestorDeArchivos(tree);

            //datos_10000
            string filepath = "C:\\Users\\smora\\Downloads\\lab01_books.csv";

            // Verificar si el archivo existe
            if (!System.IO.File.Exists(filepath))
            {
                Console.WriteLine($"El archivo {filepath} no existe.");
                return;
            }

            gestorPrincipal.ProcessLogFile(filepath);

            //Console.WriteLine("Árbol completo: ");
            //tree.PrintTree();

            // Example usage: Search for a book by name and print results
            //var searchResults = tree.SearchByName("rLf XhSfdpc");
            //Console.WriteLine("Resultados de la búsqueda de ejemplo:");
            //tree.PrintSearchResults(searchResults);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }

        Console.ReadLine();
    }
    
}