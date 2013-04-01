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
		private double ZeroEpsilon = 0.000001;
		private double TitleGridHeight;
		private double GoalGridHeight;
		private Thickness GoalGridMargin;
		private TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(200);
		private TimeSpan FastAnimationDuration = TimeSpan.FromMilliseconds(75);
		private double CountAnimationX = 30.0;

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
					Debug.WriteLine(ex.Message);

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

		private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
		{
			TitleGridHeight = TitleGrid.ActualHeight;
			GoalGridHeight = GoalGrid.ActualHeight;
			GoalGridMargin = GoalGrid.Margin;

			// I fucking *hate* the Windows Phone animation system.
			CountText.RenderTransform = new TranslateTransform();
		}

		private void AnimateCountOut(bool leftDirection)
		{
			double startX = 0;
			double endX = leftDirection ? -CountAnimationX : CountAnimationX;

			Storyboard sb = new Storyboard();
			DoubleAnimation translationAnimation = new DoubleAnimation();
			translationAnimation.Duration = FastAnimationDuration;
			translationAnimation.From = startX;
			translationAnimation.To = endX;
			DoubleAnimation fadeAnimation = new DoubleAnimation();
			fadeAnimation.Duration = translationAnimation.Duration;
			fadeAnimation.From = 1.0;
			fadeAnimation.To = 0;
			Storyboard.SetTarget(translationAnimation, CountText);
			Storyboard.SetTarget(fadeAnimation, CountText);
			Storyboard.SetTargetProperty(translationAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
			Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("UIElement.Opacity"));
			
			if (leftDirection)
			{
				sb.Completed += AnimateCountOut_Left_Completed;
			}
			else
			{
				sb.Completed += AnimateCountOut_Right_Completed;
			}

			sb.Children.Add(translationAnimation);
			sb.Children.Add(fadeAnimation);
			sb.Begin();
		}

		private void AnimateCountIn(bool leftDirection)
		{
			CountText.Text = Count.ToString();

			double startX = leftDirection ? CountAnimationX : -CountAnimationX;
			double endX = 0;

			Storyboard sb = new Storyboard();
			DoubleAnimation translationAnimation = new DoubleAnimation();
			translationAnimation.Duration = FastAnimationDuration;
			translationAnimation.From = startX;
			translationAnimation.To = endX;
			DoubleAnimation fadeAnimation = new DoubleAnimation();
			fadeAnimation.Duration = translationAnimation.Duration;
			fadeAnimation.From = 0;
			fadeAnimation.To = 1.0;
			Storyboard.SetTarget(translationAnimation, CountText);
			Storyboard.SetTarget(fadeAnimation, CountText);
			Storyboard.SetTargetProperty(translationAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
			Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("UIElement.Opacity"));
			sb.Children.Add(translationAnimation);
			sb.Children.Add(fadeAnimation);
			sb.Begin();
		}

		private void AnimateCountOut_Left_Completed(object sender, EventArgs e)
		{
			AnimateCountIn(true);
		}

		private void AnimateCountOut_Right_Completed(object sender, EventArgs e)
		{
			AnimateCountIn(false);
		}

		private void UpdateCountText(bool increment)
		{
			AnimateCountOut(increment);
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