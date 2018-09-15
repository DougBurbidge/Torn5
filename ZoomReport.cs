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
	public enum ZColumnType { String, Integer, Float };
	public enum ZNumberStyle { Comma, Plain, Brief };  // Comma: use a thousands separator.  Brief: convert larger numbers to 2k, 3M, etc.
	public enum ZAlignment { None, Left, Right, Center };

	public static class ZAlignmentExtensions
	{
		public static string ToHtml(this ZAlignment me)
		{
			switch (me)
			{
				case ZAlignment.None:   return "";
				case ZAlignment.Left:   return " align=\"left\"";
				case ZAlignment.Right:  return " align=\"right\"";
				case ZAlignment.Center: return " align=\"center\"";
				default: return "";
			}
		}
	}

	/// This is just the header row(s) and metadata for a column -- does not include the actual cells.
	public class ZColumn
	{
		/// <summary>store the heading text for each column</summary>
		public string Heading { get; set; }
		/// <summary>Optional. Appears *above* Heading.</summary>
	    public string GroupHeading { get; set; }
		/// <summary>If this column heading contains text which links to another report, put the URL of that report here.</summary>
		public string Hyper { get; set; }
		/// <summary>left, right or center</summary>
	    public ZAlignment Alignment { get; set; }
	    public ZColumnType ColumnType { get; set; }
		/// <summary>If true, we should try rotating the header text to make it fit better.</summary>
		public bool Rotate;
	    
	    public ZColumn(string heading)
	    {
	    	Heading = heading;
	    }

	    public ZColumn(string heading, ZAlignment alignment)
	    {
	    	Heading = heading;
	    	Alignment = alignment;
	    }

	    public ZColumn(string heading, ZAlignment alignment, ZColumnType columnType)
	    {
	    	Heading = heading;
	    	Alignment = alignment;
	    	ColumnType = columnType;
	    }
	}

//	public class ZColumns: List<ZColumn>
//  function  Find(heading: string): Integer;

	[Flags]
	public enum ChartType { None = 0, Bar = 1, Rug = 2, BoxPlot = 4, KernelDensityEstimate = 8 };

	/// <summary>Represents a single cell in a table. The cell can optionally have a horizontal chart bar.</summary>
	public class ZCell
	{
		string text;
		
		public string Text  // Text data to appear in this cell. 
		{
			get 
			{
				if (string.IsNullOrEmpty(text))
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

		public bool Empty()
		{
			return string.IsNullOrEmpty(text) && Number == null;
		}
		
		/// <summary>If this cell contains a number, put it here.</summary>
		public double? Number { get; set; }
		public string NumberFormat { get; set; }
		/// <summary>If this cell contains text which links to another report, put the URL of that report here.</summary>
		public string Hyper { get; set; }
		/// <summary>Optional background colour.</summary>
		public Color Color { get; set; }
		/// <summary>If true, show a chart bar for this cell. If BarCell is not set, use this cell's number; otherwise use the cell specified in BarCell.
		/// If BarCell is set, setting/clearing Bar has no effect.</summary>
		public ChartType ChartType { get; set; }
		/// <summary>Optional pointer to cell whose value we are to show as a horizontal chart bar.</summary>
		public int? ChartCell { get; set; }  
		/// <summary>Color of optional horizontal chart bar.</summary>
		public Color BarColor { get; set; }
		/// <summary>List of values to be shown as a scatter plot / quartile plot / stem-and-leaf plot / rug map / kernel density estimation.</summary>
		public List<double> Data { get; private set; }
		/// <summary>Can hold whatever data the caller wants.</summary>
    	public object Custom { get; set; }

		public ZCell(string text = "", Color color = default(Color), int? barCell = null)
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
		///<summary>If there is exactly one chart in this row (because exactly one cell has its ChartType set, or because all the  ChartCell 
		/// values specified in this row point to just one cell, return the index of the cell whose value is used for the bar.</summary>
		public int? OneBarCell()
		{
			int result = 0;
			bool foundOneBar = false;
			for (int col = 0; col < this.Count; col++)
			{
				var cell = this[col];
				if (foundOneBar)
				{
					if ((cell.ChartCell != null && cell.ChartCell != result) || (cell.ChartType != ChartType.None && result != col))
						return null;
				}
				else  // No Bar found yet.
				{
					if (cell.ChartCell != null)
					{
						foundOneBar = true;
						result = (int)cell.ChartCell;
					}
					else if (cell.ChartType != ChartType.None)
					{
						foundOneBar = true;
						result = col;
					}
				}
			}
			return foundOneBar ? (int?)result : null;
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
				Color.FromArgb(color1.R + ((Color)color2).R - 255, color1.G + ((Color)color2).G - 255, color1.B + ((Color)color2).B - 255);
		}
	}

	public abstract class ZoomReportBase
	{
		public string Title { get; set; }  // Title for the whole report.
		public ZReportColors Colors { get; set; }

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

		public ZoomReport(string title, string headings = "", string alignments = "")
		{
			Columns = new List<ZColumn>();
			Rows = new List<ZRow>();

			Title = title;

			if (!string.IsNullOrEmpty(headings))
				foreach (string heading in headings.Split(','))
					Columns.Add(new ZColumn(heading));
			
			if (!string.IsNullOrEmpty(alignments))
			{
				string[] alignmentList = alignments.Split(',');
				for (int i = 0; i < Columns.Count && i < alignmentList.Length; i++)
					Columns[i].Alignment = (ZAlignment)Enum.Parse(typeof(ZAlignment), alignmentList[i], true);
			}

			Bars = true;
		}

		//procedure SaveAsCsv(var F: TextFile; separator: char = ',');

		static void AppendStrings(StringBuilder builder, params string[] strings)
		{
			foreach (string s in strings)
				builder.Append(s);
		}

		public bool ColumnEmpty(int i)
		{
			foreach(ZRow row in Rows)
				if (i < row.Count && !row[i].Empty())
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

		void Widths(List<float> widths, List<double> mins, List<double> maxs)
		{
			var graphics = Graphics.FromImage(new Bitmap(1000, 20));
			var font = new Font("Arial", 11);

			for (int col = 0; col < Columns.Count; col++)
			{
				float widest = 0;
				float total = widest;
				int count = 1;
				double min = 0.0;
				double max = 0.0;
				string numberFormat = "";

				for (int row = 0; row < Rows.Count; row++)
				{
					if (col < Rows[row].Count)
					{
						if (Rows[row][col].Number.HasValue && !double.IsNaN((double)Rows[row][col].Number))
						{
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

				if (max > 0 || !string.IsNullOrEmpty(numberFormat))
					widths.Add(graphics.MeasureString(max.ToString(numberFormat), font, 1000).Width * 1.01f);
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
				s.Append(col.Heading);
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
			bool hasgroupheadings = false;
			foreach (ZColumn col in Columns)
				hasgroupheadings |= !string.IsNullOrEmpty(col.GroupHeading);

			StringBuilder s = new StringBuilder();

			s.Append("\n<table align=\"center\">\n");
			s.Append("  <thead>\n");

			if (!string.IsNullOrEmpty(Title))
				AppendStrings(s, "    <tr bgcolor=\"", System.Drawing.ColorTranslator.ToHtml(Colors.TitleBackColor), "\">\n",
				              "       <th colspan=\"", Columns.Count.ToString(CultureInfo.InvariantCulture), "\"><H2>", WebUtility.HtmlEncode(Title), "</H2></th>\n",
				              "    </tr>\n");

			if (hasgroupheadings)
			{
				AppendStrings(s, "    <tr bgcolor=\"", System.Drawing.ColorTranslator.ToHtml(Colors.TitleBackColor), "\">\n");

				int start = 0;
				while (start < Columns.Count)
				{
					int end = start;
					while (end < Columns.Count - 1 && Columns[start].GroupHeading == Columns[end + 1].GroupHeading)
						end++;

					if (start == end)
						AppendStrings(s, "      <th align=\"center\">", 
						              WebUtility.HtmlEncode(Columns[start].GroupHeading), "</th>\n");
					else
						AppendStrings(s, "      <th align=\"center\" colspan=\"" + (end - start + 1).ToString(CultureInfo.InvariantCulture) + "\">",
						              WebUtility.HtmlEncode(Columns[start].GroupHeading), "</th>\n");

					start = end + 1;
				}

				s.Append("    </tr>\n");
			}

			AppendStrings(s, "    <tr bgcolor=\"", System.Drawing.ColorTranslator.ToHtml(Colors.TitleBackColor), "\">\n");

			foreach (ZColumn col in Columns)
				if (string.IsNullOrEmpty(col.Hyper))
					AppendStrings(s, "      <th", col.Alignment.ToHtml(), ">", WebUtility.HtmlEncode(col.Heading), "</th>\n");
				else
					AppendStrings(s, "      <th", col.Alignment.ToHtml(), "><a href=\"", col.Hyper, "\">", WebUtility.HtmlEncode(col.Heading), "</a></th>\n");

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
				s.Append("    <tr style=\"background-color: ");
				s.Append(System.Drawing.ColorTranslator.ToHtml(Colors.GetBackColor(odd)));
				s.Append("\">\n");

				int? oneBarCell = row.OneBarCell();

				for (int col = 0; col < Columns.Count && col < row.Count; col++)
				{
					// find colour for the cell
					string cellColor;
					if (row[col].Color == Color.Empty)
						cellColor = "";
					else
						cellColor = " style=\"background-color: " + System.Drawing.ColorTranslator.ToHtml(Colors.GetBackColor(odd, row[col].Color)) + '"';

					if (Bars && /*!oneBar &&*/ row[col].Number != null && (row[col].ChartType != ChartType.None || row[col].ChartCell == col))
					{
						// find the max value to scale the bar against
//						if (MaxBarByColumn)
//						  thisbar := FMaxBars[col]
//						else if (MaxBarByRow)
//						  thisbar := FMaxBars[j]
//						else
//						  thisbar := FMaxBar;
						
						var barColor = row[col].GetBarColor(Colors.GetBackColor(odd), Colors.BarNone);

						AppendStrings(s, "      <td class=\"barcontainer\"", cellColor, ">\n");
						s.AppendFormat("        <div class=\"bar{0:X2}{1:X2}{2:X2}\" style=\"width: {3}%\" />\n", 
						               barColor.R, barColor.G, barColor.B, (int)((double)(row[col].Number) * 100 / (double)max));
						AppendStrings(s, "        <div", Columns[col].Alignment.ToHtml(), " class=\"bartext\">");
				    }
					else
						AppendStrings(s, "      <td", Columns[col].Alignment.ToHtml(), cellColor, ">");

					if (string.IsNullOrEmpty(row[col].Hyper))
						s.Append(WebUtility.HtmlEncode(row[col].Text));
					else
						AppendStrings(s, "<a href=\"", row[col].Hyper, "\">", WebUtility.HtmlEncode(row[col].Text), "</a>");

					if (Bars)
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
		void SvgRectText(StringBuilder s, int indent, int x, int y, int width, int height, Color fontColor, Color backColor, Color barColor, ZAlignment alignment, string text, string id = null, string hyper = null, double? bar = null)
		{
			SvgRect(s, indent, x, y, width, height, backColor);
			if (bar != null)
				SvgRect(s, indent, x, y, width * (double)bar, height, barColor);
			SvgText(s, indent, x, y, width, height, fontColor, alignment, text, id, hyper);
		}

		void SvgRect(StringBuilder s, int indent, double x, double y, double width, double height, Color fillColor)
		{
			s.Append('\t', indent);
			s.AppendFormat("<rect x=\"{0:F1}\" y=\"{1:F0}\" width=\"{2:F1}\" height=\"{3:F0}\" style=\"fill:", x, y, width, height);
			s.Append(System.Drawing.ColorTranslator.ToHtml(fillColor));
			s.Append("\" />\n");
		}

		void SvgText(StringBuilder s, int indent, int x, int y, int width, int height, Color fontColor, ZAlignment alignment, string text, string id = null, string hyper = null)
		{
			if (string.IsNullOrEmpty(text))
			    return;

			s.Append('\t', indent);
			if (!string.IsNullOrEmpty(hyper))
				AppendStrings(s, "<a xlink:href=\"", hyper, "\">");

			s.Append("<text ");
			if (!string.IsNullOrEmpty(id))
				AppendStrings(s, "id=\"", id, "\" ");

			if (!string.IsNullOrEmpty(hyper))
				s.Append("text-decoration=\"underline\" ");

			switch (alignment)
			{
				default: // i.e. Left: 
					s.AppendFormat("x=\"{0}\" y=\"{1}\" width=\"{2}\" font-size=\"{3}\"",
					                       x + 1, y + height * 3 / 4, width, height * 3 / 4);
				break;
				case ZAlignment.Center:
					s.AppendFormat("text-anchor=\"middle\" x=\"{0}\" y=\"{1}\" width=\"{2}\" font-size=\"{3}\"",
				                       x + width / 2, y + height * 3 / 4, width, height * 3 / 4);
				break;
				case ZAlignment.Right:
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
			s.Append(WebUtility.HtmlEncode(text));
			s.Append("</text>");

			if (!string.IsNullOrEmpty(hyper))
				s.Append("</a>");

			s.Append('\n');
		}

		void SvgPolygon(StringBuilder s, int indent, List<Tuple<double, double>> points, double rowTop, double rowHeight, double yMax, Color fillColor)
		{
			s.Append('\t', indent);
			s.Append("<polygon points=\"");
			foreach (var point in points)
				s.AppendFormat("{0:F1},{1:F1} ", point.Item1, rowTop + rowHeight - Scale(point.Item2, rowHeight, 0, yMax));
			s.Append("\" style=\"fill:");
			s.Append(System.Drawing.ColorTranslator.ToHtml(fillColor));
			s.Append("\" />\n");
		}

		// Write the opening <svg tag and the header row(s). Returns the amount of vertical height it has consumed.
		int SvgHeader(StringBuilder s, bool hasgroupheadings, int rowHeight, int height, List<float> widths, List<double> maxs, int width, double max)
		{
			s.AppendFormat("\n<svg viewBox=\"0 0 {0} {1}\" width=\"{0}\" align=\"center\">\n", width, height);  // width=\"{0}\" height=\"{1}\"

			SvgRect(s, 1, 1, 1, width - 2, rowHeight * 2, Colors.TitleBackColor);  // Paint title "row" background.

			// Add '-' and '+' zoom button text.
			s.Append("  <text text-anchor=\"middle\" x=\"15\" y=\"23\" width=\"30\" height=\"30\" font-size=\"22\" fill=\"Black\">&nbsp;+&nbsp;</text>\n");
			s.Append("  <text text-anchor=\"middle\" x=\"45\" y=\"23\" width=\"30\" height=\"30\" font-size=\"22\" fill=\"Black\">&nbsp;-&nbsp;</text>\n");

			SvgText(s, 1, 1, 1, width - 2, rowHeight * 2, Colors.TitleFontColor, ZAlignment.Center, Title);  // Paint title "row" text.
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

					SvgRectText(s, 1, (int)widths.Take(start).Sum() + start + 1, rowTop, (int)widths.Skip(start).Take(end - start + 1).Sum() + end - start, rowHeight,
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

				float widest = Columns.DefaultIfEmpty(new ZColumn("")).Max(col => graphics.MeasureString(col.Heading, font, 1000).Width);
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
	
					s.Append(WebUtility.HtmlEncode(Columns[col].Heading));
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
					ZColumn column = Columns[col];
					SvgRectText(s, 1, (int)x, rowTop, (int)widths[col], rowHeight,
					        Colors.TitleFontColor, Colors.TitleBackColor, Colors.BarColor, column.Alignment, column.Heading, null, column.Hyper);  // Paint column heading.
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

		// Return normal distribution value for this x. https://en.wikipedia.org/wiki/Normal_distribution
		double Gauss(double x, double mean, double variance)
		{
			return 1 / Math.Sqrt(2 * Math.PI * variance) * Math.Exp(-Math.Pow(x - mean, 2) / 2 / variance);
		}

		// Write a single table row.
		void SvgRow(StringBuilder s, bool hasgroupheadings, int rowHeight, int rowTop, List<float> widths, List<double> mins, List<double> maxs, int width, ZRow row, bool odd)
		{
			SvgRect(s, 1, 1, rowTop, width, rowHeight, Colors.GetBackColor(odd));  // Paint the background for the whole row.

			// Paint cell backgrounds for this row
			int start = 0;
			while (start < Math.Min(Columns.Count, row.Count))
			{
				int end = start;
				while (end < Math.Min(Columns.Count, row.Count) - 1 && row[start].ChartCell != null && row[start].ChartCell == row[end + 1].ChartCell)
					end++;

				if (start == end && row[start].ChartType != ChartType.None && row[start].ChartCell == null)  // This is a one-column wide bar.
					row[start].ChartCell = start;
				var barSource = row[start].ChartCell ?? start;
				var sourceCell = row[barSource];

				if (sourceCell.Color != Color.Empty || sourceCell.ChartCell != null)
				{
					double chartx = widths.Take(start).Sum() + start + 1;
					double chartWidth = widths.Skip(start).Take(end - start + 1).Sum() + end - start;
					double chartMin = MaxChartByColumn ? mins[barSource] : mins.Min();
					double chartMax = MaxChartByColumn ? maxs[barSource] : maxs.Max();
					Color backColor = Colors.GetBackColor(odd, sourceCell.Color);
					Color chartColor = sourceCell.GetBarColor(Colors.GetBackColor(odd), Colors.BarNone);

					if (sourceCell.Color != Color.Empty)
						SvgRect(s, 1, chartx, rowTop, chartWidth, rowHeight, backColor);  // Paint chart cell(s) background.

					if ((sourceCell.ChartType == ChartType.None || sourceCell.ChartType.HasFlag(ChartType.Bar)) && sourceCell.Number != null)  // Bar
						SvgRect(s, 1, chartx + Scale(0, chartWidth, chartMin, chartMax), rowTop, Scale(sourceCell.Number ?? 0, chartWidth, 0, chartMax - chartMin), rowHeight, chartColor);  // Paint bar.

					int count = 0;
					if (sourceCell.ChartType.HasFlag(ChartType.Rug) || sourceCell.ChartType.HasFlag(ChartType.BoxPlot) || sourceCell.ChartType.HasFlag(ChartType.KernelDensityEstimate))  // BoxPlot
					{
						sourceCell.Data.Sort();
						count = sourceCell.Data.Count;
					}

					if (sourceCell.ChartType.HasFlag(ChartType.BoxPlot))
					{
						if (sourceCell.Number != null)
							SvgRect(s, 1, chartx + chartWidth * (double)sourceCell.Number / chartMax - 0.5, rowTop, 1, rowHeight * 0.9, chartColor);  // Paint mean stripe.

						if (count > 1)
						{
							double median = count % 2 == 0 ? (sourceCell.Data[count / 2 - 1] + sourceCell.Data[count / 2]) / 2 : sourceCell.Data[count / 2];
							double firstQuartile = count % 4 == 0 ? sourceCell.Data[count / 4] : sourceCell.Data[count / 4];
							double thirdQuartile = count % 4 == 0 ? sourceCell.Data[(count * 3 / 4) - 1] : sourceCell.Data[count * 3 / 4];

							SvgRect(s, 1, chartx + Scale(sourceCell.Data[0], chartWidth, 0, chartMax), rowTop + rowHeight * 0.4,
							        Scale(sourceCell.Data[count - 1] - sourceCell.Data[0], chartWidth, 0, chartMax), rowHeight * 0.1, chartColor);  // Bottom quartile / top quartile whisker.

							SvgRect(s, 1, chartx + Scale(firstQuartile, chartWidth, 0, chartMax), rowTop + rowHeight * 0.1,
							        Scale(thirdQuartile - firstQuartile, chartWidth, 0, chartMax), rowHeight * 0.7, chartColor);  // Second quartile / third quartile box.

							SvgRect(s, 1, chartx + Scale(median, chartWidth, 0, chartMax) - 0.5, rowTop + rowHeight * 0.1, 1, rowHeight * 0.7, backColor);  // Median white stripe.
						}
					}

					if (sourceCell.ChartType.HasFlag(ChartType.KernelDensityEstimate))  // Kernel Density Estimate
					{
						if (sourceCell.Number != null)
							SvgRect(s, 1, chartx + chartWidth * (double)sourceCell.Number / chartMax - 0.5, rowTop, 1, rowHeight * 0.9, chartColor);  // Paint mean stripe.

						if(count > 1)
						{
							double sum = 0;
							double squaredSum = 0;
							foreach (double d in sourceCell.Data)
							{
								sum += d;
								squaredSum += d * d;
							}
							double mean = sum / count;
							double variance = count <= 1 ? 0 : (squaredSum - (sum * sum / count)) / (count - 1);
							double bandwidth =  1.06 * Math.Sqrt(variance) * Math.Pow(count, -0.2);
							
							int n = (int)chartWidth;
							if (n > 100)
								n /= (int)(n / 50);
							
							var points = new List<Tuple<double, double>>();
							points.Add(new Tuple<double, double>(chartx, 0));
							double yMax = 0;
							for (double xx = chartMin; xx < chartMax; xx += (chartMax - chartMin) / n)
							{
								double y = 0;
								foreach (double d in sourceCell.Data)
								{
									y += Gauss(xx, mean, variance);
									yMax = Math.Max(yMax, y);
								}
								points.Add(new Tuple<double, double>(chartx + Scale(xx, chartWidth, chartMin, chartMax), y));
							}
							points.Add(new Tuple<double, double>(chartx + chartWidth, 0));
							SvgPolygon(s, 1, points, rowTop, rowHeight, yMax, chartColor);
						}
					}

					if (sourceCell.ChartType.HasFlag(ChartType.Rug))  // Rug
				    {
						int markNumber = 0;  // This is going to be 0 for most marks, but where marks coincide or overlap we will increment this to prevent them overpainting.
						double lastCentre = double.MinValue;
						double markWidth = Math.Max(Math.Min(chartWidth / Math.Max(count * 2, 100.0), rowHeight * 0.1), 1); // Width of a mark is 1/100th of the row width, or smaller if there's lots of data points, or the mark height; whichever smallest. If less than 1, round up to 1.

						foreach (double d in sourceCell.Data)
						{
							double markCentre = Scale(d, chartWidth, chartMin, chartMax);
							if (markCentre - lastCentre < markWidth)
								markNumber++;
							else
								markNumber = 0;
							markNumber %= 9;

							Color markColor = sourceCell.ChartType.HasFlag(ChartType.KernelDensityEstimate) || (sourceCell.ChartType.HasFlag(ChartType.Bar) && 0 < d && d < sourceCell.Number) ? backColor : chartColor;
							SvgRect(s, 1, chartx + markCentre - markWidth / 2, rowTop + rowHeight * (9 - markNumber) * 0.1, 
							        markWidth, rowHeight * 0.1, markColor);  // Paint mark.

							lastCentre = markCentre;
						}
					}
				}
				start = end + 1;
			}
			s.Append('\n');

			float x = 1;
			for (int col = 0; col < Columns.Count && col < row.Count; col++)
			{
				SvgText(s, 1, (int)x, rowTop, (int)widths[col], rowHeight,
				        Colors.TextColor, Columns[col].Alignment, row[col].Text, null, row[col].Hyper);  // Write a data cell.

				x += widths[col] + 1;
			}
			s.Append('\n');
		}

		// This writes an <svg> tag -- it does not include <head> or <body> tags etc.
		public override string ToSvg(int table)
		{
			bool hasgroupheadings = false;
			foreach (ZColumn col in Columns)
				hasgroupheadings |= !string.IsNullOrEmpty(col.GroupHeading);

			int rowHeight = 15;  // This is enough to fit 11-point text.
			int height = (Rows.Count + 3 + (hasgroupheadings ? 1 : 0)) * (rowHeight + 1);

			var widths = new List<float>();  // Width of each column in pixels. "float", because MeasureString().Width returns a float.
			var mins = new List<double>();   // Minimum numeric value in each column, or if all numbers are positive, 0.
			var maxs = new List<double>();   // Maximum numeric value in each column.
			Widths(widths, mins, maxs);
			int width = (int)widths.Sum() + widths.Count + 1;  // Total width of the whole SVG -- the sum of each column, plus pixels for spacing left, right and between.
			double max = maxs.DefaultIfEmpty(1).Max();

			StringBuilder s = new StringBuilder();

			int rowTop = SvgHeader(s, hasgroupheadings, rowHeight, height, widths, maxs, width, max);

			bool odd = true;

			foreach (ZRow row in Rows)
			{
				SvgRow(s, hasgroupheadings, rowHeight, rowTop, widths, mins, maxs, width, row, odd);

				rowTop += rowHeight + 1;
				odd = !odd;
			}
	
			s.Append("</svg>\n");
	
			if (!string.IsNullOrEmpty(Description))
		    	AppendStrings(s, "<p>", Description, "</p>\n");
			    
			s.Append("<br /><br />\n\n");
			return s.ToString();
		}
	}

	public class ZoomHtmlInclusion: ZoomReportBase
	{
		public string Literal { get; set; }

		public ZoomHtmlInclusion(string literal) { Literal = literal; }

		public override string ToCsv(char separator) { return Literal; }

		public override string ToHtml() { return Literal; }

		public override string ToSvg(int table) { return Literal; }

		public override IEnumerable<Color> BarCellColors() { return new List<Color>(); }
	}
	
	public class ZoomReports: List<ZoomReportBase>
	{
		string Title { get; set; }
		/// <summary>If true, show bars in HTML reports.</summary>
		bool Bars { get; set; }

		ZReportColors Colors { get; set; }
		public ZReportColors Colours { get { return Colors; } }
		
//		int HeightUsed { get; set; }

		public ZoomReports()
		{
			Colors = new ZReportColors();
			Bars = true;
		}

		public ZoomReports(string title)
		{
			Title = title;
			Colors = new ZReportColors();
			Bars = true;
		}

		public new void Add(ZoomReportBase report)
		{
			base.Add(report);
			if (report.Colors == null)
				report.Colors = Colors;
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
							if (!result.Contains(cell.GetBarColor(Colors.GetBackColor(odd))))
							    result.Add(cell.GetBarColor(Colors.GetBackColor(odd)));
						odd = !odd;
					}
				}

			if (!result.Contains(Colors.BarNone))
				result.Add(Colors.BarNone);
			if (!result.Contains(ZReportColors.AddDark(Colors.BarNone, Colors.OddColor)))
				result.Add(ZReportColors.AddDark(Colors.BarNone, Colors.OddColor));

			return result;
//			return this.SelectMany(x => x.BarCellColors()).Distinct().ToList();
		}

		public string ToCsv()
		{
			StringBuilder sb = new StringBuilder();
			foreach (ZoomReportBase report in this) {
				sb.Append(report.ToCsv(','));
		 		sb.Append("\n");
			}
			return sb.ToString();
		}

		public string ToTsv()
		{
			StringBuilder sb = new StringBuilder();
			foreach (ZoomReportBase report in this) {
				sb.Append(report.ToCsv('\t'));
		 		sb.Append("\n");
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

			sb.Append("</head><body>\n");
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
			sb.Append("</title></head><body>\n");
			//sb.Append("");  // TODO: cellstyles.ToHtml here?

			for (int i = 0; i < Count; i++)
			{
				sb.Append(this[i].ToSvg(i));
		 		sb.Append("<br/>\n\n");
			}

			sb.Append(@"<script>
  var texts = document.querySelectorAll('text');
  for (i = 0; i < texts.length; i++) {
    var fit = texts[i].getComputedTextLength() / (texts[i].getAttribute('width') - 2);
    if (fit > 1)
      texts[i].setAttribute('font-size', texts[i].getAttribute('font-size') / fit);
  }
</script>
");

			sb.Append("</body>\n");

			return sb.ToString();
		}
	}
}
