using System;
using System.Collections.Generic;
using System.Linq;

namespace Echo.Editor
{
	/// <summary>
	/// 数据表
	/// </summary>
	public class TableModel
	{
		/// <summary>
		/// 行定义
		/// </summary>
		public class DataRow
		{
			private readonly List<object> m_cells;
			public List<object> Cells => m_cells;
		}

		/// <summary>
		/// 列定义
		/// </summary>
		public class DataColumn
		{
			/// <summary>
			/// 列名
			/// </summary>
			private readonly string m_name;
			public string Name => m_name;
			/// <summary>
			/// 数据类型
			/// </summary>
			private Type m_dataType;
			public Type DataType => m_dataType;

			public DataColumn(string name, Type dataType)
			{
				m_name = name;
				m_dataType = dataType;
			}
		}

		/// <summary>
		/// 列数据
		/// </summary>
		private readonly List<DataColumn> m_columns = null;
		public IReadOnlyList<DataColumn> Columns => m_columns;

		/// <summary>
		/// 行数据
		/// </summary>
		private readonly List<DataRow> m_rows = null;
		public IReadOnlyList<DataRow> Rows => m_rows;

		/// <summary>
		/// 列名字典
		/// </summary>
		private readonly Dictionary<string, int> m_columnNameMap = null;

		/// <summary>
		/// 行数
		/// </summary>
		public int RowCount => Rows.Count;
		/// <summary>
		/// 列数
		/// </summary>
		public int ColumnCount => m_columns.Count;

		public TableModel()
		{
			m_columns = new List<DataColumn>();
			m_rows = new List<DataRow>();
			m_columnNameMap = new Dictionary<string, int>();
		}

		/// <summary>
		/// 添加列
		/// </summary>
		/// <param name="columnName">列名</param>
		/// <param name="dataType">列对应的数据类型</param>
		public void AddColumn(string columnName, Type dataType)
		{
			if (string.IsNullOrEmpty(columnName))
				throw new ArgumentException("列名不能为空");
			if (m_columnNameMap.ContainsKey(columnName))
				throw new ArgumentException($"列已存在: {columnName}");

			var column = new DataColumn(columnName, dataType);
			m_columns.Add(column);
			m_columnNameMap[columnName] = m_columns.Count - 1;

			//补齐
			foreach (var row in m_rows)
			{
				row.Cells.Add(null);
			}
		}

		/// <summary>
		/// 移除指定列
		/// </summary>
		public void RemoveColumn(string columnName)
		{
			if (!m_columnNameMap.TryGetValue(columnName, out int columnIndex))
				throw new ArgumentException($"列不存在: {columnName}");

			m_columns.RemoveAt(columnIndex);
			m_columnNameMap.Remove(columnName);

			foreach (var row in m_rows)
			{ 
				row.Cells.RemoveAt(columnIndex);
			}

			for (int i = columnIndex; i < m_columns.Count; i++)
			{
				m_columnNameMap[m_columns[i].Name] = i;
			}
		}

		/// <summary>
		/// 添加行
		/// </summary>
		public void AddRow()
		{
			var row = new DataRow();
			for (int i = 0; i < ColumnCount; i++)
			{
				row.Cells.Add(null);
			}
			m_rows.Add(row);
		}

		/// <summary>
		/// 移除指定行
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		public void RemoveRow(int rowIndex)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));

			m_rows.RemoveAt(rowIndex);
		}

		/// <summary>
		/// 获取列名
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetHeaders()
		{
			return Columns.Select(col => col.Name);
		}

		/// <summary>
		/// 设置指定单元格值
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		/// <param name="columnIndex">列索引</param>
		/// <param name="value"></param>
		public void SetValue(int rowIndex, int columnIndex, object value)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));
			if (columnIndex < 0 || columnIndex >= ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnIndex));

			if (value == null)
				return;

			Type type = m_columns[columnIndex].DataType;
			if (!type.IsAssignableFrom(value.GetType()))
				throw new InvalidOperationException("数据类型不匹配");

			m_rows[rowIndex].Cells[columnIndex] = value;
		}

		/// <summary>
		/// 设置指定单元格值
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		/// <param name="columnName">列名</param>
		/// <param name="value"></param>
		public void SetValue(int rowIndex, string columnName, object value)
		{
			if (string.IsNullOrEmpty(columnName))
				return;

			if (!m_columnNameMap.TryGetValue(columnName, out int columnIndex))
				throw new ArgumentException($"列不存在: {columnName}");

			SetValue(rowIndex, columnIndex, value);
		}

		/// <summary>
		/// 获取指定单元格值
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		/// <param name="columnIndex">列索引</param>
		/// <returns></returns>
		public object GetValue(int rowIndex, int columnIndex)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));
			if (columnIndex < 0 || columnIndex >= ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnIndex));

			return m_rows[rowIndex].Cells[columnIndex];
		}

		/// <summary>
		/// 设置行数据
		/// </summary>
		public void SetRowValue(int rowIndex, IEnumerable<object> values)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));

			var list = values.ToList();
			for (int i = 0; i < Math.Min(list.Count, ColumnCount); i++)
			{
				SetValue(rowIndex, i, list[i]);
			}
		}

		/// <summary>
		/// 获取行数据
		/// </summary>
		public IReadOnlyList<object> GetRowValue(int rowIndex)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));

			return m_rows[rowIndex].Cells.AsReadOnly();
		}

		/// <summary>
		/// 清空表格
		/// </summary>
		public void Clear()
		{
			m_rows.Clear();
		}
	}
}
