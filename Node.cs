using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlSorter
{
    record Node(string Prefix, string Name, string Value, Node[] Children, Attr[] Attributes, string? NameSpace)
    {
        private static Node[] _emptyNodes = new Node[0];
        private static Attr[] _emptyAttrs = new Attr[0];

        public Node(string prefix, string name) : this(prefix, name, "", _emptyNodes, _emptyAttrs, null)
        {
        }

        public Node(string prefix, string name, Attr[] attrs, string? ns) : this(prefix, name, "", _emptyNodes, attrs, ns)
        {
        }
    }
}
