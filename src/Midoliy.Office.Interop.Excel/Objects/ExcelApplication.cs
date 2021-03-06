﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MsExcel = Microsoft.Office.Interop.Excel;

namespace Midoliy.Office.Interop.Objects
{
    internal class ExcelApplication : IExcelApplication
    {
        /// <summary>
        /// Excelの表示状態
        /// </summary>
        public AppVisibility Visibility { get; set; }

        /// <summary>
        /// ブック数
        /// </summary>
        public int Count
            => _children.Count;

        /// <summary>
        /// 指定したWorkbook名と一致するWorkbookを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IWorkbook this[string name] 
            => _children.First(c => c.Name == name);

        /// <summary>
        /// 指定したindex位置にあるWorkbookを取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IWorkbook this[int index] 
            => _children[index - 1];

        /// <summary>
        /// 指定したindex位置にあるWorkbookを取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IWorkbook Workbooks(int index)
            => this[index];

        /// <summary>
        /// 指定したindex位置にあるWorkbookを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IWorkbook Workbooks(string name)
            => this[name];

        /// <summary>
        /// 空のブックを作成
        /// </summary>
        /// <returns></returns>
        public IWorkbook BlankWorkbook()
        {
            var book = new ExcelWorkbook(_app.Workbooks.Add());
            _app.Calculation = MsExcel.XlCalculation.xlCalculationManual;
            _children.Add(book);
            return book;
        }

        /// <summary>
        /// テンプレートからブックを作成
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public IWorkbook CreateFrom(string templatePath)
        {
            var book = new ExcelWorkbook(_app.Workbooks.Add(Path.GetFullPath(templatePath)));
            _app.Calculation = MsExcel.XlCalculation.xlCalculationManual;
            _children.Add(book);
            return book;
        }

        /// <summary>
        /// 既存のブックを開く
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IWorkbook Open(string filePath)
        {
            var book = new ExcelWorkbook(_app.Workbooks.Open(Path.GetFullPath(filePath)));
            _app.Calculation = MsExcel.XlCalculation.xlCalculationManual;
            _children.Add(book);
            return book;
        }

        public IEnumerator<IWorkbook> GetEnumerator()
            => _children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        internal ExcelApplication()
        {
            _app = new MsExcel.Application();
            _app.IgnoreRemoteRequests = true;
            _app.DisplayAlerts = false;
            _children = new List<IWorkbook>();
            Visibility = AppVisibility.Hidden;
            _disposedValue = false;
        }

        private MsExcel.Application _app;
        private List<IWorkbook> _children;

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                if (_children.Any())
                    _app.Calculation = MsExcel.XlCalculation.xlCalculationAutomatic;

                foreach (var book in _children)
                {
                    if (Visibility == AppVisibility.Hidden)
                        book?.Close();

                    book?.Dispose();
                }

                if(_app != null)
                {
                    _app.IgnoreRemoteRequests = false;
                    _app.DisplayAlerts = true;

                    if (Visibility == AppVisibility.Hidden)
                        _app.Quit();
                    else
                        _app.Visible = true;

                    try { while (0 < Marshal.ReleaseComObject(_app)) { } } catch { }
                    _app = null;
                }
                GC.Collect();
            }

            _disposedValue = true;
        }
        
        public void Dispose()
            => Dispose(true);
        #endregion
    }
}
