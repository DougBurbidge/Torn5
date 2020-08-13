using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Torn
{
	/// <summary>Static methods, including extension methods, used in misc places.</summary>
	public static class Utility
	{
		public static string FriendlyDate(this DateTime date)
		{
			int daysAgo = (int)DateTime.Now.Date.Subtract(date.Date).TotalDays;

			if (daysAgo == 0) return "Today";
			if (daysAgo == 1) return "Yesterday";
			if (daysAgo > 0 && daysAgo < 7) return date.DayOfWeek.ToString().Substring(0, 3) + " " + Ordinate(date.Day);
			return date.ToShortDateString();
		}

		public static string JustPlayed(this DateTime date)
		{
			int hoursAgo = (int)DateTime.Now.Date.Subtract(date.Date).TotalHours;

			if (hoursAgo == 0) 
				return "Just played";
			if (hoursAgo < 48)
				return "Played " + hoursAgo.ToString() + " hours ago";
			return "Played on " + date.Date.ToLongDateString();
		}

		public static string ShortDateTime(this DateTime date)
		{
			string s = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
			int i = s.IndexOf(":ss");
			if (i > -1)
				s = s.Remove(i, 3);
			return date.ToShortDateString() + " " + date.ToString(s);
		}

/*		static string Rot13(string s)
		{
			var sb = new StringBuilder();

			foreach (char c in s)
			{
				int i = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".IndexOf(c);
				sb.Append(i == -1 ? c : "NOPQRSTUVWXYZABCDEFGHIJKLMnopqrstuvwxyzabcdefghijklm"[i]);
			}

			return sb.ToString();
		}

		static char Base62(int i)
		{
			return i < 10 ? (char)(i + '0') :
				   i < 36 ? (char)(i + 'a' - 10) :
				            (char)(i + 'A' - 36);
		}
*/
		public static string GetString(this XmlNode node, string name, string defaultValue = null)
		{
			var child = node.SelectSingleNode(name);
			return child == null ? defaultValue : child.InnerText;
		}

		public static double GetDouble(this XmlNode node, string name)
		{
			var child = node.SelectSingleNode(name);
			return child == null ? 0.0 : double.Parse(child.InnerText, CultureInfo.InvariantCulture);
		}

		public static int GetInt(this XmlNode node, string name)
		{
			var child = node.SelectSingleNode(name);
			return child == null ? 0 : int.Parse(child.InnerText, CultureInfo.InvariantCulture);
		}

		public static void AppendNode(this XmlDocument doc, XmlNode parent, string name, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				XmlNode node = doc.CreateElement(name);
				node.AppendChild(doc.CreateTextNode(value));
				parent.AppendChild(node);
			}
		}

		public static void AppendNode(this XmlDocument doc, XmlNode parent, string name, int value)
		{
			AppendNode(doc, parent, name, value.ToString());
		}

		public static void AppendNonZero(this XmlDocument doc, XmlNode parent, string name, double value)
		{
			if (value != 0)
				AppendNode(doc, parent, name, value.ToString());
		}

		/// <summary>Convert to an ordinal: 1 to 1st, 2 to 2nd, etc.</summary>
		public static string Ordinate(this int i)
		{
			var s = i.ToString();
			if (i % 10 == 1 && i % 100 != 11)
				return s + "st";
			if (i % 10 == 2 && i % 100 != 12)
				return s + "nd";
			if (i % 10 == 3 && i % 100 != 13)
				return s + "rd";
			return s + "th";
		}

		public static bool Valid<T>(this IList<T> list, int i)
		{
			return 0 <= i && i < list.Count;
		}

		static void JsonKeyValueInternal(StringBuilder sb, int indent, string key, string value, bool comma)
		{
			sb.Append('\t', indent);
			sb.Append('"');
			sb.Append(key);
			sb.Append('"');
			sb.Append(':');
			sb.Append(value);
			if (comma)
				sb.Append(',');
			sb.Append('\n');
		}
	
		public static void JsonKeyValue(StringBuilder sb, int indent, string key, string value, bool comma = true)
		{
			if (!string.IsNullOrEmpty(value) || !comma)
				JsonKeyValueInternal(sb, indent, key, "\"" + value + "\"", comma);
		}

		public static void JsonKeyValue(StringBuilder sb, int indent, string key, int? value, int? defualt = null, bool comma = true)
		{
			if ((value != null && value != defualt) || !comma)
				JsonKeyValueInternal(sb, indent, key, ((int)value).ToString(CultureInfo.InvariantCulture), comma);
		}
	}
}
