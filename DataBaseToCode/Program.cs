using DataBaseToCode;
using Newtonsoft.Json;
using SistemaRI.AccesoADatos;


List<Table> tablesCorrectas = Load.JsonWithTables();
List<Table> tablesIncorrectas = Load.LoadJsonTablasIncorrectas();
string tablesName = "tropas";
var table = tablesCorrectas.FirstOrDefault(x => x.Name.ToLower() == tablesName);

var table2 = tablesIncorrectas.FirstOrDefault(x => x.Name.ToLower() == tablesName);


Console.WriteLine(table);


//TODO: MOVERLO A OTRO ARCHIVO

//DatosConexion.Conexiones.Add(ContextosBDEnum.principal, "Data Source=10.10.0.84:1521/sanfranc;User Id=frigo;Password=frigo;Validate Connection=true;");
//Table_ORA table_ORA = new Table_ORA();
//List<string> conexiones = new List<string>()
//{
//    "Data Source=10.10.0.84:1521/sanfranc;User Id=frigo;Password=frigo;Validate Connection=true;",

//    "Data Source=10.10.0.84:1521/bustos;User Id=frigo;Password=frigo;Validate Connection=true;",

//    "Data Source=10.10.0.84:1521/morteros;User Id=frigo;Password=frigo;Validate Connection=true;",

//    "Data Source=10.10.0.122:1521/novara;User Id=frigo;Password=frigo;Validate Connection=true;",

//    "Data Source=10.10.0.211:1521/frideza;User Id=frigo;Password=frigo;Validate Connection=true;",

//    "Data Source=10.10.0.211:1521/sudeste;User Id=frigo;Password=frigo;Validate Connection=true;",

//};

//List<List<Table>> basesDatos = new List<List<Table>>();
//foreach (var conexionString in conexiones)
//{
//    IConexion conexion = new ConORAClient(conexionString, ContextosBDEnum.mant);

//    var result = table_ORA.BuscarTodos(conexion);

//    if (result.ok && result.obj is not null)
//    {
//        basesDatos.Add(result.obj);
//    }
//    else
//    {

//    }
//}


//List<Table> tablesJSON = FindCommonTables(basesDatos);
//List<Table> tableNoEncontrada = FindDifTables(basesDatos);
//var result2 = 123;
//var Json = JsonConvert.SerializeObject(tablesJSON);
//var Json2 = JsonConvert.SerializeObject(tableNoEncontrada);
//Console.WriteLine(Json);
//for (int i = 0; i < 20; i++)
//{
//    Console.WriteLine(123);
//}

//Console.WriteLine(Json2);


//List<Table> FindCommonTables(List<List<Table>> databases)
//{
//    var tableGroups = databases
//        .SelectMany(db => db)
//        .GroupBy(t => t.Name)
//        .Where(g => g.Count() == databases.Count);

//    var commonTables = new List<Table>();
//    foreach (var tableGroup in tableGroups)
//    {
//        var commonColumns = tableGroup
//            .SelectMany(t => t.Columns)
//            .GroupBy(c => c.Name)
//            .Where(g => g.Count() == databases.Count)
//            .Select(g =>
//            {
//                var firstColumn = g.First();
//                return firstColumn;
//            })
//            .ToList();

//        if (commonColumns.Any())
//        {
//            commonTables.Add(new Table
//            {
//                Name = tableGroup.Key,
//                Columns = commonColumns
//            });
//        }
//    }

//    return commonTables;
    
//}

//List<Table> FindDifTables(List<List<Table>> databases)
//{
//    var tableGroups = databases
//        .SelectMany(db => db)
//        .GroupBy(t => t.Name)
//        .Where(g => g.Count() == databases.Count);

//    var commonTables = new List<Table>();
//    foreach (var tableGroup in tableGroups)
//    {
//        var commonColumns = tableGroup
//            .SelectMany(t => t.Columns)
//            .GroupBy(c => c.Name)
//            .Where(g => g.Count() != databases.Count)
//            .Select(g =>
//            {
//                var firstColumn = g.First();
//                return firstColumn;
//            })
//            .ToList();

//        if (commonColumns.Any())
//        {
//            commonTables.Add(new Table
//            {
//                Name = tableGroup.Key,
//                Columns = commonColumns
//            });
//        }
//    }

//    return commonTables;
//    foreach (var tableGroup in tableGroups)
//    {
//        var commonColumns = tableGroup
//            .SelectMany(t => t.Columns.Select(c => c.Name))
//            .GroupBy(c => c)
//            .Where(g => g.Count() == databases.Count)
//            .Select(g => new Column { Name = g.Key })
//            .ToList();

//        if (commonColumns.Any())
//        {
//            commonTables.Add(new Table
//            {
//                Name = tableGroup.Key,
//                Columns = commonColumns
//            });
//        }
//    }

//    return commonTables;
//}