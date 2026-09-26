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
		public class DataRow
		{
			private object[] m_cells = null;
			public object[] Cells => m_cells;

			public DataRow(int columnCount)
			{
				m_cells = new object[columnCount];
			}

			public void SetCells(object[] newCells)
			{
				m_cells = newCells;
			}

			public object this[int index]
			{
				get { return m_cells[index]; }
				set { m_cells[index] = value; }
			}
		}

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
			private readonly Type m_dataType;
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
		public List<DataColumn> Columns => m_columns;

		/// <summary>
		/// 行数据
		/// </summary>
		private readonly List<DataRow> m_rows = null;
		public List<DataRow> Rows => m_rows;

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

		public void AddColumn(string columnName, Type dataType)
		{
			if (string.IsNullOrEmpty(columnName))
				throw new ArgumentException("列名不能为空");
			if (m_columnNameMap.ContainsKey(columnName))
				throw new ArgumentException($"列已存在: {columnName}");

			var column = new DataColumn(columnName, dataType);
			m_columns.Add(column);
			int newColumnCount = m_columns.Count;
			m_columnNameMap[columnName] = newColumnCount - 1;

			for (int i = 0; i < m_rows.Count; i++)
			{
				var cells = m_rows[i].Cells;
				var newCells = new object[newColumnCount];

				Array.Copy(cells, newCells, cells.Length);
				m_rows[i].SetCells(newCells);
			}
		}

		public void RemoveColumn(string columnName)
		{
			if (!m_columnNameMap.TryGetValue(columnName, out int columnIndex))
				throw new ArgumentException($"列不存在: {columnName}");

			m_columns.RemoveAt(columnIndex);
			m_columnNameMap.Remove(columnName);

			//删除对应单元格
			for (int i = 0; i < m_rows.Count; i++)
			{
				var cells = m_rows[i].Cells;
				var newCells = new object[cells.Length - 1];

				if (columnIndex > 0)
					Array.Copy(cells, 0, newCells, 0, columnIndex);
				if (columnIndex < cells.Length - 1)
					Array.Copy(cells, columnIndex + 1, newCells, columnIndex, cells.Length - columnIndex - 1);

				m_rows[i].SetCells(newCells);
			}

			//更新索引映射
			for (int i = columnIndex; i < m_columns.Count; i++)
			{
				m_columnNameMap[m_columns[i].Name] = i;
			}
		}

		public void AddRow(object[] values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));

			if (values.Length != ColumnCount)
				throw new ArgumentException($"数据列数不匹配: {values.Length} != {ColumnCount}");	

			var row = new DataRow(ColumnCount);
			Array.Copy(values, row.Cells, ColumnCount);
			m_rows.Add(row);
		}

		public void RemoveRow(int rowIndex)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));
			
			m_rows.RemoveAt(rowIndex);
		}

		/// <summary>
		/// 设置指定单元格值
		/// </summary>
		public void SetValue(int rowIndex, int columnIndex, object value)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));
			if (columnIndex < 0 || columnIndex >= ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnIndex));

			if (value != null)
			{
				Type type = m_columns[columnIndex].DataType;
				if (!type.IsAssignableFrom(value.GetType()))
					throw new InvalidOperationException("数据类型不匹配");
			}

			m_rows[rowIndex][columnIndex] = value;
		}

		/// <summary>
		/// 设置指定单元格值
		/// </summary>
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
		public object GetValue(int rowIndex, int columnIndex)
		{
			if (rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException(nameof(rowIndex));
			if (columnIndex < 0 || columnIndex >= ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(columnIndex));

			return m_rows[rowIndex].Cells[columnIndex];
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
		/// 清空表格
		/// </summary>
		public void Clear()
		{
			m_rows.Clear();
			m_columns.Clear();
			m_columnNameMap.Clear();
		}
	}
}
