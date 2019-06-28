using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Zoom
{
	public enum ZNumberStyle { Comma, Plain, Brief };  // Comma: use a thousands separator.  Brief: convert larger numbers to 2k, 3M, etc.

	public enum ZAlignment { None, Left, Right, Center, Float, Integer };  // Float and Integer are the same as Right, but give a hint as to the type of data contained in cell.Data. "Integer" means that the underlying data is made of integers (e.g. average score, average rank) not that the number displayed in the cell is an integer.

	public static class ZAlignmentExtensions
	{
		public static string ToHtml(this ZAlignment alignment)
		{
			switch (alignment)
			{
				case ZAlignment.None:   return "";
				case ZAlignment.Left:   return " align=\"left\"";
				case ZAlignment.Center: return " align=\"center\"";
				default:                return " align=\"right\"";
			}
		}
	}

	public enum OutputFormat { Svg = 0, HtmlTable, Tsv, Csv };

	public static class OutputFormatExtensions
	{
		public static string ToExtension(this OutputFormat outputFormat)
		{
			switch (outputFormat) {
				case OutputFormat.Svg:
				case OutputFormat.HtmlTable: return "html";
				case OutputFormat.Tsv: return "tsv";
				case OutputFormat.Csv: return "csv";
				default: return "";
			}
		}

		public static bool IsHtml(this OutputFormat outputFormat)
		{
			return (outputFormat == OutputFormat.Svg || outputFormat == OutputFormat.HtmlTable);
		}
	}

	/// This is the abstract parent of ZColumn and ZCell, holding only a few fields common to "rectangular areas that can contain text".
	public interface IBlock
	{
		/// <summary>Text to appear in the cell or column header.</summary>
		string Text { get; set; }
		/// <summary>If this cell or column heading contains text which links to another report, put the URL of that report here.</summary>
		string Hyper { get; set; }
	}

	/// This is just the header row(s) and metadata for a column -- does not include the actual cells.
	public class ZColumn: IBlock
	{
		public string Text { get; set; }
		public string Hyper { get; set; }
		/// <summary>Optional. Appears *above* heading text.</summary>
		public string GroupHeading { get; set; }
		/// <summary>left, right or center</summary>
		public ZAlignment Alignment { get; set; }
		/// <summary>If true, we should try rotating the header text to make it fit better.</summary>
		public bool Rotate;

		protected ZColumn() {}

		public ZColumn(string text)
		{
			Text = text;
		}

		public ZColumn(string text, ZAlignment alignment)
		{
			Text = text;
			Alignment = alignment;
		}

		public ZColumn(string text, ZAlignment alignment, string groupHeading)
		{
			Text = text;
			Alignment = alignment;
			GroupHeading = groupHeading;
		}

		public override string ToString()
		{
			return Text;
		}
	}

	/// <summary>Start or end of a ribbon. A ribbon can have multiple starts and multiple ends.</summary>
	public class ZRibbonEnd: IComparable
	{
		public int Row { get; set; }
		public double Width { get; set; }

		public ZRibbonEnd(int row, double width)
		{
			Row = row;
			Width = width;
		}

		int IComparable.CompareTo(object obj)
		{
			return Row - ((ZRibbonEnd)obj).Row;
		}
	}

	/// <summary>A Ribbon is a connection between some cells. Cells in the column to the left are "From" entries. Cells in the column to the right are "To". The ribbon will join all these in a pretty way.</summary>
	public class ZRibbonColumn: ZColumn
	{
		public List<ZRibbonEnd> From { get; set; } // Cells in the column to the left of the ribbon to draw starting points from.
		public List<ZRibbonEnd> To { get; set; }  // Cells in the column to the right of the ribbon to draw to.
		public Color Color { get; set; }
		
		public ZRibbonColumn()
		{
			From = new List<ZRibbonEnd>();
			To = new List<ZRibbonEnd>();
		}

		public double MaxWidth()
		{
			return Math.Max(From.Max(x => x.Width), To.Max(x => x.Width));
		}
	}

	[Flags]
	public enum ChartType { None = 0, Bar = 1, Rug = 2, BoxPlot = 4, Histogram = 8, KernelDensityEstimate = 16 };
	public static class ChartTypeExtensions
	{
		public static ChartType ToChartType(string value)
		{
			ChartType chartType = ChartType.None;
			if (string.IsNullOrEmpty(value)) return chartType;

			if (value.Contains("rug")) chartType |= ChartType.Rug;
			if (value.Contains("bar")) chartType |= ChartType.Bar;
			if (value.Contains("box")) chartType |= ChartType.BoxPlot;
			if (value.Contains("histogram")) chartType |= ChartType.Histogram;
			if (value.Contains("kernel")) chartType |= ChartType.KernelDensityEstimate;
			return chartType;
		}
	}

	/// <summary>Represents a single cell in a table. The cell can optionally have a horizontal chart bar.</summary>
	public class ZCell: IBlock
	{
		string text;
		
		public string Text  // Text data to appear in this cell.
		{
			get 
			{
				if (text == null)
				{
					if (string.IsNullOrEmpty(NumberFormat))
						return Number.ToString();
					else if (Number != null)
						return ((double)Number).ToString(NumberFormat, CultureInfo.CurrentCulture);
					else
						return "";
				}
				else
					return text;
			}
			
			set { text = value; }
		}

		public string Hyper { get; set; }

		/// <summary>For multiple classes, separate each class with a space. (Blame CSS.)</summary>
		public string CssClass { get; set; }

		public bool Empty()
		{
			return string.IsNullOrEmpty(text) && Number == null;
		}

		public bool EmptyOrNaN()
		{
			return string.IsNullOrEmpty(text) && (Number == null || double.IsNaN((double)Number));
		}

		/// <summary>If this cell contains a number, put it here.</summary>
		public double? Number { get; set; }
		public string NumberFormat { get; set; }
		/// <summary>Optional background colour.</summary>
		public Color Color { get; set; }
		/// <summary>If set, show a chart for this cell. If ChartCell is not set, use this cell's number; otherwise use the cell specified in ChartCell.</summary>
		public ChartType ChartType { get; set; }
		/// <summary>Optional pointer to cell whose value we are to show as a chart. If no ChartType is set, we assume ChartType.Bar.</summary>
		public ZCell ChartCell { get; set; }  
		/// <summary>Color of optional horizontal chart bar.</summary>
		public Color BarColor { get; set; }
		/// <summary>List of values to be shown as a scatter plot / quartile plot / stem-and-leaf plot / rug map / kernel density estimation.</summary>
		public List<double> Data { get; private set; }
		/// <summary>Can hold whatever data the caller wants.</summary>
    	public object Custom { get; set; }

		public ZCell(string text = "", Color color = default(Color), ZCell barCell = null)
		{
			Text = text;
			Color = color;
			ChartCell = barCell;
		}

		public ZCell(double? number, ChartType chartType = ChartType.None, string numberFormat = "", Color color = default(Color))
		{
			Number = number;
			NumberFormat = numberFormat;
			Color = color;
			ChartType = chartType;
			Data = new List<double>();
		}

		public Color GetBarColor(Color? rowBackground = null, Color? barNone = null)
		{
			if (BarColor != Color.Empty)
				return BarColor;

			if (Color != Color.Empty)
				return ZReportColors.Darken(ZReportColors.AddDark(Color, rowBackground));

			if (barNone != null && barNone != Color.Empty)
				return ZReportColors.AddDark((Color)barNone, rowBackground);
			
			return ZReportColors.Darken(Color.White);
		}

		public override string ToString()
		{
			return Text;
		}
	}

	public class ZRow: List<ZCell>
	{
		public string CssClass { get; set; }

		///<summary>Just like Add(), but returns the added cell.</summary> 
		public ZCell AddCell(ZCell cell)
		{
			Add(cell);
			return cell;
		}

		///<summary>If there is exactly one chart in this row (because exactly one cell has its ChartType set, or because all the ChartCell 
		/// values specified in this row point to just one cell, return the index of the cell whose value is used for the bar.</summary>
		public ZCell OneBarCell()
		{
			ZCell result = null;
			bool foundOneBar = false;
			for (int col = 0; col < this.Count; col++)
			{
				var cell = this[col];
				if (foundOneBar)
				{
					if ((cell.ChartCell != null && cell.ChartCell != result) || (cell.ChartType != ChartType.None && result != cell))
						return null;
				}
				else  // No Bar found yet.
				{
					if (cell.ChartCell != null)
					{
						foundOneBar = true;
						result = cell.ChartCell;
					}
					else if (cell.ChartType != ChartType.None)
					{
						foundOneBar = true;
						result = cell;
					}
				}
			}
			return foundOneBar ? result : null;
		}
	}

//  TCalcBar = procedure(report: TZoomReport; row, col: Integer; max: Real; var fill: Real);  // callback to custom-set bar cell filledness
//  TPaintBar = procedure(report: TZoomReport; row, col: Integer; canvas: TCanvas; rect: TRect; Color, BarColor: TColor);  // callback to custom-paint bar background

	public class ZReportColors
	{
		public Color TitleFontColor { get; set; }
		public Color TitleBackColor { get; set; }
		public Color TextColor { get; set; }
		public Color BackgroundColor { get; set; }
		public Color OddColor { get; set; }
		public Color BarColor { get; set; }
		/// <summary>Colour used for bars if no cell colours are set.</summary>
		public Color BarNone { get; set; }

		public ZReportColors()
		{
			TitleFontColor = Color.White;
			TitleBackColor = Color.Gray;
			TextColor = Color.Black;
			BackgroundColor = Color.White;
			OddColor = Color.FromArgb(0xEE, 0xFF, 0xFF);
			BarColor = Color.Empty;
			BarNone = Color.FromArgb(255, 192, 160);
		}

		public Color GetBackColor(bool odd)
		{
			return odd ? OddColor : BackgroundColor;
		}
		
		public Color GetBackColor(bool odd, Color color)
		{
			return color == Color.Empty ? (odd ? OddColor : BackgroundColor) : color;
		}

		public static Color Darken(Color color)
		{
			return Color.FromArgb(color.R * 9 / 11, color.G * 9 / 11, color.B * 9 / 11);
		}
		
		public static Color Mix(Color color1, Color color2, double mix)
		{
			return Color.FromArgb((int)(color1.R * mix + color2.R * (1 - mix)),
			                      (int)(color1.G * mix + color2.G * (1 - mix)),
			                      (int)(color1.B * mix + color2.B * (1 - mix)));
		}

		public static Color AddDark(Color color1, Color? color2)
		{
			return color2 == null || color2 == Color.Empty ?
				color1 :
				Color.FromArgb(Math.Max(color1.R + ((Color)color2).R - 255, 0), Math.Max(color1.G + ((Color)color2).G - 255, 0), Math.Max(color1.B + ((Color)color2).B - 255, 0));
		}
	}

	public abstract class ZoomReportBase
	{
		/// <summary>Title for the whole report.</summary>
		public string Title { get; set; }
		/// <summary>If threport title contains text which links to another report, put the URL of that report here.</summary>
		public virtual string TitleHyper { get; set; }
		public ZReportColors Colors { get; set; }

		/// <summary>Export to the specified format.</summary>
		public string ToOutput(OutputFormat outputFormat)
		{
			switch (outputFormat) {
				case OutputFormat.Svg: return ToSvg(0);
				case OutputFormat.HtmlTable: return ToHtml();
				case OutputFormat.Tsv: return ToCsv('\t');
				case OutputFormat.Csv: return ToCsv(',');
				default: return ""; 
			}
		}
		/// <summary>Export to character-separated value.</summary>
		public abstract string ToCsv(char separator);
		/// <summary>Export to an HTML table.</summary>
		public abstract string ToHtml();
		/// <summary>Export to an HTML SVG element.</summary>
		public abstract string ToSvg(int table);
		public abstract IEnumerable<Color> BarCellColors();
	}

	public class ZoomReport: ZoomReportBase
	{
		public List<ZColumn> Columns { get; private set; }
		public List<ZRow> Rows { get; set; }
		public string Description { get; set; }
		public ZNumberStyle NumberStyle { get; set; }
		public string CssClass { get; set; }
		/// <summary>If true, scale bar charts in each column separately.</summary>
		public bool MaxChartByColumn { get; set; }
		/// <summary>If true, scale bar charts in each column separately.</summary>
		public bool MaxChartByRow { get; set; }
		/// <summary>For scaling bar charts against.</summary>
		public double? MaxChart { get; set; }
		public ZoomReport Owner { get; set; }
		/// <summary>If true, it is OK to try to render this in two (or more) "columns" a la Word, thus doubling the row height and halving the width of each column.</summary>
		public bool MultiColumnOK { get; set; }
		/// <summary>True if HTML tables should show bar charts.</summary>
		public bool Bars { get; set; }

		//OnCalcBar: TCalcBar;
		//OnPaintBar: TPaintBar;

		public ZoomReport(string title, string headings = "", string alignments = "", string groupHeadings = "")
		{
			Columns = new List<ZColumn>();
			Rows = new List<ZRow>();

			Title = title;

			AddColumns(headings, alignments, groupHeadings);

			Bars = true;
		}

		public void AddColumns(string headings, string alignments = "", string groupHeadings = "")
		{
			int firstAdded = Columns.Count();

			if (!string.IsNullOrEmpty(headings))
				foreach (string heading in headings.Split(','))
					Columns.Add(new ZColumn(heading));
			
			if (!string.IsNullOrEmpty(alignments))
			{
				string[] alignmentList = alignments.Split(',');
				for (int i = 0; firstAdded + i < Columns.Count && i < alignmentList.Length; i++)
					Columns[firstAdded + i].Alignment = (ZAlignment)Enum.Parse(typeof(ZAlignment), alignmentList[i], true);
			}

			if (!string.IsNullOrEmpty(groupHeadings))
			{
				string[] groupList = groupHeadings.Split(',');
				for (int i = 0; firstAdded + i < Columns.Count && i < groupList.Length; i++)
					Columns[firstAdded + i].GroupHeading = groupList[i];
			}
		}

		///<summary>Just like Add(), but returns the added ZColumn.</summary> 
		public ZColumn AddColumn(ZColumn col)
		{
			Columns.Add(col);
			return col;
		}

		static void AppendStrings(StringBuilder builder, params string[] strings)
		{
			foreach (string s in strings)
				builder.Append(s);
		}

		public ZCell Cell(ZRow row, string columnText)
		{
			return row[Columns.FindIndex(x => x.Text == columnText)];
		}

		public bool ColumnEmpty(int i)
		{
			foreach(ZRow row in Rows)
				if (i < row.Count && !row[i].Empty())
					return false;

			return true;
		}

		public bool ColumnZeroOrNaN(int i)
		{
			foreach(ZRow row in Rows)
				if (i < row.Count && !row[i].EmptyOrNaN() && row[i].Number != 0)
					return false;

			return true;
		}

		public void RemoveColumn(int i)
		{
			if (i < Columns.Count)
				Columns.RemoveAt(i);

			foreach(ZRow row in Rows)
				if (i < row.Count)
					row.RemoveAt(i);
		}

		public void RemoveZeroColumns()
		{
			for (int i = Columns.Count - 1; i >= 0; i--)
				if (ColumnZeroOrNaN(i))
					RemoveColumn(i);
		}

		/// <summary>For each column, calculate a pixel width, and find the min and max values.</summary>
		void Widths(List<float> widths, List<double> mins, List<double> maxs)
		{
			var graphics = Graphics.FromImage(new Bitmap(1000, 20));
			var font = new Font("Arial", 11);

			for (int col = 0; col < Columns.Count; col++)
			{
				float widest = Columns[col] is ZRibbonColumn ? 15 : 1;
				float total = widest;
				int count = 1;
				double min = 0.0;
				double max = 0.0;
				string numberFormat = "";
				bool hasNumber = false;

				for (int row = 0; row < Rows.Count; row++)
				{
					if (col < Rows[row].Count)
					{
						if (Rows[row][col].Number.HasValue && !double.IsNaN((double)Rows[row][col].Number))
						{
							hasNumber = true;
							min = Math.Min(min, Math.Abs((double)Rows[row][col].Number));
							max = Math.Max(max, Math.Abs((double)Rows[row][col].Number));
							if (Rows[row][col].Data != null)
							{
								min = Math.Min(min, Rows[row][col].Data.DefaultIfEmpty(0).Min());
								max = Math.Max(max, Rows[row][col].Data.DefaultIfEmpty(0).Max());
							}
							if (!string.IsNullOrEmpty(Rows[row][col].NumberFormat))
							    numberFormat = Rows[row][col].NumberFormat;
						}
						else if (!string.IsNullOrEmpty(Rows[row][col].Text))
						{
							float width = graphics.MeasureString(Rows[row][col].Text, font, 1000).Width;
							total += width;
							count++;
							widest = Math.Max(widest, width);
						}
					}
				}

				if (hasNumber)
				{
					string stringMax = string.IsNullOrEmpty(numberFormat) ? max.ToString() : max.ToString(numberFormat);
					widths.Add(graphics.MeasureString(stringMax, font, 1000).Width * 1.01f);
				}
				else
					widths.Add(widest > 2 * total / count ?  // Are there are a few pathologically wide fields?
						       total / count * 1.4f :        // Just use the average, plus some padding.
						       widest);
				mins.Add(min);
				maxs.Add(max);
			}
		}

		public override IEnumerable<Color> BarCellColors()
		{
			return Rows.SelectMany(row => row.Where(cell => cell.ChartCell != null)
			                   .Select(cell => cell.GetBarColor())).Distinct();
		}

		public override string ToCsv(char separator)
		{
			StringBuilder s = new StringBuilder();

			if (!string.IsNullOrEmpty(Title))
				AppendStrings(s, Title, "\n");

			List<ZColumn> columns = Columns;

			bool hasgroupheadings = false;
			foreach (ZColumn col in Columns)
				hasgroupheadings |= !string.IsNullOrEmpty(col.GroupHeading);

			if (hasgroupheadings)
			{
				foreach (ZColumn col in Columns)
				{
					s.Append(col.GroupHeading);
					s.Append(separator);
				}
				s.Append('\n');
			}

			foreach (ZColumn col in Columns)
			{
				s.Append(col.Text);
				s.Append(separator);
			}
			s.Append('\n');

			foreach (ZRow row in Rows)
			{
				foreach (var cell in row)
				{
					s.Append(cell.Text);
					s.Append(separator);
				}
				s.Append('\n');
			}

			return s.ToString();
		}

		// This writes an HTML fragment -- it does not include <head> or <body> tags etc.
		public override string ToHtml()
		{
			List<ZColumn> columns = Columns;
			bool hasgroupheadings = false;
			foreach (ZColumn col in Columns)
				hasgroupheadings |= !string.IsNullOrEmpty(col.GroupHeading);

			StringBuilder s = new StringBuilder();

			s.Append("\n<table align=\"center\"");
			if (!string.IsNullOrEmpty(CssClass))
				AppendStrings(s, " class=\"", CssClass, "\"");

			s.Append(">\n");
			s.Append("  <thead>\n");

			// Title row
			if (!string.IsNullOrEmpty(Title))
				AppendStrings(s, "    <tr bgcolor=\"", System.Drawing.ColorTranslator.ToHtml(Colors.TitleBackColor), "\">\n",
				              "       <th colspan=\"", columns.Count.ToString(CultureInfo.InvariantCulture), "\"><H2>", WebUtility.HtmlEncode(Title), "</H2></th>\n",
				              "    </tr>\n");

			// Group Headings row
			if (hasgroupheadings)
			{
				AppendStrings(s, "    <tr bgcolor=\"", System.Drawing.ColorTranslator.ToHtml(Colors.TitleBackColor), "\">\n");

				int start = 0;
				while (start < columns.Count)
				{
					int end = start;
					while (end < columns.Count - 1 && columns[start].GroupHeading == columns[end + 1].GroupHeading)
						end++;

					if (start == end)
						AppendStrings(s, "      <th align=\"center\">", 
						              WebUtility.HtmlEncode(columns[start].GroupHeading), "</th>\n");
					else
						AppendStrings(s, "      <th align=\"center\" colspan=\"" + (end - start + 1).ToString(CultureInfo.InvariantCulture) + "\">",
						              WebUtility.HtmlEncode(columns[start].GroupHeading), "</th>\n");

					start = end + 1;
				}

				s.Append("    </tr>\n");
			}

			// Headings row
			AppendStrings(s, "    <tr bgcolor=\"", System.Drawing.ColorTranslator.ToHtml(Colors.TitleBackColor), "\">\n");

			foreach (ZColumn col in Columns)
				if (string.IsNullOrEmpty(col.Hyper))
					AppendStrings(s, "      <th", col.Alignment.ToHtml(), ">", WebUtility.HtmlEncode(col.Text), "</th>\n");
				else
					AppendStrings(s, "      <th", col.Alignment.ToHtml(), "><a href=\"", col.Hyper, "\">", WebUtility.HtmlEncode(col.Text), "</a></th>\n");

			s.Append("    </tr>\n");
			s.Append("  </thead>\n");
			s.Append("  <tbody>\n");

			double max = 0;
			if (Bars)
//				max = this.Rows.SelectMany(row => row.Where(cell => cell.Number != null && (cell.Bar || cell.BarCell != null))
//				                           .Select(cell => Math.Abs((double)cell.Number))).DefaultIfEmpty(0).Max();
				foreach (var row in Rows)
					foreach (var cell in row)
						if (cell.Number != null && (cell.ChartType != ChartType.None || cell.ChartCell != null))
							max = Math.Max(max, Math.Abs((double)cell.Number));
			if (max == 0)
				max = 1;

			bool odd = true;

			foreach (ZRow row in Rows)
			{
		        // Write the <tr> tag that begins the row.
				AppendStrings(s, "    <tr style=\"background-color: ", System.Drawing.ColorTranslator.ToHtml(Colors.GetBackColor(odd)), "\"");

				if (!string.IsNullOrEmpty(row.CssClass))
					AppendStrings(s, " class=\"", row.CssClass, "\"");

				s.Append(">\n");

				ZCell oneBarCell = row.OneBarCell();

				for (int col = 0; col < columns.Count && col < row.Count; col++)
				{
					// find colour for the cell
					string cellColor;
					if (row[col].Color == Color.Empty)
						cellColor = "";
					else
						cellColor = " style=\"background-color: " + System.Drawing.ColorTranslator.ToHtml(Colors.GetBackColor(odd, row[col].Color)) + '"';

					if (Bars && /*!oneBar &&*/ row[col].Number != null && (row[col].ChartType != ChartType.None || row[col].ChartCell == row[col]))
					{
						// find the max value to scale the bar against
//						thisbar = MaxBarByColumn ? FMaxBars[col] :
//								  MaxBarByRow    ? FMaxBars[j] : FMaxBar;
						
						var barColor = row[col].GetBarColor(Colors.GetBackColor(odd), Colors.BarNone);

						AppendStrings(s, "      <td class=\"barcontainer\"", cellColor, ">\n");
						s.AppendFormat("        <div class=\"bar{0:X2}{1:X2}{2:X2}\" style=\"width: {3}%\" />\n", 
						               barColor.R, barColor.G, barColor.B, (int)((double)(row[col].Number) * 100 / (double)max));
						AppendStrings(s, "        <div", columns[col].Alignment.ToHtml(), " class=\"bartext\">");
					}
					else
					{
						AppendStrings(s, "      <td", columns[col].Alignment.ToHtml(), cellColor);
						if (!string.IsNullOrEmpty(row[col].CssClass))
							AppendStrings(s, " class=\"", row[col].CssClass, "\"");
						s.Append(">");
					}

					if (string.IsNullOrEmpty(row[col].Hyper))
						s.Append(WebUtility.HtmlEncode(row[col].Text));
					else
						AppendStrings(s, "<a href=\"", row[col].Hyper, "\">", WebUtility.HtmlEncode(row[col].Text), "</a>");

					if (Bars && /*!oneBar &&*/ row[col].Number != null && (row[col].ChartType != ChartType.None || row[col].ChartCell == row[col]))
						s.Append("</div>");

					s.Append("</td>\n");
				}
				s.Append("    </tr>\n");

				odd = !odd;
			}

			s.Append("</tbody></table>\n");

			if (!string.IsNullOrEmpty(Description))
		    	AppendStrings(s, "<p>", Description, "</p>\n");

			s.Append("<br /><br />\n\n");
			return s.ToString();
		}

		// Write a <rect> tag, and a <text> tag on top of it.
		void SvgRectText(StringBuilder s, int indent, double x, double y, double width, double height, Color fontColor, Color backColor, Color barColor, ZAlignment alignment, string text, string cssClass = null, string hyper = null, double? bar = null)
		{
			SvgRect(s, indent, x, y, width, height, backColor);
			if (bar != null)
				SvgRect(s, indent, x, y, width * (double)bar, height, barColor);
			SvgText(s, indent, (int)x, (int)y, (int)width, (int)height, fontColor, alignment, text, cssClass, hyper);
		}

		void SvgRect(StringBuilder s, int indent, double x, double y, double width, double height, Color fillColor)
		{
			if (fillColor != Color.Empty)
			{
				s.Append('\t', indent);
				s.AppendFormat("<rect x=\"{0:F1}\" y=\"{1:F0}\" width=\"{2:F1}\" height=\"{3:F0}\" style=\"fill:", x, y, width, height);
				s.Append(System.Drawing.ColorTranslator.ToHtml(fillColor));
				s.Append("\" />\n");
			}
		}

		void SvgRect2(StringBuilder s, int indent, double x, double y, double width, double height, Color fillColor)
		{
			s.Append('\t', indent);
			s.AppendFormat("<rect x=\"{0:F2}\" y=\"{1:F2}\" width=\"{2:F2}\" height=\"{3:F2}\" style=\"fill:", x, y, width, height);
			s.Append(System.Drawing.ColorTranslator.ToHtml(fillColor));
			s.Append("\" />\n");
		}

		void SvgBeginText(StringBuilder s, int indent, int x, int y, int width, int height, Color fontColor, ZAlignment alignment, string cssClass = null, string hyper = null)
		{
			s.Append('\t', indent);
			if (!string.IsNullOrEmpty(hyper))
				AppendStrings(s, "<a xlink:href=\"", hyper, "\">");

			s.Append("<text ");
			if (!string.IsNullOrEmpty(cssClass))  // cssClass is so that cells can be given a CSS class that can e.g. be used later by JavaScript.
				AppendStrings(s, "class=\"", cssClass, "\" ");

			if (!string.IsNullOrEmpty(hyper))
				s.Append("text-decoration=\"underline\" ");

			switch (alignment)
			{
				case ZAlignment.Left: 
					s.AppendFormat("x=\"{0}\" y=\"{1}\" width=\"{2}\" font-size=\"{3}\"",
					                       x + 1, y + height * 3 / 4, width, height * 3 / 4);
				break;
				case ZAlignment.Center:
					s.AppendFormat("text-anchor=\"middle\" x=\"{0}\" y=\"{1}\" width=\"{2}\" font-size=\"{3}\"",
				                       x + width / 2, y + height * 3 / 4, width, height * 3 / 4);
				break;
			default: // i.e. Right, Float, Integer
					s.AppendFormat("text-anchor=\"end\" x=\"{0}\" y=\"{1}\" width=\"{2}\" font-size=\"{3}\"", 
				                       x + width - 1, y + height * 3 / 4, width, height * 3 / 4);
				break;
			}

			if (!string.IsNullOrEmpty(hyper) || fontColor != Color.Black)
			{
				s.Append(" fill=\"");
				s.Append(!string.IsNullOrEmpty(hyper) && fontColor == Color.Black ? "Navy" : System.Drawing.ColorTranslator.ToHtml(fontColor));
				s.Append('\"');
			}

			s.Append('>');
		}

		void SvgEndText(StringBuilder s, string hyper = null)
		{
			s.Append("</text>");

			if (!string.IsNullOrEmpty(hyper))
				s.Append("</a>");

			s.Append('\n');
		}

		void SvgText(StringBuilder s, int indent, int x, int y, int width, int height, ZColumn column, ZCell cell)
		{
			SvgBeginText(s, indent, x, y, width, height, Colors.TextColor, column.Alignment, cell.CssClass, cell.Hyper);

//			int decimals = 0;
//			if (!string.IsNullOrEmpty(cell.NumberFormat) && cell.NumberFormat.Length >= 2 && (cell.NumberFormat[0] == 'E' || cell.NumberFormat[0] == 'G'))
//				decimals = int.Parse(cell.NumberFormat.Substring(1));
//
//			if (cell.Number == 0)
//			{
//				s.Append('0');
//				if (decimals > 0)
//					s.Append('\u2008');  // punctutation space (width of a .)
//				s.Append('\u2002', decimals);  // en space (nut)
//			}
			if (cell.Number == 0 || cell.Number == null || double.IsNaN((double)cell.Number) || double.IsInfinity((double)cell.Number) ||
			    cell.Number < -0.0001 || cell.Number > 0.0001)
				s.Append(WebUtility.HtmlEncode(cell.Text));
			else
			{
				int magnitude = (int)Math.Floor(Math.Log10(Math.Abs((double)cell.Number)));
				string digits = "\u2070\u00B9\u00B2\u00B3\u2074\u2075\u2076\u2077\u2078\u2079";  // superscript 0123456789
				s.AppendFormat("{0:F2}", (double)cell.Number * Math.Pow(10, -magnitude));
				s.Append('\u00D7'); // multiply symbol
				s.Append("10");
				if (magnitude < 0)
					s.Append('\u207B');  // superscript -

				if (magnitude == 0)
					s.Append('\u2070');  // superscript 0
				else
					for (int i = (int)Math.Log10(Math.Abs(magnitude)); i >= 0; i--)
						s.Append(digits[(int)((Math.Abs(magnitude) / Math.Pow(10, i)) % 10)]);
			}
			SvgEndText(s, cell.Hyper);
		}

		void SvgText(StringBuilder s, int indent, int x, int y, int width, int height, Color fontColor, ZAlignment alignment, string text, string cssClass = null, string hyper = null)
		{
			if (string.IsNullOrEmpty(text))
			    return;

			SvgBeginText(s, indent, x, y, width, height, fontColor, alignment, cssClass, hyper);
			s.Append(WebUtility.HtmlEncode(text));
			SvgEndText(s, hyper);
		}

		void SvgPolygon(StringBuilder s, int indent, List<Tuple<double, double>> points, double rowTop, double rowHeight, double yMax, Color fillColor)
		{
			s.Append('\t', indent);
			s.Append("<polygon points=\"");
			for (int i = 0; i < points.Count; i++)
			{
				double y0 = i == 0 ? double.MinValue : rowTop + rowHeight - Scale(points[i - 1].Item2, rowHeight, 0, yMax);
				double y1 = rowTop + rowHeight - Scale(points[i].Item2, rowHeight, 0, yMax);
				double y2 = i == points.Count - 1 ? double.MaxValue : rowTop + rowHeight - Scale(points[i + 1].Item2, rowHeight, 0, yMax);
				if (Math.Round(y0, 1) != Math.Round(y1, 1) || Math.Round(y1, 1) != Math.Round(y2, 1))  // Only write the point if the y before or the y after is different -- otherwise we have a very expensive way of writing a horizontal line.
					s.AppendFormat("{0:F1},{1:F1} ", points[i].Item1, y1);
			}
			s.Append("\" style=\"fill:");
			s.Append(System.Drawing.ColorTranslator.ToHtml(fillColor));
			s.Append("\" />\n");
		}

		void SvgLine(StringBuilder s, int indent, double x1, double y1, double x2, double y2, double width, Color fillColor)
		{
			if (fillColor != Color.Empty)
			{
				s.Append('\t', indent);
				s.AppendFormat("<line x1=\"{0:F1}\" y1=\"{1:F0}\" x2=\"{2:F1}\" y2=\"{3:F0}\" style=\"stroke-width:{4:F0};stroke-linecap:\"round\";stroke:", x1, y1, x2, y2, width);
				s.Append(System.Drawing.ColorTranslator.ToHtml(fillColor));
				s.Append("\" />\n");
			}
		}

		enum TopBottomType { Left, Right, Both }; // Does the top of this ribbon have an end from the left? An end to the right? One of each? What about the bottom of the ribbon?

		// Draw one complete vertical ribbon plus its horizontal ends.
		void SvgRibbon(StringBuilder s, int indent, ZRibbonColumn ribbon, float left, float width, float top, float height, float rowHeight)
		{
			if (ribbon.From.Count == 0 && ribbon.To.Count == 0)
				return;

			Color c = ribbon.Color == default(Color) ? Color.Gray : ribbon.Color;
			ribbon.From.Sort();
			ribbon.To.Sort();

			var topRow = Math.Min(ribbon.From.Count == 0 ? 999 : ribbon.From.First().Row, ribbon.To.Count == 0 ? 999 : ribbon.To.First().Row);
			var bottomRow = Math.Max(ribbon.From.Count == 0 ? 0 : ribbon.From.Last().Row, ribbon.To.Count == 0 ? 0 : ribbon.To.Last().Row);

			TopBottomType topType;
			TopBottomType bottomType;

			// Determine types of top and bottom of ribbon.
			if (!ribbon.From.Any())
			{
				topType = TopBottomType.Right;
				bottomType = TopBottomType.Right;
			}
			else if (!ribbon.To.Any())
			{
				topType = TopBottomType.Left;
				bottomType = TopBottomType.Left;
			}
			else
			{
				topType = ribbon.From.First().Row == ribbon.To.First().Row ? TopBottomType.Both : ribbon.From.First().Row < ribbon.To.First().Row ? TopBottomType.Left : TopBottomType.Right;
				bottomType = ribbon.From.Last().Row == ribbon.To.Last().Row ? TopBottomType.Both : ribbon.From.Last().Row > ribbon.To.Last().Row ? TopBottomType.Left : TopBottomType.Right;
			}

			//var ribbonWidthH = ribbon.MaxWidth();  // In unscaled units.
			var halfScaleWidth = ribbon.MaxWidth() / rowHeight * 2;  // Scaling factor used to convert an unscaled ribbon width into half a scaled ribbon width. 
			var halfRibbonH = ribbon.MaxWidth() * halfScaleWidth;  // Half the width of the vertical part of the ribbon, in output SVG units.

			if (topType == TopBottomType.Right)
			{
				// Start in the horizontal middle, draw the top left corner arc. 
				s.AppendFormat("<path d=\"M {0:F1},{1:F1} ", left + width / 2 + halfRibbonH, RowMid(top, ribbon.To.First().Row, rowHeight) - ribbon.To.First().Width * halfScaleWidth);
				s.AppendFormat("a {0:F1},{0:F1} 0 0 0 {1:F1},{0:F1} ", halfRibbonH * 2, -halfRibbonH * 2);
				if (!ribbon.From.Any())
					s.AppendFormat("V {2:F1}\n", RowMid(top, ribbon.To.Last().Row, rowHeight) + ribbon.To.Last().Width * halfScaleWidth);
			}
			else  // Move cursor to top right end of first arrow.
				s.AppendFormat("<path d=\"M {0:F1},{1:F1}\n", left + width / 2 - halfRibbonH, RowMid(top, topRow, rowHeight) - ribbon.From.First().Width * halfScaleWidth);

			// Draw left-side "From" ends.
			for (int i = 0; i < ribbon.From.Count; i++)
			{
				var end = ribbon.From[i];
				var halfRibbon = end.Width * halfScaleWidth;

				s.Append('\t', indent + 1);
				if (end.Row != topRow)
				{
					s.AppendFormat("V {0:F1} ", RowMid(top, end.Row, rowHeight) - halfRibbon * 2);
					s.AppendFormat("a {0:F1},{0:F1} 0 0 1 {1:F1},{0:F1} ", halfRibbon, -halfRibbon);
				}
				// Paint a left end: horizontal left, diagonal in / diagonal out, horizontal right.
				s.AppendFormat("H {0:F1} ", left);
				s.AppendFormat("l {0:F1},{0:F1} ", halfRibbon);
				s.AppendFormat("l {0:F1},{1:F1} ", -halfRibbon, halfRibbon);
				if (end.Row != bottomRow)
				{
					s.AppendFormat("H {0:F1} ", left + width / 2 - halfRibbonH - halfRibbon);
					s.AppendFormat("a {0:F1},{0:F1} 0 0 1 {0:F1},{0:F1} ", halfRibbon);
				}
				s.Append('\n');
			}

			// Handle transition between "From" and "To": draw the very bottom.
			if (bottomType == TopBottomType.Left)
			{
				s.Append('\t', indent + 1);
				s.AppendFormat("H {0:F1} ", left + width / 2 - halfRibbonH);
				s.AppendFormat("a {0:F1},{0:F1} 0 0 0 {0:F1},{1:F1} \n", halfRibbonH * 2, -halfRibbonH * 2);
			}
			else if (bottomType == TopBottomType.Both)
			{
				s.Append('\t', indent + 1);
				s.AppendFormat("H {0:F1} ", left + width / 2 - halfRibbonH);
				if (ribbon.From.Last().Width != ribbon.To.Last().Width)
					s.AppendFormat("c {0:F1},0 {0:F1},{2:F1} {1:F1},{2:F1}\n", halfRibbonH, halfRibbonH * 2, (ribbon.To.Last().Width - ribbon.From.Last().Width) / 2);
			}
			else if (bottomType == TopBottomType.Right)
			{
				s.Append('\t', indent + 1);
				s.AppendFormat("V {0:F1} ", RowMid(top, ribbon.To.Last().Row, rowHeight) + ribbon.To.Last().Width * halfScaleWidth - halfRibbonH * 2);
				s.AppendFormat("a {0:F1},{0:F1} 0 0 0 {0:F1},{0:F1} \n", halfRibbonH * 2);
			}

			// Draw right-side "To" ends.
			for (int i = ribbon.To.Count - 1; i >= 0; i--)
			{
				var end = ribbon.To[i];
				var halfRibbon = end.Width * halfScaleWidth;

				s.Append('\t', indent + 1);
				if (end.Row != bottomRow)
				{
					s.AppendFormat("V {0:F1} ", top + (end.Row + 0.5) * rowHeight + halfRibbon * 2);
					s.AppendFormat("a {0:F1},{0:F1} 0 0 1 {0:F1},{1:F1} ", halfRibbon, -halfRibbon);
				}
				// Paint a right end, starting at its bottom left: horizontal right, down, up/right, up/left, down, left.
				s.AppendFormat("H {0:F1} ", left + width - halfRibbon);
				s.AppendFormat("v {0:F1} ", halfRibbon);
				s.AppendFormat("l {0:F1},{1:F1} ", halfRibbon * 2, -halfRibbon * 2);
				s.AppendFormat("l {0:F1},{0:F1} ", -halfRibbon * 2);
				s.AppendFormat("v {0:F1} ", halfRibbon);
				s.AppendFormat("H {0:F1} ", left + width / 2 + halfRibbonH + halfRibbon);
				if (end.Row != topRow)
					s.AppendFormat("a {0:F1},{0:F1} 0 0 1 {1:F1},{1:F1} ", halfRibbon, -halfRibbon);
				s.Append('\n');
			}

			s.Append('\t', indent + 1);

			// Bring us back to top right and finish off.
			if (topType == TopBottomType.Left)
			{
				s.AppendFormat("V {0:F1} ", top + (ribbon.From.First().Row + 0.5) * rowHeight - ribbon.From.First().Width * halfScaleWidth + halfRibbonH * 2);
				s.AppendFormat("a {0:F1},{0:F1} 0 0 0 {1:F1},{1:F1} ", halfRibbonH * 2, -halfRibbonH * 2);
			}
			else if (topType == TopBottomType.Both && ribbon.From.First().Width != ribbon.To.First().Width)
				s.AppendFormat("c {0:F1},0 {0:F1},{2:F1} {1:F1},{2:F1} ", -halfRibbonH, -halfRibbonH * 2, (ribbon.To.First().Width - ribbon.From.First().Width) / 2);

			s.Append("Z\" fill=\"");
			s.Append(System.Drawing.ColorTranslator.ToHtml(c));
			s.Append("\" />\n");
		}

		// Write the opening <svg tag and the header row(s). Returns the amount of vertical height it has consumed.
		int SvgHeader(StringBuilder s, bool hasgroupheadings, int rowHeight, int height, List<float> widths, List<double> maxs, int width, double max)
		{
			s.AppendFormat("<div><svg viewBox=\"0 0 {0} {1}\" width=\"{0}\" align=\"center\">\n", width, height);  // width=\"{0}\" height=\"{1}\"

			SvgRect(s, 1, 1, 1, width - 1, rowHeight * 2, Colors.TitleBackColor);  // Paint title "row" background.

			// Add '-' and '+' zoom button text.
			s.Append("  <text text-anchor=\"middle\" x=\"15\" y=\"23\" width=\"30\" height=\"30\" font-size=\"22\" fill=\"Black\">&nbsp;+&nbsp;</text>\n");
			s.Append("  <text text-anchor=\"middle\" x=\"45\" y=\"23\" width=\"30\" height=\"30\" font-size=\"22\" fill=\"Black\">&nbsp;-&nbsp;</text>\n");

			SvgText(s, 1, 1, 1, width - 2, rowHeight * 2, Colors.TitleFontColor, ZAlignment.Center, Title, null, TitleHyper);  // Paint title "row" text.
			s.Append('\n');

			// Add '-' and '+' zoom buttons (with transparent text, so the text added above appears behind the report title text).
			s.Append("  <text text-anchor=\"middle\" x=\"15\" y=\"23\" width=\"30\" height=\"30\" font-size=\"22\" fill-opacity=\"9\" onclick=\"this.parentNode.setAttribute('width', this.parentNode.getAttribute('width') * 1.42)\">&nbsp;+&nbsp;</text>\n");
			s.Append("  <text text-anchor=\"middle\" x=\"45\" y=\"23\" width=\"30\" height=\"30\" font-size=\"22\" fill-opacity=\"9\" onclick=\"this.parentNode.setAttribute('width', this.parentNode.getAttribute('width') / 1.42)\">&nbsp;-&nbsp;</text>\n");

			int rowTop = rowHeight * 2 + 2;
			if (hasgroupheadings)
			{
				int start = 0;
				while (start < Columns.Count)
				{
					int end = start;
					while (end < Columns.Count - 1 && Columns[start].GroupHeading == Columns[end + 1].GroupHeading)
						end++;

					SvgRectText(s, 1, widths.Take(start).Sum() + start + 1, rowTop, widths.Skip(start).Take(end - start + 1).Sum() + end - start, rowHeight,
						        Colors.TitleFontColor, Colors.TitleBackColor, Colors.BarColor, ZAlignment.Center, Columns[start].GroupHeading);  // Paint group heading.

					start = end + 1;
				}
				
				rowTop += rowHeight + 1;
				s.Append('\n');
			}

			float x = 1;
			if (Columns.Exists(col => col.Rotate))  // At least one column has Rotate set, so do complicated 45 degree stuff accordingly.
		    {
				var graphics = Graphics.FromImage(new Bitmap(1000, 20));
				var font = new Font("Arial", 11);

				float widest = Columns.DefaultIfEmpty(new ZColumn("")).Max(col => graphics.MeasureString(col.Text, font, 1000).Width);
				double headerHeight = widest / Math.Sqrt(2) + 10;
				for (int col = 0; col < Columns.Count; col++)
				{
					// Draw a parallelogram. Start at top left, then go right by widths[col], down and right at 45 degrees by headerHeight,headerHeight, left by widths[col], then let it close itself.
					s.AppendFormat("  <polygon points=\"{0:F0},{1:F0} {2:F0},{1:F0} {3:F0},{4:F0} {5:F0},{4:F0}\" style=\"fill:", 
					               x - headerHeight, rowTop, x + widths[col] - headerHeight, x + widths[col], rowTop + headerHeight, x);
					s.Append(System.Drawing.ColorTranslator.ToHtml(Colors.TitleBackColor));
					s.Append("\" />\n");
					x += widths[col] + 1;
				}
				rowTop += (int)headerHeight;
				x = 1;
				// Draw text inside parallelgram.
				for (int col = 0; col < Columns.Count; col++)
				{
					//if (string.IsNullOrEmpty(hyper))
						s.Append("  ");
					//else
					//	AppendStrings(s, "  <a xlink:href=\"", hyper, "\">");
	
					s.Append("<text ");
	
					//if (!string.IsNullOrEmpty(hyper))
					//	s.Append("text-decoration=\"underline\" ");
	
					s.AppendFormat("text-anchor=\"end\" x=\"{0:F0}\" y=\"{1:F0}\" width=\"{2:F0}\" transform=\"rotate(45 {0:F0},{1:F0})\" font-size=\"{3}\" fill=\"",
					               x + (widths[col] - rowHeight) / 2, rowTop - 3, headerHeight * 1.4, rowHeight * 3 / 4);
					s.Append(System.Drawing.ColorTranslator.ToHtml(Colors.TitleFontColor));
					s.Append("\">");
	
					s.Append(WebUtility.HtmlEncode(Columns[col].Text));
					s.Append("</text>");
	
					//if (!string.IsNullOrEmpty(hyper))
					//	s.Append("</a>");
	
					s.Append('\n');
					x += widths[col] + 1;
				}
		    }
			else  // No Rotate. Write out (column.Heading)s in the boring way.
			{
				for (int col = 0; col < Columns.Count; col++)
				{
					SvgRectText(s, 1, x, rowTop, widths[col], rowHeight,
					        Colors.TitleFontColor, Colors.TitleBackColor, Colors.BarColor, Columns[col].Alignment, Columns[col].Text, null, Columns[col].Hyper);  // Paint column heading.
					x += widths[col] + 1;
				}
				s.Append('\n');
				rowTop += rowHeight + 1;
			}

			return rowTop;
		}

		// value, scaleMin and scaleMax are all in the before-scaling ordinate system. outputWidth gives the range of the after-scaling ordinate system.
		double Scale(double value, double outputWidth, double scaleMin, double scaleMax)
		{
			return (value - scaleMin) / (scaleMax - scaleMin) * outputWidth;
		}

		// value, scaleMin and scaleMax are all in the before-scaling ordinate system. outputWidth gives the range of the after-scaling ordinate system.
		double ScaleWidth(double value, double outputWidth, double scaleMin, double scaleMax)
		{
			return value / (scaleMax - scaleMin) * outputWidth;
		}

		// scaleMin and scaleMax are all in the before-scaling ordinate system. pixelValue and outputWidth are in the after-scaling ordinate system.
		double AntiScale(double pixelValue, double outputWidth, double scaleMin, double scaleMax)
		{
			return (pixelValue / outputWidth * (scaleMax - scaleMin)) + scaleMin;
		}

		// Return the y ordinate of the middle of the stated row. top is the top of the top row of the table. row is the 0-based row number. 
		double RowMid(float top, int row, float rowHeight)
		{
			return top + (row + 0.5) * rowHeight;
		}

		// Return normal distribution value for this x. https://en.wikipedia.org/wiki/Normal_distribution
		double Gauss(double x, double mean, double variance)
		{
			return 1 / Math.Sqrt(2 * Math.PI * variance) * Math.Exp(-Math.Pow(x - mean, 2) / 2 / variance);
		}

		void SvgChart(StringBuilder s, int top, int height, double left, double width, double chartMin, double chartMax, Color backColor, Color chartColor, ZCell cell, int column)
		{
			if (cell.Color != Color.Empty)
				SvgRect(s, 1, left, top, width, height, backColor);  // Paint chart cell(s) background.

			if ((cell.ChartType == ChartType.None || cell.ChartType.HasFlag(ChartType.Bar)) && cell.Number != null)  // Bar
				SvgRect(s, 1, left + Scale(0, width, chartMin, chartMax), top, ScaleWidth(cell.Number ?? 0, width, 0, chartMax - chartMin), height, chartColor);  // Paint bar.

			int count = 0;
			if (cell.ChartType.HasFlag(ChartType.Rug) || cell.ChartType.HasFlag(ChartType.BoxPlot) || 
			    cell.ChartType.HasFlag(ChartType.Histogram) || cell.ChartType.HasFlag(ChartType.KernelDensityEstimate))
			{
				cell.Data.Sort();
				count = cell.Data.Count;
			}

			if (cell.ChartType.HasFlag(ChartType.BoxPlot) && count > 1)  // BoxPlot
			{
				if (cell.Number != null)
					SvgRect(s, 1, left + Scale((double)cell.Number, width, chartMin, chartMax) - 0.5, top, 1, height * 0.9, ZReportColors.AddDark(chartColor, Color.FromArgb(255, 223, 255)));  // Paint mean stripe.

				double median = count % 2 == 0 ? (cell.Data[count / 2 - 1] + cell.Data[count / 2]) / 2 : cell.Data[count / 2];
				double firstQuartile = count % 4 == 0 ? cell.Data[count / 4] : cell.Data[count / 4];
				double thirdQuartile = count % 4 == 0 ? cell.Data[(count * 3 / 4) - 1] : cell.Data[count * 3 / 4];
				int percentile2 = (int)(count * 0.02);
				int percentile98 = (int)(count * 0.98) - 1;

				SvgRect(s, 1, left + Scale(cell.Data[percentile2], width, chartMin, chartMax), top + height * 0.4,
				        ScaleWidth(cell.Data[percentile98] - cell.Data[percentile2], width, chartMin, chartMax), height * 0.1, chartColor);  // Whisker from 2nd percentile to 98th percentile -- contains all data within 2 std deviations.

				SvgRect(s, 1, left + Scale(firstQuartile, width, chartMin, chartMax), top + height * 0.1,
				        ScaleWidth(thirdQuartile - firstQuartile, width, chartMin, chartMax), height * 0.7, chartColor);  // Second quartile / third quartile box.

				SvgRect(s, 1, left + Scale(median, width, chartMin, chartMax) - 0.5, top + height * 0.1, 1, height * 0.7, backColor);  // Median white stripe.
				
				for (int i = 0; i < percentile2; i++)
					SvgRect2(s, 1, left + Scale(cell.Data[i], width, chartMin, chartMax) - 0.5, top + height * 0.42, 1, height * 0.06, chartColor);  // Paint outlying data point.
				for (int i = percentile98; i < count; i++)
					SvgRect2(s, 1, left + Scale(cell.Data[i], width, chartMin, chartMax) - 0.5, top + height * 0.42, 1, height * 0.06, chartColor);  // Paint outlying data point.
			}

			if (cell.ChartType.HasFlag(ChartType.Histogram) && count > 1)  // Histogram
			{
				int bins = (int)Math.Ceiling(2 * Math.Pow(count, 1.0/3));  // number of bars our histogram will have, from Rice's Rule.
				double binWidth = Math.Round(width / bins + 0.05, 1);  // in "pixels"
				bins = (int)Math.Round(width / binWidth);
				if (Columns[column].Alignment == ZAlignment.Integer)
				{
					bins = Math.Min(bins, (int)(chartMax - chartMin + 1));
					bins = (int)((chartMax - chartMin + 1) / Math.Round((chartMax - chartMin + 1) / bins));
				}
				int i = 0; // Index into sourceCell.Data for where we're up to right now.
				var heights = new List<int>();
				for (double xx = 0; xx < width; xx += binWidth)
				{
					int binHeight = 0;
					double binEnd = AntiScale(xx + binWidth, width, chartMin, chartMax);
					while (i < count && cell.Data[i] < binEnd)
					{
						binHeight++;
						i++;
					}
					heights.Add(binHeight);
				}
				
				for (i = 0; i < heights.Count; i++)
//						for (double xx = chartMin; xx < chartMax; xx += (chartMax - chartMin) / bins)
				{
					if (heights[i] > 0)
						SvgRect(s, 1, left + width * i / bins, top + height - height * heights[i] / heights.Max(), 
						        width / bins - 0.1, height * heights[i] / heights.Max(), chartColor);
				}
			}

			if (cell.ChartType.HasFlag(ChartType.KernelDensityEstimate) && count > 1)  // Kernel Density Estimate
			{
				double sum = cell.Data.Sum();
				double squaredSum = cell.Data.Sum(x => x * x);
				double mean = sum / count;
				double stddev = count <= 1 ? 0 : Math.Sqrt((squaredSum - (sum * sum / count)) / (count - 1));
				double bandwidth =  1.06 * stddev * Math.Pow(count, -0.2);
				
				int n = width < 100 ? (int)width * 10 : (int)width * 10 / (int)(width / 50);
				
				var points = new List<Tuple<double, double>>();
				points.Add(new Tuple<double, double>(left, 0));
				double yMax = 0;
				for (double xx = chartMin; xx < chartMax; xx += (chartMax - chartMin) / n)
				{
					double y = 0;
					foreach (double d in cell.Data)
						y += Gauss(xx, d, bandwidth);
					yMax = Math.Max(yMax, y);
					points.Add(new Tuple<double, double>(left + Scale(xx, width, chartMin, chartMax), y));
				}
				points.Add(new Tuple<double, double>(left + width, 0));
				SvgPolygon(s, 1, points, top, height, yMax, chartColor);
				SvgRect(s, 1, left + Scale(mean, width, chartMin, chartMax) - 0.05, top, 0.1, height, backColor);  // Paint mean stripe.
				SvgRect(s, 1, left + Scale(mean - stddev, width, chartMin, chartMax) - 0.05, top + height * 0.393, 0.1, height * 0.607, backColor);  // -1 stddev.
				SvgRect(s, 1, left + Scale(mean + stddev, width, chartMin, chartMax) - 0.05, top + height * 0.393, 0.1, height * 0.607, backColor);  // +1 stddev.
				SvgRect(s, 1, left + Scale(mean - 2 * stddev, width, chartMin, chartMax) - 0.05, top + height * 0.865, 0.1, height * 0.135, backColor);  // -2.
				SvgRect(s, 1, left + Scale(mean + 2 * stddev, width, chartMin, chartMax) - 0.05, top + height * 0.865, 0.1, height * 0.135, backColor);  // +2.
				double height3 = Math.Max(height * 0.011, 0.1);
				SvgRect2(s, 1, left + Scale(mean - 3 * stddev, width, chartMin, chartMax) - 0.05, top + height - height3, 0.1, height3, backColor);  // -3.
				SvgRect2(s, 1, left + Scale(mean + 3 * stddev, width, chartMin, chartMax) - 0.05, top + height - height3, 0.1, height3, backColor);  // +3.
			}

			if (cell.ChartType.HasFlag(ChartType.Rug))  // Rug
		    {
				int markNumber = 0;  // This is going to be 0 for most marks, but where marks coincide or overlap we will increment this to prevent them overpainting.
				double lastCentre = double.MinValue;
				double markWidth = Math.Max(Math.Min(width / Math.Max(count * 2, 100.0), height * 0.1), 0.2); // Width of a mark is 1/100th of the row width, or smaller if there's lots of data points, or the mark height; whichever smallest. If less than 0.2, round up to 0.2.
				int marksPerRow = Math.Min(Math.Max((int)Math.Sqrt(count), 9), 75);
				double markHeight = height * 0.9 / marksPerRow;
				
				foreach (double d in cell.Data)
				{
					double markCentre = Scale(d, width, chartMin, chartMax);
					if (markCentre - lastCentre < markWidth * 1.1)  // Clear of other marks? Drop down to bottom of the row. Not clear? Go in above the last mark, unless we've reached top of the row, in which case modulo back down to 0.
						markNumber = (markNumber + 1) % marksPerRow;
					else
					{
						markNumber = 0;
						lastCentre = markCentre;
					}

					Color markColor = cell.ChartType.HasFlag(ChartType.KernelDensityEstimate) || 
						(cell.ChartType.HasFlag(ChartType.Bar) && 0 < d && d < cell.Number) ? backColor : chartColor;
					SvgRect2(s, 1, left + markCentre - markWidth / 2, top + height - (markNumber + 1) * markHeight, markWidth, markHeight, markColor);  // Paint mark.
				}
			}
		}

		// Write a single table row.
		void SvgRow(StringBuilder s, int top, int height, List<float> widths, List<double> mins, List<double> maxs, int width, ZRow row, bool odd)
		{
			SvgRect(s, 1, 1, top, width, height, Colors.GetBackColor(odd));  // Paint the background for the whole row.

			// Paint any chart cells for this row.
			int start = 0;
			while (start < Math.Min(Columns.Count, row.Count))
			{
				int end = start;
				while (end < Math.Min(Columns.Count, row.Count) - 1 && row[start].ChartCell != null && row[start].ChartCell == row[end + 1].ChartCell)
					end++;

				if (start == end && row[start].ChartType != ChartType.None && row[start].ChartCell == null)  // This is a one-column wide bar.
					row[start].ChartCell = row[start];
				var sourceCell = row[start].ChartCell ?? row[start];
				int barSource = row.FindIndex(cell => cell == sourceCell);

				if (sourceCell.Color != Color.Empty || sourceCell.ChartCell != null)
					SvgChart(s, top, height, widths.Take(start).Sum() + start + 1, widths.Skip(start).Take(end - start + 1).Sum() + end - start,
					         MaxChartByColumn ? mins[barSource] : mins.Min(), MaxChartByColumn ? maxs[barSource] : maxs.Max(), 
					         Colors.GetBackColor(odd, sourceCell.Color), sourceCell.GetBarColor(Colors.GetBackColor(odd), Colors.BarNone), sourceCell, barSource);

				start = end + 1;
			}
			s.Append('\n');

			float x = 1;
			for (int col = 0; col < Columns.Count && col < row.Count; col++)
			{
				SvgText(s, 1, (int)x, top, (int)widths[col], height, Columns[col], row[col]);  // Write a data cell.

				x += widths[col] + 1;
			}
			s.Append('\n');
		}

		// This writes an <svg> tag -- it does not include <head> or <body> tags etc.
		public override string ToSvg(int table)
		{
			bool hasgroupheadings = false;
			foreach (ZColumn col in Columns)
				hasgroupheadings |= col != null && !string.IsNullOrEmpty(col.GroupHeading);

			var widths = new List<float>();  // Width of each column in pixels. "float", because MeasureString().Width returns a float.
			var mins = new List<double>();   // Minimum numeric value in each column, or if all numbers are positive, 0.
			var maxs = new List<double>();   // Maximum numeric value in each column.
			Widths(widths, mins, maxs);
			int width = (int)widths.Sum() + widths.Count + 1;  // Total width of the whole SVG -- the sum of each column, plus pixels for spacing left, right and between.
			double max = maxs.DefaultIfEmpty(1).Max();

			int rowHeight = 15;  // This is enough to fit 11-point text.
			int height = (Rows.Count + 3 + (hasgroupheadings ? 1 : 0)) * (rowHeight + 1);

			if (Columns.Exists(col => col != null && col.Rotate))  // At least one column has Rotate set, so add to height to allow room for those 45 degree headers.
			{
				var graphics = Graphics.FromImage(new Bitmap(1000, 20));
				var font = new Font("Arial", 11);

				float widest = Columns.DefaultIfEmpty(new ZColumn("")).Max(col => graphics.MeasureString(col.Text, font, 1000).Width);
				height += (int)(widest / Math.Sqrt(2) + 10 - rowHeight);
			}

			StringBuilder s = new StringBuilder();

			int rowTop = SvgHeader(s, hasgroupheadings, rowHeight, height, widths, maxs, width, max);
			int ribbonTop = rowTop;

			bool odd = true;

			foreach (ZRow row in Rows)
			{
				SvgRow(s, rowTop, rowHeight, widths, mins, maxs, width, row, odd);

				rowTop += rowHeight + 1;
				odd = !odd;
			}

			for (int col = 0; col < Columns.Count; col++)
				if (Columns[col] is ZRibbonColumn)
					SvgRibbon(s, 1, (ZRibbonColumn)Columns[col], widths.Take(col).Sum(w => w + 1) + 0.5F, widths[col] + 0.5F, ribbonTop - 0.5F, (rowHeight + 1) * Rows.Count, rowHeight + 1);

			s.Append("</svg>\n");

			if (!string.IsNullOrEmpty(Description))
		    	AppendStrings(s, "<p>", Description, "</p>\n");
			s.Append("</div>");

			return s.ToString();
		}
	}

	public class ZoomHtmlInclusion: ZoomReportBase
	{
		public string Literal { get; set; }

		public ZoomHtmlInclusion(string literal) { Literal = literal; }

		public override string ToCsv(char separator) { return ""; }

		public override string ToHtml() { return Literal; }

		public override string ToSvg(int table) { return Literal; }

		public override IEnumerable<Color> BarCellColors() { return new List<Color>(); }
	}
	
	public class ZoomReports: List<ZoomReportBase>
	{
		string Title { get; set; }
		/// <summary>If true, show bars in HTML reports.</summary>
		bool Bars { get; set; }

		ZReportColors colors;
		public ZReportColors Colors { get { return colors; } }
		
//		int HeightUsed { get; set; }

		public ZoomReports()
		{
			colors = new ZReportColors();
			Bars = true;
		}

		public ZoomReports(string title)
		{
			Title = title;
			colors = new ZReportColors();
			Bars = true;
		}

		public new void Add(ZoomReportBase report)
		{
			base.Add(report);
			if (report.Colors == null)
				report.Colors = colors;
		}

		// Return a list of colours of bar cells, over all reports.
		List<Color> BarCellColors()
		{
			var result = new List<Color>();
			foreach (var report in this)
				if (report is ZoomReport)
				{
					bool odd = true;
					foreach (var row in ((ZoomReport)report).Rows)
					{
						foreach (var cell in row)
							if (!result.Contains(cell.GetBarColor(colors.GetBackColor(odd))))
							    result.Add(cell.GetBarColor(colors.GetBackColor(odd)));
						odd = !odd;
					}
				}

			if (!result.Contains(colors.BarNone))
				result.Add(colors.BarNone);
			if (!result.Contains(ZReportColors.AddDark(colors.BarNone, colors.OddColor)))
				result.Add(ZReportColors.AddDark(colors.BarNone, colors.OddColor));

			return result;
//			return this.SelectMany(x => x.BarCellColors()).Distinct().ToList();
		}

		/// <summary>Export to the specified format.</summary>
		public string ToOutput(OutputFormat outputFormat)
		{
			switch (outputFormat) {
				case OutputFormat.Svg: return ToSvg();
				case OutputFormat.HtmlTable: return ToHtml();
				case OutputFormat.Tsv: return ToCsv('\t');
				case OutputFormat.Csv: return ToCsv(',');
				default: return "";
			}
		}

		public string ToCsv(char separator)
		{
			StringBuilder sb = new StringBuilder();
			foreach (ZoomReportBase report in this) {
				sb.Append(report.ToCsv(separator));
		 		sb.Append("\n--------\n\n");
			}
			return sb.ToString();
		}

		public string ToHtml()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"/><title>");
			if (!string.IsNullOrEmpty(Title))
				sb.Append(WebUtility.HtmlEncode(Title));
			else if (Count > 0)
				sb.Append(WebUtility.HtmlEncode(this[0].Title));
			sb.Append("</title>");

			if (Bars)
			{
				sb.Append("\n  <style type=\"text/css\">\n");
				sb.Append("    .barcontainer { position:relative }\n");
				sb.Append("    .bar { padding-bottom: 18px; background-color: #DFDFDF }\n");
				sb.Append("    .bartext { position: absolute; top: 0px; left: 0px; text-align: right; width: 100% }\n");

				foreach (var c in BarCellColors())
					sb.AppendFormat("    .bar{0:X2}{1:X2}{2:X2} {{ padding-bottom: 18px; background-color: {3} }}\n", 
					                c.R, c.G, c.B, System.Drawing.ColorTranslator.ToHtml(c));

				sb.Append("  </style>\n");
			}

			sb.Append("</head><body style=\"background-color: #EEF\">\n");
			//sb.Append("");  // TODO: cellstyles.ToHtml here?

			foreach (ZoomReportBase report in this) {
				sb.Append(report.ToHtml());
		 		sb.Append("\n<br/>\n\n");
			}

			sb.Append("</body>\n");
			return sb.ToString();
		}

		public string ToSvg()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"/><title>");

			if (!string.IsNullOrEmpty(Title))
				sb.Append(WebUtility.HtmlEncode(Title));
			else if (Count > 0)
				sb.Append(WebUtility.HtmlEncode(this[0].Title));
			sb.Append("</title></head><body style=\"background-color: #EEF\">\n");
			//sb.Append("");  // TODO: cellstyles.ToHtml here?

			if (Count == 1)
				sb.Append("<div>\n");
			else
				sb.Append("<div style=\"display: flex; flex-flow: row wrap; justify-content: space-around;\">\n");

			for (int i = 0; i < Count; i++)
				sb.Append(this[i].ToSvg(i));

			sb.Append(@"</div>
<script>
window.onload = function() {
  var texts = document.querySelectorAll('text');
  for (i = 0; i < texts.length; i++) {
    var fit = texts[i].getComputedTextLength() / (texts[i].getAttribute('width') - 2);
    if (fit > 1)
      texts[i].setAttribute('font-size', texts[i].getAttribute('font-size') / fit);
  }
}
</script>
");

			sb.Append("</body>\n");

			return sb.ToString();
		}
	}
}
