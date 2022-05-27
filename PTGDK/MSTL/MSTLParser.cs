
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PTGDK.Utility;

namespace PTGDK.MSTL
{
    public class MSTLParser
    {
        private const char MSTL_BLOCK_IDENTIFIER = '@';
        private const string MSTL_LIST_DATATYPE = "LIST";
        private const string MSTL_VALUE_DATATYPE = "VALUE";
        private const string MSTL_MAP_DATATYPE = "MAP";
        private const string MSTL_BOOL_DATATYPE = "BOOL";
        
        private readonly IDictionary<string, string> _mstlBlocksDictionary;
            
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mstlText">The MSTL text</param>
        public MSTLParser(string mstlText)
        {
            _mstlBlocksDictionary = new Dictionary<string, string>();
            
            PopulateBlockDictionary(mstlText);
        }

        private void PopulateBlockDictionary(string mstlText)
        {
            foreach (string block in mstlText.Trim().Split(MSTL_BLOCK_IDENTIFIER))
            {
                if (block.Equals(""))
                {
                    continue;
                }
                _mstlBlocksDictionary.Add(block.Substring(0, block.IndexOf(';')).Split(',')[0], block);
            }
        }
        
        /// <summary>
        /// Parse a list from MSTL as a specific type
        /// </summary>
        /// <param name="name">The name of the block to retrieve the list from</param>
        /// <typeparam name="T">The target type</typeparam>
        /// <returns>The list containing the target type</returns>
        public IList<T> ParseList<T>(string name)
        {
            IList<string> list = ParseList(name);
            T[] array = new T[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                array[i] = (T) Convert.ChangeType(list[i], typeof(T));
            }
            return array.ToList();
        }
        
        /// <summary>
        /// Parse a list from MSTL as a list of strings
        /// </summary>
        /// <param name="name">The name of the block to retrieve the list from</param>
        /// <returns>The list containing the values as string</returns>
        public IList<string> ParseList(string name)
        {
            string blockText = GetAndValidateBlock(name);
            blockText = blockText.Trim();
            return ValidateAndRemoveBlockHeader(blockText, MSTL_LIST_DATATYPE, "ParseList");
        }

        /// <summary>
        /// Parse a value from MSTL as a specific type
        /// </summary>
        /// <param name="name">The name of the block to retrieve the value from</param>
        /// <typeparam name="T">The target type</typeparam>
        /// <returns>A value of the target type</returns>
        public T ParseValue<T>(string name)
        {
            return (T) Convert.ChangeType(ParseValue(name), typeof(T));
        }
        
        /// <summary>
        /// Parse a value from MSTL as a string
        /// </summary>
        /// <param name="name">The name of the block to retrieve the value from</param>
        /// <returns>A string value</returns>
        public string ParseValue(string name)
        {
            string blockText = GetAndValidateBlock(name);
            blockText = blockText.Trim();
            IList<string> blockList = ValidateAndRemoveBlockHeader(blockText, MSTL_VALUE_DATATYPE, "ParseValue");
            return blockList.Count > 0 ? blockList[0] : "";
        }

        /// <summary>
        /// Parse a bool from MSTL
        /// </summary>
        /// <param name="name">The name of the block to retrieve the value from</param>
        /// <returns>A boolean value</returns>
        public bool ParseBool(string name)
        {
            string blockText = GetAndValidateBlock(name);
            blockText = blockText.Trim();
            IList<string> blockList = ValidateAndRemoveBlockHeader(blockText, MSTL_BOOL_DATATYPE, "ParseBool");
            return blockList.Count > 0 && bool.Parse(blockList[0]);
        }

        /// <summary>
        /// Parse a map from MSTL as a specific key value type
        /// </summary>
        /// <param name="name">Name of the block to retrieve the map from</param>
        /// <typeparam name="T">key type</typeparam>
        /// <typeparam name="V">value type</typeparam>
        /// <returns>The map of the specified key value types</returns>
        public IDictionary<T, V> ParseMap<T, V>(string name)
        {
            IDictionary<T, V> map = new Dictionary<T, V>();
            foreach (KeyValuePair<string, string> entry in ParseMap(name))
            {
                map.Add((T) Convert.ChangeType(entry.Key, typeof(T)), 
                        (V) Convert.ChangeType(entry.Value, typeof(V)));
            }
            return map;
        }
        
        /// <summary>
        /// Parse a map from MSTL where both the key and value are strings
        /// </summary>
        /// <param name="name">Name of the block to retrieve the map from</param>
        /// <returns>The map with key value strings</returns>
        public IDictionary<string, string> ParseMap(string name)
        {
            string blockText = GetAndValidateBlock(name);
            blockText = blockText.Trim();
            if (blockText == "")
            {
                return new Dictionary<string, string>();
            }
            IList<String> blockList = ValidateAndRemoveBlockHeader(blockText, MSTL_MAP_DATATYPE, "ParseMap");
            IDictionary<string, string> blockMap = new Dictionary<string, string>();
            foreach (string blockLine in blockList)
            {
                string[] keyValue = blockLine.Split(':');
                blockMap.Add(keyValue[0].Trim(), keyValue[1].Trim());
            }
            return blockMap;
        }
        
        private IList<string> ValidateAndRemoveBlockHeader(string block, string targetDataType, string method)
        {
            IList<String> blockList = block.Split('\n').ToList();
            string dataType = blockList[0].Split(',')[1].Replace(";", "");
            if (!StringUtility.EqualsCaseInsensitive(dataType, targetDataType))
            {
                throw new ArgumentException(String.Format("Method {0} can not be used to parse MSTL block of type {1}", method, targetDataType));
            }
            blockList.RemoveAt(0);
            return blockList;
        }
        
        private string GetAndValidateBlock(string name)
        {
            String block = _mstlBlocksDictionary[name];
            if (block == null)
            {
                throw new ArgumentException("No MSTL block with name %s found", name);
            }
            return block;
        }
    }
}