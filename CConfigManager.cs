using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Common
{
    public class CConfigManager
    {

        private static string _cONFIG_ROOT_NODE = "CONFIG";
        private static string _cONFIG_FILE_NAME = "config.xml";

        public static string CONFIG_FILE_NAME
        {
            get { return _cONFIG_FILE_NAME; }
            set { _cONFIG_FILE_NAME = value; }
        }
        public static string CONFIG_ROOT_NODE
        {
            get { return _cONFIG_ROOT_NODE; }
            set { _cONFIG_ROOT_NODE = value; }
        }
        public CConfigManager()
        {
        }

        public static string GetConfig(string strConfigKey)
        {
            return GetConfig(strConfigKey, string.Empty);
        }
        public static string GetConfig(string strConfigKey, string strDefault)
        {
            return GetConfig(CONFIG_FILE_NAME, "", strConfigKey, strDefault);
        }
        public static string GetConfig(string strXmlFile, string strConfigKey, string strDefault)
        {
            return GetConfig(strXmlFile, "", strConfigKey, strDefault);
        }
        //<summary>
        //获取设置值
        //</summary>
        //<param name="strXmlFile">配置文件</param>
        //<param name="strSection">配置类型</param>
        //<param name="strConfigKey">配置项</param>
        //<param name="strDefault">默认值</param>
        //<returns></returns>
        public static string GetConfig(string strXmlFile, string strSection, string strConfigKey, string strDefault)
        {
            string Ret = string.Empty;

            try
            {
                System.Uri u = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

                string FileName = System.IO.Path.GetDirectoryName(u.LocalPath) + "\\" + strXmlFile;

                System.Xml.XmlDocument XmlDoc = new System.Xml.XmlDocument();

                XmlDoc.Load(FileName);

                System.Xml.XmlNode n = XmlDoc.SelectSingleNode("/" + CONFIG_ROOT_NODE + "/" + strSection + "/" + strConfigKey);

                if (n != null)
                {
                    Ret = n.InnerText;
                }
                else
                {
                    Ret = strDefault;
                }
            }
            catch (Exception ex)
            {
               // throw ex;
            }
            return Ret;
        }


        public static void SetConfig(string strConfigKey, string strValue)
        {
            SetConfig(CONFIG_FILE_NAME, CONFIG_ROOT_NODE, strConfigKey, strValue);
        }
        public static void SetConfig(string strXmlFile, string strSection, string strConfigKey, string strValue)
        {
            try
            {
                System.Uri u = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

                string sFileName = System.IO.Path.GetDirectoryName(u.LocalPath) + "\\" + strXmlFile;

                if (!File.Exists(sFileName))
                {
                    FileStream fs = File.Create(sFileName);
                    byte[] byData;
                    char[] charData;
                    //获得字符数组
                    charData = "<CONFIG></CONFIG>".ToCharArray();
                    //初始化字节数组
                    byData = new byte[charData.Length];
                    //将字符数组转换为正确的字节格式
                    Encoder enc = Encoding.UTF8.GetEncoder();
                    enc.GetBytes(charData, 0, charData.Length, byData, 0, true);
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Write(byData, 0, byData.Length);
                    fs.Close();
                }

                System.Xml.XmlDocument XmlDoc = new System.Xml.XmlDocument();

                XmlDoc.Load(sFileName);

                string xPath = "/" + CONFIG_ROOT_NODE + "/" + strSection;

                if (!string.IsNullOrEmpty(strConfigKey))
                    xPath += "/" + strConfigKey;

                System.Xml.XmlNode n = XmlDoc.SelectSingleNode(xPath);

                if (n != null)
                {
                    n.InnerText = strValue;
                }
                else
                {
                    System.Xml.XmlNode root = XmlDoc.SelectSingleNode(CONFIG_ROOT_NODE);
                    System.Xml.XmlNode xe0 = XmlDoc.SelectSingleNode("/" + CONFIG_ROOT_NODE + "/" + strSection);
                    if (xe0 == null)
                    {
                        xe0 = XmlDoc.CreateElement(strSection);//创建一个节点  
                        root.AppendChild(xe0);
                    }
                    if (!string.IsNullOrEmpty(strConfigKey))
                    {
                        System.Xml.XmlElement xe1 = XmlDoc.CreateElement(strConfigKey);//创建一个节点   
                        xe1.InnerText = strValue;
                        xe0.AppendChild(xe1);
                    }
                    else
                    {
                        xe0.InnerText = strValue;
                    }
                }

                //保存。
                XmlDoc.Save(sFileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

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
            if(!string.IsNullOrEmpty(xpath))
                xpath="/" + xpath;
            if (singleselect)
                obj = XmlDoc.SelectSingleNode("/" + rootnode + xpath);
            else
                obj = XmlDoc.SelectNodes("/" + rootnode + xpath);
            return obj;

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
            , string attributename,string attributevalue ,string subnode)
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
        /// 取得加密文档节点值
        /// </summary>
        /// <param name="xmlpath">文件名.xml 或者 文件绝对路径</param>
        /// <param name="rootnode">根结点名称</param>
        /// <param name="xpath">xpath 语法</param>
        /// <param name="attributename">属性名称</param>
        /// <param name="attributevalue">属性值</param>
        /// <param name="subnode">子节点</param>
        /// <returns></returns>
        public static string GetEncryValue(string xmlpath, string rootnode, string xpath
            , string attributename, string attributevalue, string subnode)
        {
            string sRtnVal = string.Empty;

            try
            {
                XmlDocument XmlDoc = GetDecryptXmlDoc(xmlpath);

                if (!string.IsNullOrEmpty(xpath))
                    xpath = "/" + xpath;

                XmlNode n = XmlDoc.SelectSingleNode("/" + rootnode + xpath);

                if (!string.IsNullOrEmpty(attributename))
                {
                    XmlNodeList nodes = XmlDoc.SelectNodes("/" + rootnode + xpath);
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
                                            sRtnVal = n2.InnerText;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    sRtnVal = n1.InnerText;
                                }
                                break;

                            }
                        }
                    }
                }
                else
                {
                    sRtnVal = n.InnerText.Trim();
                }
            }
            catch (Exception)
            {
            }

            return sRtnVal;
        }

        /// <summary>
        /// 取得加密文档节点值
        /// </summary>
        /// <param name="XmlDoc">XML文档</param>
        /// <param name="rootnode">根结点名称</param>
        /// <param name="xpath">xpath 语法</param>
        /// <param name="attributename">属性名称</param>
        /// <param name="attributevalue">属性值</param>
        /// <param name="subnode">子节点</param>
        /// <returns></returns>
        public static string GetEncryValueByDoc(XmlDocument XmlDoc, string rootnode, string xpath
            , string attributename, string attributevalue, string subnode)
        {
            string sRtnVal = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(xpath))
                    xpath = "/" + xpath;

                XmlNode n = XmlDoc.SelectSingleNode("/" + rootnode + xpath);

                if (!string.IsNullOrEmpty(attributename))
                {
                    XmlNodeList nodes = XmlDoc.SelectNodes("/" + rootnode + xpath);
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
                                            sRtnVal = n2.InnerText;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    sRtnVal = n1.InnerText;
                                }
                                break;

                            }
                        }
                    }
                }
                else
                {
                    sRtnVal = n.InnerText.Trim();
                }
            }
            catch (Exception)
            {
            }

            return sRtnVal;
        }

        /// <summary>
        /// 获取解密的XML文档
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <returns></returns>
        public static XmlDocument GetDecryptXmlDoc(string xmlpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //默认密钥
            string sPwd = string.Empty;

            using (FileStream fs = new FileStream(xmlpath, FileMode.Open, FileAccess.Read))
            {
                byte[] byteAllFile = null;

                if (fs.Length > 0)
                {
                    int blockCount = ((int)fs.Length - 1) / 10000016 + 1;
                    int iCopyIndex = 0;
                    for (int i = 0; i < blockCount; i++)
                    {
                        int size = 10000016;
                        if (i == blockCount - 1) size = (int)(fs.Length - i * 10000016);
                        byte[] bArr = new byte[size];
                        fs.Read(bArr, 0, size);
                        byte[] result = AES.AESDecrypt(bArr, sPwd);
                        if (byteAllFile == null)
                        {
                            byteAllFile = new byte[result.Length];
                            System.Array.Copy(result, 0, byteAllFile, iCopyIndex, result.Length);
                            iCopyIndex = 0;
                        }
                        else
                        {

                            byte[] srcAllFile = new byte[byteAllFile.Length];

                            byteAllFile.CopyTo(srcAllFile, 0);

                            byteAllFile = new byte[result.Length + byteAllFile.Length];

                            System.Array.Copy(srcAllFile, 0, byteAllFile, 0, srcAllFile.Length);

                            iCopyIndex = srcAllFile.Length;

                            System.Array.Copy(result, 0, byteAllFile, iCopyIndex, result.Length);

                            iCopyIndex = 0;
                        }

                    }
                }

                Encoding enc = Encoding.UTF8;
                string text = enc.GetString(byteAllFile);
                string sToLoadXml = text;
                if(text.IndexOf("?>")!=-1){
                    sToLoadXml = text.Substring(text.IndexOf("?>")+2);
                }

                xmlDoc.LoadXml(sToLoadXml);
            }

            return xmlDoc;

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
        private static string AddNode(string filename, string rootnode, string node, string attributename, string attributevalue, string subnode, string subatt, string subattvalue)
        {
          return  AddNode(filename,rootnode,node,attributename,attributevalue,subnode,subatt,subattvalue,null);
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
        public static void SetEncryValue(string filename, string rootnode, string node, string value, string attname, string attvalue, string subnode, string subvalue, string subatt, string subattvalue)
        {
            Common.CEncryptDecrypt cen = new Common.CEncryptDecrypt();
            bool bIfDecry = false;
            try
            {
                //解密文件
                if (File.Exists(filename))
                {
                    cen.DecryptFile(filename, string.Empty);
                }
                bIfDecry = true;

                SetValue(filename, rootnode, node, value, attname, attvalue, subnode, subvalue, subatt, subattvalue, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bIfDecry)
                {
                    //加密文件
                    cen.EncryptFile(filename, string.Empty);
                }
            }
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
        public static void SetValue(string filename, string rootnode, string node, string value, string attname, string attvalue, string subnode, string subvalue, string subatt, string subattvalue)
        {
            SetValue(filename, rootnode, node, value, attname, attvalue, subnode, subvalue, subatt, subattvalue, null);
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
        private static string AddNode(string filename, string rootnode, string node, string attributename, string attributevalue, string subnode, string subatt, string subattvalue,object [] prams)
        {
             bool saveorno = false;
            System.Uri u = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

            string xmlpath = System.IO.Path.GetDirectoryName(u.LocalPath) + "\\" + filename;
            if(prams!=null && prams.Length>0)
            {
                if(File.Exists(filename))
                    xmlpath=filename;               
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
        /// 根据根节点复制XML文件
        /// </summary>
        /// <param name="sXmlinfile"></param>
        /// <param name="sXmloutfile"></param>
        /// <param name="sRootnode"></param>
        public static void ReadWriteCopy(string sXmlinfile, string sXmloutfile,string sRootnode)
        {
            try
            {
                //生成Layout.xml
                SetValue(sXmlinfile, sRootnode, "form", "", "", "", "", "", "", "");
                
                //in xml
                XmlDocument doc_in = new XmlDocument();
                doc_in.Load(sXmlinfile);

                //out xml
                XmlDocument doc_out = new XmlDocument();
                doc_out.Load(sXmloutfile);

                doc_in.Load(sXmlinfile);
                doc_out.Load(sXmloutfile);
                doc_in[sRootnode].InnerXml = doc_out[sRootnode].InnerXml;

                //保存需复制的文件
                doc_in.Save(sXmlinfile);
            }
            catch (Exception e)
            {
                throw e;
            }
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
        public static void SetValue(string filename, string rootnode, string node, string value, string attname, string attvalue, string subnode, string subvalue, string subatt, string subattvalue,object [] prams)
        {
            rootnode = rootnode.Trim();
            node = node.Trim();
            subnode = subnode.Trim();
            bool saveorno = false;
            string xmlpath = AddNode(filename, rootnode, node, attname, attvalue, subnode, subatt, subattvalue,prams);
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


        public static void SetNodes(string filename, string rootnode, string node, string value)
        {
            XmlDocument XmlDoc = GetDocument(filename);
            XmlNodeList nodes = XmlDoc.SelectNodes("/" + rootnode + "/" + node);
            int i = 0;//根据比较次数决定value在不在nodes 中
            foreach (XmlNode n in nodes)
            {
                if (n.InnerText.Equals(value))              
                    break;             
                else               
                    i++;               
            }
            if (i == nodes.Count)
            {
                XmlElement root = XmlDoc.SelectSingleNode("/" + rootnode) as XmlElement;
                XmlElement sub = XmlDoc.CreateElement(node);
                sub.InnerText = value;
                root.AppendChild(sub);
                XmlDoc.Save(filename);
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="messages"></param>
        public static void AppendLogFile(string messages)
        {
            try
            {
                string filePath = @"D:\tcmislog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                StreamWriter sw = null;
                sw = new StreamWriter(filePath, true, Encoding.UTF8);
                sw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + messages);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }

        public static string GetXmlStringValue(string sXmlString, string sPath)
        {
            string Ret = string.Empty;

            try
            {

                System.Xml.XmlDocument XmlDoc = new System.Xml.XmlDocument();

                XmlDoc.LoadXml(sXmlString);
                //XmlDoc.Load(FileName);

                System.Xml.XmlNode n = XmlDoc.SelectSingleNode(sPath);

                if (n != null)
                {
                    Ret = n.InnerText;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ret;
        }

    }
}
