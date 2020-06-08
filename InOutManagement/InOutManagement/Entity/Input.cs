namespace InOutManagement.Entity
{
    #region using directive

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion
    public sealed class Input
    {
        public Int32 Identity { get; set; }

        public String InputDate { get; set; }

        public Int32 Material { get; set; }

        public Double Count { get; set; }

        public Int32 Warehouse { get; set; }
        
        public Double Price { get; set; }

        public String BillArchive { get; set; }

        public String Supplier { get; set; }

        public Boolean IsDeleated  { get; set; }
    }
}

