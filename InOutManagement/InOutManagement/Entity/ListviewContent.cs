﻿namespace InOutManagement.Entity
{
    #region using directive

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion
    public sealed class ListviewContent
    {

        public String Date { get; set; }

        public String Supplier { get; set; }

        public String Status { get; set; }

        public String Name { get; set; }

        public String Model { get; set; }

        public String Unit { get; set; }
       
        public Double Price { get; set; }

        public Int32 Count { get; set; }

        public String BillArchive { get; set; }

    }
}
