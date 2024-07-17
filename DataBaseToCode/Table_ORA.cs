using Oracle.ManagedDataAccess.Client;
using SistemaRI;
using SistemaRI.AccesoADatos;

namespace DataBaseToCode
{
    public class Table_ORA : AccesoADatosORA
    {
        public Table_ORA()
        {

        }


        public Validator<List<Table>> BuscarTodos(IConexion conExterna)
        {
            string query = @"SELECT table_name, 
       column_name, 
       data_type, 
       data_length,
       char_length,
       nullable,
       data_precision,
       data_scale
FROM user_tab_columns
ORDER BY table_name, column_id";

            var result =  BuscarEntidades(CargarEntidad, query, new List<ParametroORA>(), conExterna);
            if(result.ok && result.obj is not null)
            {
                result.obj = CargarTable(result.obj);
            }
            return result;
        }
        public Table CargarEntidad(OracleDataReader _dr)
        {
            Table table = new Table();

            table.Name = _dr.DataReaderToString("TABLE_NAME");
            table.Columns = new List<Column>();
            table.Columns.Add(CargarColumna(_dr));

            return table;

        }


        public Column CargarColumna(OracleDataReader _dr)
        {
            Column column = new Column();

            column.Name = _dr.DataReaderToString("COLUMN_NAME");
            column.IsNull = _dr.DataReaderToString("NULLABLE") == "Y";
            column.DbType = _dr.DataReaderToString("Data_Type");
            if (column.DbType == "NUMBER")
            {
                string data_scale = _dr.DataReaderToString("DATA_SCALE");
                string data_precision = _dr.DataReaderToString("DATA_PRECISION");

                if (data_scale != "0")
                {
                    column.DbType = $"NUMBER({data_precision},{data_scale})";
                }
                else
                {
                    column.DbType = $"NUMBER({data_precision})";
                }
            }

            //column.Type = _dr.DataReaderToString("DATA_TYPE");
            //column.Length = _dr.DataReaderToInt("DATA_LENGTH");
            //column.Precision = _dr.DataReaderToInt("DATA_PRECISION");
            //column.Scale = _dr.DataReaderToInt("DATA_SCALE");
            //column.Nullable = _dr.DataReaderToString("NULLABLE") == "Y" ? true : false;
            //column.IsPrimaryKey = _dr.DataReaderToString("PRIMARY_KEY") == "Y" ? true : false;
            //column.IsForeignKey = _dr.DataReaderToString("FOREIGN_KEY") == "Y" ? true : false;
            //column.IsUnique = _dr.DataReaderToString("UNIQUE_KEY") == "Y" ? true : false;
            //column.IsIdentity = _dr.DataReaderToString("IDENTITY") == "Y" ? true : false;
            //column.IsComputed = _dr.DataReaderToString("COMPUTED") == "Y" ? true : false;
            //column.IsRowGuid = _dr.DataReaderToString("ROWGUIDCOL") == "Y" ? true : false;
            //column.Default = _dr.DataReaderToString("DEFAULT_VALUE");

            return column;
        }


        public List<Table> CargarTable(List<Table> list)
        {
            var newList = new List<Table>();

            foreach(Table table in list)
            {
                if (newList.Any(x => x.Name == table.Name))
                {
                    var tableAux = newList.FirstOrDefault(x => x.Name == table.Name);
                    tableAux.Columns.AddRange(table.Columns);
                }
                else
                {
                    newList.Add(table);
                }
            }

            return newList;
        }


    }
}
