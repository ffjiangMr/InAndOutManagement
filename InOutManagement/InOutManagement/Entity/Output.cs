namespace InOutManagement.Entity
{
    #region using directive

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion 

    public sealed class Output
    {
        public Int32 Identity { get; set; }

        public Int32 Input { get; set; }

        public Int32 Count { get; set; }

        public String OutputDate { get; set; }

        public String BillArchive { get; set; }

        public String Pickup { get; set; }
    }
}
