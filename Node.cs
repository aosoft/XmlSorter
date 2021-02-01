using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlSorter
{
    record Node(string Prefix, string Name, string Value, Node[] Children, Attr[] Attributes, string? NameSpace) :  IComparable<Node>
    {
        private static Node[] _emptyNodes = new Node[0];
        private static Attr[] _emptyAttrs = new Attr[0];

        public Node(string prefix, string name) : this(prefix, name, "", _emptyNodes, _emptyAttrs, null)
        {
        }

        public Node(string prefix, string name, Attr[] attrs, string? ns) : this(prefix, name, "", _emptyNodes, attrs, ns)
        {
        }

        int IComparable<Node>.CompareTo(Node? other)
        {
            if (other != null)
            {
                {
                    var r = string.Compare($"{Prefix}:{Name}", $"{other.Prefix}:{other.Name}");
                    if (r != 0)
                    {
                        return r;
                    }
                }

                {
                    var a = string.Join(" ", Attributes.Select(x => $"{x.Prefix}:{x.Name}").ToArray());
                    var b = string.Join(" ", other.Attributes.Select(x => $"{x.Prefix}:{x.Name}").ToArray());
                    var r = string.Compare(a, b);
                    if (r != 0)
                    {
                        return r;
                    }
                }

                {
                    var a = string.Join(" ", Attributes.Select(x => x.Value).ToArray());
                    var b = string.Join(" ", other.Attributes.Select(x => x.Value).ToArray());
                    var r = string.Compare(a, b);
                    if (r != 0)
                    {
                        return r;
                    }
                }

                {
                    var a = string.Join(" ", Children.Select(x => x.GetHashCode()).ToArray());
                    var b = string.Join(" ", other.Children.Select(x => x.GetHashCode()).ToArray());
                    var r = string.Compare(a, b);
                    return r;
                }
            }
            return 1;
        }
    }
}
