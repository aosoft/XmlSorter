﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Cocona;

namespace XmlSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaLiteApp.Run<Program>(args);
        }

        public void Process([Argument] string input, string? output = null)
        {
            using var r = new StreamReader(input);
            var node = LoadXML(r);
            if (node != null)
            {
                using var w = XmlWriter.Create(Console.Out, new XmlWriterSettings()
                {
                    NewLineChars = Environment.NewLine,
                    NewLineOnAttributes = true,
                    Indent = true,
                    IndentChars = "  ",
                });
                WriteNode(w, node);
            }
        }

        private Node? LoadXML(TextReader r)
        {
            using var xml = XmlReader.Create(r, new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = true });
            var nodes = ReadNode(xml, new Node("", "")).Children;
            return nodes.Length > 0 ? nodes[0] : null;
        }

        private void WriteNode(XmlWriter w, Node node)
        {
            w.WriteStartElement(node.Name, node.NameSpace);
            if (node.Attributes.Length > 0)
            {
                foreach (var item in node.Attributes)
                {
                    w.WriteAttributeString(item.Name, item.Prefix, item.Value);
                }
            }
            foreach (var item in node.Children)
            {
                WriteNode(w, item);
            }
            w.WriteEndElement();
        }

        private Node ReadNode(XmlReader r, Node node)
        {
            var value = "";
            var nodes = new List<Node>();
            while (r.Read())
            {
                if (r.MoveToContent() != XmlNodeType.None)
                {
                    switch (r.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (r.HasAttributes)
                            {
                                string? ns = null;
                                var attrs = new List<Attr>(r.AttributeCount);
                                for (int i = 0; i < r.AttributeCount; i++)
                                {
                                    r.MoveToAttribute(i);
                                    if (r.Name == "xmlns")
                                    {
                                        ns = r.Value;
                                    }
                                    else
                                    {
                                        attrs.Add(new Attr(r.Prefix, r.Name, r.Value));
                                    }
                                }
                                attrs.Sort((a, b) => string.Compare(a.Name, b.Name));
                                r.MoveToElement();
                                nodes.Add(ReadNode(r, new Node(r.Prefix, r.Name, attrs.ToArray(), ns)));
                            }
                            else
                            {
                                r.MoveToElement();
                                nodes.Add(ReadNode(r, new Node(r.Prefix, r.Name)));
                            }
                            break;

                        case XmlNodeType.Text:
                            value = r.Value;
                            break;

                        case XmlNodeType.EndElement:
                            nodes.Sort(ComparisonNode);
                            return node with { Value = value, Children = nodes.ToArray() };
                    }
                }
            }
            nodes.Sort(ComparisonNode);
            return node with { Children = nodes.ToArray() };
        }

        private int ComparisonNode(Node a, Node b)
        {
            var r = string.Compare(a.Name, b.Name);
            if (r == 0)
            {
                return a.GetHashCode() - b.GetHashCode();
            }
            return r;
        }
    }
}
