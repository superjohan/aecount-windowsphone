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

namespace Aecount
{
	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();

		}

		private int Count = 0;

		private void UpdateCountText(bool increment)
		{
			// TODO: Animation.

			CountText.Text = Count.ToString();
		}

		private void DecrementCount()
		{
			Count--;
			UpdateCountText(false);
		}

		private void IncrementCount()
		{
			Count++;
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