
namespace LAB_1_EDII;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            ArbolB tree = new ArbolB(5);
            GestorDeArchivos gestorPrincipal = new GestorDeArchivos(tree);

            //C:\Users\smora\Downloads\lab01_books.csv - C:\Users\smora\Downloads\lab01_search.csv
            
            Console.WriteLine("Ingrese la ruta del archivo: ");
            string filepath = Console.ReadLine();
            Console.WriteLine("Ingrese el archivo de busqueda: ");
            string searchFile = Console.ReadLine();

            // Verificar si el archivo existe
            if (!System.IO.File.Exists(filepath) || !System.IO.File.Exists(searchFile))
            {
                Console.WriteLine($"El archivo {filepath} o {searchFile} no existe.");
                return;
            }

            gestorPrincipal.ProcessLogFile(filepath);
            gestorPrincipal.ProcessLogFile(searchFile);
            gestorPrincipal.PrintSearchResults();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }

        Console.ReadLine();
    }
}

