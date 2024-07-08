using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseToCode;

public static class LoadTxt
{

    public static Table CreateTable(List<string> tableString)
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

    public static List<Table> CreateTables(List<string> tableString)
    {
        //   List<Table> tables = new();

        var tables = new Dictionary<string, Table>();

        foreach (string line in tableString)
        {
            string[] fields = line.Split(';');
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = fields[i].Trim('"');
            }

            string tableName = fields[0].Capitalize();
            string columnName = fields[1];
            string dbType = fields[2];
            bool isNullable = fields[5] == "Y";


            if (dbType == "NUMBER")
            {
                if (fields[7] != "0") {
                    dbType = $"NUMBER({fields[6]},{fields[7]})";
                }
                else
                {
                    dbType = $"NUMBER({fields[6]})";
                }
            }


            if (!tables.ContainsKey(tableName))
            {
                tables[tableName] = new Table { Name = tableName };
            }

            tables[tableName].Columns.Add(new Column
            {
                Name = columnName.ToLower(),
                DbType = dbType,
                IsNull = isNullable
            });
        }




        return tables.Values.ToList();


    }
    public static List<string> ReadFile(string filePath)
    {
        List<string> list = new();
        try
        {
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



    static string Capitalize(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }


}
