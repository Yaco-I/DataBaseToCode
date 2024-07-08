using DataBaseToCode;
using System.Text;

List<string> tableString = LoadTxt.ReadFile("D:\\Trabajo\\Recurosos Informaticos\\DataBaseToCode\\DataBaseToCode\\DataBaseTable.txt");

List<Table> tables = LoadTxt.CreateTables(tableString);
var table = tables.FirstOrDefault(x => x.Name.ToUpper() == "ROMANEOS");
Console.WriteLine();
Console.WriteLine("=================================== Entidad ==========================================");
Console.WriteLine();

//Console.WriteLine(table.CreateTable());


Console.WriteLine();
Console.WriteLine("=================================== Entidad Busqueda ==========================================");
Console.WriteLine();

//Console.WriteLine(table.CreateClassSearch());



//Console.WriteLine(table.CreateEntityLoad());

Console.WriteLine();
Console.WriteLine("=================================== ORA ==========================================");
Console.WriteLine();
Console.WriteLine(table.CreateORAClass());

Console.WriteLine();
Console.WriteLine("=================================== BO ==========================================");
Console.WriteLine();

//Console.WriteLine(table.CreateBOClass());

Console.WriteLine();
Console.WriteLine("=================================== Controller ==========================================");
Console.WriteLine();

//Console.WriteLine(table.CreateController());



//List<string> ReadFile(string filePath)
//{
//    List<string> list = new();
//    try
//    {
//        //string filePath = "D:\\Trabajo\\Recurosos Informaticos\\LicenseConvertExcelToDatabase\\LicenseConvertExcelToDatabase\\Administracion.txt";

//        if (File.Exists(filePath))
//        {
//            using (StreamReader reader = new StreamReader(filePath))
//            {
//                string line;

//                while ((line = reader.ReadLine()) != null)
//                {
//                    list.Add(line);
//                }
//            }
//        }
//        else
//        {
//            Console.WriteLine("The file is not exist.");
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex.Message);
//    }
//    return list;
//}


