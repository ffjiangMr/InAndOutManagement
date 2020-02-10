using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InOutManagement.Common
{
    public enum MessageEnum
    {
        NONE = 0,
        InsertSuccess = 1,
        InsertError = 2,
        Initialed = 3,
        QuerySuccessful,
        InsertFailed,
        OutputSuccess,
        OutputError,
        OutputFailed,
    }
}
