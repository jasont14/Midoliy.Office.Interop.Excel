﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Midoliy.Office.Interop
{
    public interface IExcelRange : IDisposable
    {
        dynamic Value { get; set; }
        dynamic Formula { get; set; }

        IExcelRange Copy();
        IExcelRange Paste(IExcelRange from, PasteType type = PasteType.All, PasteOperation operation = PasteOperation.None, bool skipBlanks = false, bool transpose = false);

        void Clear();
    }
}
