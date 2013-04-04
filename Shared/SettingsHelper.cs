using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace Aecount
{
	public static class SettingsHelper
	{
		public static object GetValueForKey(string key)
		{
			IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

			try
			{
				if (settings.Contains(key))
				{
					object value = settings[key];
					return value;
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public static void SetValue(string key, object value)
		{
			IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

			if (settings.Contains(key))
			{
				settings[key] = value;
			}
			else
			{
				try
				{
					settings.Add(key, value);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}

			settings.Save();
		}

		public static int GetIntegerValueForKey(string key)
		{
			object value = GetValueForKey(key);

			if (value == null)
			{
				return 0;
			}

			return Convert.ToInt32(value);
		}

		public static bool GetBoolValueForKey(string key)
		{
			object value = GetValueForKey(key);

			if (value == null)
			{
				return false;
			}

			return Convert.ToBoolean(value);
		}
	}
}
