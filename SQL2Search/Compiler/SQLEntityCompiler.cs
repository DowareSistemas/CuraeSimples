using SQL2Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SQL2Search.Compiler
{
    public class SQLEntityCompiler
    {
        public void Compile(string scriptFile, string compiledFilePath)
        {
            StreamReader reader = null;
            FileStream fs = null;
            try
            {
                SQLEntity entity = new SQLEntity();
                entity.Create();

                reader = new StreamReader(scriptFile);
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;
                    //field nome text required Nome do produto
                    if (line.StartsWith("field"))
                    {
                        string[] parts = line.Split(' ');

                        SQLField field = new SQLField();
                        field.Name = parts[1];
                        field.Type = parts[2];
                        field.Required = (parts[3] == "true");

                        for (int i = 4; i < parts.Length; i++)
                            field.Description += parts[i] + " ";

                        entity.Fields.Add(field);
                        continue;
                    }

                    entity.FullSQLCommand += line + "\n";
                }

                fs =new FileStream(compiledFilePath + $@"Output {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.csql", FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, entity);
                fs.Close();
            }
            catch
            {
                if (reader != null)
                    reader.Close();

                if (fs != null)
                    fs.Close();

                throw;
            }
        }
    }

    public class SQLEntityDecompiler
    {
        public SQLEntity Decompile(string scriptFile)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(scriptFile, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                SQLEntity entity = (SQLEntity)formatter.Deserialize(fs);

                fs.Close();

                return entity;
            }
            catch
            {
                if (fs != null)
                    fs.Close();

                throw;
            }
        }
    }
}
