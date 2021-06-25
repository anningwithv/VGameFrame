//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections.Generic;
using UnityEngine;
#if !USE_TABLE_XC
//&& (UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_STANDALONE_OSX)
namespace VGameFramework
{
    class TxtReader
    {
        struct LineData
        {
            public string[] col;
        }

        string          m_FileData = "";
        int             m_ColCount;
        string[]        m_ColName;
        List<LineData>  _lines = new List<LineData>();
        List<string>    lineList = new List<string>(512);

        public bool ParseData(string fileData, int start_line = 3, int colNameLine = 0)
        {
            m_FileData = fileData;

            if (null == m_FileData)
            {
                return false;
            }

            int line_num = 0;
            int filed_cur = 0;
            string field;
            int fieldLen;
            for (int i = 0; i < m_FileData.Length; ++i)
            {
                switch (m_FileData[i])
                {
                    case '\t':
                        field = m_FileData.Substring(filed_cur, i - filed_cur);
                        lineList.Add(field);
                        filed_cur = i + 1;
                        continue;
                    case '\r':
                        {
                            // 忽略
                            continue;
                        }
                    case '\n':
                        {
                            ++line_num;

                            if ((start_line > 0) && ((int)line_num < start_line))
                            {
                                continue;
                            }

                            if (m_FileData[i - 1] == '\r')
                            {
                                fieldLen = i - filed_cur - 1;
                            }
                            else
                            {
                                fieldLen = i - filed_cur;
                            }
                            field = m_FileData.Substring(filed_cur, fieldLen);
                            lineList.Add(field);
                            LineData ld;
                            ld.col = lineList.ToArray();
                            _lines.Add(ld);
                            filed_cur = i + 1;
                            if (0 == m_ColCount)
                            {
                                m_ColCount = lineList.Count;
                            }
                            if (line_num - 1 == colNameLine)
                            {
                                m_ColName = ld.col;
                            }
                            lineList.Clear();

                            //if (i <= last_start)
                            //{
                            //    last_start = i + 1;
                            //    continue;
                            //}
                            //
                            //int end = i - last_start;
                            //
                            //
                            ////string line = m_FileData.Substring(last_start, end);
                            ////string[] lineData = line.Split(new char[] { '\t' });
                            //if (m_FileData[i] == '\t')
                            //{
                            //    string field = m_FileData.Substring(filed_cur, i - filed_cur);
                            //    lineList.Add(field);
                            //    continue;
                            //}
                            //
                            //if (m_FileData[i - 1] == '\r')
                            //{
                            //    --end;
                            //}
                            //string[] lineData = lineList.ToArray();
                            //lineList.Clear();
                            //
                            //if (0 == m_ColCount)
                            //{
                            //    m_ColCount = lineData.Length;
                            //    m_ColName = lineData;
                            //}
                            //
                            //if (m_ColCount != lineData.Length)
                            //{
                            //    //Log.e("[Error]TxtReader::ParseData Error line is " + line);
                            //}
                            //else
                            //{
                            //    LineData ld;
                            //    ld.col = lineData;
                            //
                            //    _lines.Add(ld);
                            //}
                            //
                            //// 跳过\n
                            //last_start = i + 1;
                        }
                        break;

                    default:
                        break;
                }
            }

            return true;
        }

        public string[] GetColName()
        {
            return m_ColName;
        }

        public string GetField(int row, int colomu)
        {
            return _lines[row].col[colomu];
        }

        public string[] GetARow(int row)
        {
            return _lines[row].col;
        }

        public int GetCols()
        {
            return m_ColCount;
        }

        public int GetRows()
        {
            return _lines.Count;
        }

        public int ReadInt(int row, string colName)
        {
            string data = GetData(row, colName);

            if (null == data)
            {
                return 0;
            }
            else
            {
                return Helper.String2Int(data);
            }
        }

        public bool ReadBool(int row, string colName)
        {
            string data = GetData(row, colName);

            if (null == data)
            {
                return false;
            }
            else
            {
                return Helper.String2Bool(data);
            }
        }

        public long ReadInt64(int row, string colName)
        {
            string data = GetData(row, colName);

            if (null == data)
            {
                return 0;
            }
            else
            {
                return Helper.String2Int64(data);
            }
        }

        public string ReadString(int row, string colName)
        {
            string data = GetData(row, colName);

            if (null == data)
            {
                return "";
            }
            else
            {
                return data;
            }
        }

        public float ReadFloat(int row, string colName)
        {
            string data = GetData(row, colName);

            if (null == data)
            {
                return 0f;
            }
            else
            {
                return Helper.String2Float(data);
            }
        }

        public int[] ReadIntArray(int row, string colName, char split)
        {
            string data = GetData(row, colName);

            if (!Helper.IsConfigStringValid(data))
            {
                return new int[0];
            }

            string[] datas = data.Split(new char[] { split });

            int[] rets = new int[datas.Length];

            for (int i = 0; i < rets.Length; ++i)
            {
                rets[i] = Helper.String2Int(datas[i]);
            }

            return rets;
        }

        public long[] ReadInt64Array(int row, string colName, char split)
        {
            string data = GetData(row, colName);

            if (!Helper.IsConfigStringValid(data))
            {
                return new long[0];
            }

            string[] datas = data.Split(new char[] { split });

            long[] rets = new long[datas.Length];

            for (int i = 0; i < rets.Length; ++i)
            {
                rets[i] = Helper.String2Int64(datas[i]);
            }

            return rets;
        }

        public string[] ReadStringArray(int row, string colName, char split)
        {
            string data = GetData(row, colName);

            if (!Helper.IsConfigStringValid(data))
            {
                return new string[0];
            }

            string[] datas = data.Split(new char[] { split });

            return datas;
        }

        string GetData(int row, string colName)
        {
            if (row >= _lines.Count)
            {
                return null;
            }

            int col = GetColIndex(colName);

            if (col < 0)
            {
                return null;
            }

            return _lines[row].col[col];
        }

        int GetColIndex(string colName)
        {
            if (null == colName)
            {
                return -1;
            }

            for (int i = 0; i < m_ColName.Length; ++i)
            {
                if (m_ColName[i] == colName)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
#endif
