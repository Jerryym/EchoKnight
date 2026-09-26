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

		/// <summary>
		/// 单元格上下文: 用于事件回调中识别单元格位置
		/// </summary>
		private class CellContext
		{
			public int Row;
			public int Column;
		}

		private MultiColumnListView m_listView = null;

		/// <summary>
		/// 数据表
		/// </summary>
		private TableModel m_model = null;
		public TableModel Model => m_model;

		/// <summary>
		/// 表格列项
		/// </summary>
		private List<ColumnItem> m_columnItems = null;

		/// <summary>
		/// 行数
		/// </summary>
		public int RowCount => m_model.RowCount;
		/// <summary>
		/// 列数
		/// </summary>
		public int ColumnCount => m_model.ColumnCount;

		public TableWidget()
		{
			m_columnItems = new List<ColumnItem>();
			InitWidget();
		}

		public TableWidget(TableModel model)
		{
			InitWidget();
			SetModel(model);
		}

		#region Public Funcs
		/// <summary>
		/// 设置表格数据源
		/// </summary>
		/// <param name="model">表格数据源</param>
		public void SetModel(TableModel model)
		{
			if (model == null) 
				throw new ArgumentNullException(nameof(model));
			m_model = model;

			//初始化表格列项
			if (m_columnItems.Count == 0)
			{
				m_columnItems = new List<ColumnItem>(model.ColumnCount);
				for (int i = 0; i < model.ColumnCount; i++)
				{
					m_columnItems.Add(new ColumnItem { Type = ColumnType.None });
				}
			}
			InitTable();

			//绑定数据
			m_listView.itemsSource = m_model.Rows;
			m_listView.RefreshItems();
		}

		public void SetColumns(List<ColumnItem> columnItems)
		{
			m_columnItems = columnItems;
		}

		/// <summary>
		/// 设置列类型和属性
		/// </summary>
		/// <param name="index">列索引</param>
		/// <param name="item">列项配置</param>
		public void SetColumnType(int index, ColumnItem item)
		{
			if (index < 0 || index >= ColumnCount)
				throw new ArgumentOutOfRangeException(nameof(index), $"列索引越界: {index}，当前列数为 {ColumnCount}");
			if (item == null)
				throw new ArgumentNullException(nameof(item), "ColumnItem 参数不能为空，必须传入有效的列项配置!");

			//初始化
			m_columnItems[index] = item;
			InitColumn(index);
			m_listView.RefreshItems();
		}

		/// <summary>
		/// 设置指定单元格值
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		/// <param name="colIndex">列索引</param>
		/// <param name="value">单元格值</param>
		public void SetCellValue(int rowIndex, int colIndex, object value)
		{
			if (m_model == null)
				return;

			m_model.SetValue(rowIndex, colIndex, value);
			m_listView.RefreshItem(rowIndex);
		}

		/// <summary>
		/// 获取指定单元格值
		/// </summary>
		/// <param name="rowIndex">行索引</param>
		/// <param name="colIndex">列索引</param>
		/// <returns>单元格值</returns>
		public object GetCellValue(int rowIndex, int colIndex)
		{
			if (m_model == null)
				return null;

			return m_model.GetValue(rowIndex, colIndex);
		}

		public void Reset()
		{
			m_model = null;

			m_columnItems?.Clear();
			m_listView.itemsSource = null;
			m_listView.columns.Clear();

			m_listView.RefreshItems();
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

		private void InitTable()
		{
			if (m_model == null)
				return;

			m_listView.columns.Clear();
			for (int i = 0; i < ColumnCount; ++i)
			{
				int colIndex = i;
				var column = new Column { title = m_model.Columns[colIndex].Name };
				m_listView.columns.Add(column);
				InitColumn(colIndex);
			}

			m_listView.RefreshItems();
		}

		private void InitColumn(int colIndex)
		{
			var colItem = m_columnItems[colIndex];
			if (colItem == null)
				return;

			Action<VisualElement, int> unbindCell = (element, rowIndex) =>
			{
				element.userData = null;
			};

			var result = CreateColumn(colItem, colIndex);
			var column = m_listView.columns[colIndex];
			column.makeCell = result.Item1;
			column.bindCell = result.Item2;
			column.unbindCell = unbindCell;
			column.width = colItem.Width;
		}

		private (Func<VisualElement>, Action<VisualElement, int>) CreateColumn(ColumnItem colItem, int colIndex)
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
					return CreateDefaultColumn(colIndex);
			}
		}

		/// <summary>
		/// 创建默认列
		/// </summary>
		private (Func<VisualElement>, Action<VisualElement, int>) CreateDefaultColumn(int colIndex)
		{
			Func<VisualElement> makeCell = () => new Label();
			Action<VisualElement, int> bindCell = (element, rowIndex) =>
			{
				var label = element as Label;
				label.text = m_model.Rows[rowIndex][colIndex]?.ToString();
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
				field.SetValueWithoutNotify(m_model.Rows[rowIndex][colIndex]?.ToString());
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

				var value = m_model.Rows[rowIndex][colIndex] as string;
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

				var value = m_model.Rows[rowIndex][colIndex] as Enum;
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

				var cellValue = m_model.Rows[rowIndex][colIndex];
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

				var value = m_model.Rows[rowIndex][colIndex] as UnityEngine.Object;
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

			m_model.SetValue(ctx.Row, ctx.Column, evt.newValue);
		}

		private void OnToggleChanged(ChangeEvent<bool> evt)
		{
			var toggle = evt.target as Toggle;
			var ctx = (CellContext)toggle.userData;
			if (ctx == null)
				return;

			m_model.SetValue(ctx.Row, ctx.Column, evt.newValue);
		}

		private void OnCurrentTextChanged(ChangeEvent<string> evt)
		{
			var field = evt.target as PopupField<string>;
			var ctx = (CellContext)field.userData;
			if (ctx == null)
				return;

			m_model.SetValue(ctx.Row, ctx.Column, evt.newValue);
		}

		private void OnCurrentIndexChanged(ChangeEvent<Enum> evt)
		{
			var field = evt.target as EnumField;
			var ctx = (CellContext)field.userData;
			if (ctx == null)
				return;

			m_model.SetValue(ctx.Row, ctx.Column, evt.newValue);
		}

		private void OnObjectChanged(ChangeEvent<UnityEngine.Object> evt)
		{
			var field = evt.target as ObjectField;
			var ctx = (CellContext)field.userData;
			if (ctx == null)
				return;

			m_model.SetValue(ctx.Row, ctx.Column, evt.newValue);
		}
		#endregion
	}
}
