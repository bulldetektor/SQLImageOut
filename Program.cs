using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.IO;

namespace RunSQLOLEDB
{
    class Program
    {
        enum ErrorLevel 
        { 
            OK = 0,
            CommandLineSyntaxError = 1,
            Exception = 2,
            Unknwon = 3
        };

        static int Main(string[] args)
        {
            ErrorLevel errorLevel = ErrorLevel.Unknwon;
            try
            {
                if(args.Length < 2) 
                {
                    Console.WriteLine(GetHelpString());
                    errorLevel = ErrorLevel.CommandLineSyntaxError;
                }
                else
                {
                    try
                    {
                        #region @parameters
                        string connectionString = args[0];
                        if (connectionString[0] == '@')
                        {
                            connectionString = ReadParameterFromFile(connectionString.Substring(1));
                        }

                        string sql = args[1];
                        if (sql[0] == '@')
                        {
                            sql = ReadParameterFromFile(sql.Substring(1));
                        }
                        #endregion                    


                        using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
                        {
                            oleDbConnection.Open();
                            using (OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection))
                            {
                                oleDbCommand.CommandType = System.Data.CommandType.Text;
                                using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                                {
                                    const int bufferSize = 2014;

                                    byte[] buffer = new byte[bufferSize];

                                    long bytesRead;

                                    while (oleDbDataReader.Read())
                                    {
                                        using (FileStream stream = new FileStream(oleDbDataReader[0].ToString(), FileMode.Create))
                                        {
                                            Console.Write("Extracting "+oleDbDataReader[0].ToString()+"...");
                                            using (BinaryWriter writer = new BinaryWriter(stream))
                                            {
                                                bytesRead = oleDbDataReader.GetBytes(1, stream.Position, buffer, 0, bufferSize);
                                                while(bytesRead > 0)
                                                {
                                                    writer.Write(buffer, 0, (int)bytesRead);
                                                    bytesRead = oleDbDataReader.GetBytes(1, stream.Position, buffer, 0, bufferSize);
                                                }

                                                writer.Close();
                                                Console.WriteLine("Done");
                                            }
                                        }
                                    }
                                    errorLevel = ErrorLevel.OK;
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        errorLevel = ErrorLevel.Exception;
                        Console.WriteLine(GetHelpString());
                        Console.WriteLine(string.Empty);
                        Console.WriteLine("ERROR:"+exception.Message);
                    }
                } 
            }
            catch
            {
                //We should have already caught everything.  This should at least allow the errorlevel to be reported OK.
            }
            return (int)errorLevel;
        }

        private static string ReadParameterFromFile(string path)
        {
            StringBuilder parameter = new StringBuilder();
            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(path))
            {
                while(!streamReader.EndOfStream)
                {
                    if (parameter.Length != 0)
                    {
                        parameter.AppendLine(string.Empty);
                    }
                    parameter.Append(streamReader.ReadLine());
                }
            }
            return parameter.ToString();
        }

        private static string GetHelpString()
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("SQLImageOut v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            message.AppendLine(@"
    Copyright 2012 Andrew Joiner

Licensed under the Apache License, Version 2.0 (the "+"\""+@"License"+"\""+@");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an " + "\"" + @"AS IS" + "\"" + @" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.");
            message.AppendLine(string.Empty);
            message.AppendLine("--------------------------------------------------");
            message.AppendLine(string.Empty);
            message.AppendLine("Usage:");
            message.AppendLine("RunSQLOLEDB \"[connection string]\" \"[query]\"");
            message.AppendLine(string.Empty);
            message.AppendLine("e.g.");
            message.AppendLine("To extact all images from table Image into separate files 1.jpg, 2.jpg, etc.");
            message.AppendLine("SQLImageOut \"Provider=SQLOLEDB;Data Source=(local);...\" \"SELECT CONVERT(varchar(50), ImageID)+'.jpg', ImageData from Image\"");
            message.AppendLine("");
            message.AppendLine("SQLImageOut @ConnectionString.txt @Query.sql");
            message.AppendLine(string.Empty);
            return message.ToString();
        }
    }
}
