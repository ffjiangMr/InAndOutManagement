namespace InOutManagement.Entity
{
    #region using directive

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion
  
    public sealed class Surplus
    {
        public Int32  Identity  { get; set; }

        public Int32 Material  { get; set; }

        public Int32 Count { get; set; }

        public Double Amount { get; set; }
    }
}
