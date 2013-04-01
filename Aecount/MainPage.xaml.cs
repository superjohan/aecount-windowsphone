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

		private double TitleGridHeight;
		private double GoalGridHeight;
		private Thickness GoalGridMargin;

		public MainPage()
		{
			InitializeComponent();

			CountText.Text = Count.ToString();
		}

		private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
		{
			TitleGridHeight = TitleGrid.ActualHeight;
			GoalGridHeight = GoalGrid.ActualHeight;
			GoalGridMargin = GoalGrid.Margin;
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

		private void ResetTopAndBottomGrid()
		{
			TitleGrid.Height = TitleGridHeight;
			GoalGrid.Height = GoalGridHeight;
			GoalGrid.Margin = GoalGridMargin;
		}

		private void ResetCounter()
		{
			this.Count = 0;
			CountText.Text = Count.ToString();
			ResetTopAndBottomGrid();
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

		private double YValueForManipulationDelta(ManipulationDelta delta)
		{
			double topY = (1.0 - delta.Scale.Y) * CounterGrid.ActualHeight;
			return topY;
		}

		private double ZeroEpsilon = 0.000001;

		private void LayoutRoot_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			ManipulationDelta delta = e.CumulativeManipulation;
			double y = YValueForManipulationDelta(delta);

			// FIXME: This margin may have to be adjusted. It doesn't quite reach the center.
			if (y > (CounterGrid.ActualHeight / 2.0))
			{
				return;
			}
			else if (y < 0)
			{
				return;
			}
			else if (delta.Scale.X < ZeroEpsilon && delta.Scale.Y < ZeroEpsilon)
			{
				// Not a pinch event.
				return;
			}

			TitleGrid.Height = TitleGridHeight + y;

			GoalGrid.Height = GoalGridHeight + y;
			Thickness margin = GoalGrid.Margin;
			margin.Top = GoalGridMargin.Top - y;
			GoalGrid.Margin = margin;
		}

		private void LayoutRoot_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			ManipulationDelta delta = e.TotalManipulation;

			if (delta.Scale.X < ZeroEpsilon && delta.Scale.Y < ZeroEpsilon)
			{
				// Not a pinch event.
				return;
			} 
			
			double y = YValueForManipulationDelta(delta);

			if (y < 10.0)
			{
				ResetTopAndBottomGrid();
			}
			else
			{
				ResetCounter();
			}
		}
	}
}