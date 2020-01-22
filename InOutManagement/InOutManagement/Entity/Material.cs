namespace InOutManagement.Entity
{
    #region using directive

    using System;

    #endregion

    public sealed class Material
    {
        public Int32 Identity  { get; set; }

        public String Name { get; set; }

        public String Model { get; set; }

        public String Unit { get; set; }
    }
}
