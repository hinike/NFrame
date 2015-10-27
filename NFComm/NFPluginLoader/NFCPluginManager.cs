﻿//-----------------------------------------------------------------------
// <copyright file="NFCPluginManager.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFrame>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFrame;
using System.Collections.Concurrent;
using System.Xml;

namespace NFrame
{
    public class NFCPluginManager : NFIPluginManager
    {
        private static NFCPluginManager _instance;
        private static readonly object _syncLock = new object();
        public static NFCPluginManager Intance()
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NFCPluginManager();
                    }
                }
            }

            return _instance;
        }

        public override void Install()
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load("Plugin.xml");
            XmlNode xRoot = xmldoc.SelectSingleNode("XML");

            XmlNodeList xPluginNodeList = xRoot.SelectNodes("Plugin");
            for (int i = 0; i < xPluginNodeList.Count; ++i)
            {
                XmlNode xNodeClass = xPluginNodeList.Item(i);
                XmlAttribute strID = xNodeClass.Attributes["Name"];
                string strLibName = strID.Value;

                NFCDynLib xPlugin = new NFCDynLib(strLibName);
                mxPluginDic.TryAdd(strLibName, xPlugin);
            }

            XmlNode xAPPIDNode = xRoot.SelectSingleNode("APPID");
            XmlAttribute strAppID = xAPPIDNode.Attributes["Name"];
            int nID = 0;
            int.TryParse(strAppID.Value, out nID);
            SetSelf(new NFIDENTID(nID, 0));

            XmlNode xClassPathNode = xRoot.SelectSingleNode("ClassPath");
            XmlAttribute strClassPathID = xClassPathNode.Attributes["Name"];
            mstrClassPath = strClassPathID.Value;
        }

        public override void UnInstall()
        {

        }

        public override void Init() 
        {
            foreach(var x in mxPluginDic)
            {
                x.Value.Init();
            }
        }

        public override void AfterInit() 
        {
            foreach (var x in mxPluginDic)
            {
                x.Value.AfterInit();
            }
        }

        public override void BeforeShut() 
        {
            foreach (var x in mxPluginDic)
            {
                x.Value.BeforeShut();
            }
        }

        public override void Shut() 
        {
            foreach (var x in mxPluginDic)
            {
                x.Value.Shut();
            }
        }

        public override void Execute() 
        {
            foreach (var x in mxPluginDic)
            {
                x.Value.Execute();
            }
        }

        /////////////////////////////////////////////////////////        
        private readonly ConcurrentDictionary<string, NFCDynLib> mxPluginDic = new ConcurrentDictionary<string, NFCDynLib>();
        private string mstrClassPath;
    }
}