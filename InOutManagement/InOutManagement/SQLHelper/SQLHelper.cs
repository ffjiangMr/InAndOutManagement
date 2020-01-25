namespace InOutManagement.SQLHelper
{
    #region using directive

    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using InOutManagement.Entity;

    #endregion

    public sealed class SQLHelper : IDisposable
    {
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
            Boolean result = false;
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
            return result;
        }

        public List<T> Query<T>(T entity, List<String> queryColumnList = null)
        {
            Type type = typeof(T);
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
                return this.SetColumn<T>(queryColumnList ?? names, ref reader);
            }
        }
      
        public void InsertInput(ref Input input)
        {
            //using (var transition = this.connect.BeginTransaction())
            //{
            //    var command = this.connect.CreateCommand();
            //    command.CommandText = "insert into Input ('InputDate','Material','Unit') values(@Name,@Model,@Unit)";
            //    this.AddParameters(command.Parameters, material);
            //    command.ExecuteNonQuery();
            //    transition.Commit();
            //}
            //this.QueryMaterialIdentity(ref material);
        }

        private void Initial()
        {
            this.CreateTables(File.Exists(@"data.db") == false);
        }

        private void CreateTables(Boolean isCreateTable)
        {
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
                //TODO
            }
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
