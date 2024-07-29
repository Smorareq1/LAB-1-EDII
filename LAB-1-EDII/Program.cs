
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

            string filepath = "C:\\Users\\smora\\Downloads\\Csv\\logfile.csv";

            // Verificar si el archivo existe
            if (!System.IO.File.Exists(filepath))
            {
                Console.WriteLine($"El archivo {filepath} no existe.");
                return;
            }

            gestorPrincipal.ProcessLogFile(filepath);

            Console.WriteLine("Árbol completo: ");
            tree.PrintTree();

            // Example usage: Search for a book by name and print results
            var searchResults = tree.SearchByName("Cien Años de Soledad");
            Console.WriteLine("Resultados de la búsqueda:");
            tree.PrintSearchResults(searchResults);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }

        Console.ReadLine();
    }

    private void pruebasConsola()
    {
        BPlusTree tree = new BPlusTree(3);

        // Insertar algunos libros
        tree.Insert(new Book { Isbn = "1234567890", Name = "Cien Años de Soledad", Author = "Gabriel Garcia Marquez", Category = "Ficción", Price = "20.00", Quantity = "10" });
        tree.Insert( new Book { Isbn = "0987654321", Name = "El Principito", Author = "Antoine de Saint-Exupéry", Category = "Ficción", Price = "15.00", Quantity = "5" });
        tree.Insert( new Book { Isbn = "1122334455", Name = "Don Quijote de la Mancha", Author = "Miguel de Cervantes", Category = "Clásicos", Price = "25.00", Quantity = "7" });

        // Buscar por nombre
        
        Console.WriteLine("Resultados de la búsqueda:");
        var searchResults = tree.SearchByName("El Principito");
        //tree.PrintSearchResults(searchResults);

        // Actualizar un libro
        //tree.Patch("0987654321", new Dictionary<string, string> { { "Price", "18.00" } });

        // Buscar de nuevo después del parche
        searchResults = tree.SearchByName("El Principito");
        Console.WriteLine("Resultados de la búsqueda después del parche:");
        //tree.PrintSearchResults(searchResults);
        
        tree.Delete("1234567890");

        // Imprimir el árbol completo
        Console.WriteLine("Árbol completo:");
        //tree.PrintTree();

        Console.ReadLine();
    }
}