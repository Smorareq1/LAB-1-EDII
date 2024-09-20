
namespace LAB_1_EDII;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            ArbolB tree = new ArbolB(5);
            GestorDeArchivos gestorPrincipal = new GestorDeArchivos(tree);
            
            string archivoResultados = "C:\\Users\\smora\\Downloads\\resultados_busquedas.txt";

            // C:\Users\smora\Downloads\100Klab01\100Klab01_books.csv - C:\Users\smora\Downloads\100Klab01\100Klab01_search.csv
            // C:\Users\smora\Downloads\200Klab01\200Klab01_books.csv - C:\Users\smora\Downloads\200Klab01\200Klab01_search.csv
            
            
            Console.WriteLine("Ingrese la ruta del archivo: ");
            string filepath = "C:\\Users\\smora\\Downloads\\200Klab01\\200Klab01_books.csv";
            Console.WriteLine("Ingrese el archivo de busqueda: ");
            string searchFile = "C:\\Users\\smora\\Downloads\\200Klab01\\200Klab01_search.csv";

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

