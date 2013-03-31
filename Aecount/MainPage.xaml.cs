using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.IO.IsolatedStorage;

namespace Aecount
{
	public partial class MainPage : PhoneApplicationPage
	{
		private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
		private string CountKey = "count";
		private string TitleKey = "title";
		private string GoalKey = "goal";

		private int Count
		{
			get
			{
				try
				{
					int count = (int)settings[CountKey];
					return count;
				}
				catch (Exception ex)
				{
					this.Count = 0;
					return 0;
				}
			}

			set
			{
				Debug.WriteLine("Setting value to " + value);

				if (settings.Contains(CountKey))
				{
					settings[CountKey] = value;
				}
				else
				{
					try
					{
						settings.Add(CountKey, value);
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Unable to add count to settings: " + ex.Message);
					}
				}

				settings.Save();
			}
		}

		public MainPage()
		{
			InitializeComponent();

			CountText.Text = Count.ToString();
		}

		private void UpdateCountText(bool increment)
		{
			// TODO: Animation.

			CountText.Text = Count.ToString();
		}

		private void DecrementCount()
		{
			this.Count--;
			UpdateCountText(false);
		}

		private void IncrementCount()
		{
			this.Count++;
			UpdateCountText(true);
		}

		private void Grid_Tap_1(object sender, GestureEventArgs e)
		{
			IncrementCount();
		}

		private void CounterGrid_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			ManipulationDelta delta = e.TotalManipulation;
			Point translation = delta.Translation;

			if (translation.X > 50.0)
			{
				DecrementCount();
			}
			else if (translation.X < -50.0)
			{
				IncrementCount();
			}
		}
	}
}