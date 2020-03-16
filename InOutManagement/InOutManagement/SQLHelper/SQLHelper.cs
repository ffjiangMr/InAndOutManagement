namespace InOutManagement.SQLHelper
{
    #region using directive

    using log4net;

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;
    using System.Text;

    #endregion

    public sealed class SQLHelper : IDisposable
    {
        private static ILog Logger = LogManager.GetLogger(typeof(SQLHelper));

        #region singal instance

        private SQLHelper()
        {
            this.Initial();
        }

        private static SQLHelper instance;

        public static SQLHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new SQLHelper();
            }
            return instance;
        }

        #endregion

        public Boolean Insert<T>(T entity)
        {
            Logger.Info("Func in.");
            Boolean result = false;
            try
            {
                Type type = entity.GetType();
                using (var transition = this.connect.BeginTransaction())
                {
                    var command = this.connect.CreateCommand();
                    StringBuilder names = new StringBuilder(256);
                    StringBuilder values = new StringBuilder(256);
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        var propertyValue = property.GetValue(entity, null)?.ToString();
                        if ((String.IsNullOrEmpty(propertyValue) == false) && (propertyValue != 0.ToString()))
                        {
                            names.Append("'" + property.Name + "',");
                            values.Append("@" + property.Name + ",");
                        }
                    }
                    if ((names.Length > 0) && (values.Length > 0))
                    {
                        names.Remove(names.Length - 1, 1);
                        values.Remove(values.Length - 1, 1);
                        command.CommandText = "insert into " + type.Name + " (";
                        command.CommandText += names.ToString() + ") values (";
                        command.CommandText += values.ToString() + ");";
                        this.AddParameters(command.Parameters, entity);
                        if (command.ExecuteNonQuery() > 0)
                        {
                            result = true;
                        }
                    }
                    transition.Commit();
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            Logger.Info("Func out.");
            return result;
        }

        public List<T> Query<T>(T entity, List<String> queryColumnList = null)
        {
            Logger.Info("Func in.");
            List<T> result = null;
            Type type = typeof(T);
            try
            {
                using (var command = this.connect.CreateCommand())
                {
                    List<String> names = new List<String>();
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    String column = String.Empty;
                    foreach (var item in queryColumnList ?? (new List<String>()))
                    {
                        column += " " + item + ",";
                    }
                    column = column.Trim(',');
                    column = String.IsNullOrEmpty(column.Trim()) ? "*" : column;
                    command.CommandText = "Select distinct " + column + " from " + type.Name + " ";
                    String condition = String.Empty;
                    foreach (var property in properties)
                    {
                        names.Add(property.Name);
                        var propertyValue = property.GetValue(entity, null)?.ToString();
                        if ((String.IsNullOrEmpty(propertyValue) == false) && (propertyValue != 0.ToString()))
                        {
                            condition += " " + property.Name + " = @" + property.Name;
                            condition += " and";
                        }
                    }
                    condition = condition.Contains("and") ?
                                " where " + condition.Remove(condition.Length - 4, 4) :
                                condition;
                    command.CommandText += condition;
                    this.AddParameters<T>(command.Parameters, entity);
                    var reader = command.ExecuteReader();
                    result = this.SetColumn<T>(queryColumnList ?? names, ref reader);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            Logger.Info("Func out.");
            return result;
        }

        public Int32 Delete<T>(T entity)
        {
            Logger.Info("Func in.");
            Int32 result = 0;
            try
            {
                Type type = entity.GetType();
                using (var transition = this.connect.BeginTransaction())
                {
                    var command = this.connect.CreateCommand();
                    StringBuilder parameter = new StringBuilder(256);                   
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        var propertyValue = property.GetValue(entity, null)?.ToString();                       
                        if ((String.IsNullOrEmpty(propertyValue) == false) && (propertyValue != 0.ToString()))
                        {
                            parameter.Append(property.Name);
                            parameter.Append(" = ");
                            parameter.Append("@" + property.Name + " ");
                            parameter.Append(" and ");
                        }
                    }
                    if (parameter.Length > 0)
                    {
                        parameter.Remove(parameter.Length - 5, 5);                       
                        command.CommandText = "update " + type.Name + " set IsDeleated = 'True' where ";
                        command.CommandText += parameter.ToString();                        
                        this.AddParameters(command.Parameters, entity);
                        result = command.ExecuteNonQuery();                        
                    }
                    transition.Commit();
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            Logger.Info("Func out.");
            return result;
        }

        private void Initial()
        {
            Logger.Info("Func in.");
            this.CreateTables(File.Exists(@"data.db") == false);
            Logger.Info("Func out.");
        }

        private void CreateTables(Boolean isCreateTable)
        {
            Logger.Info("Func in.");
            try
            {
                using (StreamReader reader = new StreamReader(@"SQLHelper\Script\CreateScript.sql"))
                {
                    connect = new SQLiteConnection("data source=data.db");
                    connect.Open();
                    if (isCreateTable)
                    {
                        var transaction = connect.BeginTransaction();
                        SQLiteCommand command = new SQLiteCommand(reader.ReadToEnd(), connect);
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        command.Dispose();
                        transaction.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                //TODO
            }
            Logger.Info("Func out.");
        }

        private void AddParameters<T>(SQLiteParameterCollection paramCollection, T entity)
        {
            Type type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(entity, null)?.ToString();
                if ((String.IsNullOrEmpty(propertyValue) == false) && (propertyValue != 0.ToString()))
                {
                    paramCollection.Add(new SQLiteParameter(property.Name, propertyValue));
                }
            }
        }

        private List<T> SetColumn<T>(List<String> columnName, ref SQLiteDataReader reader)
        {
            Logger.Info("Func in.");
            List<T> result = new List<T>();
            Type type = typeof(T);
            while (reader.Read())
            {
                var entity = (T)Activator.CreateInstance(type);
                foreach (var item in columnName)
                {
                    var property = type.GetProperty(item, BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        var value = Convert.ChangeType(reader[item], property.PropertyType);
                        property.SetValue(entity, value, null);
                    }
                }
                result.Add(entity);
            }
            Logger.Info("Func out.");
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(Boolean disposing)
        {
            if (this.isDisposed != true)
            {
                if (disposing == true)
                {
                    //释放托管资源
                }
                this.connect?.Dispose();
                this.isDisposed = true;
            }
        }

        ~SQLHelper()
        {
            this.Dispose(false);
            this.connect = null;
        }

        private SQLiteConnection connect;
        private Boolean isDisposed = false;
    }
}
