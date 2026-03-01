using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor.UI
{
	/// <summary>
	/// 表格控件
	/// </summary>
	public class TableWidget : VisualElement
	{
		/// <summary>
		/// 列类型
		/// </summary>
		public enum ColumnType
		{
			None,
			Edit,
			Combo,
			EnumCombo,
			Toggle,
			Object
		}

		/// <summary>
		/// 行数据
		/// </summary>
		private class RowData
		{
			public List<object> Cells = new List<object>();
		}

		/// <summary>
		/// 列项配置
		/// </summary>
		public class ColumnItem
		{
			/// <summary>
			/// 类型
			/// </summary>
			public ColumnType Type = ColumnType.None;
			/// <summary>
			/// 宽度
			/// </summary>
			public float Width = 80;

			/// <summary>
			/// 下拉选项（仅当 ColumnType 为 Combo 时有效）
			/// </summary>
			public List<string> Options;
			/// <summary>
			/// 枚举类型（仅当 ColumnType 为 EnumCombo 时有效）
			/// </summary>
			public Type EnumType;
			/// <summary>
			/// 对象类型（仅当 ColumnType 为 Object 时有效）
			/// </summary>
			public Type ObjectType;
		}

		private MultiColumnListView m_listView = null;
		
		/// <summary>
		/// 行数
		/// </summary>
		private int m_rowCount = 0;
		public int RowCount => m_rowCount;

		/// <summary>
		/// 列数
		/// </summary>
		private int m_columnCount = 0;
		public int ColumnCount => m_columnCount;

		/// <summary>
		/// 表头
		/// </summary>
		private List<string> m_headerLabels = null;
		public IReadOnlyList<string> HeaderLabels => m_headerLabels;

		/// <summary>
		/// 表格行数据
		/// </summary>
		private List<RowData> m_rowDatas = null;
		/// <summary>
		/// 表格列项
		/// </summary>
		private List<ColumnItem> m_columnItems = null;

		/// <summary>
		/// 单元格上下文: 用于事件回调中识别单元格位置
		/// </summary>
		private class CellContext
		{
			public int Row;
			public int Column;
		}

		public TableWidget()
		{
			m_headerLabels = new List<string>();
			m_rowDatas = new List<RowData>();
			m_columnItems = new List<ColumnItem>();
			
			InitWidget();
		}

		public TableWidget(int row, int column)
		{
			m_rowCount = row;
			m_columnCount = column;
			m_headerLabels = new List<string>();
			m_rowDatas = new List<RowData>();
			m_columnItems = new List<ColumnItem>();

			InitWidget();
			InitColumns();
			InitRows();

			//绑定数据
			m_listView.itemsSource = m_rowDatas;
			m_listView.RefreshItems();
		}

		#region Public Funcs
		/// <summary>
		/// 设置表格大小
		/// </summary>
		/// <param name="rows">行数</param>
		/// <param name="columns">列数</param>
		public void SetTableSize(int rows, int columns)
		{
			if (rows < 0)
				throw new ArgumentException("行数不可小于0!");
			if (columns < 0)
				throw new ArgumentException("列数不可小于0!");

			if (columns != m_columnCount)
			{
				m_columnCount = columns;
			}

			m_rowCount = rows;
			InitRows();

			//绑定数据
			m_listView.itemsSource = m_rowDatas;
			m_listView.RefreshItems();
		}

		/// <summary>
		/// 设置表头
		/// </summary>
		public void SetHeaderLabels(IEnumerable<string> labels)
		{
			if (labels == null)
				throw new ArgumentNullException(nameof(labels));

			var list = new List<string>(labels);
			if (m_columnCount == 0)
			{
				m_columnCount = list.Count;
			}
			else if (list.Count != m_columnCount)
			{
				throw new ArgumentException($"表头数量({list.Count})必须等于列数({m_columnCount})!");
			}

			m_headerLabels = list;
			InitColumns();
		}

		/// <summary>
		/// 设置列类型和属性
		/// </summary>
		/// <param name="index">列索引</param>
		/// <param name="item">列项配置</param>
		public void SetColumnType(int index, ColumnItem item)
		{
			if (index < 0 || index >= m_columnCount)
				throw new ArgumentOutOfRangeException(nameof(index), $"列索引越界: {index}，当前列数为 {m_columnCount}");
			if (item == null)
				throw new ArgumentNullException(nameof(item), "ColumnItem 参数不能为空，必须传入有效的列项配置!");

			if (m_columnItems == null)
			{
				m_columnItems = new List<ColumnItem>(new ColumnItem[m_columnCount]);
			}
			m_columnItems[index] = item;

			//初始化
			InitColumn(index);
			m_listView.RefreshItems();
		}

		/// <summary>
		/// 设置指定单元格值
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		/// <param name="colIndex">列索引</param>
		/// <param name="value">值</param>
		public void SetCellValue(int rowIndex, int colIndex, object value)
		{
			if (rowIndex < 0 || rowIndex >= m_rowCount)
				return;
			if (colIndex < 0 || colIndex >= m_columnCount)
				return;

			m_rowDatas[rowIndex].Cells[colIndex] = value;

			//刷新
			m_listView.RefreshItem(rowIndex);
		}

		/// <summary>
		/// 获取制定单元格值
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		/// <param name="colIndex">列索引</param>
		/// <returns></returns>
		public object GetCellValue(int rowIndex, int colIndex)
		{
			if (rowIndex < 0 || rowIndex >= m_rowCount)
				return null;
			if (colIndex < 0 || colIndex >= m_columnCount)
				return null;

			return m_rowDatas[rowIndex].Cells[colIndex];
		}

		/// <summary>
		/// 设置整行数据
		/// </summary>
		public void SetRow(int rowIndex, IEnumerable<object> values)
		{
			if (rowIndex < 0 || rowIndex >= m_rowCount)
				return;

			var list = values.ToList();
			for (int i = 0; i < Math.Min(list.Count, m_columnCount); i++)
			{
				m_rowDatas[rowIndex].Cells[i] = list[i];
			}

			//刷新
			m_listView.RefreshItem(rowIndex);
		}

		/// <summary>
		/// 获取整行数据
		/// </summary>
		public IReadOnlyList<object> GetRow(int rowIndex)
		{
			if (rowIndex < 0 || rowIndex >= m_rowCount)
				return null;

			return m_rowDatas[rowIndex].Cells.AsReadOnly();
		}
		#endregion

		private void InitWidget()
		{
			m_listView = new MultiColumnListView();
			this.Add(m_listView);

			m_listView.fixedItemHeight = 25;
			m_listView.showBorder = true;
			m_listView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
			m_listView.selectionType = SelectionType.None;
		}

		private void InitRows()
		{
			m_rowDatas.Clear();
			for (int i = 0; i < m_rowCount; i++)
			{
				var row = new RowData();
				for (int j = 0; j < m_columnCount; j++)
				{
					row.Cells.Add(null);
				}
				m_rowDatas.Add(row);
			}
		}

		private void InitColumns()
		{
			m_columnItems.Clear();
			for (int i = 0; i < m_columnCount; i++)
				m_columnItems.Add(new ColumnItem());

			m_listView.columns.Clear();
			for (int i = 0; i < m_columnCount; i++)
			{
				int colIndex = i;
				var column = new Column
				{
					title = GetHeaderLabel(colIndex),
					width = 80
				};
				m_listView.columns.Add(column);
				InitColumn(colIndex, false);
			}

			//刷新
			m_listView.RefreshItems();
		}

		/// <summary>
		/// 初始化指定列
		/// </summary>
		/// <param name="colIndex">列索引</param>
		/// <param name="refresh">是否立刻刷新列表</param>
		private void InitColumn(int colIndex, bool refresh = true)
		{
			if (colIndex >= m_listView.columns.Count)
				return;

			var colItem = m_columnItems[colIndex];
			Action<VisualElement, int> unbindCell = (element, rowIndex) =>
			{
				element.userData = null;
			};

			var result = CreateColumnFactory(colItem, colIndex);
			var column = m_listView.columns[colIndex];
			column.makeCell = result.Item1;
			column.bindCell = result.Item2;
			column.unbindCell = unbindCell;
			column.width = colItem.Width;

			//刷新
			if (refresh)
				m_listView.RefreshItems();
		}

		private string GetHeaderLabel(int index)
		{
			if (m_headerLabels.Count == 0 || index >= m_headerLabels.Count)
			{
				return string.Empty;
			}
			return m_headerLabels[index];
		}

		private (Func<VisualElement>, Action<VisualElement, int>) CreateColumnFactory(ColumnItem colItem, int colIndex)
		{
			switch (colItem.Type)
			{
				case ColumnType.None:
					return CreateDefaultColumn(colIndex);
				case ColumnType.Edit:
					return CreateEditColumn(colIndex);
				case ColumnType.Combo:
					return CreateComboColumn(colItem, colIndex);
				case ColumnType.EnumCombo:
					return CreateEnumComboColumn(colItem, colIndex);
				case ColumnType.Toggle:
					return CreateToggleColumn(colIndex);
				case ColumnType.Object:
					return CreateObjectColumn(colItem, colIndex);
				default:
					break;
			}
			return (null, null);
		}

		/// <summary>
		/// 创建默认列
		/// </summary>
		private (Func<VisualElement>, Action<VisualElement, int>) CreateDefaultColumn(int colIndex)
		{
			Func<VisualElement>  makeCell = () => new Label();
			Action<VisualElement, int>  bindCell = (element, rowIndex) =>
			{
				var label = element as Label;
				label.text = m_rowDatas[rowIndex].Cells[colIndex]?.ToString();
			};

			return (makeCell, bindCell);
		}

		/// <summary>
		/// 创建编辑文本列
		/// </summary>
		private (Func<VisualElement>, Action<VisualElement, int>) CreateEditColumn(int colIndex)
		{
			Func<VisualElement> makeCell = () =>
			{
				var field = new TextField { isDelayed = true };
				field.style.unityTextAlign = TextAnchor.MiddleLeft;
				field.RegisterCallback<ChangeEvent<string>>(OnTextChanged);
				return field;
			};

			Action<VisualElement, int> bindCell = (element, rowIndex) =>
			{
				var field = element as TextField;
				field.userData = new CellContext { Row = rowIndex, Column = colIndex };
				field.SetValueWithoutNotify(m_rowDatas[rowIndex].Cells[colIndex]?.ToString());
			};

			return (makeCell, bindCell);
		}

		/// <summary>
		/// 创建字符串下拉列
		/// </summary>
		private (Func<VisualElement>, Action<VisualElement, int>) CreateComboColumn(ColumnItem colItem, int colIndex)
		{
			Func<VisualElement> makeCell = () =>
			{
				var field = new PopupField<string> { choices = colItem.Options };
				field.RegisterCallback<ChangeEvent<string>>(OnCurrentTextChanged);
				return field;
			};

			Action<VisualElement, int> bindCell = (element, rowIndex) =>
			{
				var field = element as PopupField<string>;
				field.userData = new CellContext { Row = rowIndex, Column = colIndex };

				var value = m_rowDatas[rowIndex].Cells[colIndex] as string;
				field.SetValueWithoutNotify(value);
			};

			return (makeCell, bindCell);
		}

		/// <summary>
		/// 创建枚举下拉列
		/// </summary>
		private (Func<VisualElement>, Action<VisualElement, int>) CreateEnumComboColumn(ColumnItem colItem, int colIndex)
		{
			Func<VisualElement> makeCell = () =>
			{
				var enumValue = (Enum)Enum.GetValues(colItem.EnumType).GetValue(0);
				var field = new EnumField(enumValue);
				field.RegisterCallback<ChangeEvent<Enum>>(OnCurrentIndexChanged);
				return field;
			};

			Action<VisualElement, int> bindCell = (element, rowIndex) =>
			{
				var field = element as EnumField;
				field.userData = new CellContext { Row = rowIndex, Column = colIndex };

				var value = m_rowDatas[rowIndex].Cells[colIndex] as Enum;
				if (value != null)
					field.SetValueWithoutNotify(value);
			};

			return (makeCell, bindCell);
		}
		
		/// <summary>
		/// 创建开关列
		/// </summary>
		private (Func<VisualElement>, Action<VisualElement, int>) CreateToggleColumn(int colIndex)
		{
			Func<VisualElement> makeCell = () =>
			{
				var toggle = new Toggle();
				toggle.RegisterCallback<ChangeEvent<bool>>(OnToggleChanged);
				return toggle;
			};

			Action<VisualElement, int> bindCell = (element, rowIndex) =>
			{
				var toggle = element as Toggle;
				toggle.userData = new CellContext { Row = rowIndex, Column = colIndex };

				var cellValue = m_rowDatas[rowIndex].Cells[colIndex];
				bool value = cellValue is bool boolVal ? boolVal : false;
				toggle.SetValueWithoutNotify(value);
			};

			return (makeCell, bindCell);
		}

		/// <summary>
		/// 创建对象选择列
		/// </summary>
		private (Func<VisualElement>, Action<VisualElement, int>) CreateObjectColumn(ColumnItem colItem, int colIndex)
		{
			Func<VisualElement> makeCell = () =>
			{
				var field = new ObjectField { objectType = colItem.ObjectType, allowSceneObjects = false };
				field.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnObjectChanged);
				return field;
			};

			Action<VisualElement, int> bindCell = (element, rowIndex) =>
			{
				var field = element as ObjectField;
				field.userData = new CellContext { Row = rowIndex, Column = colIndex };

				var value = m_rowDatas[rowIndex].Cells[colIndex] as UnityEngine.Object;
				field.SetValueWithoutNotify(value);
			};

			return (makeCell, bindCell);
		}

		#region Event Funcs
		private void OnTextChanged(ChangeEvent<string> evt)
		{
			var field = evt.target as TextField;
			var ctx = (CellContext)field.userData;
			if (ctx == null)
				return;

			m_rowDatas[ctx.Row].Cells[ctx.Column] = evt.newValue;
		}

		private void OnToggleChanged(ChangeEvent<bool> evt)
		{
			var toggle = evt.target as Toggle;
			var ctx = (CellContext)toggle.userData;
			if (ctx == null) 
				return;

			m_rowDatas[ctx.Row].Cells[ctx.Column] = evt.newValue;
		}

		private void OnCurrentTextChanged(ChangeEvent<string> evt)
		{
			var field = evt.target as PopupField<string>;
			var ctx = (CellContext)field.userData;
			if (ctx == null)
				return;

			m_rowDatas[ctx.Row].Cells[ctx.Column] = evt.newValue;
		}

		private void OnCurrentIndexChanged(ChangeEvent<Enum> evt)
		{
			var field = evt.target as EnumField;
			var ctx = (CellContext)field.userData;
			if (ctx == null)
				return;

			m_rowDatas[ctx.Row].Cells[ctx.Column] = evt.newValue;
		}

		private void OnObjectChanged(ChangeEvent<UnityEngine.Object> evt)
		{
			var field = evt.target as ObjectField;
			var ctx = (CellContext)field.userData;
			if (ctx == null)
				return;

			m_rowDatas[ctx.Row].Cells[ctx.Column] = evt.newValue;
		}
		#endregion
	}
}
