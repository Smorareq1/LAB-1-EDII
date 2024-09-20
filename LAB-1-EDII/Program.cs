
namespace LAB_1_EDII;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            // C:\Users\smora\Downloads\100Klab01\100Klab01_books.csv - C:\Users\smora\Downloads\100Klab01\100Klab01_search.csv
            
            ArbolB tree = new ArbolB(30);
            GestorDeArchivos gestorPrincipal = new GestorDeArchivos(tree);
            
            string archivoResultados = "C:\\Users\\smora\\Downloads\\resultados_busquedas.txt";
            
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

            gestorPrincipal.ProcesarArchivoInsertar(filepath);
            Console.WriteLine("Procesando archivo de busqueda");
            gestorPrincipal.ProcesarArchivoBusqueda(searchFile, archivoResultados);
            Console.WriteLine("Archivo de salida genrado en descargas con el nombre de resultados_busquedas.txt");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }

        Console.ReadLine();
    }
}

