using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Diagnostics;
using System.Threading;

namespace VP.Playground.Lego.EV3Test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Brick _brick;
		int _forward = 40;
		int _backward = -30;
		uint _time = 300;
		int _distanceMeasured = Int32.MaxValue;

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// TODO: Update port value with the connected port on your machine
			_brick = new Brick(new BluetoothCommunication("COM6"));
			_brick.BrickChanged += _brick_BrickChanged;
			await _brick.ConnectAsync();
			await _brick.DirectCommand.SetMotorPolarity(OutputPort.B | OutputPort.C, Polarity.Backward);
			await _brick.DirectCommand.StopMotorAsync(OutputPort.All, false);
		}

		private void _brick_BrickChanged(object sender, BrickChangedEventArgs e)
		{
			Debug.WriteLine("Brick changed!");

			this._distanceMeasured = (int)e.Ports[InputPort.Four].SIValue;
			txtDistance.Text = this._distanceMeasured.ToString();

			Debug.WriteLine(this._distanceMeasured.ToString());
		}

		private async void ForwardButton_Click(object sender, RoutedEventArgs e)
		{
			await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _forward, _time, false); 
		}

		private async void BackwardButton_Click(object sender, RoutedEventArgs e)
		{
			await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _backward, _time, false); 
		}

		private async void LeftButton_Click(object sender, RoutedEventArgs e)
		{
			_brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, _forward, _time, false);
			_brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, _backward, _time, false);
			await _brick.BatchCommand.SendCommandAsync();
		}

		private async void RightButton_Click(object sender, RoutedEventArgs e)
		{
			_brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, _backward, _time, false);
			_brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, _forward, _time, false);
			await _brick.BatchCommand.SendCommandAsync();
		}

		private async void StartButton_Click(object sender, RoutedEventArgs e)
		{
			do
			{
				await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _forward, _time, false);
				Thread.Sleep(10);
			 } while (this._distanceMeasured >= 10);

			await _brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
		}

		private async void StopButton_Click(object sender, RoutedEventArgs e)
		{
			await _brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
		}
	}
}
