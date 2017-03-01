using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SQL2Search.Model
{
    [Serializable]
    public class SQLEntity
    {
        public void Create()
        {
            CreationTime = DateTime.Now;
            Fields = new List<SQLField>();
        }

        public Provider Provider { get; set; }
        public DateTime CreationTime { get; set; }
        public List<SQLField> Fields { get; set; }
        public string FullSQLCommand { get; set; }

        public SqlCommand GetSqlCommand()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(FullSQLCommand);

                foreach (SQLField field in Fields)
                {
                    if (field.Required)
                    {
                        if (field.Value == null)
                            throw new Exception($"O campo ${field.Name} é obrigatório");

                        if (string.IsNullOrEmpty(field.Value.ToString()))
                            throw new Exception($"O campo ${field.Name} é obrigatório");
                    }

                    #region Text fields
                    if (field.Type == "text")
                    {
                        switch (field.MachMode)
                        {
                            case SQLField.MatchMode.EXACT:
                                cmd.Parameters.AddWithValue($"@{field.Name}", field.Value);
                                break;

                            case SQLField.MatchMode.ANYWHERE:
                                field.Value = $"%{field.Value}%";
                                cmd.Parameters.AddWithValue($"@{field.Name}", field.Value);
                                break;

                            case SQLField.MatchMode.START:
                                field.Value = $"{field.Value}%";
                                cmd.Parameters.AddWithValue($"@{field.Name}", field.Value);
                                break;

                            case SQLField.MatchMode.END:
                                field.Value = $"%{field.Value}";
                                cmd.Parameters.AddWithValue($"@{field.Name}", field.Value);
                                break;
                        }

                        continue;
                    }
                    #endregion

                    #region Date fields
                    if (field.Type.Equals("DATE"))
                    {
                        DateTime datetime = (DateTime)field.Value;
                        cmd.Parameters.AddWithValue($"@{field.Name}", datetime.ToString("yyyy-MM-dd"));
                        continue;
                    }
                    #endregion

                    #region Other Fields
                    cmd.Parameters.Add(field.Value);
                    #endregion
                }

                return cmd;
            }
            catch
            {
                throw;
            }
        }

        public void SetFieldValue(string fieldName, object fieldValue)
        {
            foreach (SQLField field in Fields)
            {
                if (field.Name.Equals(fieldName))
                    field.Value = fieldValue;
            }
        }
    }
}
