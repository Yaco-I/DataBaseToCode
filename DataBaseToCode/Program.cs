using System.Text;

List<string> tableString = ReadFile("C:\\Git\\DataBaseToCode\\DataBaseToCode\\DataBaseTable.txt");

Table table = CreateTable(tableString);
Console.WriteLine();
Console.WriteLine("=================================== Entidad ==========================================");
Console.WriteLine();

Console.WriteLine(table.CreateTable());
//Console.WriteLine(table.CreateEntityLoad());

Console.WriteLine();
Console.WriteLine("=================================== ORA ==========================================");
Console.WriteLine();
Console.WriteLine(table.CreateORAClass());



Console.WriteLine(table.CreateBOClass());


Table CreateTable(List<string> tableString)
{
    Table table = new();

    foreach (string item in tableString)
    {
        if (item.ToLower().Contains("create") || item.ToLower().Contains("alter"))
        {

            table.Name = item.Split(" ")[2];
            continue;
        }
        string[] list = item.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        if (list.Length > 1)
        {
            Column column = new();
            column.Name = list[0];
            column.DbType = list[1];
            if (list.Length > 2)
            {
                column.IsNull = false;
            }
            else
            {
                column.IsNull = true;
            }
            
            table.Columns.Add(column);
        }

    }

    return table;
}

List<string> ReadFile(string filePath)
{
    List<string> list = new();
    try
    {
        //string filePath = "D:\\Trabajo\\Recurosos Informaticos\\LicenseConvertExcelToDatabase\\LicenseConvertExcelToDatabase\\Administracion.txt";

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
        }
        else
        {
            Console.WriteLine("The file is not exist.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    return list;
}


public class Table
{
    public string Name { get; set; }
    public List<Column> Columns { get; set; } = new();


    public string CreateTable()
    {
        StringBuilder sb = new();

        sb.AppendLine($"public class {Name}");
        sb.AppendLine("{");
        foreach (Column item in Columns)
        {
            sb.AppendLine($"    public {item.Type}{(item.IsNull ? "?" : string.Empty)} {item.Name} {{ get; set; }}");
        }
        sb.AppendLine("}");


        return sb.ToString();
    }

    public string CreateEntityLoad()
    {
        StringBuilder sb = new();


        sb.Append($"private {Name} CargarEntidad(OracleDataReader _dr)");
        sb.AppendLine("{");

        sb.AppendLine($"    {Name} entidad = new {Name}();");
        sb.AppendLine();
        foreach (Column column in Columns)
        {
            sb.AppendLine($"    entidad.{column.Name} = _dr.DataReaderTo{column.TypeCamelCase}(nameof({this.Name}.{column.Name}));");
        }
        sb.AppendLine();
        sb.AppendLine("    return entidad;");

        sb.AppendLine("}}}");





        return sb.ToString();
    }

    public string CreateORAClass()
    {
        StringBuilder sb = new();


        sb.AppendLine(
                    @"using System;
            using System.Collections.Generic;
            using System.Text;
            using Oracle.ManagedDataAccess.Client;
            using SistemaRI;
            using SistemaRI.AccesoADatos;
            using ClasesFrigo.Entidades;
            using ClasesFrigo.Interfaces;

            namespace ClasesFrigo.AccesoDatos
            {"
            );
        sb.AppendLine($@"
                public class {this.Name}_Ora : AccesoADatosORA, I{this.Name}");

        sb.AppendLine("{");
        sb.AppendLine($@"
                    public {this.Name}_Ora() : base()
            ");
        sb.Append("{}");

        sb.AppendLine(
        $@"#region publicos
        public Validator<List<{this.Name}>> DevolverXCodigo(int codigo)"
            );
        sb.AppendLine("{");


        StringBuilder sb2 = new();

        sb2.Append("SELECT ");

        int count = 0;

        foreach (Column column in Columns)
        {
            if (count == Columns.Count - 1)
                sb2.Append($"{column.Name} ");
            else
                sb2.Append($"{column.Name}, ");


            if (count % 5 == 0 && count != 0)
            {
                sb2.AppendLine();

            }


            count++;

        }

        sb2.AppendLine($" FROM {this.Name}");




        sb.AppendLine($@"

            StringBuilder sb = new StringBuilder();
            List<ParametroORA> parameters = new List<ParametroORA>();

            sb.Append(@""{sb2.ToString()}"");
                       
           //TODO: AGREGAR WHERE CON LOS PK DE LA TABLA
            return BuscarEntidades(CargarEntidad, sb.ToString(), parameters);
        ");
        sb.AppendLine("}");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine(
        $@"
        public Validator<List<{this.Name}>> DevolverTodos(int codigo)"
            );
        sb.AppendLine("{");

        sb.AppendLine($@"

            StringBuilder sb = new StringBuilder();
            List<ParametroORA> parameters = new List<ParametroORA>();

            sb.Append(@""{sb2.ToString()}"");
                       
           //TODO: AGREGAR WHERE CON LOS PK DE LA TABLA
            return BuscarEntidades(CargarEntidad, sb.ToString(), parameters);
        ");
        sb.AppendLine("}");

        sb.AppendLine("#endregion");

        sb.AppendLine(CreateEntityLoad());

        return sb.ToString();
    }


    public string CreateBOClass()
    {
        StringBuilder sb = new();
        sb.Append(@"
        using ClasesFrigo.AccesoDatos;
        using ClasesFrigo.Entidades;
        using SistemaRI;
        using System.Collections.Generic;
        
        namespace ClasesFrigo.Negocio
        {
");

        sb.AppendLine($"  public class {this.Name}_BO : BO");
        sb.Append("    {");

        sb.AppendLine($@"private {this.Name}_Ora _interfaz => (({this.Name}_Ora)base._iBase);

        public {this.Name}_BO() : base(new {this.Name}_Ora())

");

        sb.AppendLine("{}");

        sb.AppendLine($@"
            #region ""publicos""
            public Validator<List<{this.Name}>> DevolverXCodigo(int codigo)
");

        sb.AppendLine(@"
                {
                    return _interfaz.DevolverXCodigo(codigo);
                }
"

            );

        sb.AppendLine($"     public Validator<List<{this.Name}>> DevolverTodos()");
        sb.AppendLine(@" {

            return _interfaz.DevolverTodos();
   }
#endregion
    }


}
");






        return sb.ToString();
    }

}

public class Column
{
    public string Name { get; set; }
    public string DbType { get; set; }
    public string Type
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(type))
                return type;

            string dbType = DbType.ToLower();

            if (dbType.Contains("number"))
            {
                type = "int";
                if (dbType.Contains("("))
                {
                    string number = dbType.Split("(")[1].Split(")")[0];
                    if (number.Contains(","))
                    {
                        type = "decimal";
                    }
                    else if (number == "1")
                    {
                        type = "bool";
                    }
                }
            }
            if (dbType.Contains("varchar") || dbType.Contains("char"))
            {
                type = "string";
            }
            if (dbType.Contains("date") || dbType.Contains("timestamp"))
            {
                type = "DateTime";
            }




            return type;

        }
    }

    private string type { get; set; } = string.Empty;

    public string TypeCamelCase
    {
        get
        {
            if (string.IsNullOrWhiteSpace(type))
                return string.Empty;

            return type.Substring(0, 1).ToUpper() + type.Substring(1);
        }
    }

    public string TypeSearch
    {
        get
        {
            if (string.IsNullOrWhiteSpace(type))
                return string.Empty;




            return type.Substring(0, 1).ToUpper() + type.Substring(1);
        }
    }



    public bool IsNull { get; set; }

    public override string ToString()
    {
        return $"Name: {Name}, Type: {Type}, IsNull: {IsNull}";
    }

}