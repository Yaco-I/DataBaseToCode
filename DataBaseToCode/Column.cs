namespace DataBaseToCode;


public class Column
{
    private string _name;
    public string Name { get => _name.ToLower(); set => _name = value; }
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
