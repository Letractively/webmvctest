using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMVCTest.Model
{
    public interface IKeyValueContainer
    {
        void AddKeyValueData(string key, string value);
    }
}
