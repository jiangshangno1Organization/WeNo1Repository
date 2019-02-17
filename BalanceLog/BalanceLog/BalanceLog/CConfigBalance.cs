using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BalanceLog
{
    public class CConfigBalance
    {
        /// <summary>
        /// 设置节点值,如果节点不存在则自动增加节点,文件不存在则在启动目录增加文件
        /// </summary>
        /// <param name="filename">文件名称.xml</param>
        /// <param name="rootnode">根结点</param>
        /// <param name="node">rootnode的子节点</param>
        /// <param name="value">rootnode的子节点值</param>
        /// <param name="attname">rootnode的子节点属性名称</param>
        /// <param name="attvalue">rootnode的子节点属性值</param>
        /// <param name="subnode">node的子节点</param>
        /// <param name="subvalue">node的子节点值</param>
        /// <param name="subatt">node的子节点属性名称</param>
        /// <param name="subattvalue">node的子节点属性值</param>
        public static void SetValue(string filename, string rootnode, string node, string value, string attname, string attvalue, string subnode, string subvalue, string subatt, string subattvalue)
        {
            SetValue(filename, rootnode, node, value, attname, attvalue, subnode, subvalue, subatt, subattvalue, null);
        }

        /// <summary>
        /// 设置节点值,如果节点不存在则自动增加节点,文件不存在则在启动目录增加文件
        /// </summary>
        /// <param name="filename">文件名称.xml</param>
        /// <param name="rootnode">根结点</param>
        /// <param name="node">rootnode的子节点</param>
        /// <param name="value">rootnode的子节点值</param>
        /// <param name="attname">rootnode的子节点属性名称</param>
        /// <param name="attvalue">rootnode的子节点属性值</param>
        /// <param name="subnode">node的子节点</param>
        /// <param name="subvalue">node的子节点值</param>
        /// <param name="subatt">node的子节点属性名称</param>
        /// <param name="subattvalue">node的子节点属性值</param>
        public static void SetValue(string filename, string rootnode, string node, string value, string attname, string attvalue, string subnode, string subvalue, string subatt, string subattvalue, object[] prams)
        {
            rootnode = rootnode.Trim();
            node = node.Trim();
            subnode = subnode.Trim();
            bool saveorno = false;
            string xmlpath = AddNode(filename, rootnode, node, attname, attvalue, subnode, subatt, subattvalue, prams);
            System.Xml.XmlDocument XmlDoc = GetDocument(xmlpath);
            XmlNodeList nodes = XmlDoc.SelectNodes("/" + rootnode + "/" + node);
            if (nodes != null)
            {
                foreach (XmlNode n in nodes)
                {
                    if (string.IsNullOrEmpty(attname))
                    {
                        if (!string.IsNullOrEmpty(subnode))
                        {
                            XmlNode n1 = n.SelectSingleNode(subnode);
                            if (n1 != null)
                                n1.InnerText = subvalue;
                        }
                        else
                        {
                            n.InnerText = value;
                        }
                        saveorno = true;
                        break;
                    }
                    else
                    {
                        if (n.Attributes[attname].Value.Equals(attvalue))
                        {
                            if (!string.IsNullOrEmpty(subnode))
                            {
                                XmlNode n1 = n.SelectSingleNode(subnode);
                                if (n1 != null)
                                {
                                    n1.InnerText = subvalue;
                                    saveorno = true;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            if (saveorno)
                XmlDoc.Save(xmlpath);
        }
        
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="filename">文件名称.xml</param>
        /// <param name="rootnode">根结点</param>
        /// <param name="node">rootnode的子节点</param>
        /// <param name="attributename">rootnode的子节点属性名称</param>
        /// <param name="attributevalue">rootnode的子节点属性值</param>
        /// <param name="subnode">node的子节点</param>
        /// <param name="subatt">node的子节点属性名称</param>
        /// <param name="subattvalue">node的子节点属性名称</param>
        private static string AddNode(string filename, string rootnode, string node, string attributename, string attributevalue, string subnode, string subatt, string subattvalue, object[] prams)
        {
            bool saveorno = false;
            System.Uri u = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

            string xmlpath = System.IO.Path.GetDirectoryName(u.LocalPath) + "\\" + filename;
            if (prams != null && prams.Length > 0)
            {
                if (File.Exists(filename))
                    xmlpath = filename;
            }
            System.Xml.XmlDocument XmlDoc = GetDocument(xmlpath);

            if (XmlDoc == null)
            {
                XmlDoc = new XmlDocument();
                saveorno = true;
            }
            if (XmlDoc.ChildNodes.Count == 0)
            {
                XmlDeclaration dec = XmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                XmlDoc.AppendChild(dec);
                saveorno = true;
            }

            XmlElement root = (XmlElement)XmlDoc.SelectSingleNode("/" + rootnode);
            if (root == null)
            {
                root = XmlDoc.CreateElement(rootnode);
                XmlDoc.AppendChild(root);
                saveorno = true;
            }
            XmlElement ele = XmlDoc.SelectSingleNode("/" + rootnode + "/" + node) as XmlElement;
            if (ele == null)
            {
                ele = XmlDoc.CreateElement(node);
                if (!string.IsNullOrEmpty(attributename))
                {
                    ele.SetAttribute(attributename, attributevalue);
                }
                if (!string.IsNullOrEmpty(subnode))
                {
                    XmlElement sub = XmlDoc.CreateElement(subnode);
                    if (!string.IsNullOrEmpty(subatt))
                        sub.SetAttribute(subatt, subattvalue);
                    ele.AppendChild(sub);
                }
                root.AppendChild(ele);
                saveorno = true;
            }
            else
            {
                XmlNodeList nodes = XmlDoc.SelectNodes("/" + rootnode + "/" + node);
                XmlNode existnode = null;
                if (!string.IsNullOrEmpty(attributename))
                {

                    foreach (XmlNode n1 in nodes)
                    {
                        if (n1.Attributes[attributename].Value.Equals(attributevalue))
                        {
                            existnode = n1;
                            break;
                        }
                    }
                    if (existnode == null)
                    {
                        XmlElement ele1 = XmlDoc.CreateElement(node);
                        ele1.SetAttribute(attributename, attributevalue);
                        if (!string.IsNullOrEmpty(subnode))
                        {
                            XmlElement sub = XmlDoc.CreateElement(subnode);
                            ele1.AppendChild(sub);
                        }
                        root.InsertBefore(ele1, ele);
                        saveorno = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(subnode))
                        {
                            XmlElement ele2 = (XmlElement)existnode.SelectSingleNode(subnode);
                            if (ele2 == null)
                            {
                                ele2 = XmlDoc.CreateElement(subnode);
                                existnode.AppendChild(ele2);
                                saveorno = true;
                            }
                        }

                    }
                }
                else
                {
                    if (nodes != null)
                    {
                        if (!string.IsNullOrEmpty(subnode))
                        {
                            existnode = nodes[0];
                            XmlElement ele2 = (XmlElement)existnode.SelectSingleNode(subnode);
                            if (ele2 == null)
                            {
                                ele2 = XmlDoc.CreateElement(subnode);
                                existnode.AppendChild(ele2);
                                saveorno = true;
                            }
                        }
                    }
                }

            }
            if (saveorno)
                XmlDoc.Save(xmlpath);
            return xmlpath;
        }
        
        /// <summary>
        /// 获取文档节点集合
        /// </summary>
        /// <param name="xmlpath">文件名.xml 或者 文件绝对路径</param>
        /// <param name="rootnode">根结点名称</param>
        /// <param name="xpath">xpath 语法</param>
        /// <returns></returns>
        public static XmlNodeList GetNodes(string xmlpath, string rootnode, string xpath)
        {
            XmlNodeList nodes = null;
            object obj = GetNode(xmlpath, rootnode, xpath, false);
            if (obj != null)
                nodes = obj as XmlNodeList;
            return nodes;
        }

        /// <summary>
        /// 获取文档节点值
        /// </summary>
        /// <param name="xmlpath">文件名.xml 或者 文件绝对路径</param>
        /// <param name="rootnode">根结点名称</param>
        /// <param name="xpath">xpath 语法</param>
        /// <param name="attributename">属性名称</param>
        /// <param name="attributevalue">属性值</param>
        /// <param name="subnode">子节点</param>
        /// <returns></returns>
        public static string GetValue(string xmlpath, string rootnode, string xpath
            , string attributename, string attributevalue, string subnode)
        {
            string value = string.Empty;
            try
            {
                XmlNode n = GetSingleNode(xmlpath, rootnode, xpath);

                if (!string.IsNullOrEmpty(attributename))
                {
                    XmlNodeList nodes = GetNodes(xmlpath, rootnode, xpath);
                    if (nodes != null)
                    {
                        foreach (XmlNode n1 in nodes)
                        {
                            if (n1.Attributes[attributename].Value.Equals(attributevalue))
                            {
                                if (n1.HasChildNodes)
                                {
                                    foreach (XmlNode n2 in n1.ChildNodes)
                                    {
                                        if (n2.Name.Equals(subnode))
                                        {
                                            value = n2.InnerText;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    value = n1.InnerText;
                                }
                                break;

                            }
                        }
                    }
                }
                else
                {
                    value = n.InnerText.Trim();
                }
            }
            catch (Exception)
            {
            }
            return value.Trim();
        }
        
        /// <summary>
        /// 获取单个文档节点
        /// </summary>
        /// <param name="xmlpath">文件名.xml 或者 文件绝对路径</param>
        /// <param name="rootnode">根结点名称</param>
        /// <param name="xpath">xpath 语法</param>
        /// <returns></returns>
        public static XmlNode GetSingleNode(string xmlpath, string rootnode, string xpath)
        {
            XmlNode node = null;
            object obj = GetNode(xmlpath, rootnode, xpath, true);
            if (obj != null)
                node = obj as XmlNode;
            return node;
        }

        /// <summary>
        ///  获取xml 配制文件文档
        /// </summary>
        /// <param name="xmlpath">文件名.xml 或者 文件绝对路径</param>
        /// <returns></returns>
        public static XmlDocument GetDocument(string xmlpath)
        {
            XmlDocument XmlDoc = new XmlDocument();
            try
            {
                if (!File.Exists(xmlpath))
                    return null;
                XmlDoc.Load(xmlpath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return XmlDoc;
        }
        
        /// <summary>
        /// 获取文档节点(singleselect为true 查询单节点，false 则查询节点集合)
        /// </summary>
        /// <param name="xmlpath">文件名.xml 或者 文件绝对路径</param>
        /// <param name="rootnode">根结点名称</param>
        /// <param name="xpath">xpath 语法</param>
        /// <param name="singleselect">是否查询单节点 （注：同一父节点下可能存在相同的多个子节点）</param>
        /// <returns></returns>
        public static object GetNode(string xmlpath, string rootnode, string xpath, bool singleselect)
        {
            object obj = null;
            if (!File.Exists(xmlpath) || (File.Exists(xmlpath) && !xmlpath.Contains(":")))
            {
                System.Uri u = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                xmlpath = System.IO.Path.GetDirectoryName(u.LocalPath) + "\\" + xmlpath;
            }

            XmlDocument XmlDoc = GetDocument(xmlpath);
            if (!string.IsNullOrEmpty(xpath))
                xpath = "/" + xpath;
            if (singleselect)
                obj = XmlDoc.SelectSingleNode("/" + rootnode + xpath);
            else
                obj = XmlDoc.SelectNodes("/" + rootnode + xpath);
            return obj;

        }
    }
}
