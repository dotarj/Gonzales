// Copyright (c) Arjen Post. See License.txt in the project root for license information. Credits go to Marc Gravell 
// for the original idea, which found here https://code.google.com/p/fast-member/, and some parts of the code.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Gonzales.Test
{
    [ExcludeFromCodeCoverage]
    public class Class
    {
        public int A { get; set; }
        public int AA { get; set; }
        public string B { get; set; }
        public DateTime? C { get; set; }
        public decimal? D { get; set; }
        public int E;
        public int EE;
        public string F;
        public DateTime? G;
        public decimal? H;

        private int I { get; set; }
        public int J { get; private set; }
        public int K { private get; set; }
        private int L;

        public object this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
