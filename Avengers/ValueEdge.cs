using QuikGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avengers
{
    public class ValueEdge<TValue, TVertex> : Edge<TVertex>
    {
        public TValue Value { get; init; }

        public ValueEdge(TValue value,TVertex source, TVertex target) : base(source, target)
        {
            Value = value;
        }
    }
}
