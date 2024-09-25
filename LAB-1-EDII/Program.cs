
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

            try
            {
                    // Crear la ruta del archivo de resultados en la misma carpeta que el archivo de inserción
                    string carpetaResultados = System.IO.Path.GetDirectoryName(filepath);
                    string archivoResultados = System.IO.Path.Combine(carpetaResultados, "resultados.txt");
                    // Crear el archivo de resultados si no existe
                    if (!System.IO.File.Exists(archivoResultados))
                    {
                        System.IO.File.Create(archivoResultados).Close();
                    }

                    
                    Console.WriteLine("Procesando archivos...");
                    gestorPrincipal.ProcesarArchivoInsertar(filepath);
                    Console.WriteLine("Procesando archivo de busqueda");
                    gestorPrincipal.ProcesarArchivoBusqueda(searchFile, archivoResultados);
                    Console.WriteLine($"Archivo de salida generado en {archivoResultados}");
                    Console.WriteLine("Listo :) ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }

        Console.ReadLine();
    }
}

