namespace InOutManagement.Entity
{
    #region using directive

    using System;

    #endregion

    public sealed class Warehouse
    {
        public Int32 Identity { get; set; }

        public String Name { get; set; }

        public Boolean IsDeleated { get; set; }
    }
}
