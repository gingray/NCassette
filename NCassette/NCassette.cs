﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NCassette.Common;

namespace NCassette
{
    public static class NCassette
    {
        public static NRecord<T> Record<T>(Func<T> func)
        {
            return new NRecord<T>(func);
        }
    }
}