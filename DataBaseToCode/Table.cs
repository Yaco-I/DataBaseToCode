using System.Text;

namespace DataBaseToCode;

public class Table
{

    public string Name { get; set; }
    public List<Column> Columns { get; set; } = new();


    public string CreateTable()
    {
        StringBuilder sb = new();
        sb.AppendLine(@"using System;
namespace ClasesFrigo.Entidades
{");
        sb.AppendLine($"public class {Name}");
        sb.AppendLine("{");
        foreach (Column item in Columns)
        {
            sb.AppendLine($"    public {item.Type}{(item.IsNull && item.Type != "string" ? "?" : string.Empty)} {item.Name} {{ get; set; }}");
        }
        sb.AppendLine("}}");


        return sb.ToString();
    }

    public string CreateClassSearch()
    {
        StringBuilder sb = new();
        sb.AppendLine(@"using System;
namespace ClasesFrigo.Entidades
{");
        sb.AppendLine($"public class {Name}Busqueda");
        sb.AppendLine("{");
        foreach (Column item in Columns)
        {
            if (item.Type == "DateTime")
            {
                sb.AppendLine($"    public {item.Type}? {item.Name}d {{ get; set; }}");
                sb.AppendLine($"    public {item.Type}? {item.Name}h {{ get; set; }}");
            }
            else
                sb.AppendLine($"    public {item.Type}{(item.Type != "string" ? "?" : string.Empty)} {item.Name} {{ get; set; }}");

        }
        sb.AppendLine("}}");


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
        public Validator<{this.Name}> DevolverXCodigo(int codigo)"
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
            return DevolverEntidad(CargarEntidad, sb.ToString(), parameters);
        ");
        sb.AppendLine("}");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine("");
        sb.AppendLine(
        $@"
        public Validator<List<{this.Name}>> DevolverTodos()"
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


        sb.AppendLine(@$" public Validator<List<{this.Name}>> DevolverPorBusqueda({this.Name}Busqueda {this.Name.ToLower()}Busqueda)
        {{
            StringBuilder sb = new StringBuilder();

            sb.Append(@""{sb2.ToString()}  
                    "");

           sb.AppendLine(@"" WHERE 
");


        foreach (var item in Columns)
        {
            if (item.Type == "string")
                sb.AppendLine($"(:p_{item.Name} IS NULL OR {item.Name} LIKE '%' || :p_{item.Name} || '%') AND ");
            else if (item.Type == "DateTime")
            {
                sb.AppendLine($"--(:p_{item.Name}d IS NULL OR {item.Name} <= :p_{item.Name}) AND ");
                sb.AppendLine($"--(:p_{item.Name}h IS NULL OR {item.Name} >= :p_{item.Name}) AND ");

            }
            else
                sb.AppendLine($"(:p_{item.Name} IS NULL OR {item.Name} = :p_{item.Name}) AND ");

        }

        sb.Remove(sb.Length - 6, 6);
        sb.AppendLine(@"
            "");

            List<ParametroORA> parameters = new List<ParametroORA>()
            {
                ");


        foreach (var item in Columns)
        {
            if (item.Type == "DateTime")
            {
                sb.AppendLine($"//new ParametroORA({this.Name.ToLower()}Busqueda.{item.Name}d, \"p_{item.Name}d\").NullIfNull(),");
                sb.AppendLine($"//new ParametroORA({this.Name.ToLower()}Busqueda.{item.Name}d, \"p_{item.Name}d\").NullIfNull(),");
                sb.AppendLine($"//new ParametroORA({this.Name.ToLower()}Busqueda.{item.Name}h, \"p_{item.Name}h\").NullIfNull(),");
                sb.AppendLine($"//new ParametroORA({this.Name.ToLower()}Busqueda.{item.Name}h, \"p_{item.Name}h\").NullIfNull(),");

            }
            else
            {
                sb.AppendLine($"new ParametroORA({this.Name.ToLower()}Busqueda.{item.Name}, \"p_{item.Name}\").NullIfNull(),");
                sb.AppendLine($"new ParametroORA({this.Name.ToLower()}Busqueda.{item.Name}, \"p_{item.Name}\").NullIfNull(),");

            }
        }

        sb.Remove(sb.Length - 3, 3);




        sb.AppendLine($@"}};
            return BuscarEntidades(CargarEntidad, sb.ToString(), parameters);
    }}

");

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

    public string CreateController()
    {
        StringBuilder sb = new();
        sb.Append(@$"
    
using Microsoft.AspNetCore.Mvc;
using SistemaRI;
using SistemaRI.API;
using ClasesFrigo.Negocio;
using ClasesFrigo.Entidades;
using SistemaRI.AccesoADatos;

namespace APIFrigo.Controllers
{{
    public class {this.Name}Controller : BaseController
    {{
        {this.Name}_BO _{this.Name.ToLower()}BO;
        
        public {this.Name}Controller( )
        {{
            _{this.Name.ToLower()}BO = new {this.Name}_BO();
        }}

        [HttpGet]
        public ActionResult<List<{this.Name}>> DevolverTodos()
        {{

          return  _{this.Name.ToLower()}BO.DevolverTodos().ToActionResult();

        }}




        [HttpGet(""{{codigo}}"")]
        public ActionResult<{this.Name}> DevolverXCodigo(int codigo)
        {{

          return  _{this.Name.ToLower()}BO.DevolverXCodigo(codigo).ToActionResult();

        }}


        [HttpPost]
        [Route(""buscar"")]
        public ActionResult<List<TipoDoc>> DevolverBusqueda({this.Name}Busqueda {this.Name.ToLower()})
        {{
            
            return _{this.Name.ToLower()}BO.DevolverBusqueda({this.Name.ToLower()}).ToActionResult();
            
        }}

    }}
}}
");

        return sb.ToString();
    }


    public override string ToString()
    {
        string print = $@"


                =================================== Entidad ==========================================

{CreateTable()}

                






                =================================== Entidad Busqueda ========================================== 

{CreateClassSearch()}







                =================================== ORA ========================================================


{CreateORAClass()}



                =================================== BO ========================================================


{CreateBOClass()}






                =================================== Controller ========================================================



{CreateController()}
";


        return print;
    }
}


